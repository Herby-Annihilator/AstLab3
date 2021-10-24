using AstLab3.Infrastructure.Commands;
using AstLab3.Models.Schedules;
using AstLab3.Models.Services.Data;
using AstLab3.Models.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;

namespace AstLab3.ViewModels
{
	public class AddWorkWindowViewModel : ClosableViewModel
	{
		private ILogger _logger;
		private NetworkSchedule _networkSchedule;

		private string _worksLength = "";
		public string WorksLength { get => _worksLength; set => Set(ref _worksLength, value); }

		private string _startVertexIndex = "";
		public string StartVertexIndex { get => _startVertexIndex; set => Set(ref _startVertexIndex, value); }

		private string _endVertexIndex = "";
		public string EndVertexIndex { get => _endVertexIndex; set => Set(ref _endVertexIndex, value); }

		public AddWorkWindowViewModel(NetworkSchedule networkSchedule, ILogger logger)
		{
			_logger = logger;
			_networkSchedule = networkSchedule;
			CancelCommand = new LambdaCommand(OnCancelCommandExecuted, CanCancelCommandExecute);
			ApplyCommand = new LambdaCommand(OnApplyCommandExecuted, CanApplyCommandExecute);
			_logger.LogMessage("Инициализировано окно для добавления новой работы.");
		}

		public ICommand CancelCommand { get; }
		private void OnCancelCommandExecuted(object p)
		{
			_logger.LogMessage("Отказ добавлять новую работу. Окно закрыто.");
			OnCloseWindow(new UserDialogEventArgs(false));
		}
		private bool CanCancelCommandExecute(object p) => true;

		public ICommand ApplyCommand { get; }
		private void OnApplyCommandExecuted(object p)
		{
			try
			{
				int startIndex = Convert.ToInt32(StartVertexIndex);
				int endIndex = Convert.ToInt32(EndVertexIndex);
				int length = Convert.ToInt32(WorksLength);
				CheckLength(length);
				List<Vertex> vertices = _networkSchedule.Vertices;
				bool startVertexExist = false;
				bool endVertexExist = false;
				foreach (var item in vertices)
				{
					if (startIndex == item.ID)
						startVertexExist = true;
					if (endIndex == item.ID)
						endVertexExist = true;
				}
				if (!startVertexExist)
				{
					_logger.LogMessage($"Событие {startIndex} не сущетвует и будет создано");
					MessageBox.Show($"Событие {startIndex} не сущетвует и будет создано",
						"Information", MessageBoxButton.OK, MessageBoxImage.Information);
				}
				if (!endVertexExist)
				{
					_logger.LogMessage($"Событие {endIndex} не сущетвует и будет создано");
					MessageBox.Show($"Событие {endIndex} не сущетвует и будет создано", 
						"Information", MessageBoxButton.OK, MessageBoxImage.Information);
				}
				_networkSchedule.CreateWorkInTable(startIndex, endIndex, length);
				_logger.LogMessage($"Работа {startIndex} → {endIndex} = {length} успешно создана. Окно закрыто.");
				OnCloseWindow(new UserDialogEventArgs(true));
			}
			catch (Exception e)
			{
				MessageBox.Show(e.Message, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
			}
		}
		private bool CanApplyCommandExecute(object p) => true;

		private void CheckLength(int length)
		{
			if (length < 0)
				throw new Exception("Продолжительность работы не может быть меньше 0");
		}
	}
}
