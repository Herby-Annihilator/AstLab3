using AstLab3.Infrastructure.Common;
using AstLab3.Models.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace AstLab3.Models.Services
{
	public class LoggerService : ILogger
	{
		public event EventHandler<LoggerEventArgs> RegisterLogMessage;

		public void LogMessage(string message) => OnRegisterLogMessage(message);

		protected void OnRegisterLogMessage(string message) => RegisterLogMessage?.Invoke(this, new LoggerEventArgs(message));
	}
}
