using Microsoft.VisualStudio.TestTools.UnitTesting;
using TcCodeFormatter;

namespace TcCodeFormatterTests
{
	[TestClass]
	public class LineSplitterTests
	{
		[TestMethod]
		public void Should_setMultilineSegmentToUnknown_When_resetCalled()
		{
			// Arrange
			LineSplitter lineSplitter = new LineSplitter();
			var multilineSegmentField = lineSplitter
				.GetType()
				.GetField(
					"multilineSegment",
					System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			multilineSegmentField.SetValue(lineSplitter, SegmentType.StringLiteralSingleQuote);

			// Act
			lineSplitter.reset();

			// Assert
			Assert.AreEqual(SegmentType.Unkown, multilineSegmentField.GetValue(lineSplitter), "Segment type not reset to unknown");
		}
		[TestMethod]
		public void Should_returnListWithOneCodeLineSegment_When_splitCalledOnJustCode()
		{
			// Arrange
			string code1 = "\tlrLreal : LREAL := 12.3;";
			string code2 = "THIS^.append(F_DateStringReversed(ePrecision := 3, actTime := timestruct));";

			LineSplitter lineSplitter = new LineSplitter();

			// Act
			List<CodeLineSegment> list1 = lineSplitter.split(code1);
			List<CodeLineSegment> list2 = lineSplitter.split(code2);

			// Assert
			Assert.AreEqual(1, list1.Count, "list 1 is not length 1");
			Assert.AreEqual(SegmentType.Code, list1[0].SegmentType, "code 1 segment type is not code");
			Assert.AreEqual(code1, list1[0].Text, "code 1 differs from input");

			Assert.AreEqual(1, list2.Count, "list 2 is not length 1");
			Assert.AreEqual(SegmentType.Code, list2[0].SegmentType, "code 2 segment type is not code");
			Assert.AreEqual(code2, list2[0].Text, "code 2 differs from input");
		}
		[TestMethod]
		public void Should_returnListWithTwoElements_When_splitCalledOnCodeWithEndlineComment()
		{
			// Arrange
			string code1 = "\tlrLreal : LREAL := 12.3; // comment";
			LineSplitter lineSplitter = new LineSplitter();

			// Act
			List<CodeLineSegment> list1 = lineSplitter.split(code1);

			// Assert
			Assert.AreEqual(2, list1.Count, "list 1 is not length 2");
			Assert.AreEqual(SegmentType.Code, list1[0].SegmentType, "code 1 1st segment type is not code");
			Assert.AreEqual("\tlrLreal : LREAL := 12.3; ", list1[0].Text, "code 1 1st segment differs from input");
			Assert.AreEqual(SegmentType.EndlineComment, list1[1].SegmentType, "code 1 2nd segment type is not endline comment");
			Assert.AreEqual(" comment", list1[1].Text, "code 1 2nd segment differs from input");

		}
		[TestMethod]
		public void Should_returnListWithOneElement_When_splitCalledOnSpecialSegmentWithOtherSegmentsInside()
		{
			// Arrange
			string code1 = "(*iInt := 5; 'string' {pragma} \"string2\" // endline*)";
			string code2 = "'iInt := 5; (* comment *) {pragma} \"string2\" // endline'";
			string code3 = "{iInt := 5; (* comment *) 'string' \"string2\" // endline}";
			string code4 = "//iInt := 5; (* comment *) 'string' {pragma} \"string2\"";
			string code5 = "\"iInt := 5; (* comment *) 'string' {pragma} // endline\"";
			LineSplitter lineSplitter = new LineSplitter();

			// Act
			List<CodeLineSegment> list1 = lineSplitter.split(code1);
			List<CodeLineSegment> list2 = lineSplitter.split(code2);
			List<CodeLineSegment> list3 = lineSplitter.split(code3);
			List<CodeLineSegment> list4 = lineSplitter.split(code4);
			List<CodeLineSegment> list5 = lineSplitter.split(code5);

			// Assert
			Assert.AreEqual(1, list1.Count, "list 1 is not length 1");
			Assert.AreEqual(1, list2.Count, "list 2 is not length 1");
			Assert.AreEqual(1, list3.Count, "list 3 is not length 1");
			Assert.AreEqual(1, list4.Count, "list 4 is not length 1");
			Assert.AreEqual(1, list5.Count, "list 5 is not length 1");

			Assert.AreEqual(SegmentType.MultilineComment, list1[0].SegmentType, "code 1 segment type is not MultilineComment");
			Assert.AreEqual(SegmentType.StringLiteralSingleQuote, list2[0].SegmentType, "code 2 segment type is not StringLiteralSingleQuote");
			Assert.AreEqual(SegmentType.Pragma, list3[0].SegmentType, "code 3 segment type is not Pragma");
			Assert.AreEqual(SegmentType.EndlineComment, list4[0].SegmentType, "code 4 segment type is not EndlineComment");
			Assert.AreEqual(SegmentType.StringLiteralDoubleQuote, list5[0].SegmentType, "code 5 segment type is not StringLiteralDoubleQuote");

			Assert.AreEqual("iInt := 5; 'string' {pragma} \"string2\" // endline", list1[0].Text, "code 1 differs from input");
			Assert.AreEqual("iInt := 5; (* comment *) {pragma} \"string2\" // endline", list2[0].Text, "code 2 differs from input");
			Assert.AreEqual("iInt := 5; (* comment *) 'string' \"string2\" // endline", list3[0].Text, "code 3 differs from input");
			Assert.AreEqual("iInt := 5; (* comment *) 'string' {pragma} \"string2\"", list4[0].Text, "code 4 differs from input");
			Assert.AreEqual("iInt := 5; (* comment *) 'string' {pragma} // endline", list5[0].Text, "code 5 differs from input");
		}
		[TestMethod]
		public void Should_returnListWithThreeElements_When_splitCalledOnCodeWithSpecialSegmentInside()
		{
			// Arrange
			string code1 = "sString := 'text';";
			string code2 = "iInt := (* comment *) 5;";
			LineSplitter lineSplitter = new LineSplitter();

			// Act
			List<CodeLineSegment> list1 = lineSplitter.split(code1);
			List<CodeLineSegment> list2 = lineSplitter.split(code2);

			// Assert
			Assert.AreEqual(3, list1.Count, "list 1 is not length 3");
			Assert.AreEqual(3, list2.Count, "list 2 is not length 3");

			Assert.AreEqual(SegmentType.Code, list1[0].SegmentType, "code 1 segment 0 type is not code");
			Assert.AreEqual(SegmentType.StringLiteralSingleQuote, list1[1].SegmentType, "code 1 segment 1 type is not StringLiteral");
			Assert.AreEqual(SegmentType.Code, list1[2].SegmentType, "code 1 segment 2 type is not code");

			Assert.AreEqual(SegmentType.Code, list2[0].SegmentType, "code 2 segment type is not code");
			Assert.AreEqual(SegmentType.MultilineComment, list2[1].SegmentType, "code 2 segment type is not multiline comment");
			Assert.AreEqual(SegmentType.Code, list2[2].SegmentType, "code 2 segment type is not code");

			Assert.AreEqual("sString := ", list1[0].Text, "code 1 segment 0 differs from input");
			Assert.AreEqual("text", list1[1].Text, "code 1 segment 1 differs from input");
			Assert.AreEqual(";", list1[2].Text, "code 1 segment 2 differs from input");

			Assert.AreEqual("iInt := ", list2[0].Text, "code 2 segment 0 differs from input");
			Assert.AreEqual(" comment ", list2[1].Text, "code 2 segment 1 differs from input");
			Assert.AreEqual(" 5;", list2[2].Text, "code 2 segment 2 differs from input");
		}
		[TestMethod]
		public void Should_setMultilineSegmentToCorrectSegmentType_When_SpecialSegmentEndNotFound()
		{
			// Arrange
			string code1 = "iInt := 5; (* comment starts";
			string code2 = "comment continues";
			string code3 = "comment ends *)";

			LineSplitter lineSplitter = new LineSplitter();
			var multilineSegmentField = lineSplitter
				.GetType()
				.GetField(
					"multilineSegment",
					System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

			// Act
			List<CodeLineSegment> list1 = lineSplitter.split(code1);
			// Assert
			Assert.AreEqual(2, list1.Count, "list 1 is not length 2");

			Assert.AreEqual(SegmentType.Code, list1[0].SegmentType, "code 1 segment 0 type is not code");
			Assert.AreEqual(SegmentType.MultilineComment, list1[1].SegmentType, "code 1 segment 1 type is not MultilineComment");

			Assert.AreEqual("iInt := 5; ", list1[0].Text, "code 1 segment 0 differs from input");
			Assert.AreEqual(" comment starts", list1[1].Text, "code 1 segment 0 differs from input");

			Assert.AreEqual(SegmentType.MultilineComment, multilineSegmentField.GetValue(lineSplitter), "Segment type not set to MultilineComment");

			Assert.IsTrue(list1[1].HasStartMarker);
			Assert.IsFalse(list1[1].HasEndMarker);

			// Act
			List<CodeLineSegment> list2 = lineSplitter.split(code2);
			// Assert
			Assert.AreEqual(1, list2.Count, "list 2 is not length 1");
			Assert.AreEqual(SegmentType.MultilineComment, list2[0].SegmentType, "code 2 segment 0 type is not MultilineComment");
			Assert.AreEqual("comment continues", list2[0].Text, "code 2 segment 0 differs from input");
			Assert.AreEqual(SegmentType.MultilineComment, multilineSegmentField.GetValue(lineSplitter), "Segment type not set to MultilineComment");

			Assert.IsFalse(list2[0].HasStartMarker);
			Assert.IsFalse(list2[0].HasEndMarker);

			// Act
			List<CodeLineSegment> list3 = lineSplitter.split(code3);
			// Assert
			Assert.AreEqual(1, list3.Count, "list 3 is not length 1");
			Assert.AreEqual(SegmentType.MultilineComment, list3[0].SegmentType, "code 3 segment 0 type is not MultilineComment");
			Assert.AreEqual("comment ends ", list3[0].Text, "code 3 segment 0 differs from input");
			Assert.AreEqual(SegmentType.Unkown, multilineSegmentField.GetValue(lineSplitter), "Segment type not reset to unknown");

			Assert.IsFalse(list3[0].HasStartMarker);
			Assert.IsTrue(list3[0].HasEndMarker);
		}
		[TestMethod]
		public void Should_CorrectlyDetectMultilineSegment_When_splitCalledOnMultilineSegmentWithNoContent()
		{
			// Arrange
			string code1 = "(*";
			string code2 = "";
			string code3 = "*)";

			LineSplitter lineSplitter = new LineSplitter();

			// Act
			List<CodeLineSegment> list1 = lineSplitter.split(code1);
			List<CodeLineSegment> list2 = lineSplitter.split(code2);
			List<CodeLineSegment> list3 = lineSplitter.split(code3);

			// Assert
			Assert.AreEqual(1, list1.Count);
			Assert.AreEqual(SegmentType.MultilineComment, list1[0].SegmentType);
			Assert.AreEqual("", list1[0].Text);
			Assert.IsTrue(list1[0].HasStartMarker);
			Assert.IsFalse(list1[0].HasEndMarker);

			Assert.AreEqual(1, list2.Count);
			Assert.AreEqual(SegmentType.MultilineComment, list2[0].SegmentType);
			Assert.AreEqual("", list2[0].Text);
			Assert.IsFalse(list2[0].HasStartMarker);
			Assert.IsFalse(list2[0].HasEndMarker);

			Assert.AreEqual(1, list3.Count);
			Assert.AreEqual(SegmentType.MultilineComment, list3[0].SegmentType);
			Assert.AreEqual("", list3[0].Text);
			Assert.IsFalse(list3[0].HasStartMarker);
			Assert.IsTrue(list3[0].HasEndMarker);
		}
		[TestMethod]
		public void Should_returnListWithThreeElements_When_splitCalledOnCodeWithSpecialSegmentWithEscapeCharactersInside()
		{
			// Arrange
			string code1 = "sString := 'a$'b$'c';";
			string code2 = "sString := \"a$\"b$\"c\";";
			LineSplitter lineSplitter = new LineSplitter();

			// Act
			List<CodeLineSegment> list1 = lineSplitter.split(code1);
			List<CodeLineSegment> list2 = lineSplitter.split(code2);

			// Assert
			Assert.AreEqual(3, list1.Count, "list 1 is not length 3");
			Assert.AreEqual(3, list2.Count, "list 2 is not length 3");

			Assert.AreEqual(SegmentType.Code, list1[0].SegmentType, "code 1 segment 0 type is not code");
			Assert.AreEqual(SegmentType.StringLiteralSingleQuote, list1[1].SegmentType, "code 1 segment 1 type is not StringLiteralSingleQuote");
			Assert.AreEqual(SegmentType.Code, list1[2].SegmentType, "code 1 segment 2 type is not code");

			Assert.AreEqual(SegmentType.Code, list2[0].SegmentType, "code 2 segment type is not code");
			Assert.AreEqual(SegmentType.StringLiteralDoubleQuote, list2[1].SegmentType, "code 2 segment type is not StringLiteralDoubleQuote");
			Assert.AreEqual(SegmentType.Code, list2[2].SegmentType, "code 2 segment type is not code");

			Assert.AreEqual("sString := ", list1[0].Text, "code 1 segment 0 differs from input");
			Assert.AreEqual("a$'b$'c", list1[1].Text, "code 1 segment 1 differs from input");
			Assert.AreEqual(";", list1[2].Text, "code 1 segment 2 differs from input");

			Assert.AreEqual("sString := ", list2[0].Text, "code 2 segment 0 differs from input");
			Assert.AreEqual("a$\"b$\"c", list2[1].Text, "code 2 segment 1 differs from input");
			Assert.AreEqual(";", list2[2].Text, "code 2 segment 2 differs from input");
		}
		[TestMethod]
		public void Should_MarkFirstAndLastSegment_When_OnlyOneSegment()
		{
			// Arrange
			string line = "sTmpPath := LEFT(sTmpPath, iSlashPlace );";
			LineSplitter lineSplitter = new LineSplitter();

			// Act
			List<CodeLineSegment> segments = lineSplitter.split(line);

			// Assert
			Assert.IsTrue(segments[0].IsFirstSegmentInLine);
			Assert.IsTrue(segments[0].IsLastSegmentInLine);
		}
		[TestMethod]
		public void Should_MarkFirstAndLastSegment_When_TwoSegments()
		{
			// Arrange
			string line = "sTmpPath := LEFT(sTmpPath, iSlashPlace ); // comment";
			LineSplitter lineSplitter = new LineSplitter();

			// Act
			List<CodeLineSegment> segments = lineSplitter.split(line);

			// Assert
			Assert.IsTrue(segments[0].IsFirstSegmentInLine);
			Assert.IsFalse(segments[0].IsLastSegmentInLine);

			Assert.IsFalse(segments[1].IsFirstSegmentInLine);
			Assert.IsTrue(segments[1].IsLastSegmentInLine);
		}
		[TestMethod]
		public void Should_MarkFirstAndLastSegment_When_ThreeSegments()
		{
			// Arrange
			string line = "sTmpPath := LEFT(sTmpPath, iSlashPlace (* -1 *) );";
			LineSplitter lineSplitter = new LineSplitter();

			// Act
			List<CodeLineSegment> segments = lineSplitter.split(line);

			// Assert
			Assert.IsTrue(segments[0].IsFirstSegmentInLine);
			Assert.IsFalse(segments[0].IsLastSegmentInLine);

			Assert.IsFalse(segments[1].IsFirstSegmentInLine);
			Assert.IsFalse(segments[1].IsLastSegmentInLine);

			Assert.IsFalse(segments[2].IsFirstSegmentInLine);
			Assert.IsTrue(segments[2].IsLastSegmentInLine);
		}
	}
}
