using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using CommandLine;

namespace TcCodeFormatter
{
	class Program
	{
		static void Main(string[] args)
		{
			CommandLine.Parser.Default.ParseArguments<Options>(args)
				.WithParsed(runOptions)
				.WithNotParsed(handleParseError);

			string[] filesPaths = new[] { "C:\\Projects\\CSharp\\TcCodeFormatter\\POUs\\FB_StringBuilder.TcPOU" };

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
