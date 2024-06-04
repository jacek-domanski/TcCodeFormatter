using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TcCodeFormatter.Utilities
{
	public class Regexes
	{
		public static Regex whitespaceOnly = new Regex(@"^\s+$");
		public static Regex emptyOrWhitespaceOnly = new Regex(@"^\s?$");
		public static Regex declarationNoEmptyLineBeforeOrAfter = 
			new Regex(@"(^|\s)(VAR|VAR_INPUT|VAR_OUTPUT|VAR_IN_OUT|VAR_GLOBAL|VAR_TEMP|VAR_STAT|VAR_EXTERNAL|VAR_INST|END_VAR|TYPE|END_TYPE|STRUCT|END_STRUCT|FUNCTION_BLOCK|METHOD|INTERFACE|PROGRAM|FUNCTION)(\s|;|$)");

		public static Regex equalButNotSimilar = new Regex(@"(?<!:|REF|<|>)=(?!>)");
		public static Regex lesserButNotSimilar = new Regex(@"<(?!=|>)");
		public static Regex greaterButNotSimilar = new Regex(@"(?<!<|=)>(?!=)");
		public static Regex referenceAssignment = new Regex(@"(^|\s)REF=");
		public static Regex whitespacesButNotAtTheStart = new Regex(@"(?<=[^\s])\s+");
		public static Regex commaMidline = new Regex(@",(?! |$)");

		public static Regex endsWithOpeningBracket = new Regex(@"[\(|\[]\s*$");
		public static Regex startsWithClosingBracket = new Regex(@"^\s*[\)|\]]");

		public static Regex whitespacesBeforeSemicolon = new Regex(@"(?<!^|\s)\s+(?=;)");
		public static Regex colonNotInAssignment = new Regex(@":(?!=)");
	}
}
