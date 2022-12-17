using System;
using System.Collections.Generic;
using System.Xml;

namespace TcCodeFormatter
{
	abstract class Formatter
	{
		public void format(XmlNode node)
		{
			string[] oldLines = splitNodeTextIntoLines(node);
			List<string> newLines = new List<string>();

			foreach (string oldLine in oldLines)
			{
				newLines.Add(oldLine + " // dupa");
				Console.WriteLine(newLines[newLines.Count-1]);
			}

			node.InnerText = string.Join("\n", newLines);

			Console.WriteLine("============================================");
		}
		private string[] splitNodeTextIntoLines(XmlNode node)
		{
			return node.InnerText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
		}
	}
}
