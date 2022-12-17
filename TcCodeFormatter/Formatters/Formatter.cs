using System;
using System.Collections.Generic;
using System.Xml;

namespace TcCodeFormatter
{
	abstract class Formatter
	{
		private const string ENDLINE = "\n";
		public void run(XmlNode node)
		{
			string[] oldLines = splitNodeTextIntoLines(node);
			List<string> newLines = new List<string>();
			foreach (string oldLine in oldLines)
			{
				newLines.Add(oldLine + " // dupa");
				Console.WriteLine(newLines[newLines.Count-1]);
			}
			string innerText = string.Join(ENDLINE, newLines);
			XmlCDataSection cData = node.OwnerDocument.CreateCDataSection(innerText);
			node.InnerXml = cData.OuterXml;

			Console.WriteLine("============================================");
		}
		private string[] splitNodeTextIntoLines(XmlNode node)
		{
			return node.InnerText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
		}
	}
}
