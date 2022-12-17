using System;
using System.Collections.Generic;
using System.Xml;

namespace TcCodeFormatter
{
	abstract class Formatter
	{
		public void format(XmlNode node)
		{
			string[] lines = splitNodeTextIntoLines(node);

			foreach (string line in lines)
			{
				Console.WriteLine(line);
			}

			Console.WriteLine("============================================");
		}
		private string[] splitNodeTextIntoLines(XmlNode node)
		{
			return node.InnerText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
		}
	}
}
