using System;
using System.Collections.Generic;
using System.Text;

namespace AstLab3.Models.Schedules.Exceptions
{
	/// <summary>
	/// Найдены частично совпадающие работы
	/// </summary>
	public class OverlappingWorksFoundException : Exception
	{
		public Work FirstWorkToDelete { get; set; }
		public Work SecondWorkToDelete { get; set; }

		public OverlappingWorksFoundException(string message, Work firstWorkToDelete, Work secondWorkToDelete) : base(message)
		{
			FirstWorkToDelete = firstWorkToDelete;
			SecondWorkToDelete = secondWorkToDelete;
		}
	}
}
