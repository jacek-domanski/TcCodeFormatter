using Microsoft.VisualStudio.TestTools.UnitTesting;
using TcCodeFormatter;
using Moq;

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
			multilineSegmentField.SetValue(lineSplitter, SegmentType.StringLiteral);

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
			string code1 = "(*iInt := 5; 'string' {pragma} // endline*)";
			string code2 = "'iInt := 5; (* comment *) {pragma} // endline'";
			string code3 = "{iInt := 5; (* comment *) 'string' // endline}";
			string code4 = "//iInt := 5; (* comment *) 'string' {pragma}";
			LineSplitter lineSplitter = new LineSplitter();

			// Act
			List<CodeLineSegment> list1 = lineSplitter.split(code1);
			List<CodeLineSegment> list2 = lineSplitter.split(code2);
			List<CodeLineSegment> list3 = lineSplitter.split(code3);
			List<CodeLineSegment> list4 = lineSplitter.split(code4);

			// Assert
			Assert.AreEqual(1, list1.Count, "list 1 is not length 1");
			Assert.AreEqual(1, list2.Count, "list 2 is not length 1");
			Assert.AreEqual(1, list3.Count, "list 3 is not length 1");
			Assert.AreEqual(1, list4.Count, "list 4 is not length 1");

			Assert.AreEqual(SegmentType.MultilineComment, list1[0].SegmentType, "code 1 segment type is not multiline comment");
			Assert.AreEqual(SegmentType.StringLiteral, list2[0].SegmentType, "code 2 segment type is not multiline comment");
			Assert.AreEqual(SegmentType.Pragma, list3[0].SegmentType, "code 3 segment type is not multiline comment");
			Assert.AreEqual(SegmentType.EndlineComment, list4[0].SegmentType, "code 4 segment type is not multiline comment");

			Assert.AreEqual("iInt := 5; 'string' {pragma} // endline", list1[0].Text, "code 1 differs from input");
			Assert.AreEqual("iInt := 5; (* comment *) {pragma} // endline", list2[0].Text, "code 2 differs from input");
			Assert.AreEqual("iInt := 5; (* comment *) 'string' // endline", list3[0].Text, "code 3 differs from input");
			Assert.AreEqual("iInt := 5; (* comment *) 'string' {pragma}", list4[0].Text, "code 4 differs from input");
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
			Assert.AreEqual(SegmentType.StringLiteral, list1[1].SegmentType, "code 1 segment 1 type is not StringLiteral");
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
	}
}
