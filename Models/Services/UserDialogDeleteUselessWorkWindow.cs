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
			DeleteUselessWorkWindow deleteUselessWindow = new DeleteUselessWorkWindow();
			DeleteUselessWorkWindowViewModel deleteUselessWorkWindowViewModel =
				new DeleteUselessWorkWindowViewModel(_deleteUselessWorkWindowData, toEdit, _logger);
			deleteUselessWorkWindowViewModel.CloseWindow += (_, e) =>
			{
				deleteUselessWindow.DialogResult = e.DialogResult;
				deleteUselessWindow.Close();
			};
			return deleteUselessWindow.ShowDialog() ?? false;
		}
	}
}
