namespace MedicalCenter.Server;

public class Program
{
    public static async Task Main()
    {
        var service = new MedicalCenterService();

        var server = new Server(service);

        await server.StartAsync();
    }
}