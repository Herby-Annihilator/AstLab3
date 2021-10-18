using AstLab3.Infrastructure.Commands;
using AstLab3.Models.Schedules;
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
		private ILogger _logger;
		private List<Work> toRemove = new List<Work>();
		public DeleteWorksInCycleWindowViewModel()
		{
			DeleteSelectedWorkCommand = new LambdaCommand(OnDeleteSelectedWorkCommandExecuted, CanDeleteSelectedWorkCommandExecute);
			AcceptChangesCommand = new LambdaCommand(OnAcceptChangesCommandExecuted, CanAcceptChangesCommandExecute);
			CancelCommand = new LambdaCommand(OnCancelCommandExecuted, CanCancelCommandExecute);
		}
		public ObservableCollection<Work> WorksInCycles { get; set; } = new ObservableCollection<Work>();

		public Work SelectedWork { get; set; }

		public ICommand DeleteSelectedWorkCommand { get; }
		private void OnDeleteSelectedWorkCommandExecuted(object p)
		{
			toRemove.Add(SelectedWork);
			WorksInCycles.Remove(SelectedWork);
			SelectedWork = null;
		}
		private bool CanDeleteSelectedWorkCommandExecute(object p) => SelectedWork != null;

		public ICommand AcceptChangesCommand { get; }
		private void OnAcceptChangesCommandExecuted(object p)
		{

		}
		private bool CanAcceptChangesCommandExecute(object p) => true;

		public ICommand CancelCommand { get; }
		private void OnCancelCommandExecuted(object p)
		{

		}
		private bool CanCancelCommandExecute(object p) => true;
	}
}
