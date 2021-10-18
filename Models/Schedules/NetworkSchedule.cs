using AstLab3.Models.Services.Interfaces;
using System;
using System.Collections.Generic;
using AstLab3.Models.Schedules.Exceptions;
using System.Text;
using AstLab3.ViewModels;

namespace AstLab3.Models.Schedules
{
	public class NetworkSchedule
	{
		public List<Work> Table { get; private set; }
		public List<Vertex> Vertices { get; private set; }

		public Vertex GetVertexById(int id)
		{
			foreach (var item in Vertices)
			{
				if (id == item.ID)
					return item;
			}
			return null;
		}

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

		private void CopySourceTableToWorkingTable(ICollection<Work> source, ICollection<Work> working)
		{
			foreach (Work work in source)
			{
				working.Add(work.Clone());
			}
		}

		private int SortAscending(Work first, Work second)
		{
			if (first.StartVertex > second.StartVertex) return 1;
			if (first.StartVertex < second.StartVertex) return -1;
			if (first.StartVertex == second.StartVertex)
			{
				if (first.EndVertex > second.EndVertex) return 1;
				if (first.EndVertex < second.EndVertex) return -1;
			}
			return 0;
		}

		private void RemoveRepeatedWorksFromTable(List<Work> works, ILogger logger = null)
		{
			List<Work> toRemove = new List<Work>();
			Work prevWork = null;
			foreach (Work work in works)
			{
				if (prevWork == null)
				{
					prevWork = work;
					continue;
				}
				if (prevWork.StartVertex == work.StartVertex)
				{
					if (prevWork.EndVertex == work.EndVertex)
					{
						if (prevWork.Length == work.Length)  // полное совпадение
						{
							toRemove.Add(work);
						}
						else // частичное совпадение - нужно вмешательство пользователя
						{
							throw new OverlappingWorksFoundException("Частичное совпадение - нужно вмешательство пользователя",
								prevWork,
								work);
						}
					}
				}
				prevWork = work;
			}
			foreach (var work in toRemove)
			{
				works.Remove(work);
				logger?.LogMessage($"Работа {work} удалена из-за полного повтора");
			}
		}

		private Vertex FindStartVertex(List<Work> works)
		{
			List<Vertex> vertecies = GetVerticesList(works);
			List<Vertex> startVertecies = new List<Vertex>();
			foreach (var vertex in vertecies)
			{
				if (VerterxHasNoInsideEdges(works, vertex.ID))
				{
					startVertecies.Add(vertex);
				}
			}
			if (startVertecies.Count > 1)
			{
				throw new SeveralVerticesFoundException("Найдено несколько " +
					"начальных вершин", startVertecies, EditingMode.StartVertexMode);
			}
			else if (startVertecies.Count == 0)
			{
				throw new NoVerticesFoundException("Не найдено начальных вершин", EditingMode.StartVertexMode);
			}
			return startVertecies[0];
		}

		private bool VertexHasNoOutsideEdges(List<Work> edges, int vertexID)
		{
			foreach (Work work in edges)
			{
				if (work.StartVertex.ID == vertexID)
					return false;
			}
			return true;
		}

		private bool VerterxHasNoInsideEdges(List<Work> edges, int vertexID)
		{
			foreach (Work edge in edges)
			{
				if (edge.EndVertex.ID == vertexID)
					return false;
			}
			return true;
		}
	}
}
