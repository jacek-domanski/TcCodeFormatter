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
		private bool startMarker;
		private bool endMarker;
		private bool isFirstSegmentInLine;
		private bool isLastSegmentInLine;

		public CodeLineSegment(
			string segment, 
			SegmentType segmentType, 
			bool hasStartMarker = true, 
			bool hasEndMarker = true)
		{
			this.text = segment;
			this.segmentType = segmentType;
			this.startMarker = hasStartMarker;
			this.endMarker = hasEndMarker;
		}

		public string Text { get => text; set => text = value;  }
		public SegmentType SegmentType { get => segmentType;}
		public bool HasStartMarker { get => startMarker;}
		public bool HasEndMarker { get => endMarker;}
		public bool IsFirstSegmentInLine { get => isFirstSegmentInLine; set => isFirstSegmentInLine = value; }
		public bool IsLastSegmentInLine { get => isLastSegmentInLine; set => isLastSegmentInLine = value; }

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
