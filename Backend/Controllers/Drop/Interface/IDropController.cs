using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Controllers.Drop.Interface
{
	public interface IDropController
	{
		bool IsDropOpen { get; }
		Task CreateDrop();
		Task OpenDrop();
		Task Tick();
		Task RemoveDrop();
	}
}