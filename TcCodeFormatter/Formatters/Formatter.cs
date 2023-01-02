using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace TcCodeFormatter
{
	public abstract class Formatter
	{
		private const string ENDLINE = "\r\n";
		private LineSplitter lineSplitter;
		private LineBuilder lineBuilder;

		public Formatter()
		{
			this.lineSplitter = new LineSplitter();
			this.lineBuilder = new LineBuilder();

			this.lineSplitter.reset();
		}
		public void run(XmlNode node)
		{
			string[] oldLines = splitNodeTextIntoLines(node);
			List<string> newLines = new List<string>();

			foreach (string oldLine in oldLines)
			{
				oldLineToNew(oldLine, newLines);
			}

			addEmptyLineAtTheEnd(newLines);

			string innerText = string.Join(ENDLINE, newLines);
			XmlCDataSection cData = node.OwnerDocument.CreateCDataSection(innerText);
			node.InnerXml = cData.OuterXml;
		}
		private string[] splitNodeTextIntoLines(XmlNode node)
		{
			return node.InnerText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None);
		}
		private void oldLineToNew(string oldLine, List<string> newLines)
		{
			List <CodeLineSegment> segments = this.lineSplitter.split(oldLine);

			segments
				.FindAll(x => x.SegmentType == SegmentType.Code)
				.ForEach(x => formatCode(x));

			this.lineBuilder.reset();
			segments.ForEach(x => lineBuilder.append(x));

			newLines.Add(this.lineBuilder.getLine());
		}
		private void formatCode(CodeLineSegment codeSegment)
		{
			if (codeSegment.SegmentType != SegmentType.Code)
			{
				throw new ArgumentException("Segment to be formatted as code is not code");
			}

			// formatting
		}
		private static void addEmptyLineAtTheEnd(List<string> newLines)
		{
			if (newLines.Last() != "")
			{
				Console.WriteLine("Added empty line at the end");
				newLines.Add("");
			}
		}
	}
}
