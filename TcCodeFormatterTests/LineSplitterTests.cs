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
	}
}
