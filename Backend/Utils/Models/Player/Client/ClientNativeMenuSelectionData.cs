namespace Backend.Utils.Models.Player.Client;

public class ClientNativeMenuSelectionData
{
    public string Label { get; set; }
    public object? Value { get; set; }
    
    public ClientNativeMenuSelectionData(string label, object? value)
    {
        Label = label;
        Value = value;
    }
}