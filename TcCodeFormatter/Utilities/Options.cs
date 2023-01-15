using CommandLine.Text;
using CommandLine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcCodeFormatter
{
	internal class Options
	{
		[Option('v', "verbose", Required = false, HelpText = "TcCodeFormatter becomes talkative")]
		public bool Verbose { get; set; }

		[Option('f', "files", Required = false, HelpText = "Input files to be formatted, separated by spaces. Current directory will be searched for matches. File extension is not necessary")]
		public IEnumerable<string> InputFiles { get; set; }

		[Option('a', "all", Required = false, HelpText = "Searches current directory recursively, formats all known files found")]
		public bool All { get; set; }

		[Option('d', "diff", Required = false, HelpText = "Formats only files found by git diff --stat. Requires git installed")]
		public bool Diff { get; set; }

		[Option('n', "no-confirmation", Required = false, HelpText = "No confirmation in --files mode is required. Should be used as -nf or -n -f")]
		public bool NoConfirmation { get; set; }
	}
}
