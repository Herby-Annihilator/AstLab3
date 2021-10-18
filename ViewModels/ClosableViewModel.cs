using System;
using System.Collections.Generic;
using System.Text;
using AstLab3.Models.Services.Data;
using AstLab3.ViewModels.Base;

namespace AstLab3.ViewModels
{
	public class ClosableViewModel : ViewModel
	{
		public event EventHandler<UserDialogEventArgs> CloseWindow;

		protected virtual void OnCloseWindow(UserDialogEventArgs e) => CloseWindow?.Invoke(this, e);
	}
}
