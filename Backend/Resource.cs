using AltV.Net;
using AltV.Net.Async;
using AltV.Net.Elements.Entities;
using Backend.Utils;
using Backend.Utils.Factories;
using Backend.Utils.Interfaces;

namespace Backend
{
	public class Resource : AsyncResource
	{
		public static bool DevMode { get; set; } = true;

		private IApplication? _application;
		private ModuleContainer? _moduleContainer;
		
		public override async void OnStart()
		{
			_moduleContainer = new ModuleContainer();

			await _moduleContainer.RegisterTypes();
			await _moduleContainer.ResolveTypes();

			_application = _moduleContainer.Resolve<IApplication>();

			_application.Start();
			//RequestHandler.Register();
		}

		public override void OnStop()
		{
			_application!.Stop();
		}

		public override IEntityFactory<IPlayer> GetPlayerFactory()
		{
			return new PlayerFactory();
		}

		public override IEntityFactory<IVehicle> GetVehicleFactory()
		{
			return new VehicleFactory();
		}

		public override IBaseObjectFactory<IColShape> GetColShapeFactory()
		{
			return new ColshapeFactory();
		}
	}
}