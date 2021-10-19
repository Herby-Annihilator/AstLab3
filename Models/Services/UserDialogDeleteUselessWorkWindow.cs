using AstLab3.Models.Schedules;
using AstLab3.Models.Services.Interfaces;
using AstLab3.ViewModels;
using AstLab3.Views.Windows;
using AstLab3.Models.Services.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace AstLab3.Models.Services
{
	public class UserDialogDeleteUselessWorkWindow : IUserDialog<NetworkSchedule>
	{
		private ILogger _logger;
		private DeleteUselessWorkWindowData _deleteUselessWorkWindowData;

		public UserDialogDeleteUselessWorkWindow(ILogger logger, DeleteUselessWorkWindowData deleteUselessWorkWindowData)
		{
			_logger = logger;
			_deleteUselessWorkWindowData = deleteUselessWorkWindowData;
		}

		public bool Edit(NetworkSchedule toEdit)
		{
			DeleteUselessWorkWindow window = new DeleteUselessWorkWindow();
			DeleteUselessWorkWindowViewModel model =
				new DeleteUselessWorkWindowViewModel(_deleteUselessWorkWindowData, toEdit, _logger);
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
