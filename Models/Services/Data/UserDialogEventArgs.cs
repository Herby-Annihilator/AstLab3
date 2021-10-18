using System;
using System.Collections.Generic;
using System.Text;

namespace AstLab3.Models.Services.Data
{
	public class UserDialogEventArgs : EventArgs
	{
		public UserDialogEventArgs(bool? result)
		{
			DialogResult = result;
		}

		public bool? DialogResult { get; set; } = false;
	}
}
