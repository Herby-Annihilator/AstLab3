﻿using AstLab3.Models.Schedules;
using AstLab3.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AstLab3.Models.Services.Data
{
	public class EditingWindowData
	{
		public List<Vertex> Vertices { get; set; }
		public EditingMode EditingMode { get; set; }
		public PreferedAction PreferedAction { get; set; }

		public EditingWindowData(List<Vertex> vertices, EditingMode editingMode, PreferedAction preferedAction = PreferedAction.AddFakeVertex)
		{
			Vertices = vertices;
			EditingMode = editingMode;
			PreferedAction = preferedAction;
		}
	}

	public enum PreferedAction
	{
		AddFakeVertex,
		DeleteVertices,
		DeleteWorks
	}
}
