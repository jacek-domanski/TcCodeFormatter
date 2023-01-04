using System;
using System.Collections.Generic;
using System.Xml;

namespace TcCodeFormatter
{
	public class ImplementationFormatter : Formatter
	{
		private static ImplementationFormatter instance = null;

		private ImplementationFormatter()
		{
		}
		public static ImplementationFormatter Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new ImplementationFormatter();
				}

				return instance;
			}
		}
		protected override bool canNextLineBeEmpty(List<CodeLineSegment> segments) { return true; }
	}
}
