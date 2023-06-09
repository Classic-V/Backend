using AltV.Net;
using AltV.Net.Data;
using Backend.Controllers.PostJob.Interface;
using Backend.Services.PostJob.Interface;
using Backend.Utils.Enums;
using Backend.Utils.Models;
using Backend.Utils.Models.Database;
using Backend.Utils.Models.Entities;
using Backend.Utils.Streamer;

namespace Backend.Controllers.PostJob;

public class PostJobController : IPostJobController
{
    private readonly IPostJobService _postJobService;

    public PostJobController(IPostJobService postJobService)
    {
        _postJobService = postJobService;

        _postJobService.PostJobs.ForEach(LoadPostJob);
        PedStreamer.AddObject(new Ped(0x7367324F, PostJobModel.StartPosition.X, PostJobModel.StartPosition.Y, PostJobModel.StartPosition.Z, 0, true, true, 0));

        var blip = Alt.CreateBlip(AltV.Net.Elements.Entities.BlipType.Destination, PostJobModel.StartPosition);
        blip.ShortRange = true;
        blip.Sprite = 616;
        blip.Name = "Post Job";
    }

    public void LoadPostJob(PostJobModel model)
    {
        var startShape = (ClShape)Alt.CreateColShapeSphere(PostJobModel.StartPosition, 2.5f);
        startShape.Id = model.Id;
        startShape.Size = 2.5f;
        startShape.Dimension = 0;
        startShape.ShapeType = ColshapeType.POST_JOB_START;

    }

    public async Task<PostJobModel> GetJob(int id)
    {
        return await _postJobService.GetJob(id);
    }
}