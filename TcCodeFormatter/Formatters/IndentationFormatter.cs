using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcCodeFormatter.Utilities;

namespace TcCodeFormatter.Formatters
{
	public class IndentationFormatter
	{
		private const string _indentation = "\t";
		private uint[] _indentationsCount;
		public IndentationFormatter() { }

		public static string Indentation => _indentation;

		public uint[] IndentationsCount { get => _indentationsCount; private set => _indentationsCount = value; }

		public void removeIndentations(List<CodeLineSegment> segments)
		{
			if (segments is null || segments.Count == 0) return;

			if (segments[0].SegmentType == SegmentType.Code)
			{
				segments[0].Text = Regexes.whitespacesAtLineStart.Replace(segments[0].Text, "");
			}
		}

		public void newIndentationsArray(uint linesCount)
		{
			IndentationsCount = new uint[linesCount];
		}
	}
}
