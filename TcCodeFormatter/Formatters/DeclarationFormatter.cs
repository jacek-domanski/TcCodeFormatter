using System;
using System.Collections.Generic;
using System.Xml;
using TcCodeFormatter.Utilities;

namespace TcCodeFormatter
{
	public class DeclarationFormatter : Formatter
	{
		private static DeclarationFormatter instance = null;

		private DeclarationFormatter()
		{
		}
		public static DeclarationFormatter Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new DeclarationFormatter();
				}

				return instance;
			}
		}
		protected override bool canNextLineBeEmpty(List<CodeLineSegment> segments)
		{ 
			foreach(CodeLineSegment segment in segments)
			{
				if (segment.SegmentType != SegmentType.Code) continue;
				if (Regexes.declarationNoEmptyLineAfter.IsMatch(segment.Text)) return false;
			}
			return true;
		}
	}
}
