using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace AstLab3.Models.Schedules
{
	public class Vertex : IEquatable<Vertex>
	{
		public int ID { get; set; }
		public int EarlyCompletionDate { get; set; } = 0;
		public int LateCompletionDate { get; set; } = int.MaxValue;
		public int ReserveTime => LateCompletionDate - EarlyCompletionDate;

		public Vertex(int id)
		{
			ID = id;
		}

		internal Vertex Clone()
		{
			Vertex result = new Vertex(ID);
			result.EarlyCompletionDate = EarlyCompletionDate;
			result.LateCompletionDate = LateCompletionDate;
			return result;
		}

		public override bool Equals(object obj)
		{
			if (obj == null)
				return false;
			Vertex vertex = obj as Vertex;
			return Equals(vertex);
		}

		public static bool operator ==(Vertex left, Vertex right)
		{
			if (((object)left) == null || ((object)right) == null)
				return Object.Equals(left, right);

			return left.Equals(right);
		}

		public static bool operator !=(Vertex left, Vertex right)
		{
			if (((object)left) == null || ((object)right) == null)
				return !Object.Equals(left, right);

			return !(left.Equals(right));
		}

		public override int GetHashCode() => ID;

		public override string ToString() => ID.ToString();

		public bool Equals([AllowNull] Vertex other)
		{
			if (other == null)
				return false;
			return ID == other.ID;
		}

		public static bool operator <(Vertex left, Vertex right) => left.ID < right.ID;

		public static bool operator >(Vertex left, Vertex right) => left.ID > right.ID;

		public static bool operator <=(Vertex left, Vertex right) => left.ID <= right.ID;

		public static bool operator >=(Vertex left, Vertex right) => left.ID >= right.ID;
	}
}
