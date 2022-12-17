using System;
using System.Collections.Generic;
using System.Xml;

namespace TcCodeFormatter
{
	class Program
	{
		static List<XmlNode> declarations;
		static List<XmlNode> implementations;
		static void Main(string[] args)
		{
			string filePath = new string("C:\\Projects\\CSharp\\TcCodeFormatter\\POUs\\FB_StringBuilder.TcPOU");

			FileFormatter fileFormatter = new FileFormatter(filePath);

			fileFormatter.formatCode();


		}
	}
}
