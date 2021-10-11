using System;
using System.Collections.Generic;
using System.Text;

namespace AstLab3.Models.Services.Interfaces
{
	public interface IUserDialog<T>
	{
		bool Edit(T toEdit);
	}
}
