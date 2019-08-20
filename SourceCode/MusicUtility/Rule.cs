using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MusicUtility
{
	public class Rule
	{
		public object Subject { get; set; }

		public Condition Condition { get; set; }

		public object Conditional { get; set; }

		public Operations Operation { get; set; }
	}
}
