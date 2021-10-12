using AstLab3.Models.Schedules;
using AstLab3.Models.Services.Interfaces;
using AstLab3.ViewModels;
using AstLab3.Views.Windows;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AstLab3.Models.Services
{
	public class UserDialogEditingWindow : IUserDialog<NetworkSchedule>
	{
		private ILogger _logger;
		public bool Edit(NetworkSchedule toEdit)
		{
			EditWindow editWindow = new EditWindow();
			EditWindowViewModel editWindowViewModel = new EditWindowViewModel(toEdit, _logger);
			editWindow.DataContext = editWindowViewModel;

			return editWindow.ShowDialog() ?? false;
		}

		public UserDialogEditingWindow(ILogger logger)
		{
			_logger = logger;
		}
	}
}
