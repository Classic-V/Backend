namespace Backend.Utils.Models.ClothesShop;

public class ClothingModel
{
    public int Component { get; set; } = 0;
    public int Drawable { get; set; } = 0;
    public int Texture { get; set; } = 0;
    public uint Dlc { get; set; } = 0;

    public ClothingModel() { }

    public ClothingModel(int component, int drawable, int texture, uint dlc)
    {
        Component = component;
        Drawable = drawable;
        Texture = texture;
        Dlc = dlc;
    }
}