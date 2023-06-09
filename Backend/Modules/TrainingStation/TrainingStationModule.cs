using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Backend.Controllers.Event.Interface;
using Backend.Controllers.TrainingStation.Interface;
using Backend.Data;
using Backend.Utils.Enums;
using Backend.Utils.Interfaces.Events;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;

namespace Backend.Modules.TrainingStation;

public class TrainingStationModule : Module<TrainingStationModule>, IEventColshape
{
    private readonly ITrainingStationController _trainingStationController;

    public TrainingStationModule(IEventController eventController, ITrainingStationController trainingStationController) : base("TrainingStation")
    {
        _trainingStationController = trainingStationController;

        eventController.OnClient("Server:TrainingStation:Start", Start);
    }

    private async void Start(ClPlayer player, string eventKey)
    {
        if (player.DbModel == null! || player.CurrentShape == null) return;

        var shape = player.CurrentShape;
        if (shape.ShapeType != ColshapeType.TRAINING_STATION) return;

        var stationId = shape.Id;
        var stationData = await _trainingStationController.GetTrainingStation(stationId);
        if (stationData == null!) return;

        if (stationData.Occupied)
        {
            await player.Notify("Training", "Diese Trainings Station wird bereits verwendet!", NotificationType.ERROR);
            return;
        }

        if (player.DbModel.Strength >= 125)
        {
            await player.Notify("Training", "Du hast heute schon genug Trainiert!", NotificationType.ERROR);
            return;
        }

        stationData.OccupantId = player.DbModel.Id;
        stationData.Occupied = true;

        player.IsTraining = true;
        player.TrainingStart = DateTime.Now;

        await player.Freeze(true);
        await player.SetPosition(stationData.Position.Position);
        player.Rotation = new Rotation(0, 0, stationData.Position.H);

        var animationDictionary = stationData.Type == TrainingType.PULL_UPS ? "amb@prop_human_muscle_chin_ups@male@base" :
            stationData.Type == TrainingType.BENCH_PRESS ? "amb@world_human_sit_ups@male@idle_a" :
            "amb@world_human_muscle_free_weights@male@barbell@base";

        var animationBase = stationData.Type == TrainingType.PULL_UPS ? "base" :
            stationData.Type == TrainingType.BENCH_PRESS ? "idle_a" :
            "base";

        player.PlayAnimation(animationDictionary, animationBase, 33);

        await player.StartInteraction(() =>
        {
            if (player == null!) return;

            player.DbModel.Strength = (float) Math.Clamp(player.DbModel.Strength + 0.5, 80, 125);
            player.Emit("Client:PlayerModule:SetRunSpeedMultiplier", Math.Clamp(player.DbModel.Strength / 100, 1, 1.1));
        }, 1000 * 60 * 2, async () =>
        {
            stationData.OccupantId = 0;
            stationData.Occupied = false;
            player.IsTraining = false;
            await player.Freeze(false);
        });
    }

    public async Task OnColshape(ClShape shape, IEntity entity, bool entered)
    {
        if (entity.Type != BaseObjectType.Player || shape.ShapeType != ColshapeType.TRAINING_STATION || !entered) return;

        ClPlayer player = (ClPlayer)entity;
        if (player == null!) return;

        player.SetInteraction(Interactions.KEY_E, Interactions.E_TRAINING);
    }
}