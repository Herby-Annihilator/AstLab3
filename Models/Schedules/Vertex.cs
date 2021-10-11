using System;
using System.Collections.Generic;
using System.Text;

namespace AstLab3.Models.Schedules
{
	public class Vertex
	{
		public int ID { get; set; }
		public int EarlyCompletionDate { get; set; }
		public int LateCompletionDate { get; set; }
		public int ReserveTime { get; set; }

		public Vertex(int id)
		{
			ID = id;
		}

		internal Vertex Clone()
		{
			Vertex result = new Vertex(ID);
			result.EarlyCompletionDate = EarlyCompletionDate;
			result.LateCompletionDate = LateCompletionDate;
			result.ReserveTime = ReserveTime;
			return result;
		}
	}
}
