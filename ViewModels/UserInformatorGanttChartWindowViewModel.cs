using AstLab3.Infrastructure.Commands;
using AstLab3.Models.Schedules;
using AstLab3.Models.Services.Data;
using AstLab3.Models.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using OxyPlot;

namespace AstLab3.ViewModels
{
	public class UserInformatorGanttChartWindowViewModel : ClosableViewModel
	{
		private ILogger _logger;
		private NetworkSchedule _networkSchedule;
		public UserInformatorGanttChartWindowViewModel(ILogger logger, NetworkSchedule toShow)
		{
			_networkSchedule = toShow;
			_logger = logger;
			CloseCommand = new LambdaCommand(OnCloseCommandExecuted, CanCloseCommandExecute);
			StartPerformance();
		}
		public ICommand CloseCommand { get; }
		private void OnCloseCommandExecuted(object p)
		{
			OnCloseWindow(new UserDialogEventArgs(true));
		}
		private bool CanCloseCommandExecute(object p) => true;

		private void StartPerformance()
		{
			PlotModel model = new PlotModel();
		}
	}
}
