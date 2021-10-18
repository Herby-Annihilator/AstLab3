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

		public int EarlyStartDate => StartVertex.EarlyCompletionDate;
		public int LateStartDate => EndVertex.LateCompletionDate - Length;
		public int EarlyEndDate => StartVertex.EarlyCompletionDate + Length;

		public int LateEndDate => EndVertex.LateCompletionDate;

		public int FullTimeReserve => EndVertex.LateCompletionDate - StartVertex.EarlyCompletionDate - Length;
		public int PrivateTimeReserve { get; set; }
		public int FreeTimeReserve { get; set; }
		public int IndependentTimeReserve => EndVertex.EarlyCompletionDate - StartVertex.LateCompletionDate - Length;

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
			result.PrivateTimeReserve = PrivateTimeReserve;
			result.FreeTimeReserve = FreeTimeReserve;
			return result;
		}

		public string Self => ToString();
	}
}
