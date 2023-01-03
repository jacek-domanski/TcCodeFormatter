using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcCodeFormatter
{
	public static class Functions
	{
		public static void printIfVerbose(string text)
		{
			if (Flags.Instance.Verbose) Console.WriteLine(text);
		}
	}
}
