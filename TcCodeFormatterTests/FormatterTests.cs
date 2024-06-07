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
		[TestMethod]
		public void Should_DeleteEmptyLine_When_EmptyLineIsBeforeEndVarInDeclaration()
		{
			// Arrange
			List<string> lines = new List<string>();
			lines.Add("iInt : INT;");
			lines.Add("\t");
			lines.Add(" ");
			lines.Add("END_VAR");
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
		[TestMethod]
		public void Should_AddSpacesAroundAssignmentOperator_When_NoSpacesAround()
		{
			// Arrange
			List<string> lines = new List<string>();
			lines.Add("iInt:=5;");
			lines.Add("iInt :=5;");
			lines.Add("iInt:= 5;");

			XmlNode node = linesToNode(lines);
			ImplementationFormatter formatter = ImplementationFormatter.Instance;

			// Act
			formatter.run(node);

			// Assert
			string expected =
				"iInt := 5;" + ENDLINE
				+ "iInt := 5;" + ENDLINE
				+ "iInt := 5;" + ENDLINE;

			string actual = node.InnerText;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Should_RemoveExtraWhitespacesAroundAssignmentOperator_When_MoreThanOneSpaceEachSide()
		{
			// Arrange
			List<string> lines = new List<string>();
			lines.Add("iInt		 := 5;");
			lines.Add("iInt :=		5;");
			lines.Add("iInt	 	 :=	  5;");
			lines.Add("iInt						 := 5;");
			lines.Add("iInt :=						5;");

			XmlNode node = linesToNode(lines);
			ImplementationFormatter formatter = ImplementationFormatter.Instance;

			// Act
			formatter.run(node);

			// Assert
			string expected =
				"iInt := 5;" + ENDLINE
				+ "iInt := 5;" + ENDLINE
				+ "iInt := 5;" + ENDLINE
				+ "iInt := 5;" + ENDLINE
				+ "iInt := 5;" + ENDLINE;

			string actual = node.InnerText;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Should_NotAddSpaceAfterAssignmentOperator_When_AssignmentOperatorIsLast()
		{
			// Arrange
			List<string> lines = new List<string>();
			lines.Add("iInt :=");
			lines.Add("5;");
			lines.Add("iInt := // comment");
			lines.Add("5;");

			XmlNode node = linesToNode(lines);
			ImplementationFormatter formatter = ImplementationFormatter.Instance;

			// Act
			formatter.run(node);

			// Assert
			string expected =
				"iInt :=" + ENDLINE
				+ "5;" + ENDLINE
				+ "iInt := // comment" + ENDLINE
				+ "5;" + ENDLINE;

			string actual = node.InnerText;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Should_DeleteEmptyLine_When_EmptyLineIsAfterOpeningBracket()
		{
			// Arrange
			List<string> lines = new List<string>();
			lines.Add("function(");
			lines.Add("\t");
			lines.Add(" ");
			lines.Add("5");
			lines.Add(");");
			lines.Add("");

			XmlNode node = linesToNode(lines);
			DeclarationFormatter formatter = DeclarationFormatter.Instance;

			// Act
			formatter.run(node);

			// Assert
			string expected =
				lines[0] + ENDLINE
				+ lines[3] + ENDLINE
				+ lines[4] + ENDLINE;

			string actual = node.InnerText;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Should_DeleteEmptyLine_When_EmptyLineIsBeforeClosingBracket()
		{
			// Arrange
			List<string> lines = new List<string>();
			lines.Add("function(");
			lines.Add("5");
			lines.Add("\t");
			lines.Add(" ");
			lines.Add(");");
			lines.Add("");

			XmlNode node = linesToNode(lines);
			DeclarationFormatter formatter = DeclarationFormatter.Instance;

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
		public void Should_RemoveWhitespaces_When_WhitespacesBeforeSemicolon()
		{
			// Arrange
			List<string> lines = new List<string>();
			lines.Add("iInt := 5 ;");
			lines.Add("iInt := 6 		;");
			lines.Add("");

			XmlNode node = linesToNode(lines);
			ImplementationFormatter formatter = ImplementationFormatter.Instance;

			// Act
			formatter.run(node);

			// Assert
			string expected = "iInt := 5;" + ENDLINE + "iInt := 6;" + ENDLINE;
			string actual = node.InnerText;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Should_NotRemoveWhitespaces_When_WhitespacesBeforeSemicolonAtTheStartOfLine()
		{
			// Arrange
			List<string> lines = new List<string>();
			lines.Add("     ;");
			lines.Add("");

			XmlNode node = linesToNode(lines);
			ImplementationFormatter formatter = ImplementationFormatter.Instance;

			// Act
			formatter.run(node);

			// Assert
			string expected = lines[0] + ENDLINE;
			string actual = node.InnerText;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Should_RemoveWhitespaces_When_TooManyWhitespacesAroundColon()
		{
			// Arrange
			List<string> lines = new List<string>();
			lines.Add("bBool  : BOOL;");
			lines.Add("bBool   : BOOL;");
			lines.Add("bBool : BOOL;");
			lines.Add("bBool :  BOOL;");
			lines.Add("bBool :   BOOL;");
			lines.Add("bBool   :   BOOL;");
			lines.Add("");

			XmlNode node = linesToNode(lines);
			DeclarationFormatter formatter = DeclarationFormatter.Instance;

			// Act
			formatter.run(node);

			// Assert
			string expected =
				"bBool : BOOL;" + ENDLINE
				+ "bBool : BOOL;" + ENDLINE
				+ "bBool : BOOL;" + ENDLINE
				+ "bBool : BOOL;" + ENDLINE
				+ "bBool : BOOL;" + ENDLINE
				+ "bBool : BOOL;" + ENDLINE;

			string actual = node.InnerText;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Should_AddWhitespaces_When_MissingWhitespacesAroundColon()
		{
			// Arrange
			List<string> lines = new List<string>();
			lines.Add("bBool: BOOL;");
			lines.Add("bBool :BOOL;");
			lines.Add("bBool:BOOL;");
			lines.Add("");

			XmlNode node = linesToNode(lines);
			DeclarationFormatter formatter = DeclarationFormatter.Instance;

			// Act
			formatter.run(node);

			// Assert
			string expected =
				"bBool : BOOL;" + ENDLINE
				+ "bBool : BOOL;" + ENDLINE
				+ "bBool : BOOL;" + ENDLINE;

			string actual = node.InnerText;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Should_NotAddWhitespaces_When_ColonInAssignmentOperatorFound()
		{
			// Arrange
			List<string> lines = new List<string>();
			lines.Add("bBool : BOOL := TRUE;");
			lines.Add("");

			XmlNode node = linesToNode(lines);
			DeclarationFormatter formatter = DeclarationFormatter.Instance;

			// Act
			formatter.run(node);

			// Assert
			string expected = "bBool : BOOL := TRUE;" + ENDLINE;

			string actual = node.InnerText;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Should_NotAddWhitespacesBeforeColon_When_CodeStartsWithColon()
		{
			// Arrange
			List<string> lines = new List<string>();
			lines.Add(":BOOL;");
			lines.Add(" :BOOL;");
			lines.Add("  :BOOL;");
			lines.Add("");

			XmlNode node = linesToNode(lines);
			DeclarationFormatter formatter = DeclarationFormatter.Instance;

			// Act
			formatter.run(node);

			// Assert
			string expected =
				": BOOL;" + ENDLINE
				+ " : BOOL;" + ENDLINE
				+ "  : BOOL;" + ENDLINE;

			string actual = node.InnerText;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Should_NotAddWhitespacesAfterColon_When_CodeEndsWithColon()
		{
			// Arrange
			List<string> lines = new List<string>();
			lines.Add("bBool :");
			lines.Add("bBool : ");
			lines.Add("bBool :  ");
			lines.Add("");

			XmlNode node = linesToNode(lines);
			DeclarationFormatter formatter = DeclarationFormatter.Instance;

			// Act
			formatter.run(node);

			// Assert
			string expected =
				"bBool :" + ENDLINE
				+ "bBool :" + ENDLINE
				+ "bBool :" + ENDLINE;

			string actual = node.InnerText;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Should_RemoveRedundantSpaces_When_RedundantSpacesBetweenInlineCommentAndClosingBracket()
		{
			// Arrange
			List<string> lines = new List<string>();
			lines.Add("sTmpPath := LEFT(sTmpPath, iSlashPlace (* -1 *)         );");
			lines.Add("");

			XmlNode node = linesToNode(lines);
			DeclarationFormatter formatter = DeclarationFormatter.Instance;

			// Act
			formatter.run(node);

			// Assert
			string expected = "sTmpPath := LEFT(sTmpPath, iSlashPlace (* -1 *) );" + ENDLINE;

			string actual = node.InnerText;
			Assert.AreEqual(expected, actual);
		}
	}
}
