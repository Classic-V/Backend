using Backend.Utils.Models.Database;

namespace Backend.Controllers.TrainingStation.Interface;

public interface ITrainingStationController
{
    void LoadTrainingStation(TrainingStationModel model);
    Task<TrainingStationModel?> GetTrainingStation(int id);
}