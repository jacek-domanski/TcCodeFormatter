using System;
using System.Collections.Generic;
using System.Xml;

namespace TcCodeFormatter
{
	class Program
	{
		static void Main(string[] args)
		{
			string[] filesPaths = new[] { "C:\\Projects\\CSharp\\TcCodeFormatter\\POUs\\FB_StringBuilder.TcPOU" };

			FileFormatter fileFormatter = null;

			foreach (string filePath in filesPaths)
			{
				fileFormatter = new FileFormatter(filePath);
				fileFormatter.formatCode();
				fileFormatter.save(filePath + "2");
			}
		}
	}
}
