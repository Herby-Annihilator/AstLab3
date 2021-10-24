using AstLab3.Models.Schedules;
using System;
using System.Collections.Generic;
using System.Text;

namespace AstLab3.Models.Services.Data
{
	public class ChangeWorkWindowData
	{
		public Work WorkToChange { get; set; }

		public ChangeWorkWindowData(Work workToChange)
		{
			WorkToChange = workToChange;
		}
	}
}
