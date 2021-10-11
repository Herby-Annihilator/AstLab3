using System;
using System.Collections.Generic;
using System.Text;

namespace AstLab3.Infrastructure.Common
{
	public class LogMessage
	{
		private string _message;
		public string Message { get => $"{DateTime.Now}: {_message}"; set => _message = value; }
		public LogMessage(string message)
		{
			Message = message;
		}
	}
}
