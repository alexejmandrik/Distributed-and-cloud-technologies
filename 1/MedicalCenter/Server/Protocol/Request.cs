namespace MedicalCenter.Server.Protocol;

public class Request
{
    public string Operation { get; set; } = string.Empty;

    public Dictionary<string, string> Data { get; set; } = new();
}