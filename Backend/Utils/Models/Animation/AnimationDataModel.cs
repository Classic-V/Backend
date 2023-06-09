namespace Backend.Utils.Models.Animation;

public class AnimationDataModel
{
    public int Id { get; set; }
    public string Label { get; set; }

    public AnimationDataModel() {}

    public AnimationDataModel(int id, string label)
    {
        Id = id;
        Label = label;
    }
}