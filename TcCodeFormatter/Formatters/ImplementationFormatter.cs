using System;
using System.Collections.Generic;
using System.Xml;

namespace TcCodeFormatter
{
	class ImplementationFormatter : Formatter
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
	}
}
