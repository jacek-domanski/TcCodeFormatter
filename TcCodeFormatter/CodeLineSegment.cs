using System;

namespace TcCodeFormatter
{
	public enum SegmentType
	{
		Unkown,
		Code,
		MultilineComment,
		EndlineComment,
		StringLiteralSingleQuote,
		StringLiteralDoubleQuote,
		Pragma
	}

	public class CodeLineSegment
	{
		private string text;
		private SegmentType segmentType;

		public CodeLineSegment(string segment, SegmentType segmentType)
		{
			this.text = segment;
			this.segmentType = segmentType;
		}

		public string Text { get => text;}
		public SegmentType SegmentType { get => segmentType;}
		public void convertUnknownTypeToCode()
		{
			if (this.segmentType != SegmentType.Unkown)
			{
				throw new Exception(
					"Cannot convert " + segmentType.ToString() + " to code"
					);
			}

			this.segmentType = SegmentType.Code;
		}
	}
}
