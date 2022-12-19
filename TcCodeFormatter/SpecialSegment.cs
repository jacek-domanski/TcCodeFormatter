using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TcCodeFormatter
{
	class SpecialSegment
	{
		public string start;
		public string end;
		public int startingIndex;

		public SpecialSegment(string start, string end)
		{
			this.start = start;
			this.end = end;
			this.startingIndex = -1;
		}
	}
}
