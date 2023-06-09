using Backend.Controllers.Module.Interface;
using Backend.Utils.Interfaces;
using Backend.Utils.Models.Entities;

namespace Backend.Utils
{
	public class Application : IApplication
	{
		private readonly IModuleController _moduleController;

		public Application(IModuleController moduleController)
		{
			_moduleController = moduleController;
		}

		public void Start()
		{
			_moduleController.Load();
		}

		public void Stop()
		{
		}
	}
}