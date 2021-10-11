using System;
using System.Collections.Generic;
using System.Text;

namespace AstLab3.Infrastructure.Common
{
	public class LoggerEventArgs : EventArgs
	{
		private LogMessage _logMessage;
		public LoggerEventArgs(string message)
		{
			_logMessage = new LogMessage(message);
		}
		public string Message { get => _logMessage.Message; set => _logMessage.Message = value; }
	}
}
