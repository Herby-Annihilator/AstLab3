using AstLab3.Models.Services.Interfaces;
using AstLab3.ViewModels.Base;
using System;
using System.Collections.Generic;
using AstLab3.Models.Schedules;
using System.Text;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace AstLab3.ViewModels
{
	public class EditWindowViewModel : ViewModel
	{
		private Exchanger _exchanger;

		private AdjacencyList _adjacencyList;

		public EditingMode EditingMode { get; set; } = EditingMode.StartVertexMode;

		private ILogger _logger;
		private NetworkSchedule _networkSchedule;

		public EditWindowViewModel(NetworkSchedule networkSchedule, ILogger logger)
		{
			_networkSchedule = networkSchedule;
			_logger = logger;
			PrepareVerticesDeleting();
			PrepareEdgesDeleting();
			ChoiceOfActionCommand = new LambdaCommand(OnChoiceOfActionCommandExecuted,
				CanChoiceOfActionCommandExecute);
			AddFakeVertexCommand = new LambdaCommand(OnAddFakeVertexCommandExecuted,
				CanAddFakeVertexCommandExecute);
			DeleteVerticesCommand = new LambdaCommand(OnDeleteVerticesCommandExecuted,
				CanDeleteVerticesCommandExecute);
			DeleteEdgesCommand = new LambdaCommand(OnDeleteEdgesCommandExecuted,
				CanDeleteEdgesCommandExecute);
			AcceptCommand = new LambdaCommand(OnAcceptCommandExecuted,
				CanAcceptCommandExecute);
		}

		#region Properties

		private string _title = "Редактироване вершин";
		public string Title { get => _title; set => Set(ref _title, value); }

		private string _status = "Редактироване вершин";
		public string Status { get => _status; set => Set(ref _status, value); }

		private string _meaningLine = "Редактироване вершин";
		public string MeaningLine { get => _meaningLine; set => Set(ref _meaningLine, value); }

		private string _fakeVertexIndex;
		public string FakeVertexIndex
		{
			get => _fakeVertexIndex;
			set => Set(ref _fakeVertexIndex, value);
		}

		private string _indexesToConnectWithFakeVertex;
		public string IndexesToConnectWithFakeVertex
		{
			get => _indexesToConnectWithFakeVertex;
			set => Set(ref _indexesToConnectWithFakeVertex, value);
		}

		private bool _addingFakeVertexIsNecessary = true;
		public bool AddingFakeVertexIsNecessary
		{
			get => _addingFakeVertexIsNecessary;
			set => Set(ref _addingFakeVertexIsNecessary, value);
		}

		private bool _deletingVerticesIsNecessary = false;
		private List<int> _deletedVertices = new List<int>();
		public bool DeletingVerticesIsNecessary
		{
			get => _deletingVerticesIsNecessary;
			set => Set(ref _deletingVerticesIsNecessary, value);
		}

		private bool _deletingEdgesIsNecessary = false;
		public bool DeletingEdgesIsNecessary
		{
			get => _deletingEdgesIsNecessary;
			set => Set(ref _deletingEdgesIsNecessary, value);
		}

		private List<Work> _createdWorks = new List<Work>();

		public ObservableCollection<int> VerticesCanBeDeleted { get; set; } =
			new ObservableCollection<int>();

		private bool _deletingCommandExecute = false;
		private int _selectedVertexToDelete;
		public int SelectedVertexToDelete
		{
			get => _selectedVertexToDelete;
			set
			{
				Set(ref _selectedVertexToDelete, value);
				AdjacencyEdgesWillBeDeleted.Clear();
				_deletingCommandExecute = false;
				List<Work> works;
				if (!_adjacencyList.Edges.TryGetValue(_selectedVertexToDelete, out works))
					return;
				_deletingCommandExecute = true;
				foreach (Work work in works)
				{
					AdjacencyEdgesWillBeDeleted.Add(work);
				}
			}
		}

		public ObservableCollection<Work> AdjacencyEdgesWillBeDeleted { get; set; } =
			new ObservableCollection<Work>();

		public ObservableCollection<Work> WorksThatCanBeDeleted { get; set; } =
			new ObservableCollection<Work>();

		public Work SelectedWorkToDelete { get; set; }

		private List<Work> _deletedWorks = new List<Work>();

		#endregion

		#region Commands

		#region ChoiceOfActionCommand
		public ICommand ChoiceOfActionCommand { get; }
		private void OnChoiceOfActionCommandExecuted(object p)
		{
			AddingFakeVertexIsNecessary = false;
			DeletingEdgesIsNecessary = false;
			DeletingVerticesIsNecessary = false;
			if ((string)p == "add fake")
				AddingFakeVertexIsNecessary = true;
			else if ((string)p == "delete edges")
				DeletingEdgesIsNecessary = true;
			else
				DeletingVerticesIsNecessary = true;
		}
		private bool CanChoiceOfActionCommandExecute(object p) => true;
		#endregion

		#region AddFakeVertexCommand
		public ICommand AddFakeVertexCommand { get; }
		private void OnAddFakeVertexCommandExecuted(object p)
		{
			try
			{
				int fakeIndex = Convert.ToInt32(FakeVertexIndex);
				foreach (int vertex in _exchanger.Vertices)
				{
					if (vertex == fakeIndex)
						throw new Exception($"Вершина {vertex} уже существует");
				}
				string[] buffer = IndexesToConnectWithFakeVertex.Split(' ',
					StringSplitOptions.RemoveEmptyEntries);
				int[] indexesToConnect = new int[buffer.Length];
				for (int i = 0; i < buffer.Length; i++)
				{
					indexesToConnect[i] = Convert.ToInt32(buffer[i]);
				}
				ValidateIndexesToConnect(indexesToConnect, fakeIndex);
				Connect(fakeIndex, indexesToConnect);
				Status = "Вершина создана. Для того, чтобы изменения вступили в силу, нажмите 'ОК'";
			}
			catch (Exception e)
			{
				Status = e.Message;
			}
		}
		private bool CanAddFakeVertexCommandExecute(object p) => true;
		#endregion

		#region DeleteVerticesCommand
		public ICommand DeleteVerticesCommand { get; }
		private void OnDeleteVerticesCommandExecuted(object p)
		{
			try
			{
				_deletedVertices.Add(SelectedVertexToDelete);
				VerticesCanBeDeleted.Remove(SelectedVertexToDelete);
				AdjacencyEdgesWillBeDeleted.Clear();
				_deletingCommandExecute = false;
				Status = $"Вершина {SelectedVertexToDelete} удалена. Чтобы изменения вступили в силу, нажмите 'OK'";
			}
			catch (Exception e)
			{
				Status = e.Message;
			}
		}
		private bool CanDeleteVerticesCommandExecute(object p) => _deletingCommandExecute;
		#endregion

		#region DeleteEdgesCommand
		public ICommand DeleteEdgesCommand { get; }
		private void OnDeleteEdgesCommandExecuted(object p)
		{
			try
			{
				_deletedWorks.Add(SelectedWorkToDelete);
				WorksThatCanBeDeleted.Remove(SelectedWorkToDelete);
				Status = $"Работа {SelectedWorkToDelete} удалена";
				SelectedWorkToDelete = null;
			}
			catch (Exception e)
			{
				Status = e.Message;
			}
		}
		private bool CanDeleteEdgesCommandExecute(object p) => SelectedWorkToDelete != null;
		#endregion

		#region AcceptCommand  
		public ICommand AcceptCommand { get; } // будут приняты только те изменения, которые 
											   // редактируются в данный момент, т.е. на 
											   // которые указывают флаги
		private void OnAcceptCommandExecuted(object p)
		{
			try
			{
				List<Work> toRemove = new List<Work>();
				if (AddingFakeVertexIsNecessary)
				{
					if (EditingMode == EditingMode.StartVertexMode)
						_exchanger.CurrentTable.InsertRange(0, _createdWorks);
					else
						_exchanger.CurrentTable.AddRange(_createdWorks);
				}
				else if (DeletingVerticesIsNecessary)
				{

					foreach (int vertex in _deletedVertices)
					{
						foreach (Work work in _exchanger.CurrentTable)
						{
							if (work.FirstEventID == vertex || work.SecondEventID == vertex)
								toRemove.Add(work);
						}
					}
				}
				else if (DeletingEdgesIsNecessary)
				{
					foreach (Work work in _deletedWorks)
					{
						toRemove.Add(work);
					}
				}
				foreach (var work in toRemove)
				{
					_exchanger.CurrentTable.Remove(work);
				}
				Status = "Изменения применены.";
				App.ActivedWindow.Close();
			}
			catch (Exception e)
			{
				Status = e.Message;
			}
		}
		private bool CanAcceptCommandExecute(object p) => true;
		#endregion

		#endregion

		private void ValidateIndexesToConnect(int[] indexes, int fakeVertexIndex)
		{
			Array.Sort(indexes);
			if (indexes[0] == fakeVertexIndex)
				throw new Exception($"Попытка соединить врешину {fakeVertexIndex} и" +
					$" {indexes[0]} недопустима");
			if (!_exchanger.Vertices.Contains(indexes[0]))
				throw new Exception($"Вершина {indexes[0]} не существует " +
					$"в исходной таблице");
			for (int i = 0; i < indexes.Length - 1; i++)
			{
				if (indexes[i] == indexes[i + 1])
					throw new Exception($"Повторение вершин в списке для соединения: " +
						$"{indexes[i]} и {indexes[i + 1]}");
				else if (indexes[i + 1] == fakeVertexIndex)
					throw new Exception($"Попытка соединить врешину {fakeVertexIndex} и" +
						$" {indexes[i + 1]} недопустима");
				else if (!_exchanger.Vertices.Contains(indexes[i + 1]))
					throw new Exception($"Вершина {indexes[i + 1]} не существует " +
						$"в исходной таблице");
			}
		}

		private void Connect(int fakeVertexIndex, int[] indexes)
		{
			_createdWorks.Clear();
			if (EditingMode == EditingMode.StartVertexMode)
			{
				for (int i = 0; i < indexes.Length; i++)
				{
					_createdWorks.Add(new Work(fakeVertexIndex, indexes[i], 0));
				}
			}
			else
			{
				for (int i = 0; i < indexes.Length; i++)
				{
					_createdWorks.Add(new Work(indexes[i], fakeVertexIndex, 0));
				}
			}
		}

		private void PrepareVerticesDeleting()
		{
			VerticesCanBeDeleted.Clear();
			AdjacencyEdgesWillBeDeleted.Clear();
			_adjacencyList = new AdjacencyList();
			List<Work> works;
			foreach (var vertex in _exchanger.Vertices)
			{
				VerticesCanBeDeleted.Add(vertex);
				_adjacencyList.Vertices.Add(vertex);
				works = new List<Work>();
				foreach (Work work in _exchanger.CurrentTable)
				{
					if (work.FirstEventID == vertex || work.SecondEventID == vertex)
						works.Add(work);
				}
				_adjacencyList.Edges.Add(vertex, works);
			}

			SelectedVertexToDelete = VerticesCanBeDeleted.Count > 0 ? VerticesCanBeDeleted[0] :
				0;
		}

		private void PrepareEdgesDeleting()
		{
			WorksThatCanBeDeleted.Clear();
			foreach (int vertex in _exchanger.Vertices)
			{
				foreach (Work work in _exchanger.CurrentTable)
				{
					if (work.FirstEventID == vertex || work.SecondEventID == vertex)
						WorksThatCanBeDeleted.Add(work);
				}
			}
		}
	}


	public enum EditingMode
	{
		StartVertexMode,
		EndVertexMode
	}
}
