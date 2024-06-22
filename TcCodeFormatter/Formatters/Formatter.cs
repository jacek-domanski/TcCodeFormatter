using System;
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
		private bool prevAndNextLineCantBeEmpty;
		private bool nextLineCantBeEmpty;

		public Formatter()
		{
			this.lineSplitter = new LineSplitter();
			this.lineBuilder = new LineBuilder();

			this.lineSplitter.reset();
		}
		public void run(XmlNode node)
		{
			lastLineWasEmpty = false;
			prevAndNextLineCantBeEmpty = false;
			nextLineCantBeEmpty = false;

			string[] oldLines = splitNodeTextIntoLines(node);
			List<string> newLines = new List<string>();
			List<List<CodeLineSegment>> newSegments = new();

			foreach (string oldLine in oldLines)
			{
				oldLineToNew(oldLine, newLines, newSegments);
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
		private void oldLineToNew(string oldLine, List<string> newLines, List<List<CodeLineSegment>> allSegments)
		{
			List<CodeLineSegment> thisLineSegments = this.lineSplitter.split(oldLine);

			bool thisLineCantBeEmpty =
				prevAndNextLineCantBeEmpty
				|| nextLineCantBeEmpty
				|| lastLineWasEmpty
				|| isThisLineFirst(newLines);

			if (isThisLineEmpty(thisLineSegments))
			{
				if (thisLineCantBeEmpty)
				{
					Functions.printIfVerbose("Removed empty line");
					return;
				}
				lastLineWasEmpty = true;
			}
			else lastLineWasEmpty = false;

			thisLineSegments
				.FindAll(x => x.SegmentType == SegmentType.Code)
				.ForEach(x => formatCode(x));

			prevAndNextLineCantBeEmpty = !canPrevOrNextLineBeEmpty(thisLineSegments);
			nextLineCantBeEmpty = !canNextLineBeEmpty(thisLineSegments);
			removePreviousLineIfEmpty(newLines, allSegments, thisLineSegments);

			allSegments.Add(thisLineSegments);
			this.lineBuilder.reset();
			thisLineSegments.ForEach(x => lineBuilder.append(x));

			newLines.Add(this.lineBuilder.getLine());
		}
		private static bool isThisLineFirst(List<string> newLines)
		{
			return newLines.Count == 0;
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
		protected virtual void formatCode(CodeLineSegment codeSegment)
		{
			if (codeSegment.SegmentType != SegmentType.Code)
			{
				throw new ArgumentException("Segment to be formatted as code is not code");
			}

			codeSegment.Text = codeSegment.Text.Replace(":=", " := ");
			codeSegment.Text = Regexes.referenceAssignment.Replace(codeSegment.Text, " REF= ");
			codeSegment.Text = Regexes.equalButNotSimilar.Replace(codeSegment.Text, " = ");

			codeSegment.Text = codeSegment.Text.Replace(">=", " >= ");
			codeSegment.Text = Regexes.lesserButNotSimilar.Replace(codeSegment.Text, " < ");

			codeSegment.Text = codeSegment.Text.Replace("<=", " <= ");
			codeSegment.Text = Regexes.greaterButNotSimilar.Replace(codeSegment.Text, " > ");

			codeSegment.Text = codeSegment.Text.Replace("<>", " <> ");
			codeSegment.Text = Regexes.commaMidline.Replace(codeSegment.Text, ", ");

			if (codeSegment.IsFirstSegmentInLine)
			{
				codeSegment.Text = Regexes.whitespacesNotAtTheStart.Replace(codeSegment.Text, " ");
			}
			else
			{
				codeSegment.Text = Regexes.whitespaces.Replace(codeSegment.Text, " ");
			}

			codeSegment.Text = Regexes.whitespacesBeforeSemicolon.Replace(codeSegment.Text, "");
		}
		protected abstract bool canPrevOrNextLineBeEmpty(List<CodeLineSegment> segments);

		private bool canNextLineBeEmpty(List<CodeLineSegment> segments)
		{
			CodeLineSegment lastCodeSegment = null;

			foreach (CodeLineSegment segment in segments)
			{
				if (segment.SegmentType == SegmentType.Code) lastCodeSegment = segment;
			}

			if (lastCodeSegment != null)
			{
				return !Regexes.endsWithOpeningBracket.IsMatch(lastCodeSegment.Text);
			}
			return true;
		}
		private static void addEmptyLineAtTheEnd(List<string> newLines)
		{
			if (newLines.Count == 0 || newLines.Last() != "")
			{
				Functions.printIfVerbose("Added empty line at the end");
				newLines.Add("");
			}
		}
		private void removePreviousLineIfEmpty(List<string> newLines, List<List<CodeLineSegment>> allSegments, List<CodeLineSegment> thisLineSegments)
		{
			bool prevLineCantBeEmpty = prevAndNextLineCantBeEmpty || !canPrevLineBeEmpty(thisLineSegments);
			while (prevLineCantBeEmpty && newLines.Count > 0 && Regexes.emptyOrWhitespaceOnly.IsMatch(newLines.Last()))
			{
				newLines.RemoveAt(newLines.Count - 1);
				allSegments.RemoveAt(newLines.Count - 1);
				Functions.printIfVerbose("Removed empty line before keyword");
			}
		}
		private bool canPrevLineBeEmpty(List<CodeLineSegment> segments)
		{
			if (segments[0].SegmentType != SegmentType.Code) return true;

			return !Regexes.startsWithClosingBracket.IsMatch(segments[0].Text);
		}
	}
}
