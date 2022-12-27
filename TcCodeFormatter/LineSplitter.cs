using System;
using System.Collections.Generic;

namespace TcCodeFormatter
{
	public class LineSplitter
	{
		private SegmentType multilineSegment = SegmentType.Unkown;
		private List<CodeLineSegment> segments;
		public LineSplitter()
		{
		}
		public void reset()
		{
			this.multilineSegment = SegmentType.Unkown;
		}
		public List<CodeLineSegment> split(string line)
		{
			this.segments = new List<CodeLineSegment>();
			this.segments.Add(new CodeLineSegment(line, SegmentType.Unkown));

			bool allSegmentsKnown = false;
			while (allSegmentsKnown == false)
			{
				allSegmentsKnown = continuouslySplitUnkownSegments();
			}

			return this.segments;
		}
		private bool continuouslySplitUnkownSegments()
		{
			bool allSegmentsKnown = true;

			for (int i = 0; i < this.segments.Count; i++)
			{
				if (this.segments[i].SegmentType == SegmentType.Unkown)
				{
					splitUnknownSegment(i);
					allSegmentsKnown = false;
					break;
				}
			}

			return allSegmentsKnown;
		}
		private void splitUnknownSegment(int segmentIndex)
		{
			string segmentText = this.segments[segmentIndex].Text;
			Dictionary<SegmentType, SpecialSegment> specialSegments =
				initializeSpecialSegments();
			int specialSegmentStartIndex;
			SegmentType specialSegmentType;

			if (segmentStartsInCurrentLine())
			{
				findOutIfSegmentContainsSpecialSegments(segmentText, specialSegments);

				findFirstSpecialSegment(
					segmentText, specialSegments,
					out specialSegmentStartIndex, out specialSegmentType);
				splitSegmentBySpecialSegment(
					segmentIndex, segmentText, specialSegments, specialSegmentStartIndex, specialSegmentType);
			} else
			{
				specialSegmentStartIndex = 0;
				specialSegmentType = this.multilineSegment;
				splitSegmentBySpecialSegment(
					segmentIndex, segmentText, specialSegments, specialSegmentStartIndex, specialSegmentType);
			}
		}

		private void splitSegmentBySpecialSegment(
			int segmentIndex, 
			string segmentText, 
			Dictionary<SegmentType, SpecialSegment> specialSegments,
			int specialSegmentStartIndex, 
			SegmentType specialSegmentType)
		{
			CodeLineSegment code, specialSegment, unknown = null;
			bool endNotFound = false;
			switch (specialSegmentType)
			{
				case SegmentType.Code:
					throw new Exception("Segment identified as code too early");

				case SegmentType.Unkown:
					this.segments[segmentIndex].convertUnknownTypeToCode();
					return;

				case SegmentType.EndlineComment:
					code = AddCodeSegment(segmentText, specialSegmentStartIndex);
					specialSegment =
						new CodeLineSegment(
							segmentText.Substring(
								specialSegmentStartIndex
								+ specialSegments[specialSegmentType].start.Length),
							specialSegmentType
						);
					break;

				default:
					int specialSegmentEndIndex = getSpecialSegmentEndIndex(segmentText, specialSegments, specialSegmentStartIndex, specialSegmentType);

					code = AddCodeSegment(segmentText, specialSegmentStartIndex);
					int specialSegmentContentStartIndex =
						getSpecialSegmentContentStartIndex(specialSegments, specialSegmentStartIndex, specialSegmentType);

					if (specialSegmentEndIndex == -1 || specialSegmentEndIndex <= specialSegmentStartIndex)
					{
						specialSegment =
							new CodeLineSegment(
								segmentText.Substring(specialSegmentContentStartIndex),
								specialSegmentType,
								segmentStartsInCurrentLine(),
								false
							);
						this.multilineSegment = specialSegmentType;
						endNotFound = true;
					}
					else
					{
						int specialSegmentContentLength = specialSegmentEndIndex - specialSegmentContentStartIndex;

						specialSegment =
							new CodeLineSegment(
								segmentText.Substring(
									specialSegmentContentStartIndex,
									specialSegmentContentLength),
								specialSegmentType,
								segmentStartsInCurrentLine(),
								true
							);

						unknown = addRemainingUnknownSegment(
							segmentText, specialSegments, specialSegmentType, unknown, specialSegmentEndIndex);
					}
					break;
			}
			this.segments.RemoveAt(segmentIndex);
			if (unknown != null) this.segments.Insert(segmentIndex, unknown);
			this.segments.Insert(segmentIndex, specialSegment);
			if (code != null) this.segments.Insert(segmentIndex, code);
			if (!endNotFound) this.reset();
		}

		private static CodeLineSegment addRemainingUnknownSegment(string segmentText, Dictionary<SegmentType, SpecialSegment> specialSegments, SegmentType specialSegmentType, CodeLineSegment unknown, int specialSegmentEndIndex)
		{
			string remainingUnknownString = segmentText.Substring(specialSegmentEndIndex + specialSegments[specialSegmentType].end.Length);
			if (remainingUnknownString.Length > 0)
			{
				unknown =
					new CodeLineSegment(
						remainingUnknownString,
						SegmentType.Unkown
					);
			}

			return unknown;
		}

		private int getSpecialSegmentContentStartIndex(Dictionary<SegmentType, SpecialSegment> specialSegments, int specialSegmentStartIndex, SegmentType specialSegmentType)
		{
			int specialSegmentContentStartIndex = specialSegmentStartIndex;
			if (segmentStartsInCurrentLine())
			{
				specialSegmentContentStartIndex +=
					specialSegments[specialSegmentType].start.Length;
			}

			return specialSegmentContentStartIndex;
		}

		private static int getSpecialSegmentEndIndex(string segmentText, Dictionary<SegmentType, SpecialSegment> specialSegments, int specialSegmentStartIndex, SegmentType specialSegmentType)
		{
			int specialSegmentEndIndex;
			try
			{
				specialSegmentEndIndex =
						segmentText.IndexOf(
							specialSegments[specialSegmentType].end,
							specialSegmentStartIndex + specialSegments[specialSegmentType].start.Length
						);
			}
			catch (ArgumentOutOfRangeException)
			{
				specialSegmentEndIndex = -1;
			}

			return specialSegmentEndIndex;
		}

		private bool segmentStartsInCurrentLine()
		{
			return this.multilineSegment == SegmentType.Unkown;
		}

		private static CodeLineSegment AddCodeSegment(string segmentText, int specialSegmentStartIndex)
		{
			if (specialSegmentStartIndex == 0) return null;

			return new CodeLineSegment(
				segmentText.Substring(0, specialSegmentStartIndex),
				SegmentType.Code
			);			
		}

		private static void findFirstSpecialSegment(
			string segmentText, 
			Dictionary<SegmentType, SpecialSegment> specialSegments, 
			out int specialSegmentStartIndex, 
			out SegmentType firstSegmentType)
		{
			specialSegmentStartIndex = segmentText.Length;
			firstSegmentType = SegmentType.Unkown;
			foreach (SegmentType segmentType in specialSegments.Keys)
			{
				int thisIndex = specialSegments[segmentType].startingIndex;
				if (thisIndex < specialSegmentStartIndex && thisIndex > -1)
				{
					firstSegmentType = segmentType;
					specialSegmentStartIndex = thisIndex;
				}
			}
		}

		private void findOutIfSegmentContainsSpecialSegments(string segmentText, Dictionary<SegmentType, SpecialSegment> specialSegments)
		{
			foreach (SpecialSegment specialSegment in specialSegments.Values)
			{
				specialSegment.startingIndex = segmentText.IndexOf(specialSegment.start);
			}
		}

		private static Dictionary<SegmentType, SpecialSegment> initializeSpecialSegments()
		{
			return new Dictionary<SegmentType, SpecialSegment>
				{
					{ SegmentType.MultilineComment, new SpecialSegment("(*", "*)") },
					{ SegmentType.StringLiteralSingleQuote, new SpecialSegment("'", "'") },
					{ SegmentType.StringLiteralDoubleQuote, new SpecialSegment("\"", "\"") },
					{ SegmentType.Pragma, new SpecialSegment("{", "}") },
					{ SegmentType.EndlineComment, new SpecialSegment("//", "\n") }
				};
		}
	}
}
