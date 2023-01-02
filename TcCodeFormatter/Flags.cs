using System;
using System.Collections.Generic;
using System.Xml;

namespace TcCodeFormatter
{
	class Flags 
	{
		private static Flags instance = null;
		private bool verbose;

		private Flags()
		{
		}
		public static Flags Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new Flags();
				}

				return instance;
			}
		}

		public bool Verbose { get => verbose; }

		public void setFlags(Options options)
		{
			verbose = options.Verbose;
		}
	}
}
