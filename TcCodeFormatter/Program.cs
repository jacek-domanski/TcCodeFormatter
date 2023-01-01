using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using CommandLine;

namespace TcCodeFormatter
{
	class Program
	{
		private static List<string> filesPaths = new List<string>();
		private static readonly string[] EXTENSIONS = new[] { ".TcPOU", ".TcGVL", ".TcDUT", ".TcIO" };
		static void Main(string[] args)
		{
			CommandLine.Parser.Default.ParseArguments<Options>(args)
				.WithParsed(runOptions)
				.WithNotParsed(handleParseError);

			FileFormatter fileFormatter = null;

			foreach (string filePath in filesPaths)
			{
				fileFormatter = new FileFormatter(filePath);
				fileFormatter.formatCode();
				fileFormatter.save(filePath + "2");
			}
		}
		static void runOptions(Options options)
		{
			Console.WriteLine("Files: " + options.InputFiles.Count() + ", All: " + options.All + ", Diff: " + options.Diff);
			if (options.InputFiles.Count() > 0)
			{
				throw new NotImplementedException("--files is not implemented yet");
			} else if (options.Diff)
			{
				throw new NotImplementedException("--diff is not implemented yet");
			} else if (options.All)
			{
				getAllFilesPaths();
			}
		}
		static void getAllFilesPaths()
		{
			Console.WriteLine("Getting all files paths:");
			string currentDirectory = System.IO.Directory.GetCurrentDirectory();
			getAllFilesRecursively(currentDirectory);
			foreach (string filePath in filesPaths)
			{
				Console.WriteLine(filePath);
			}
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
