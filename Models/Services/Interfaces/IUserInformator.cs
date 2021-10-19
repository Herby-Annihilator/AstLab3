using System;
using System.Collections.Generic;
using System.Text;

namespace AstLab3.Models.Services.Interfaces
{
	public interface IUserInformator<T>
	{
		void Show(T instance);
	}
}
