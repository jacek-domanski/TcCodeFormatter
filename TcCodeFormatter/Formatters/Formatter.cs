﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Text.RegularExpressions;
using TcCodeFormatter.Utilities;

namespace TcCodeFormatter
{
	public abstract class Formatter
	{
		private const string ENDLINE = "\r\n";
		private LineSplitter lineSplitter;
		private LineBuilder lineBuilder;

		private bool lastLineWasEmpty;

		public Formatter()
		{
			this.lineSplitter = new LineSplitter();
			this.lineBuilder = new LineBuilder();

			this.lineSplitter.reset();
		}
		public void run(XmlNode node)
		{
			lastLineWasEmpty = false;
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
			List<CodeLineSegment> segments = this.lineSplitter.split(oldLine);

			if (isThisLineEmpty(segments))
			{
				if (lastLineWasEmpty || newLines.Count == 0) return;
				lastLineWasEmpty = true;
			}
			else
			{
				lastLineWasEmpty = false;
			}

			segments
				.FindAll(x => x.SegmentType == SegmentType.Code)
				.ForEach(x => formatCode(x));

			this.lineBuilder.reset();
			segments.ForEach(x => lineBuilder.append(x));

			newLines.Add(this.lineBuilder.getLine());
		}

		private static bool isThisLineEmpty(List<CodeLineSegment> segments)
		{
			bool textIsEmptyOrWhitespace = 
				segments[0].Text == "" || Regexes.whitespaceOnly.IsMatch(segments[0].Text);

			return 
				segments.Count == 1 
				&& segments[0].SegmentType == SegmentType.Code 
				&& textIsEmptyOrWhitespace;
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
