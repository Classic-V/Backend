using AltV.Net.Data;
using AltV.Net.Elements.Entities;
using Backend.Controllers.Event.Interface;
using Backend.Controllers.Jail.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Interfaces.Events;
using Backend.Utils.Models;
using Backend.Utils.Models.Entities;
using Backend.Utils.Models.Player;

namespace Backend.Modules.Jail;

public class JailModule : Module<JailModule>, IEventColshape, IEventIntervalMinute
{
    private readonly IJailController _jailController;

    public JailModule(IEventController eventController, IJailController jailController) : base("Jail")
    {
        _jailController = jailController;

        eventController.OnClient<int>("Server:Jail:Imprison", Imprison);
    }

    private async void Imprison(ClPlayer player, string eventKey, int id)
    {
        if (player.DbModel == null || player.DbModel.Team > 2) return;

        var shape = player.CurrentShape;
        if (shape == null || shape.ShapeType != ColshapeType.JAIL_IMPRISON) return;

        ClPlayer prisoner = ClPlayer.All.FirstOrDefault(x => x.DbModel.Id == id)!;
        if (prisoner == null! || !prisoner.HasCrimes())
        {
            await player.Notify("Inhaftierung", "Der Spieler hat keine offene Akte!", NotificationType.ERROR);
            return;
        }

        var duration = await _jailController.GetPlayerJailDuration(prisoner.DbModel.Id);
        var fine = await _jailController.GetPlayerJailFine(prisoner.DbModel.Id);

        if (duration <= 0)
        {
            await player.Notify("Inhaftierung", "Der Spieler hat keine Hafteinheiten!", NotificationType.ERROR);
            return;
        }

        if (prisoner.Position.Distance(player.Position) > 15)
        {
            await player.Notify("Inhaftierung", "Der Spieler ist zu weit entfernt!", NotificationType.INFO);
            return;
        }

        if ((prisoner.DbModel.BankMoney - fine) <= 0)
        {
            var extendedFine = fine - prisoner.DbModel.BankMoney;
            if (extendedFine >= 5000 && extendedFine <= 10000)
            {
                duration += (int)duration / 100 * 15;
            }
            else if (extendedFine >= 10001)
            {
                duration += (int)duration / 100 * 25;
            }

            prisoner.DbModel.BankMoney = 0;
        }

        prisoner.DbModel.Jailtime = duration;
        prisoner.DbModel.BankMoney -= fine;

        await prisoner.SetPosition(new Position(1691.6044f, 2565.956f, 45.9f));
        await prisoner.ApplyClothes(player.DbModel.Customization.Gender == 1
            ? ClothesModel.MalePrisonClothes
            : ClothesModel.FemalePrisonClothes);
        await prisoner.Notify("Inhaftierung",
            $"Du wurdest mit {duration} Hafteinheiten und {fine}$ Geldstrafe Inhaftiert.", NotificationType.INFO);
        prisoner.DbModel.Crimes = new List<PlayerCrimeModel>();
    }

    public async Task OnColshape(ClShape shape, IEntity entity, bool entered)
    {
        if (entity.Type != BaseObjectType.Player || entity == null! || shape == null!) return;

        ClPlayer player = (ClPlayer)entity;
        if (player == null!) return;
        if (shape.ShapeType != ColshapeType.JAIL) return;

        player.IsInPrison = entered;
    }

    public Task OnEveryMinute()
    { 
        ClPlayer.All.Where(x => x.DbModel != null && x.DbModel.Alive && x.DbModel.Jailtime > 0).ToList().ForEach(async player =>
        {
            player.DbModel.Jailtime--;

            if (player.DbModel.Jailtime <= 0)
            {
                if (player.HasCrimes())
                {
                    var duration = await _jailController.GetPlayerJailDuration(player.DbModel.Id);
                    var fine = await _jailController.GetPlayerJailFine(player.DbModel.Id);

                    if (duration <= 0)
                    {
                        await player.Notify("Inhaftierung", "Der Spieler hat keine Hafteinheiten!", NotificationType.ERROR);
                        return;
                    }

                    if ((player.DbModel.BankMoney - fine) <= 0)
                    {
                        var extendedFine = fine - player.DbModel.BankMoney;
                        if (extendedFine >= 5000 && extendedFine <= 10000)
                        {
                            duration += (int)duration / 100 * 15;
                        }
                        else if (extendedFine >= 10001)
                        {
                            duration += (int)duration / 100 * 25;
                        }

                        player.DbModel.BankMoney = 0;
                    }

                    player.DbModel.Jailtime = duration;
                    player.DbModel.BankMoney -= fine;

                    await player.Notify("Gefängnis", "Du hast eine Aktenerweiterung bekommen!", NotificationType.INFO);
                    await player.Notify("Inhaftierung",
                        $"Du wurdest mit {duration} Hafteinheiten und {fine}$ Geldstrafe Inhaftiert.", NotificationType.INFO);
                    player.DbModel.Crimes = new List<PlayerCrimeModel>();

                    if (player.IsInPrison)
                    {
                        await player.ApplyClothes(player.DbModel.Customization.Gender == 1
                            ? ClothesModel.MalePrisonClothes
                            : ClothesModel.FemalePrisonClothes);
                    }

                    return;
                }

                // TODO: set player position outside jail
                var shape = player.CurrentShape;
                if (shape == null || shape.ShapeType != ColshapeType.JAIL)
                {
                    await player.Notify("Gefängnis", "Du hast deine Hafteinheiten abgesessen!", NotificationType.INFO);
                    return;
                }

                var prisonId = shape.Id;
                var prison = await _jailController.GetJail(prisonId);
                if (prison == null) return;

                await player.SetPosition(prison.ReleasePoint);
                await player.Notify("Gefängnis", "Du hast deine Hafteinheiten abgesessen!", NotificationType.INFO);
                return;
            }
        });

        return Task.CompletedTask;
    }
}