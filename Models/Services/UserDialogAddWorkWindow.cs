using AstLab3.ViewModels;
using AstLab3.Views.Windows;
using AstLab3.Models.Schedules;
using AstLab3.Models.Services.Interfaces;

namespace AstLab3.Models.Services
{
	public class UserDialogAddWorkWindow : IUserDialog<NetworkSchedule>
	{
		private ILogger _logger;

		public UserDialogAddWorkWindow(ILogger logger)
		{
			_logger = logger;
		}

		public bool Edit(NetworkSchedule toEdit)
		{
			var window = new AddWorkWindow();
			var data = new AddWorkWindowViewModel(toEdit, _logger);
			window.DataContext = data;
			window.Owner = App.Current.MainWindow;
			window.WindowStartupLocation = System.Windows.WindowStartupLocation.CenterOwner;
			data.CloseWindow += (_, e) =>
			{
				window.DialogResult = e.DialogResult;
				window.Close();
			};
			return window.ShowDialog() ?? false;
		}
	}
}
