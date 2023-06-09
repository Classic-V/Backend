using AltV.Net;
using Backend.Controllers.Workstation.Interface;
using Backend.Services.Workstation.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using Backend.Utils.Streamer;

namespace Backend.Controllers.Workstation
{
	public class WorkstationController : IWorkstationController
	{
		private readonly IWorkstationService _workstationService;

		public WorkstationController(IWorkstationService workstationService)
		{
			_workstationService = workstationService;

			_workstationService.Workstations.ForEach(LoadWorkstation);
		}

		public void LoadWorkstation(WorkstationModel model)
		{
			var shape = (ClShape)Alt.CreateColShapeSphere(model.Position, 2f);
			shape.Id = model.Id;
			shape.ShapeType = ColshapeType.WORKSTATION;
			shape.Size = 2f;

			PedStreamer.AddObject(new Ped(0xE7565327, model.Ped.X, model.Ped.Y, model.Ped.Z, model.Ped.H, true, true, 0));
		}
	}
}