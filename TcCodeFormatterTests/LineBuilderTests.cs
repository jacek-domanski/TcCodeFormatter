using Microsoft.VisualStudio.TestTools.UnitTesting;
using TcCodeFormatter;

namespace TcCodeFormatterTests
{
	[TestClass]
	public class LineBuilderTests
	{
		[TestMethod]
		public void Should_ResetInnerStringToEmpty_When_ResetCalled()
		{
			// Arrange
			LineBuilder lineBuilder = new LineBuilder();
			var lineField = lineBuilder
				.GetType()
				.GetField(
					"line",
					System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			lineField.SetValue(lineBuilder, "iInt := 5;");

			// Act
			lineBuilder.reset();

			// Assert
			Assert.AreEqual("", lineField.GetValue(lineBuilder), "Line not reset to empty");
		}
		[TestMethod]
		public void Should_ReturnLineContent_When_getLineCalled()
		{
			// Arrange
			string expected = "iInt := 5;";

			LineBuilder lineBuilder = new LineBuilder();
			var lineField = lineBuilder
				.GetType()
				.GetField(
					"line",
					System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
			lineField.SetValue(lineBuilder, expected);

			// Act
			string actual = lineBuilder.getLine();

			// Assert
			Assert.AreEqual(expected, actual, "inner string changed");
		}
		[TestMethod]
		public void Should_ReturnAppendedString_When_OneCodeSegmentAppended()
		{
			// Arrange
			string expected = "iInt := 5;";
			LineBuilder lineBuilder = new LineBuilder();
			lineBuilder.append(new CodeLineSegment(expected, SegmentType.Code));

			// Act
			string actual = lineBuilder.getLine();

			// Assert
			Assert.AreEqual(expected, actual, "Appended string modified");
		}
		[TestMethod]
		public void Should_ReturnLineWithOneMultilineComment_When_OneMultilineCommentSegmentAppended()
		{
			// Arrange
			string input = "multiline comment";
			LineBuilder lineBuilder = new LineBuilder();
			lineBuilder.append(new CodeLineSegment(input, SegmentType.MultilineComment));

			// Act
			string actual = lineBuilder.getLine();

			// Assert
			string expected = "(* " + input + " *)";
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Should_ReturnLineWithOneEndlineComment_When_OneEndlineCommentSegmentAppended()
		{
			// Arrange
			string input = "endline comment";
			LineBuilder lineBuilder = new LineBuilder();
			lineBuilder.append(new CodeLineSegment(input, SegmentType.EndlineComment));

			// Act
			string actual = lineBuilder.getLine();

			// Assert
			string expected = "// " + input;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Should_ReturnLineWithOneStringLiteralSingleQuote_When_OneStringLiteralSingleQuoteSegmentAppended()
		{
			// Arrange
			string input = "StringLiteralSingleQuote";
			LineBuilder lineBuilder = new LineBuilder();
			lineBuilder.append(new CodeLineSegment(input, SegmentType.StringLiteralSingleQuote));

			// Act
			string actual = lineBuilder.getLine();

			// Assert
			string expected = "'" + input + "'";
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Should_ReturnLineWithOneStringLiteralDoubleQuote_When_OneStringLiteralDoubleQuoteSegmentAppended()
		{
			// Arrange
			string input = "StringLiteralDoubleQuote";
			LineBuilder lineBuilder = new LineBuilder();
			lineBuilder.append(new CodeLineSegment(input, SegmentType.StringLiteralDoubleQuote));

			// Act
			string actual = lineBuilder.getLine();

			// Assert
			string expected = "\"" + input + "\"";
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Should_ReturnLineWithOnePragma_When_OnePragmaAppended()
		{
			// Arrange
			string input = "Pragma";
			LineBuilder lineBuilder = new LineBuilder();
			lineBuilder.append(new CodeLineSegment(input, SegmentType.Pragma));

			// Act
			string actual = lineBuilder.getLine();

			// Assert
			string expected = "{" + input + "}";
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Should_AddSpacesAroundMultilineCommentStart_When_CodeAndMultilineCommentAppended()
		{
			// Arrange
			string input1 = "iInt := 5;";
			string input2 = "multiline comment";

			LineBuilder lineBuilder = new LineBuilder();

			lineBuilder.append(new CodeLineSegment(input1, SegmentType.Code));
			lineBuilder.append(new CodeLineSegment(input2, SegmentType.MultilineComment));

			// Act
			string actual = lineBuilder.getLine();

			// Assert
			string expected = input1 + " (* " + input2 + " *)";
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Should_AddSpacesAroundMultilineCommentEnd_When_MultilineCommentAndCodeAppended()
		{
			// Arrange
			string input1 = "multiline comment";
			string input2 = "iInt := 5;";

			LineBuilder lineBuilder = new LineBuilder();

			lineBuilder.append(new CodeLineSegment(input1, SegmentType.MultilineComment));
			lineBuilder.append(new CodeLineSegment(input2, SegmentType.Code));

			// Act
			string actual = lineBuilder.getLine();

			// Assert
			string expected = "(* " + input1 + " *) " + input2;
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Should_AddNoSpacesAroundMultilineCommentStart_When_TextIsEmpty()
		{
			// Arrange
			LineBuilder lineBuilder = new LineBuilder();
			lineBuilder.append(new CodeLineSegment("", SegmentType.MultilineComment, true, false));

			// Act
			string actual = lineBuilder.getLine();

			// Assert
			string expected = "(*";
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Should_AddNoSpacesAroundMultilineCommentContent_When_TextIsEmpty()
		{
			// Arrange
			LineBuilder lineBuilder = new LineBuilder();
			lineBuilder.append(new CodeLineSegment("", SegmentType.MultilineComment, false, false));

			// Act
			string actual = lineBuilder.getLine();

			// Assert
			string expected = "";
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Should_AddNoSpacesAroundMultilineCommentEnd_When_TextIsEmpty()
		{
			// Arrange
			LineBuilder lineBuilder = new LineBuilder();
			lineBuilder.append(new CodeLineSegment("", SegmentType.MultilineComment, false, true));

			// Act
			string actual = lineBuilder.getLine();

			// Assert
			string expected = "*)";
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Should_PutOnlyOneSpaceBetweenMarkerAndEndlineCommentText_When_EndlineCommentTextStartsWithSpace()
		{
			// Arrange
			LineBuilder lineBuilder = new LineBuilder();
			lineBuilder.append(new CodeLineSegment(" text", SegmentType.EndlineComment, false, false));

			// Act
			string actual = lineBuilder.getLine();

			// Assert
			string expected = "// text";
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Should_NotPutSpaceAfterQuotationMark_When_CodeIsNext()
		{
			// Arrange
			LineBuilder lineBuilder = new LineBuilder();
			lineBuilder.append(new CodeLineSegment("a", SegmentType.StringLiteralSingleQuote));
			lineBuilder.append(new CodeLineSegment(";", SegmentType.Code));

			// Act
			string actual = lineBuilder.getLine();

			// Assert
			string expected = "'a';";
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Should_AddNoSpace_When_OpeningBracketIsFollowedByStringLiteral()
		{
			// Arrange
			LineBuilder lineBuilder = new LineBuilder();
			lineBuilder.append(new CodeLineSegment("str.append(", SegmentType.Code));
			lineBuilder.append(new CodeLineSegment("_", SegmentType.StringLiteralSingleQuote));
			lineBuilder.append(new CodeLineSegment(");", SegmentType.Code));

			// Act
			string actual = lineBuilder.getLine();

			// Assert
			string expected = "str.append('_');";
			Assert.AreEqual(expected, actual);
		}
		[TestMethod]
		public void Should_AddNoSpace_When_MultilineCommentAlreadyStartsOrEndsWithSpace()
		{
			// Arrange
			LineBuilder lineBuilder = new LineBuilder();
			lineBuilder.append(new CodeLineSegment("iInt := 5;", SegmentType.Code));
			lineBuilder.append(new CodeLineSegment(" comment ", SegmentType.MultilineComment));

			// Act
			string actual = lineBuilder.getLine();

			// Assert
			string expected = "iInt := 5; (* comment *)";
			Assert.AreEqual(expected, actual);
		}
	}
}
