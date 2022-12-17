using System;
using System.Collections.Generic;
using System.Xml;

namespace TcCodeFormatter
{
	class FileFormatter
	{
		private XmlDocument document = new XmlDocument();

		public FileFormatter(string filePath)
		{
			document.Load(filePath);
		}

		public void formatCode()
		{
			formatDeclarationsAndImplementations(this.document.DocumentElement);
		}

		public void save(string filePath)
		{
			document.Save(filePath);
		}

		private void formatDeclarationsAndImplementations(XmlNode node)
		{
			string declarationName = "Declaration";
			string implementationName = "Implementation";

			if (node.Name == declarationName)
			{
				DeclarationFormatter.Instance.run(node);
			} else if (node.Name == implementationName && isImplementationLanguageSt(node))
			{
				ImplementationFormatter.Instance.run(node.FirstChild);
			}

			if (node.HasChildNodes)
			{
				foreach(XmlNode child in node.ChildNodes)
				{
					formatDeclarationsAndImplementations(child);
				}
			}
		}

		private bool isImplementationLanguageSt(XmlNode node)
		{
			return node.FirstChild.Name == "ST";
		}
	}
}
