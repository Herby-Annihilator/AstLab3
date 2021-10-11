using AstLab3.Models.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace AstLab3.Models.Services
{
	public static class ServicesRegistrator
	{
		public static IServiceCollection AddServices(this IServiceCollection services) => services
			.AddSingleton<ILogger, LoggerService>()
		// Register your services here
		;
	}
}
