using AltV.Net;
using AltV.Net.Data;
using Backend.Controllers.TrainingStation.Interface;
using Backend.Services.TrainingStation.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using Backend.Utils.Streamer;

namespace Backend.Controllers.TrainingStation;

public class TrainingStationController : ITrainingStationController
{
    private readonly ITrainingStationService _trainingStationService;

    public TrainingStationController(ITrainingStationService trainingStationService)
    {
        _trainingStationService = trainingStationService;

        _trainingStationService.TrainingStations.ForEach(LoadTrainingStation);
    }

    public void LoadTrainingStation(TrainingStationModel model)
    {
        var shape = (ClShape)Alt.CreateColShapeSphere(model.Position.Position, 0.5f);
        shape.Id = model.Id;
        shape.Dimension = 0;
        shape.ShapeType = ColshapeType.TRAINING_STATION;
        shape.Size = 0.5f;
    }

    public async Task<TrainingStationModel?> GetTrainingStation(int id)
    {
        return await _trainingStationService.GetTrainingStation(id);
    }
}