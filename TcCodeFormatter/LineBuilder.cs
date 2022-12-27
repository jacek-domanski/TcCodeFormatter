using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcCodeFormatter
{
	public class LineBuilder
	{
		private string line;
		public LineBuilder()
		{
			this.line = "";
		}

		public void reset()
		{
			this.line = "";
		}

		public void append(CodeLineSegment segment)
		{

		}

		public string getLine()
		{
			return "";
		}
	}
}
