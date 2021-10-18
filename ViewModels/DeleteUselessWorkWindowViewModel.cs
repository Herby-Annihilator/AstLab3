using AstLab3.Infrastructure.Commands;
using AstLab3.Models.Schedules;
using AstLab3.Models.Services.Data;
using AstLab3.Models.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;

namespace AstLab3.ViewModels
{
	public class DeleteUselessWorkWindowViewModel : ClosableViewModel
	{
		private DeleteUselessWorkWindowData _deleteUselessWorkWindowData;
		private NetworkSchedule _networkSchedule;
		private ILogger _logger;
		//
		// нужен только для дизайнера
		//
		public DeleteUselessWorkWindowViewModel()
		{

		}
		public DeleteUselessWorkWindowViewModel(DeleteUselessWorkWindowData deleteUselessWorkWindowData, NetworkSchedule toEidt, ILogger logger)
		{
			_logger = logger;
			_networkSchedule = toEidt;
			_deleteUselessWorkWindowData = deleteUselessWorkWindowData;
			DeleteFirstWorkIsNecessary = true;
			FirstWorkToDelete = deleteUselessWorkWindowData.FirstWorkToDelete.Self;
			SecondWorkToDelete = deleteUselessWorkWindowData.SecondWorkToDelete.Self;
			SelectWorkToDeleteCommand = new LambdaCommand(OnSelectWorkToDeleteCommandExecuted, CanSelectWorkToDeleteCommandExecute);
			DeleteCommand = new LambdaCommand(OnDeleteCommandExecuted, CanDeleteCommandExecute);
			CancelCommand = new LambdaCommand(OnCancelCommandExecuted, CanCancelCommandExecute);
			_logger.LogMessage("Инициализировано окно для удаления частично повторяющихся работ");
		}
		private bool _deleteFirstWorkIsNecessary = false;
		public bool DeleteFirstWorkIsNecessary { get => _deleteFirstWorkIsNecessary; set => Set(ref _deleteFirstWorkIsNecessary, value); }

		private bool _deleteSecondWorkIsNecessary = false;
		public bool DeleteSecondWorkIsNecessary { get => _deleteSecondWorkIsNecessary; set => Set(ref _deleteSecondWorkIsNecessary, value); }

		private string _firstWorkToDelete = "";
		public string FirstWorkToDelete { get => _firstWorkToDelete; set => Set(ref _firstWorkToDelete, value); }

		private string _secondWorkToDelete = "";
		public string SecondWorkToDelete { get => _secondWorkToDelete; set => Set(ref _secondWorkToDelete, value); }

		public ICommand DeleteCommand { get; }
		private void OnDeleteCommandExecuted(object p)
		{
			bool? result;
			if (DeleteFirstWorkIsNecessary)
			{
				if (_networkSchedule.Table.Remove(_deleteUselessWorkWindowData.FirstWorkToDelete))
				{
					_logger.LogMessage($"Работа {_deleteUselessWorkWindowData.FirstWorkToDelete} успешно удалена");
					result = true;
				}
				else
				{
					_logger.LogMessage($"Не удалось удалить работу {_deleteUselessWorkWindowData.FirstWorkToDelete}");
					result = false;
				}
			}
			else
			{
				if (_networkSchedule.Table.Remove(_deleteUselessWorkWindowData.SecondWorkToDelete))
				{
					_logger.LogMessage($"Работа {_deleteUselessWorkWindowData.SecondWorkToDelete} успешно удалена");
					result = true;
				}
				else
				{
					_logger.LogMessage($"Не удалось удалить работу {_deleteUselessWorkWindowData.SecondWorkToDelete}");
					result = false;
				}
			}
			OnCloseWindow(new UserDialogEventArgs(result));
			_logger.LogMessage("Окно удаления частично повторяющихся работ закрыто.");
		}
		private bool CanDeleteCommandExecute(object p) => true;


		public ICommand CancelCommand { get; }
		private void OnCancelCommandExecuted(object p)
		{
			_logger.LogMessage("Пользователь не удалил ни одну из частично повторяющихся работ. Окно закрыто.");
			OnCloseWindow(new UserDialogEventArgs(false));
		}
		private bool CanCancelCommandExecute(object p) => true;

		public ICommand SelectWorkToDeleteCommand { get; }
		private void OnSelectWorkToDeleteCommandExecuted(object p)
		{
			string param = (string)p;
			if (param == "first")
			{
				DeleteFirstWorkIsNecessary = true;
				DeleteSecondWorkIsNecessary = false;
				_logger.LogMessage($"Пользователь отметил работу {FirstWorkToDelete}");
			}
			else
			{
				DeleteSecondWorkIsNecessary = true;
				DeleteFirstWorkIsNecessary = false;
				_logger.LogMessage($"Пользователь отметил работу {SecondWorkToDelete}");
			}
		}

		private bool CanSelectWorkToDeleteCommandExecute(object p) => true;
	}
}
