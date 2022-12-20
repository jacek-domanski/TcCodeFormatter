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
			Assert.AreEqual(segment1.Text, "text1", "Segment text changed after instantiation");
		}
	}
}