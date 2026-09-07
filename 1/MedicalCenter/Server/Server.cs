using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using MedicalCenter.Server.Protocol;

namespace MedicalCenter.Server;

public class Server
{
    private const int Port = 5000;

    private readonly MedicalCenterService service;

    public Server(MedicalCenterService service)
    {
        this.service = service;
    }

    public async Task StartAsync()
    {
        var listener = new TcpListener(
            IPAddress.Any,
            Port);

        listener.Start();

        Console.WriteLine("=================================");
        Console.WriteLine("   MEDICAL CENTER SERVER");
        Console.WriteLine("=================================");
        Console.WriteLine($"Сервер запущен на порту {Port}");
        Console.WriteLine("Ожидание подключений...");

        while (true)
        {
            TcpClient client = await listener.AcceptTcpClientAsync();

            Console.WriteLine(
                $"Подключен клиент: {client.Client.RemoteEndPoint}");

            _ = HandleClientAsync(client);
        }
    }

    private async Task HandleClientAsync(TcpClient client)
    {
        try
        {
            using (client)
            using (NetworkStream stream = client.GetStream())
            {
                byte[] buffer = new byte[8192];

                int bytesRead = await stream.ReadAsync(buffer);

                if (bytesRead == 0)
                    return;

                string json = Encoding.UTF8.GetString(
                    buffer,
                    0,
                    bytesRead);

                Console.WriteLine($"Получен запрос: {json}");

                Request? request =
                    JsonSerializer.Deserialize<Request>(json);

                Response response;

                if (request == null)
                {
                    response = new Response
                    {
                        Success = false,
                        Message = "Некорректный запрос",
                        Error = "Не удалось распознать JSON"
                    };
                }
                else
                {
                    response = service.ProcessRequest(request);
                }

                var jsonOptions = new JsonSerializerOptions
                {
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                };

                string responseJson =
                    JsonSerializer.Serialize(response, jsonOptions);

                byte[] responseBytes =
                    Encoding.UTF8.GetBytes(responseJson);

                await stream.WriteAsync(responseBytes);

                Console.WriteLine(
                    $"Отправлен ответ: {responseJson}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(
                $"Ошибка сетевого взаимодействия: {ex.Message}");
        }
    }
}