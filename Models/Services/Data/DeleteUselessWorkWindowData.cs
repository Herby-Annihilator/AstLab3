using AstLab3.Models.Schedules;
using System;
using System.Collections.Generic;
using System.Text;

namespace AstLab3.Models.Services.Data
{
	public class DeleteUselessWorkWindowData
	{
		public Work FirstWorkToDelete { get; set; }
		public Work SecondWorkToDelete { get; set; }

		public DeleteUselessWorkWindowData(Work firstWorkToDelete, Work secondWorkToDelete)
		{
			FirstWorkToDelete = firstWorkToDelete;
			SecondWorkToDelete = secondWorkToDelete;
		}
	}
}
