using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcCodeFormatter
{
	public class LineBuilder
	{
		private string line;
		public LineBuilder()
		{
			this.line = "";
		}
		public void reset()
		{
			this.line = "";
		}
		public void append(CodeLineSegment segment)
		{
			switch (segment.SegmentType)
			{
				case SegmentType.Unkown:
					appendUnknown(segment);
					break;
				case SegmentType.Code:
					appendCode(segment);
					break;
				case SegmentType.MultilineComment:
					appendMultilineComment(segment);
					break;
				case SegmentType.EndlineComment:
					appendEndlineComment(segment);
					break;
				case SegmentType.StringLiteralSingleQuote:
					appendStringLiteralSingleQuote(segment);

					break;
				case SegmentType.StringLiteralDoubleQuote:
					appendStringLiteralDoubleQuote(segment);

					break;
				case SegmentType.Pragma:
					appendPragma(segment);

					break;
				default:
					throw new NotImplementedException("Appending " + segment.SegmentType.ToString() + " segment is not implemented");
			}
		}
		public string getLine()
		{
			return this.line.TrimEnd();
		}
		private void appendUnknown(CodeLineSegment segment)
		{
			throw new ArgumentException("Cannot append unknown type segment");
		}
		private void appendCode(CodeLineSegment segment)
		{
			appendSpaceIfNoWhitespaceAtTheEnd();

			bool trimEnd = this.line.EndsWith("\" ") || this.line.EndsWith("' ") || (line.EndsWith(" ") && segment.Text.StartsWith(" "));
			if (trimEnd) this.line = this.line.TrimEnd();

			this.line += segment.Text;
		}
		private void appendMultilineComment(CodeLineSegment segment)
		{
			if (segment.HasStartMarker)
			{
				appendSpaceIfNoWhitespaceAtTheEnd();
				line += "(*";

				bool startingSpace =
					segment.Text.Length > 0
					&& !segment.Text.StartsWith(" ");
				if (startingSpace) line += " ";
			}

			this.line += segment.Text;

			if (segment.HasEndMarker)
			{
				bool endingSpace =
					segment.Text.Length > 0
					&& !segment.Text.EndsWith(" ");
				if (endingSpace) line += " ";
				line += "*)";
			}
		}
		private void appendEndlineComment(CodeLineSegment segment)
		{
			appendSpaceIfNoWhitespaceAtTheEnd();
			this.line += "// ";
			this.line += trimmed(segment.Text);
		}
		private void appendStringLiteralSingleQuote(CodeLineSegment segment)
		{
			if (segment.HasStartMarker)
			{
				appendSpaceIfNoWhitespaceAtTheEnd(new[] { "(" });
				line += "'";
			}

			this.line += segment.Text;

			if (segment.HasEndMarker)
			{
				line += "'";
			}
		}
		private void appendStringLiteralDoubleQuote(CodeLineSegment segment)
		{
			if (segment.HasStartMarker)
			{
				appendSpaceIfNoWhitespaceAtTheEnd(new[] { "(" });
				line += "\"";
			}

			this.line += segment.Text;

			if (segment.HasEndMarker)
			{
				line += "\"";
			}
		}
		private void appendPragma(CodeLineSegment segment)
		{
			if (segment.HasStartMarker)
			{
				appendSpaceIfNoWhitespaceAtTheEnd();
				line += "{";
			}

			this.line += segment.Text;

			if (segment.HasEndMarker)
			{
				line += "}";
			}
		}
		private void appendSpaceIfNoWhitespaceAtTheEnd(string[] stringsToNotAddSpaceAfter = null)
		{
			bool appendSpace =
				!this.line.EndsWith(" ")
				&& !this.line.EndsWith("\t")
				&& this.line.Length > 0;

			if (appendSpace && stringsToNotAddSpaceAfter != null) 
			{ 
				foreach(string str in stringsToNotAddSpaceAfter) 
				{
					if (this.line.EndsWith(str) && !String.IsNullOrEmpty(str))
					{
						return;
					}
				}
			}

			if (appendSpace) this.line += " ";
		}
		private string trimmed(string text)
		{
			return text.TrimStart().TrimEnd();
		}
	}
}
