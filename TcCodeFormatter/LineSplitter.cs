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
			// TODO handle if already in special segment
			string segmentText = this.segments[segmentIndex].Text;
			Dictionary<SegmentType, SpecialSegment> specialSegments =
				initializeSpecialSegments();
			int specialSegmentStartIndex;
			SegmentType specialSegmentType;

			if (this.multilineSegment == SegmentType.Unkown)
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
			bool bEndNotFound = false;
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
					int specialSegmentEndIndex =
							segmentText.IndexOf(
								specialSegments[specialSegmentType].end, 
								specialSegmentStartIndex+1
							);

					code = AddCodeSegment(segmentText, specialSegmentStartIndex);

					int specialSegmentContentStartIndex =
						specialSegmentStartIndex
						+ specialSegments[specialSegmentType].start.Length;

					if (specialSegmentEndIndex == -1 || specialSegmentEndIndex <= specialSegmentStartIndex)
					{
						specialSegment =
							new CodeLineSegment(
								segmentText.Substring(specialSegmentContentStartIndex),
								specialSegmentType
							);
						this.multilineSegment = specialSegmentType;
						bEndNotFound = true;
					}
					else
					{
						int specialSegmentContentLength =
							specialSegmentEndIndex
							- specialSegmentContentStartIndex;

						specialSegment =
							new CodeLineSegment(
								segmentText.Substring(
									specialSegmentContentStartIndex,
									specialSegmentContentLength),
								specialSegmentType
							);

						string unknownString = segmentText.Substring(specialSegmentEndIndex + specialSegments[specialSegmentType].end.Length);
						if (unknownString.Length > 0)
						{
							unknown =
								new CodeLineSegment(
									unknownString,
									SegmentType.Unkown
								);
						}
					}
					break;
			}
			this.segments.RemoveAt(segmentIndex);
			if (unknown != null) this.segments.Insert(segmentIndex, unknown);
			this.segments.Insert(segmentIndex, specialSegment);
			if (code != null) this.segments.Insert(segmentIndex, code);
			if (!bEndNotFound) this.reset();
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
					{ SegmentType.StringLiteral, new SpecialSegment("'", "'") },
					{ SegmentType.Pragma, new SpecialSegment("{", "}") },
					{ SegmentType.EndlineComment, new SpecialSegment("//", "\n") }
				};
		}
	}
}
