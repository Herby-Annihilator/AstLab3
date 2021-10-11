using AstLab3.Infrastructure.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace AstLab3.Models.Services.Interfaces
{
	public interface ILogger
	{
		public event EventHandler<LoggerEventArgs> RegisterLogMessage;
		void LogMessage(string message);
	}
}
