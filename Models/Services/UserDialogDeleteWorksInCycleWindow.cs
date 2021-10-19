using AstLab3.Models.Schedules;
using AstLab3.Models.Services.Data;
using AstLab3.Models.Services.Interfaces;
using AstLab3.ViewModels;
using AstLab3.Views.Windows;
using System;
using System.Collections.Generic;
using System.Text;

namespace AstLab3.Models.Services
{
	public class UserDialogDeleteWorksInCycleWindow : IUserDialog<NetworkSchedule>
	{
		private ILogger _logger;
		private DeleteWorksInCycleWindowData _deleteWorksInCycleWindowData;

		public UserDialogDeleteWorksInCycleWindow(ILogger logger, DeleteWorksInCycleWindowData deleteWorksInCycleWindowData)
		{
			_logger = logger;
			_deleteWorksInCycleWindowData = deleteWorksInCycleWindowData;
		}

		public bool Edit(NetworkSchedule toEdit)
		{
			DeleteWorksInCycleWindow window = new DeleteWorksInCycleWindow();
			DeleteWorksInCycleWindowViewModel model = new DeleteWorksInCycleWindowViewModel(_deleteWorksInCycleWindowData, toEdit, _logger);
			window.DataContext = model;
			window.Owner = App.Current.MainWindow;
			window.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
			model.CloseWindow += (_, e) =>
			{
				window.DialogResult = e.DialogResult;
				window.Close();
			};
			return window.ShowDialog() ?? false;
		}
	}
}
