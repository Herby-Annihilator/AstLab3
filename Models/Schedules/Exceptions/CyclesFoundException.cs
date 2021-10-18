using System;
using System.Collections.Generic;
using System.Text;

namespace AstLab3.Models.Schedules.Exceptions
{
	public class CyclesFoundException : Exception
	{
		public List<Work> WorksInCycle { get; set; }

		public CyclesFoundException(string message, List<Work> worksInCycle) : base(message)
		{
			WorksInCycle = worksInCycle;
		}
	}
}
