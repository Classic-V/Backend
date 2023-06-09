using Backend.Utils.Enums;

namespace Backend.Utils.Models.Database;

public class AnimationModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public AnimationCategoryType Category { get; set; }
    public string AnimationDictionary { get; set; }
    public string AnimationName { get; set; }
    public int AnimationFlag { get; set; }

    public AnimationModel(){}

    public AnimationModel(string name, AnimationCategoryType category, string animationDictionary, string animationName,
        int animationFlag)
    {
        Name = name;
        Category = category;
        AnimationDictionary = animationDictionary;
        AnimationName = animationName;
        AnimationFlag = animationFlag;
    }
}