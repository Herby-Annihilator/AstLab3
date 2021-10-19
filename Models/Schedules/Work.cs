using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace AstLab3.Models.Schedules
{
	public class Work : IEquatable<Work>
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

		public bool Equals([AllowNull] Work other)
		{
			if (other == null)
				return false;
			return other.StartVertex == StartVertex
				&& other.EndVertex == EndVertex
				&& other.Length == Length;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			Work work = obj as Work;
			return Equals(work);
		}

		public override int GetHashCode() => base.GetHashCode();

		public static bool operator ==(Work left, Work right)
		{
			if (((object)left) == null || ((object)right) == null)
				return Object.Equals(left, right);

			return left.Equals(right);
		}

		public static bool operator !=(Work left, Work right)
		{
			if (((object)left) == null || ((object)right) == null)
				return !Object.Equals(left, right);

			return !(left.Equals(right));
		}

		public string Self => ToString();
	}
}
