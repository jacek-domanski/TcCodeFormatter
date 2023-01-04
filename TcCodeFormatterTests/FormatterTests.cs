using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Runtime.Serialization;
using System.Xml;
using TcCodeFormatter;

namespace TcCodeFormatterTests
{
	[TestClass]
	public class FormatterTests
	{
		private string ENDLINE = "\r\n";
		public XmlNode linesToNode(List<string> lines)
		{
			string innerText = string.Join(ENDLINE, lines);

			XmlDocument doc = new XmlDocument();
			XmlNode node = doc.CreateNode(XmlNodeType.Element, "name", "");

			XmlCDataSection cData = node.OwnerDocument.CreateCDataSection(innerText);
			node.InnerXml = cData.OuterXml;
			return node;
		}
		[TestMethod]
		public void Should_AddEmptyLineAtTheEnd_When_NoEmptyLineAtTheEnd()
		{
			// Arrange
			List<string> lines = new List<string>();
			lines.Add("iInt := 5;");
			lines.Add("iInt := 6;");

			XmlNode node = linesToNode(lines);
			ImplementationFormatter formatter = ImplementationFormatter.Instance;

			// Act
			formatter.run(node);
			Console.WriteLine(node.InnerText);

			// Assert
			string expected = lines[0] + ENDLINE + lines[1] + ENDLINE;
			string actual = node.InnerText;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Should_NotAddEmptyLineAtTheEnd_When_EmptyLineAtTheEnd()
		{
			// Arrange
			List<string> lines = new List<string>();
			lines.Add("iInt := 5;");
			lines.Add("iInt := 6;");
			lines.Add("");

			XmlNode node = linesToNode(lines);
			ImplementationFormatter formatter = ImplementationFormatter.Instance;

			// Act
			formatter.run(node);

			// Assert
			string expected = lines[0] + ENDLINE + lines[1] + ENDLINE;
			string actual = node.InnerText;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Should_DeleteEmptyLine_When_MoreThanOneInSuccession()
		{
			// Arrange
			List<string> lines = new List<string>();
			lines.Add("iInt := 5;");
			lines.Add("");
			lines.Add("");
			lines.Add("");
			lines.Add("iInt := 6;");
			lines.Add("");

			XmlNode node = linesToNode(lines);
			ImplementationFormatter formatter = ImplementationFormatter.Instance;

			// Act
			formatter.run(node);

			// Assert
			string expected =
				lines[0] + ENDLINE
				+ lines[1] + ENDLINE
				+ lines[4] + ENDLINE;

			string actual = node.InnerText;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Should_DeleteEmptyLine_When_MoreThanOneEmptyOrWhitespaceLineInSuccession()
		{
			// Arrange
			List<string> lines = new List<string>();
			lines.Add("iInt := 5;");
			lines.Add("");
			lines.Add("\t");
			lines.Add(" ");
			lines.Add("iInt := 6;");
			lines.Add("");

			XmlNode node = linesToNode(lines);
			ImplementationFormatter formatter = ImplementationFormatter.Instance;

			// Act
			formatter.run(node);

			// Assert
			string expected =
				lines[0] + ENDLINE
				+ lines[1] + ENDLINE
				+ lines[4] + ENDLINE;

			string actual = node.InnerText;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Should_DeleteEmptyLines_When_FirstLinesAreEmptyOrWhitespaceOnly()
		{
			// Arrange
			List<string> lines = new List<string>();
			lines.Add("");
			lines.Add("\t");
			lines.Add(" ");
			lines.Add("iInt := 6;");
			lines.Add("");

			XmlNode node = linesToNode(lines);
			ImplementationFormatter formatter = ImplementationFormatter.Instance;

			// Act
			formatter.run(node);

			// Assert
			string expected =
				lines[3] + ENDLINE;

			string actual = node.InnerText;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Should_NotThrowException_When_TextIsEmpty()
		{
			// Arrange
			List<string> lines = new List<string>();
			lines.Add("");

			XmlNode node = linesToNode(lines);
			ImplementationFormatter formatter = ImplementationFormatter.Instance;

			// Act
			formatter.run(node);

			// Assert
			Assert.IsTrue(true);
		}
		[TestMethod]
		public void Should_DeleteEmptyLine_When_EmptyLineIsAfterVarInDeclaration()
		{
			// Arrange
			List<string> lines = new List<string>();
			lines.Add("VAR");
			lines.Add("\t");
			lines.Add(" ");
			lines.Add("iInt : INT;");
			lines.Add("");

			XmlNode node = linesToNode(lines);
			DeclarationFormatter formatter = DeclarationFormatter.Instance;

			// Act
			formatter.run(node);

			// Assert
			string expected =
				lines[0] + ENDLINE
				+ lines[3] + ENDLINE;

			string actual = node.InnerText;
			Assert.AreEqual(expected, actual);
		}
	}
}
