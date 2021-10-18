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
		public DeleteUselessWorkWindowViewModel() : this(new DeleteUselessWorkWindowData(null, null), new NetworkSchedule(null), null) { }
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

		}
		private bool CanDeleteCommandExecute(object p) => true;


		public ICommand CancelCommand { get; }
		private void OnCancelCommandExecuted(object p)
		{

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
			}
			else
			{
				DeleteSecondWorkIsNecessary = true;
				DeleteFirstWorkIsNecessary = false;
			}
		}

		private bool CanSelectWorkToDeleteCommandExecute(object p) => true;
	}
}
