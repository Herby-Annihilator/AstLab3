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
		public List<Vertex> Vertices { get => GetVerticesList(Table); }

		public Vertex GetVertexById(int id)
		{
			var vertices = GetVerticesList(Table);
			foreach (var item in vertices)
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
				CreateWorkInTable(item.StartVertex.ID, item.EndVertex.ID, item.Length);
			}
		}

		public void CreateWorkInTable(int startVertexID, int endVertexID, int length)
		{
			Vertex startVertex, endVertex;
			Work work;
			if ((startVertex = GetVertexById(startVertexID)) == null)
			{
				startVertex = new Vertex(startVertexID);
			}
			if ((endVertex = GetVertexById(endVertexID)) == null)
			{
				endVertex = new Vertex(endVertexID);
			}
			work = new Work(startVertex, endVertex, length);
			Table.Add(work);
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
			Vertex startVertex, endVertex;
			Work work;
			bool startVertexContains, endVertexContains;
			List<Vertex> tmp = new List<Vertex>();
			foreach (Work item in source)
			{
				startVertexContains = false;
				endVertexContains = false;
				startVertex = item.StartVertex.Clone();
				endVertex = item.EndVertex.Clone();
				foreach (var vertex in tmp)
				{
					if (vertex.ID == startVertex.ID)
					{
						startVertex = vertex;
						startVertexContains = true;
						break;
					}
				}
				if (!startVertexContains)
					tmp.Add(startVertex);
				foreach (var vertex in tmp)
				{
					if (vertex.ID == endVertex.ID)
					{
						endVertex = vertex;
						endVertexContains = true;
						break;
					}
				}
				if (!endVertexContains)
					tmp.Add(endVertex);
				work = new Work(startVertex, endVertex, item.Length);
				working.Add(work);
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

		private Vertex FindEndVertex(List<Work> source)
		{
			List<Vertex> vertices = GetVerticesList(source);
			List<Vertex> endVertices = new List<Vertex>();
			foreach (var vertex in vertices)
			{
				if (VertexHasNoOutsideEdges(source, vertex.ID))
					endVertices.Add(vertex);
			}
			if (endVertices.Count > 1)
				throw new SeveralVerticesFoundException("Найдено несколько конечных вершин", endVertices, EditingMode.EndVertexMode);
			else if (endVertices.Count == 0)
				throw new NoVerticesFoundException("Конечных вершин не найдено", EditingMode.EndVertexMode);
			return endVertices[0];
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

		private void DFS(List<Work> table, int blockStartIndex, int currentVertex, int endVertex, List<int> currentPath,
			Action<List<int>, int> action)
		{
			currentPath.Add(currentVertex);
			if (currentVertex == endVertex)
			{
				action?.Invoke(currentPath, currentVertex);
				currentPath.Remove(currentVertex);
				return;
			}
			int blockEndIndex = blockStartIndex + 1;
			for (; blockEndIndex < table.Count; blockEndIndex++) // поиск конца блока работ
			{
				if (table[blockEndIndex].StartVertex.ID != table[blockStartIndex].StartVertex.ID)
				{
					break;
				}
			}
			int nextBlockIndex;
			for (int i = blockStartIndex; i < blockEndIndex && i < table.Count; i++) // обход блока работ
			{
				// найти блок, к которому принадлежит следующее событие
				for (nextBlockIndex = 0; nextBlockIndex < table.Count; nextBlockIndex++)
				{
					if (table[nextBlockIndex].StartVertex.ID == table[i].EndVertex.ID)
						break;
				}
				DFS(table, nextBlockIndex, table[i].EndVertex.ID, endVertex, currentPath, action);
			}
			currentPath.Remove(currentVertex);
			return;
		}

		private List<Work> Streamline(List<Work> source, int startVertex)
		{
			List<int> processedVerticies = new List<int>();
			List<Work> works = new List<Work>();
			CopySourceTableToWorkingTable(source, works);
			//
			// найти начальную вершину в таблице и переместить все работы в начало
			//
			MoveStartWorksToTheBegining(works, startVertex);
			List<Work> result = new List<Work>();
			Queue<int> toProcess = new Queue<int>(source.Count / 2);
			toProcess.Enqueue(works[0].StartVertex.ID);
			int currentVertex;
			List<Work> toRemove = new List<Work>();
			while (toProcess.Count > 0)
			{
				currentVertex = toProcess.Dequeue();
				foreach (Work work in works)
				{
					if (work.StartVertex.ID == currentVertex)
					{
						if (!processedVerticies.Contains(work.EndVertex.ID))
						{
							toProcess.Enqueue(work.EndVertex.ID);
						}
						result.Add(work);
						toRemove.Add(work);
					}
				}
				foreach (var item in toRemove)
				{
					works.Remove(item);
				}
				toRemove.Clear();
			}
			return result;
		}

		private void MoveStartWorksToTheBegining(List<Work> source, int startVertex)
		{
			List<Work> buffer = new List<Work>();
			foreach (Work work in source)
			{
				if (work.StartVertex.ID == startVertex)
				{
					buffer.Add(work);
				}
			}
			foreach (Work work in buffer)
			{
				source.Remove(work);
			}
			source.InsertRange(0, buffer);
		}

		private void RemoveEdgesWithSpecifiedVertex(List<Work> works, int vertexID)
		{
			List<Work> toRemove = new List<Work>();
			foreach (Work work in works)
			{
				if (work.StartVertex.ID == vertexID || work.EndVertex.ID == vertexID)
					toRemove.Add(work);
			}
			foreach (Work work in toRemove)
			{
				works.Remove(work);
			}
		}

		private void FindCycles(List<Work> works)
		{
			List<int> vertecies = new List<int>();
			foreach (Work work in works)
			{
				if (!vertecies.Contains(work.StartVertex.ID))
					vertecies.Add(work.StartVertex.ID);
				if (!vertecies.Contains(work.EndVertex.ID))
					vertecies.Add(work.EndVertex.ID);
			}
			List<Work> edges = new List<Work>();
			CopySourceTableToWorkingTable(works, edges);
			int currentVertex;
			int currentVertexIndex = 0;
			while (currentVertexIndex < vertecies.Count)
			{
				currentVertex = vertecies[currentVertexIndex];
				currentVertexIndex++;
				if (VertexHasNoOutsideEdges(edges, currentVertex)
					|| VerterxHasNoInsideEdges(edges, currentVertex))
				{
					RemoveEdgesWithSpecifiedVertex(edges, currentVertex);
					vertecies.Remove(currentVertex);
					currentVertexIndex = 0;
				}
			}
			if (edges.Count > 0)
				throw new CyclesFoundException("Найден(ы) цикл(ы)", edges);
		}

		private void RemoveLoops(List<Work> works, ILogger logger = null)
		{
			List<Work> toRemove = new List<Work>();
			foreach (Work work in works)
			{
				if (work.StartVertex.ID == work.EndVertex.ID)
				{
					toRemove.Add(work);
				}
			}
			foreach (Work work in toRemove)
			{
				works.Remove(work);
				logger?.LogMessage($"Петля {work.StartVertex.ID} → {work.EndVertex.ID} удалена");
			}
		}

		public void Streamline(ILogger logger = null)
		{
			Vertex startVertex;
			Vertex endVertex;

			Table.Sort(SortAscending);
			RemoveLoops(Table, logger);
			RemoveRepeatedWorksFromTable(Table, logger);
			startVertex = FindStartVertex(Table);
			endVertex = FindEndVertex(Table);
			FindCycles(Table);
			Table = Streamline(Table, startVertex.ID);
		}

		public void FindAllPaths(Action<List<int>, int> toDoWithEqualVertices, List<int> currentPath, ILogger logger = null)
		{
			Vertex startVertex;
			Vertex endVertex;

			Table.Sort(SortAscending);
			RemoveLoops(Table, logger);
			RemoveRepeatedWorksFromTable(Table, logger);
			startVertex = FindStartVertex(Table);
			endVertex = FindEndVertex(Table);
			FindCycles(Table);
			Table = Streamline(Table, startVertex.ID);
			DFS(Table, 0, startVertex.ID, endVertex.ID, currentPath, toDoWithEqualVertices);
		}

		#region CalculateVerticesParameters
		public void CalculateVerticesParameters()
		{
			VerticesParametersToDefault();
			FirstStep();
			SecondStep();
			if (Table[0].StartVertex.LateCompletionDate != 0)
				throw new Exception("Позднее время наступления начального события != 0");
		}
		private void VerticesParametersToDefault()
		{
			foreach (var item in Table)
			{
				item.StartVertex.EarlyCompletionDate = 0;
				item.StartVertex.LateCompletionDate = int.MaxValue;
				item.EndVertex.EarlyCompletionDate = 0;
				item.EndVertex.LateCompletionDate = int.MaxValue;
			}
		}
		private void FirstStep()
		{
			Table[0].StartVertex.EarlyCompletionDate = 0;
			List<Vertex> processedVertices = new List<Vertex>();
			processedVertices.Add(Table[0].StartVertex);
			int earlyCompletionDate;
			foreach (Work work in Table)
			{
				if (!processedVertices.Contains(work.EndVertex))
				{
					processedVertices.Add(work.EndVertex);
				}
				earlyCompletionDate = work.StartVertex.EarlyCompletionDate + work.Length;
				if (work.EndVertex.EarlyCompletionDate < earlyCompletionDate)
				{
					work.EndVertex.EarlyCompletionDate = earlyCompletionDate;
					UpdateProcessedVertices(processedVertices, work.EndVertex);
				}
			}
		}
		private void UpdateProcessedVertices(List<Vertex> processedVertices, Vertex previousVertex)
		{
			if (!processedVertices.Contains(previousVertex))
				return;
			foreach (var item in Table)
			{
				if (item.StartVertex == previousVertex)
				{
					if (processedVertices.Contains(item.EndVertex))
					{
						if (item.EndVertex.EarlyCompletionDate < previousVertex.EarlyCompletionDate + item.Length)
						{
							item.EndVertex.EarlyCompletionDate = previousVertex.EarlyCompletionDate + item.Length;
							UpdateProcessedVertices(processedVertices, item.EndVertex);
						}
					}
				}
			}
		}
		private void SecondStep()
		{
			int lastIndex = Table.Count - 1, lateCompletionDate;
			Table[lastIndex].EndVertex.LateCompletionDate = Table[lastIndex].EndVertex.EarlyCompletionDate;
			List<Vertex> processedVertices = new List<Vertex>();
			processedVertices.Add(Table[lastIndex].EndVertex);
			for (int i = lastIndex; i >= 0; i--)
			{
				if (!processedVertices.Contains(Table[i].StartVertex))
				{
					processedVertices.Add(Table[i].StartVertex);
				}
				lateCompletionDate = Table[i].EndVertex.LateCompletionDate - Table[i].Length;
				if (Table[i].StartVertex.LateCompletionDate > lateCompletionDate)
				{
					Table[i].StartVertex.LateCompletionDate = lateCompletionDate;
					UpdateProcessedVerticesOnSecondStep(processedVertices, Table[i].StartVertex);
				}
			}
		}
		private void UpdateProcessedVerticesOnSecondStep(List<Vertex> processedVertices, Vertex endVertex)
		{
			if (!processedVertices.Contains(endVertex))
				return;
			foreach (var item in Table)
			{
				if (item.EndVertex == endVertex)
				{
					if (processedVertices.Contains(item.StartVertex))
					{
						if (item.StartVertex.LateCompletionDate > endVertex.LateCompletionDate - item.Length)
						{
							item.StartVertex.LateCompletionDate = endVertex.LateCompletionDate - item.Length;
							UpdateProcessedVerticesOnSecondStep(processedVertices, item.StartVertex);
						}
					}
				}
			}
		}
		#endregion

		public List<Work> GetCritalcWorks(bool isNetworkScheduleStreamlined = true)
		{
			List<Work> result = new List<Work>();
			if (!isNetworkScheduleStreamlined)
				Streamline();
			foreach (var item in Table)
			{
				if (item.FullTimeReserve == 0)
					result.Add(item);
			}
			return result;
		}
	}
}
