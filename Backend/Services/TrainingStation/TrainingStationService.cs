using Backend.Services.TrainingStation.Interface;
using Backend.Utils;
using Backend.Utils.Models.Database;

namespace Backend.Services.TrainingStation;

public class TrainingStationService : ITrainingStationService
{
    public List<TrainingStationModel> TrainingStations { get; private set; }

    public TrainingStationService()
    {
        var ctx = new DBContext();
        TrainingStations = new List<TrainingStationModel>(ctx.TrainingStation);
    }

    public async Task<TrainingStationModel?> GetTrainingStation(int id)
    {
        return TrainingStations.FirstOrDefault(x => x.Id == id);
    }

    public async Task AddTrainingStation(TrainingStationModel model)
    {
        TrainingStations.Add(model);

        var ctx = new DBContext();
        ctx.TrainingStation.Add(model);
        await ctx.SaveChangesAsync();
    }
}