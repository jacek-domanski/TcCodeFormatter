using System;
using System.Collections.Generic;
using System.Xml;

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
	}
}
