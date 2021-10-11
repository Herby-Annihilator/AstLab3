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
using AstLab3.Models.NetworkSchedule;
using AstLab3.Models.Services;
using AstLab3.Models.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AstLab3.ViewModels
{
	[MarkupExtensionReturnType(typeof(MainWindowViewModel))]
	public class MainWindowViewModel : ViewModel
	{
		private ILogger _logger;

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
		}

		private void LogMessageToLogFile(object sender, LoggerEventArgs e)
		{
			StreamWriter writer = new StreamWriter("log.txt", true);
			writer.WriteLine(e.Message);
			writer.Close();
		}

		private void LogMessage(object sender, LoggerEventArgs e)
		{
			LogBuffer.Add(e.Message);
		}

		#region Properties
		public ObservableCollection<string> LogBuffer { get; private set; } = new ObservableCollection<string>();

		public ObservableCollection<Work> SourceTable { get; private set; } = new ObservableCollection<Work>();

		public ObservableCollection<string> FullPathsInTheGraph { get; private set; } = new ObservableCollection<string>();

		public ObservableCollection<Work> FinalTable { get; private set; } = new ObservableCollection<Work>();

		public Work SelectedWork { get; set; }

		private string _title = "Title";
		public string Title { get => _title; set => Set(ref _title, value); }

		private string _status = "Status";
		public string Status { get => _status; set => Set(ref _status, value); }

		private string _path = "";
		public string Path { get => _path; set => Set(ref _path, value); }
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
					Status = "Таблица перезагружена";
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
	}
}
