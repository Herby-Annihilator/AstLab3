using AstLab3.Infrastructure.Commands;
using AstLab3.Infrastructure.Common;
using AstLab3.ViewModels.Base;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using System.Windows.Markup;
using System.IO;
using AstLab3.Models.Schedules;
using AstLab3.Models.Services;
using AstLab3.Models.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using AstLab3.Models.Schedules.Exceptions;
using AstLab3.Models.Services.Data;

namespace AstLab3.ViewModels
{
	[MarkupExtensionReturnType(typeof(MainWindowViewModel))]
	public class MainWindowViewModel : ViewModel
	{
		private ILogger _logger;
		private NetworkSchedule _networkSchedule;

		public MainWindowViewModel()
		{
			_logger = App.Services.GetRequiredService<ILogger>();
			_logger.RegisterLogMessage += LogMessage;
			_logger.RegisterLogMessage += LogMessageToLogFile;
			BrowseCommand = new LambdaCommand(OnBrowseCommandExecuted, CanBrowseCommandExecute);
			AddWorkCommand = new LambdaCommand(OnAddWorkCommandExecuted, CanAddWorkCommandExecute);
			RemoveSelectedWorkCommand = new LambdaCommand(OnRemoveSelectedWorkCommandExecuted, CanRemoveSelectedWorkCommandExecute);
			ClearSourceTableCommand = new LambdaCommand(OnClearSourceTableCommandExecuted, CanClearSourceTableCommandExecute);
			ReloadSourceTableCommand = new LambdaCommand(OnReloadSourceTableCommandExecuted, CanReloadSourceTableCommandExecute);
			ClearFinalTableCommand = new LambdaCommand(OnClearFinalTableCommandExecuted, CanClearFinalTableCommandExecute);
			ClearFullPathsInGraphCommand = new LambdaCommand(OnClearFullPathsInGraphCommandExecuted, CanClearFullPathsInGraphCommandExecute);
			ClearWorksInCriticalPathsCommand = new LambdaCommand(OnClearWorksInCriticalPathsCommandExecuted,
				CanClearWorksInCriticalPathsCommandExecute);
			ClearVerticescParamsTableCommand = new LambdaCommand(OnClearVerticescParamsTableCommandExecuted, 
				CanClearVerticescParamsTableCommandExecute);
			StreamlineCommand = new LambdaCommand(OnStreamlineCommandExecuted, CanStreamlineCommandExecute);
			ShowGanttChartCommand = new LambdaCommand(OnShowGanttChartCommandExecuted, CanShowGanttChartCommandExecute);
			AddWorkToNetworkScheduleCommand = new LambdaCommand(OnAddWorkToNetworkScheduleCommandExecuted,
				CanAddWorkToNetworkScheduleCommandExecute);
			ChangeWorkCommand = new LambdaCommand(OnChangeWorkCommandExecuted, CanChangeWorkCommandExecute);
			RemoveWorkCommand = new LambdaCommand(OnRemoveWorkCommandExecuted, CanRemoveWorkCommandExecute);
		}

		private void LogMessageToLogFile(object sender, LoggerEventArgs e)
		{
			StreamWriter writer = new StreamWriter("log.txt", true);
			writer.WriteLine(e.Message);
			writer.Close();
		}

		private void LogMessage(object sender, LoggerEventArgs e)
		{
			LogBuffer.Insert(0, e.Message);
		}

		#region Properties
		public ObservableCollection<string> LogBuffer { get; private set; } = new ObservableCollection<string>();

		public ObservableCollection<Work> SourceTable { get; private set; } = new ObservableCollection<Work>();

		public ObservableCollection<string> FullPathsInTheGraph { get; private set; } = new ObservableCollection<string>();

		public ObservableCollection<Work> FinalTable { get; private set; } = new ObservableCollection<Work>();

		public ObservableCollection<Work> WorksInCriticalPaths { get; private set; } = new ObservableCollection<Work>();

		public ObservableCollection<Vertex> Vertices { get; private set; } = new ObservableCollection<Vertex>();

		public Work SelectedWork { get; set; }

		public Work WorkToChangeOrRemove { get; set; }

		private string _title = "Title";
		public string Title { get => _title; set => Set(ref _title, value); }

		private string _status = "Status";
		public string Status { get => _status; set => Set(ref _status, value); }

		private string _path = "";
		public string Path { get => _path; set => Set(ref _path, value); }

		private int _criticalPathLength = 0;
		public int CriticalPathLength { get => _criticalPathLength; set => Set(ref _criticalPathLength, value); }
		#endregion

		#region Commands
		#region BrowseCommand
		public ICommand BrowseCommand { get; }
		private void OnBrowseCommandExecuted(object p)
		{
			try
			{
				Status = "Открытие файла";
				_logger.LogMessage(Status);
				OpenFileDialog dialog = new OpenFileDialog();
				dialog.InitialDirectory = Environment.CurrentDirectory;
				dialog.Filter = "txt files (*.txt)|*.txt|All files (*.*)|*.*";
				if (dialog.ShowDialog() == true)
				{
					Path = dialog.FileName;
				}
				Status = "Открытие файла завершено";
				_logger.LogMessage(Status);
			}
			catch (Exception e)
			{
				Status = e.Message;
				_logger.LogMessage(Status);
			}
		}
		private bool CanBrowseCommandExecute(object p) => true;
		#endregion

		#region AddWorkCommand
		public ICommand AddWorkCommand { get; }
		private void OnAddWorkCommandExecuted(object p)
		{
			try
			{
				SourceTable.Add(new Work(new Vertex(0), new Vertex(0), 0));
				Status = "Добавлена новая работа с параметрами: 0, 0, 0";
				_logger.LogMessage(Status);
			}
			catch (Exception e)
			{
				Status = e.Message;
				_logger.LogMessage(Status);
			}
		}
		private bool CanAddWorkCommandExecute(object p) => true;
		#endregion

		#region RemoveSelectedWorkCommand
		public ICommand RemoveSelectedWorkCommand { get; }
		private void OnRemoveSelectedWorkCommandExecuted(object p)
		{
			try
			{
				SourceTable.Remove(SelectedWork);
				SelectedWork = null;
				Status = "Выбранная работа удалена";
				_logger.LogMessage(Status);
			}
			catch (Exception e)
			{
				Status = e.Message;
				_logger.LogMessage(Status);
			}
		}
		private bool CanRemoveSelectedWorkCommandExecute(object p) => SelectedWork != null;
		#endregion

		#region ClearSourceTableCommand
		public ICommand ClearSourceTableCommand { get; }
		private void OnClearSourceTableCommandExecuted(object p)
		{
			try
			{
				SourceTable.Clear();
				Status = "Начальная таблица очищена";
				_logger.LogMessage(Status);
			}
			catch (Exception e)
			{
				Status = e.Message;
				_logger.LogMessage(Status);
			}
		}
		private bool CanClearSourceTableCommandExecute(object p) => SourceTable.Count > 0;
		#endregion

		#region ReloadSourceTableCommand
		public ICommand ReloadSourceTableCommand { get; }
		private void OnReloadSourceTableCommandExecuted(object p)
		{
			try
			{
				if (File.Exists(Path))
				{
					SourceTable.Clear();
					LoadSourceTable(Path);
					_networkSchedule = new NetworkSchedule(SourceTable);
					Status = "Таблица перезагружена. Сетевой график по таблице инициализирован.";
					_logger.LogMessage(Status);
				}
				else
				{
					Status = $"Файл {Path} не существует";
					_logger.LogMessage(Status);
				}
			}
			catch (Exception e)
			{
				Status = e.Message;
				_logger.LogMessage(Status);
			}
		}
		private bool CanReloadSourceTableCommandExecute(object p) => !string.IsNullOrWhiteSpace(Path);
		#endregion

		#region ClearFinalTableCommand
		public ICommand ClearFinalTableCommand { get; }
		private void OnClearFinalTableCommandExecuted(object p)
		{
			try
			{
				FinalTable.Clear();
				Status = "Итоговая таблица очищена";
				_logger.LogMessage(Status);
			}
			catch (Exception e)
			{
				Status = e.Message;
				_logger.LogMessage(Status);
			}
		}
		private bool CanClearFinalTableCommandExecute(object p) => FinalTable.Count > 0;
		#endregion

		#region ClearFullPathsInGraphCommand
		public ICommand ClearFullPathsInGraphCommand { get; }
		private void OnClearFullPathsInGraphCommandExecuted(object p)
		{
			try
			{
				FullPathsInTheGraph.Clear();
				Status = "Список путей очищен";
				_logger.LogMessage(Status);
			}
			catch (Exception e)
			{
				Status = e.Message;
				_logger.LogMessage(Status);
			}
		}
		private bool CanClearFullPathsInGraphCommandExecute(object p) => FullPathsInTheGraph.Count > 0;
		#endregion

		#region ClearWorksInCriticalPathsCommand
		public ICommand ClearWorksInCriticalPathsCommand { get; }
		private void OnClearWorksInCriticalPathsCommandExecuted(object p)
		{
			try
			{
				WorksInCriticalPaths.Clear();
				Status = "Список путей очищен";
				_logger.LogMessage(Status);
			}
			catch (Exception e)
			{
				Status = e.Message;
				_logger.LogMessage(Status);
			}
		}
		private bool CanClearWorksInCriticalPathsCommandExecute(object p) => WorksInCriticalPaths.Count > 0;

		#endregion

		#region ClearVerticescParamsTableCommand
		public ICommand ClearVerticescParamsTableCommand { get; }
		private void OnClearVerticescParamsTableCommandExecuted(object p)
		{
			try
			{
				Vertices.Clear();
				Status = "Таблица параметров событий очищена";
				_logger.LogMessage(Status);
			}
			catch (Exception e)
			{
				Status = e.Message;
				_logger.LogMessage(Status);
			}
		}
		private bool CanClearVerticescParamsTableCommandExecute(object p) => Vertices.Count > 0;
		#endregion

		#region StreamlineCommand
		public ICommand StreamlineCommand { get; }
		private void OnStreamlineCommandExecuted(object p)
		{
			try
			{
				Streamline();
				Status = "Упорядочивание прошло успешно";
				_logger.LogMessage(Status);
			}
			catch (Exception e)
			{
				Status = e.Message;
				_logger.LogMessage(Status);
			}
		}
		private bool CanStreamlineCommandExecute(object p) => SourceTable.Count > 0;
		#endregion

		#region AddWorkToNetworkScheduleCommand
		public ICommand AddWorkToNetworkScheduleCommand { get; }
		private void OnAddWorkToNetworkScheduleCommandExecuted(object p)
		{
			try
			{
				UserDialogAddWorkWindow dialog = new UserDialogAddWorkWindow(_logger);
				if (dialog.Edit(_networkSchedule))
					Streamline();
			}
			catch (Exception e)
			{
				Status = e.Message;
				_logger.LogMessage(Status);
			}
		}
		private bool CanAddWorkToNetworkScheduleCommandExecute(object p) => true;
		#endregion

		#region ChangeWorkCommand
		public ICommand ChangeWorkCommand { get; }
		private void OnChangeWorkCommandExecuted(object p)
		{
			try
			{
				UserDialogChangeWorkWindow dialog = new UserDialogChangeWorkWindow(
					new ChangeWorkWindowData(WorkToChangeOrRemove), _logger);
				if (dialog.Edit(_networkSchedule))
				{
					_networkSchedule.CalculateVerticesParameters();
					CopySourceCollectionToOtherCollection(_networkSchedule.Table, FinalTable);
					CriticalPathLength = _networkSchedule.Table[^1].EndVertex.LateCompletionDate;
					CopySourceCollectionToOtherCollection(_networkSchedule.GetCritalcWorks(), WorksInCriticalPaths);
					CopySourceCollectionToOtherCollection(_networkSchedule.Vertices, Vertices);
				}
			}
			catch (Exception e)
			{
				Status = e.Message;
				_logger.LogMessage(Status);
			}
		}
		private bool CanChangeWorkCommandExecute(object p) => WorkToChangeOrRemove != null;
		#endregion

		#region RemoveWorkCommand
		public ICommand RemoveWorkCommand { get; }
		private void OnRemoveWorkCommandExecuted(object p)
		{
			try
			{
				if (!_networkSchedule.Table.Remove(WorkToChangeOrRemove))
				{
					_logger.LogMessage($"Работа {WorkToChangeOrRemove} не была удалена.");
				}
				else
				{
					_logger.LogMessage($"Работа удалена.");
					Streamline();
				}
			}
			catch (Exception e)
			{
				Status = e.Message;
				_logger.LogMessage(Status);
			}
		}
		private bool CanRemoveWorkCommandExecute(object p) => WorkToChangeOrRemove != null; 
		#endregion

		#region ShowGanttChartCommand
		public ICommand ShowGanttChartCommand { get; }
		private void OnShowGanttChartCommandExecuted(object p)
		{
			try
			{
				UserInformatorGanttChart informator = new UserInformatorGanttChart(_logger);
				informator.Show(_networkSchedule);
			}
			catch(Exception e)
			{
				Status = e.Message;
				_logger.LogMessage(Status);
			}
		}
		private bool CanShowGanttChartCommandExecute(object p) => true; 
		#endregion

		#endregion

		private void LoadSourceTable(string fileName)  //  формат файла: число число число
		{
			StreamReader reader = new StreamReader(fileName);
			string buffer;
			string[] numbers;
			int firstEventID, secondEventID, length;
			while ((buffer = reader.ReadLine()) != null)
			{
				if (!string.IsNullOrWhiteSpace(buffer))
				{
					numbers = buffer.Split(" ", StringSplitOptions.RemoveEmptyEntries);
					firstEventID = Convert.ToInt32(numbers[0]);
					secondEventID = Convert.ToInt32(numbers[1]);
					length = Convert.ToInt32(numbers[2]);
					SourceTable.Add(new Work(new Vertex(firstEventID), new Vertex(secondEventID), length));
				}
			}
			reader.Close();
		}

		private void AddVertexToFullPaths(List<int> currentPath, int vertexIndex)
		{
			string path = "";
			foreach (int vertex in currentPath)
			{
				path += vertex.ToString() + " ";
			}
			FullPathsInTheGraph.Add(path);
		}

		private static void CopySourceCollectionToOtherCollection<T>(ICollection<T> source, ICollection<T> other)
		{
			other.Clear();
			foreach (var item in source)
			{
				other.Add(item);
			}
		}

		private void Streamline()
		{
			bool isNetworkScheduleStreamlined = false;
			while (!isNetworkScheduleStreamlined)
			{
				try
				{
					_logger.LogMessage("Старт процедуры 'частичного упорядочивания'");
					FullPathsInTheGraph.Clear();
					_networkSchedule.FindAllPaths(AddVertexToFullPaths, new List<int>(), _logger);
					isNetworkScheduleStreamlined = true;
					_networkSchedule.CalculateVerticesParameters();
					CopySourceCollectionToOtherCollection(_networkSchedule.Table, FinalTable);
					CriticalPathLength = _networkSchedule.Table[^1].EndVertex.LateCompletionDate;
					CopySourceCollectionToOtherCollection(_networkSchedule.GetCritalcWorks(), WorksInCriticalPaths);
					CopySourceCollectionToOtherCollection(_networkSchedule.Vertices, Vertices);
					_logger.LogMessage("Конец процедуры 'частичного упорядочивания'");
				}
				catch (CyclesFoundException e)
				{
					DeleteWorksInCycleWindowData data = new DeleteWorksInCycleWindowData(e.WorksInCycle);
					UserDialogDeleteWorksInCycleWindow dialog = new UserDialogDeleteWorksInCycleWindow(_logger, data);
					dialog.Edit(_networkSchedule);
				}
				catch (SeveralVerticesFoundException e)
				{
					EditingWindowData data = new EditingWindowData(e.Vertices, e.EditingMode, e.Message);
					UserDialogEditingWindow dialog = new UserDialogEditingWindow(data, _logger);
					dialog.Edit(_networkSchedule);
				}
				catch (NoVerticesFoundException e)
				{
					EditingWindowData data = new EditingWindowData(new List<Vertex>(), e.EditingMode, e.Message);
					UserDialogEditingWindow dialog = new UserDialogEditingWindow(data, _logger);
					dialog.Edit(_networkSchedule);
				}
				catch (OverlappingWorksFoundException e)
				{
					DeleteUselessWorkWindowData data = new DeleteUselessWorkWindowData(e.FirstWorkToDelete, e.SecondWorkToDelete);
					UserDialogDeleteUselessWorkWindow dialog = new UserDialogDeleteUselessWorkWindow(_logger, data);
					dialog.Edit(_networkSchedule);
				}
			}
		}
	}
}
