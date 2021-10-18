using AstLab3.Models.Schedules;
using System;
using System.Collections.Generic;
using System.Text;

namespace AstLab3.Models.Services.Data
{
	public class DeleteWorksInCycleWindowData
	{
		public List<Work> WorksInCycle { get; set; }

		public DeleteWorksInCycleWindowData(List<Work> worksInCycle)
		{
			WorksInCycle = worksInCycle;
		}
	}
}
