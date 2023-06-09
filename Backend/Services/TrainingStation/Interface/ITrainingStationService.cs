using Backend.Utils.Models.Database;

namespace Backend.Services.TrainingStation.Interface;

public interface ITrainingStationService
{
    List<TrainingStationModel> TrainingStations { get; }
    Task<TrainingStationModel?> GetTrainingStation(int id);
    Task AddTrainingStation(TrainingStationModel model);
}