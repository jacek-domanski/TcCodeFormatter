using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using CommandLine;
using TcCodeFormatter.Utilities;

namespace TcCodeFormatter
{
	class Program
	{
		private static List<string> filesPaths = new List<string>();
		private static readonly string[] EXTENSIONS = new[] { ".TcPOU", ".TcGVL", ".TcDUT", ".TcIO" };
		static void Main(string[] args)
		{
			System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();

			CommandLine.Parser.Default.ParseArguments<Options>(args)
				.WithParsed(runOptions)
				.WithNotParsed(handleParseError);
			stopwatch.Start();

			FileFormatter fileFormatter = null;

			foreach (string filePath in filesPaths)
			{
				fileFormatter = new FileFormatter(filePath);
				fileFormatter.formatCode();
				fileFormatter.save(filePath);
			}

			stopwatch.Stop();
			Console.WriteLine("Formatted " + filesPaths.Count + " files in " + stopwatch.ElapsedMilliseconds + " ms");
		}
		static void runOptions(Options options)
		{
			if (!(options.All ^ options.Diff ^ options.InputFiles.Count() > 0))
			{
				Console.WriteLine("Exactly one argument of -a, -d, -f is required");
				Environment.Exit(1);
			}
			Flags flags = Flags.Instance;
			flags.setFlags(options);

			Console.WriteLine("Files: " + options.InputFiles.Count() + ", All: " + options.All + ", Diff: " + options.Diff + ", No confirmation: " + options.NoConfirmation);
			if (options.InputFiles.Count() > 0)
			{
				getAllFilesPaths();
				pickOnlyRequestedFiles(options);
			} 
			else if (options.Diff)
			{
				getChangedFiles();
			} 
			else if (options.All)
			{
				getAllFilesPaths();
			}
		}
		static void getAllFilesPaths()
		{
			string currentDirectory = System.IO.Directory.GetCurrentDirectory();
			getAllFilesRecursively(currentDirectory);
		}
		static void getAllFilesRecursively(string directory)
		{
			string[] files = System.IO.Directory.GetFiles(directory);
			foreach (string file in files)
			{
				foreach (string extension in EXTENSIONS)
				{
					if (file.EndsWith(extension))
					{
						filesPaths.Add(file);
						break;
					}
				}
			}

			string[] subdirectories = System.IO.Directory.GetDirectories(directory);
			foreach (string subdirectory in subdirectories)
			{
				getAllFilesRecursively(subdirectory);
			}
		}
		static void pickOnlyRequestedFiles(Options options)
		{
			List<string> requestedFilesPaths = new List<string>();
			List<Regex> regexes = new List<Regex>();
			foreach (string reqestedFileNames in options.InputFiles)
			{
				regexes.Add(new Regex(reqestedFileNames, RegexOptions.IgnoreCase));
			}

			foreach (string filePath in filesPaths)
			{
				foreach(Regex regex in regexes)
				{
					if (regex.IsMatch(filePath))
					{
						requestedFilesPaths.Add(filePath);
						break;
					}
				}
			}

			if (requestedFilesPaths.Count == 0)
			{
				Console.WriteLine("No matching files found");
				Environment.Exit(0);
				return;
			}

			if (options.NoConfirmation)
			{
				filesPaths = requestedFilesPaths;
				return;
			}

			while (true)
			{
				Console.WriteLine("Files to be formatted:");
				requestedFilesPaths.ForEach(x => Console.WriteLine(x));
				Console.WriteLine("Is that correct? (y/n)");
				string response = Console.ReadLine().ToLower();

				if (response == "y" || response == "yes")
				{
					filesPaths = requestedFilesPaths;
					return;
				}
				else if (response == "n" || response == "no")
				{
					Console.WriteLine("No file was formatted");
					Environment.Exit(0);
					return;
				}
				else
				{
					Console.WriteLine();
				}
			}
		}
		static void getChangedFiles()
		{
			string changedFilesRelativePathsTextBlob = "";
			try
			{
				changedFilesRelativePathsTextBlob = getChangedFilesRelativePaths();
			} 
			catch (System.ComponentModel.Win32Exception e)
			{
				Console.WriteLine("Git cannot be started");
				Console.WriteLine(e);
				Environment.Exit(1);
			}


			List<string> changedFilesRelativePaths = changedFilesRelativePathsTextBlob.Split('\n').ToList();
			string currentDirectory = System.IO.Directory.GetCurrentDirectory().Replace('/', '\\');

			changedFilesRelativePaths
				.FindAll(filePath => filePath != "")
				.ForEach(filePath => {
					foreach (string extension in EXTENSIONS)
					{
						if (filePath.EndsWith(extension))
						{
							filesPaths.Add(currentDirectory + '\\' + filePath);
							break;
						}
					}
				});
		}

		private static string getChangedFilesRelativePaths()
		{
			ProcessStartInfo startInfo = new ProcessStartInfo("git.exe");

			startInfo.UseShellExecute = false;
			startInfo.WorkingDirectory = System.IO.Directory.GetCurrentDirectory();
			startInfo.RedirectStandardInput = true;
			startInfo.RedirectStandardOutput = true;
			startInfo.Arguments = "diff --name-only";

			Process process = new Process();
			process.StartInfo = startInfo;
			process.Start();

			string output = process.StandardOutput.ReadToEnd();
			process.WaitForExit();
			return output;
		}

		static void handleParseError(IEnumerable<Error> errors)
		{
			Console.WriteLine("Parsing error:");
			foreach (Error error in errors)
			{
				Console.WriteLine(error);
			}
			Environment.Exit(1);
		}
	}
}
