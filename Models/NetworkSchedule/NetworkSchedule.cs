using System;
using System.Collections.Generic;
using System.Text;

namespace AstLab3.Models.NetworkSchedule
{
	public class NetworkSchedule
	{
		public List<Work> Table { get; private set; }
		public List<Vertex> Vertices { get; private set; }

		public NetworkSchedule(ICollection<Work> works)
		{
			Table = new List<Work>();
			foreach (var item in works)
			{
				Table.Add(item.Clone());
			}
			Vertices = GetVerticesList(Table);
		}

		private List<Vertex> GetVerticesList(List<Work> table)
		{
			List<Vertex> result = new List<Vertex>();
			foreach (var item in table)
			{
				if (!result.Contains(item.StartVertex))
					result.Add(item.StartVertex);
				if (!result.Contains(item.EndVertex))
					result.Add(item.EndVertex);
			}
			return result;
		}


	}
}
