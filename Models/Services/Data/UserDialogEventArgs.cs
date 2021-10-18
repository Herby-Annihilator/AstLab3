using System;
using System.Collections.Generic;
using System.Text;

namespace AstLab3.Models.Services.Data
{
	public class UserDialogEventArgs : EventArgs
	{
		public bool? DialogResult { get; set; } = false;
	}
}
