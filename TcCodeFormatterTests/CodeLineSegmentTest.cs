using Microsoft.VisualStudio.TestTools.UnitTesting;
using TcCodeFormatter;

namespace TcCodeFormatterTests
{
	[TestClass]
	public class CodeLineSegmentTest
	{
		[TestMethod]
		public void Should_CreateCorrectObject_When_Instantiated()
		{
			// Arrange

			// Act
			CodeLineSegment segment1 =
				new CodeLineSegment("text1", SegmentType.Unkown);
			CodeLineSegment segment2 =
				new CodeLineSegment("text2", SegmentType.EndlineComment);

			// Assert
			Assert.AreEqual(
				"text1", 
				segment1.Text, 
				"Segment text changed after instantiation");
			Assert.AreEqual(
				"text2", 
				segment2.Text, 
				"Segment text changed after instantiation");

			Assert.AreEqual(
				SegmentType.Unkown, 
				segment1.SegmentType, 
				"Segment type changed after instantiation");
			Assert.AreEqual(
				SegmentType.EndlineComment, 
				segment2.SegmentType, 
				"Segment type changed after instantiation");
		}
		[TestMethod]
		public void Should_ChangeTypeToCode_When_convertUnknownTypeToCodeCalledOnUnknown()
		{
			// Arrange
			CodeLineSegment segment1 =
				new CodeLineSegment("text1", SegmentType.Unkown);

			// Act
			segment1.convertUnknownTypeToCode();

			// Assert
			Assert.AreEqual(
				SegmentType.Code,
				segment1.SegmentType,
				"Segment type not changed to code");
		}
		[TestMethod]
		public void Should_ThrowException_When_convertUnknownTypeToCodeCalledOnNotUnknown()
		{
			// Arrange
			CodeLineSegment segment1 =
				new CodeLineSegment("text1", SegmentType.Code);
			CodeLineSegment segment2 =
				new CodeLineSegment("text2", SegmentType.EndlineComment);

			// Act

			// Assert
			Assert.ThrowsException<Exception>(
				() => segment1.convertUnknownTypeToCode(),
				"Did not throw an exception");

			Assert.ThrowsException<Exception>(
				() => segment2.convertUnknownTypeToCode(), 
				"Did not throw an exception");
		}
	}	
}