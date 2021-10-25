using AstLab3.Infrastructure.Commands;
using AstLab3.Models.Schedules;
using AstLab3.Models.Services.Data;
using AstLab3.Models.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Input;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;

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
			logger.LogMessage("Инициализировано окно показа диаграммы Ганта");
			StartPerformance();
		}

		public PlotModel PlotModel { get; set; } = new PlotModel();
		public ICommand CloseCommand { get; }
		private void OnCloseCommandExecuted(object p)
		{
			OnCloseWindow(new UserDialogEventArgs(true));
		}
		private bool CanCloseCommandExecute(object p) => true;

		private void StartPerformance()
		{
			PlotModel.Title = "Диаграмма Ганта сетевого графика";
			LinearAxis bottom = new LinearAxis
			{
				MajorGridlineStyle = LineStyle.Solid,
				MinorGridlineStyle = LineStyle.Dot,
				TickStyle = TickStyle.Outside,
				Position = AxisPosition.Bottom,
				Title = "Продолжительность в у.е."
			};
			PlotModel.Axes.Add(bottom);
			CategoryAxis left = new CategoryAxis
			{
				Position = AxisPosition.Left,
				MajorGridlineStyle = LineStyle.Solid,
				MinorGridlineStyle = LineStyle.Dot,
				Title = "Номера работ в таблице",
			};
			PlotModel.Axes.Add(left);
			IntervalBarSeries barSeries = new IntervalBarSeries
			{
				LabelMargin = 0,
			};
			Random random = new Random();
			IntervalBarItem item;
			int currenrtCategoryIndex = _networkSchedule.Table.Count - 1;
			foreach (Work work in _networkSchedule.Table)
			{
				item = new IntervalBarItem
				{
					Start = work.StartVertex.EarlyCompletionDate,
					End = work.EndVertex.EarlyCompletionDate,
					CategoryIndex = currenrtCategoryIndex,
					Title = work.Self,
					Color = OxyColor.FromRgb((byte)random.Next(107, 222), (byte)random.Next(0, 227), (byte)random.Next(0, 82)),
				};
				barSeries.Items.Add(item);
				currenrtCategoryIndex--;
			}
			PlotModel.Series.Add(barSeries);
		}
	}
}
