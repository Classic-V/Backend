using AltV.Net.Elements.Entities;
using Backend.Controllers.Event.Interface;
using Backend.Controllers.Processor.Interface;
using Backend.Data;
using Backend.Services.Vehicles.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Interfaces.Events;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Player.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend.Modules.Processor
{
	public class ProcessorModule : Module<ProcessorModule>, IEventColshape
	{
		private readonly IProcessorController _processorController;
		private readonly IVehicleService _vehicleService;

		public ProcessorModule(IEventController eventController, IProcessorController processorController, IVehicleService vehicleService) : base("Processor")
		{
			_processorController = processorController;
			_vehicleService = vehicleService;

			eventController.OnClient<int, int>("Server:Processor:ProcessVehicle", ProcessVehicle);
			eventController.OnClient<int>("Server:Processor:ProcessInventory", ProcessInventory);
			eventController.OnClient("Server:Processor:Start", Start);
		}

		private async void ProcessVehicle(ClPlayer player, string eventKey, int id, int vehid)
		{
			await _processorController.ProcessVehicle(player, id, vehid);
		}

		private async void ProcessInventory(ClPlayer player, string eventKey, int id)
		{
			await _processorController.ProcessInventory(player, id);
		}

		private async void Start(ClPlayer player, string eventKey)
		{
			var shape = player.CurrentShape;
			if (shape == null || shape.ShapeType != ColshapeType.PROCESSOR) return;

			var data = new List<ClientNativeMenuItem>()
			{
				new ClientNativeMenuItem("Inventar verarbeiten") { Close = true, CallbackEvent = "Server:Processor:ProcessInventory", CallbackArgs = new object[] { shape.Id }}
			};

			foreach(var veh in ClVehicle.All.Where(x => x.DbModel != null && (x.DbModel.Type == VehicleType.PLAYER && x.DbModel.Owner == player.DbModel.Id || x.DbModel.Type == VehicleType.TEAM && x.DbModel.Owner == player.DbModel.Team) && x.Position.Distance(player.Position) <= 18))
			{
				var info = await _vehicleService.GetVehicleInfo(veh.DbModel!.InfoModelId);
				if(info == null) continue;

				data.Add(new ClientNativeMenuItem($"#{veh.DbModel.Id} - {info.Name} verarbeiten") { Close = true, CallbackEvent = "Server:Processor:ProcessVehicle", CallbackArgs = new object[] { shape.Id, veh.DbModel.Id }});
			}

			await player.ShowNativeMenu(true, new ClientNativeMenu("Verarbeiter", data));
		}

		public Task OnColshape(ClShape shape, IEntity entity, bool entered)
		{
			if (entity.Type != BaseObjectType.Player || shape.ShapeType != ColshapeType.PROCESSOR) return Task.CompletedTask;

			var player = (ClPlayer)entity;
			player.SetInteraction(Interactions.KEY_E, entered ? Interactions.E_PROCESSOR : Interactions.NONE);
			return Task.CompletedTask;
		}
	}
}