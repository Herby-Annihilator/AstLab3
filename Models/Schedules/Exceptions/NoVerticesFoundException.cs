using AstLab3.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AstLab3.Models.Schedules.Exceptions
{
	public class NoVerticesFoundException : Exception
	{
		public EditingMode EditingMode { get; set; }

		public NoVerticesFoundException(string message, EditingMode editingMode) : base(message)
		{
			EditingMode = editingMode;
		}
	}
}
