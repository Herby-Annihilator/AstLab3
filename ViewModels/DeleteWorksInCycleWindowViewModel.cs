using AstLab3.Infrastructure.Commands;
using AstLab3.Models.Schedules;
using AstLab3.Models.Services.Data;
using AstLab3.Models.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;

namespace AstLab3.ViewModels
{
	public class DeleteWorksInCycleWindowViewModel : ClosableViewModel
	{
		private bool _isCollectionChanged = false;
		private ILogger _logger;
		private List<Work> toRemove = new List<Work>();
		private NetworkSchedule _networkSchedule;
		public DeleteWorksInCycleWindowViewModel(DeleteWorksInCycleWindowData data, NetworkSchedule networkSchedule, ILogger logger)
		{
			_logger = logger;
			_networkSchedule = networkSchedule;
			foreach (var item in data.WorksInCycle)
			{
				WorksInCycles.Add(item);
			}
			DeleteSelectedWorkCommand = new LambdaCommand(OnDeleteSelectedWorkCommandExecuted, CanDeleteSelectedWorkCommandExecute);
			AcceptChangesCommand = new LambdaCommand(OnAcceptChangesCommandExecuted, CanAcceptChangesCommandExecute);
			CancelCommand = new LambdaCommand(OnCancelCommandExecuted, CanCancelCommandExecute);
			_logger.LogMessage("Инициализировано окно удаления работ из цикла");
		}
		public ObservableCollection<Work> WorksInCycles { get; set; } = new ObservableCollection<Work>();

		public Work SelectedWork { get; set; }

		public ICommand DeleteSelectedWorkCommand { get; }
		private void OnDeleteSelectedWorkCommandExecuted(object p)
		{
			toRemove.Add(SelectedWork);
			WorksInCycles.Remove(SelectedWork);
			SelectedWork = null;
			_isCollectionChanged = true;
		}
		private bool CanDeleteSelectedWorkCommandExecute(object p) => SelectedWork != null;

		public ICommand AcceptChangesCommand { get; }
		private void OnAcceptChangesCommandExecuted(object p)
		{
			bool? result = true;
			foreach (var item in toRemove)
			{
				if (_networkSchedule.Table.Remove(item))
				{
					_logger.LogMessage($"Работа {item} удалена");
				}
				else
				{
					_logger.LogMessage($"Не удалось удалить работу {item}");
					result = false;
				}
			}
			OnCloseWindow(new UserDialogEventArgs(result));
			_logger.LogMessage("Окно удаления работ из цикла закрыто");
		}
		private bool CanAcceptChangesCommandExecute(object p) => _isCollectionChanged;

		public ICommand CancelCommand { get; }
		private void OnCancelCommandExecuted(object p)
		{
			OnCloseWindow(new UserDialogEventArgs(false));
			_logger.LogMessage("Пользователь не удалил ни одну работу из цикла. Окно закрыто.");
		}
		private bool CanCancelCommandExecute(object p) => true;
	}
}
