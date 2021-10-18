using AstLab3.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AstLab3.Models.Schedules.Exceptions
{
	public class SeveralVerticesFoundException : Exception
	{
		public List<Vertex> Vertices { get; set; }

		public EditingMode EditingMode { get; set; }

		public SeveralVerticesFoundException(string message, List<Vertex> vertices, EditingMode editingMode) : base(message)
		{
			Vertices = vertices;
			EditingMode = editingMode;
		}
	}
}
