using System;
using System.Collections.Generic;
using System.Text;

namespace AstLab3.Models.Schedules
{
	public class Work
	{
		public Vertex StartVertex { get; set; }
		public Vertex EndVertex { get; set; }

		public int Length { get; set; }

		public int EarlyStartDate { get; set; }
		public int LateStartDate { get; set; }
		public int EarlyEndDate { get; set; }

		public int LateEndDate { get; set; }

		public int FullTimeReserve { get; set; }
		public int PrivateTimeReserve { get; set; }
		public int FreeTimeReserve { get; set; }
		public int IndependentTimeReserve { get; set; }

		public Work(Vertex startVertex, Vertex endVertex, int length)
		{
			StartVertex = startVertex;
			EndVertex = endVertex;
			Length = length;
		}

		public override string ToString() => $"{StartVertex.ID} → {EndVertex.ID} = {Length}";

		internal Work Clone()
		{
			Work result = new Work(StartVertex.Clone(), EndVertex.Clone(), Length);
			result.EarlyStartDate = EarlyStartDate;
			result.EarlyEndDate = EarlyEndDate;
			result.LateStartDate = LateStartDate;
			result.LateEndDate = LateEndDate;
			result.FullTimeReserve = FullTimeReserve;
			result.PrivateTimeReserve = PrivateTimeReserve;
			result.FreeTimeReserve = FreeTimeReserve;
			result.IndependentTimeReserve = IndependentTimeReserve;
			return result;
		}

		public string Self => ToString();
	}
}
