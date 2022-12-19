using System;
using System.Collections.Generic;

namespace TcCodeFormatter
{
	class LineSplitter
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
			// TODO refactor this method
			// TODO handle if already in special segment

			string segmentText = this.segments[segmentIndex].Text;
			Dictionary<SegmentType, SpecialSegment> specialSegments = 
				initializeSpecialSegments();
			findSpecialSegmentsStartingIndexes(segmentText, specialSegments);

			int specialSegmentStartIndex = segmentText.Length;
			SegmentType firstSegmentType = SegmentType.Unkown;
			foreach (SegmentType segmentType in specialSegments.Keys)
			{
				int thisIndex = specialSegments[segmentType].startingIndex;
				if (thisIndex < specialSegmentStartIndex && thisIndex > -1)
				{
					firstSegmentType = segmentType;
					specialSegmentStartIndex = thisIndex;
				}
			}
			CodeLineSegment code;
			switch (firstSegmentType)
			{
				case SegmentType.Unkown:
					this.segments[segmentIndex].convertUnknownTypeToCode();
					break;

				case SegmentType.EndlineComment:
					code =
						new CodeLineSegment(
							segmentText.Substring(0, specialSegmentStartIndex),
							SegmentType.Code
						);
					CodeLineSegment comment =
						new CodeLineSegment(
							segmentText.Substring(
								specialSegmentStartIndex 
								+ specialSegments[firstSegmentType].start.Length),
							firstSegmentType
						);

					this.segments.RemoveAt(segmentIndex);
					this.segments.Insert(segmentIndex, comment);
					this.segments.Insert(segmentIndex, code);
					break;

				case SegmentType.Code:
					throw new Exception("Segment identified as code too early");
				default:
					int specialSegmentEndIndex = segmentText.IndexOf(specialSegments[firstSegmentType].start);

					code =
						new CodeLineSegment(
							segmentText.Substring(0, specialSegmentStartIndex),
							SegmentType.Code
						);

					int specialSegmentContentStartIndex =
						specialSegmentStartIndex
						+ specialSegments[firstSegmentType].start.Length;
					CodeLineSegment specialSegment, unknown = null;

					if (specialSegmentEndIndex == -1 || specialSegmentEndIndex < specialSegmentStartIndex)
					{
						specialSegment =
							new CodeLineSegment(
								segmentText.Substring(specialSegmentContentStartIndex),
								firstSegmentType
							);
						this.multilineSegment = firstSegmentType;
					} else
					{
						int specialSegmentContentLength =
							specialSegmentEndIndex
							- specialSegmentContentStartIndex;

						specialSegment =
							new CodeLineSegment(
								segmentText.Substring(
									specialSegmentContentStartIndex,
									specialSegmentContentLength),
								firstSegmentType
							);

						unknown =
							new CodeLineSegment(
								segmentText.Substring(specialSegmentEndIndex + specialSegments[firstSegmentType].end.Length - 1),
								SegmentType.Unkown
							);
					}

					this.segments.RemoveAt(segmentIndex);
					if (unknown != null) this.segments.Insert(segmentIndex, unknown); 
					this.segments.Insert(segmentIndex, specialSegment);
					this.segments.Insert(segmentIndex, code);

					break;
			}
			
	}

		private void findSpecialSegmentsStartingIndexes(string segmentText, Dictionary<SegmentType, SpecialSegment> specialSegments)
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
