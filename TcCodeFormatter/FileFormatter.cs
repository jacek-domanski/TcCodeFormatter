using System;
using System.Collections.Generic;
using System.Xml;

namespace TcCodeFormatter
{
	class FileFormatter
	{
		private XmlDocument document = new XmlDocument();
		List<XmlNode> declarations = new List<XmlNode>();
		List<XmlNode> implementations = new List<XmlNode>();

		public FileFormatter(string filePath)
		{
			document.Load(filePath);
		}

		public void formatCode()
		{
			addDeclarationsAndImplementations(this.document.DocumentElement);

			Console.WriteLine("Declarations count: " + this.declarations.Count);

			foreach (XmlNode declaration in this.declarations)
			{
				string[] lines = splitNodeTextIntoLines(declaration);

				foreach (string line in lines)
				{
					Console.WriteLine(line);
				}

				Console.WriteLine("============================================");
			}

			Console.WriteLine("Implementations count: " + this.implementations.Count);

			foreach (XmlNode implementation in this.implementations)
			{
				string[] lines = splitNodeTextIntoLines(implementation);
				foreach (string line in lines)
				{
					Console.WriteLine(line);
				}

				Console.WriteLine("============================================");
			}
		}

		private void addDeclarationsAndImplementations(XmlNode node)
		{
			string declarationName = "Declaration";
			string implementationName = "Implementation";

			if (node.Name == declarationName)
			{
				this.declarations.Add(node);
			} else if (node.Name == implementationName && node.FirstChild.Name == "ST")
			{
				this.implementations.Add(node.FirstChild);
			}

			if (node.HasChildNodes)
			{
				foreach(XmlNode child in node.ChildNodes)
				{
					addDeclarationsAndImplementations(child);
				}
			}
		}

		private string[] splitNodeTextIntoLines(XmlNode node)
		{
			return node.InnerText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
		}
	}
}
