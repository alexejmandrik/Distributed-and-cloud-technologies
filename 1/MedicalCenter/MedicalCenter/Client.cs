using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using MedicalCenter.Server.Protocol;

namespace MedicalCenter.Client;

public class Client
{
    private const string Host = "127.0.0.1";
    private const int Port = 5000;

    public async Task<Response?> SendRequestAsync(Request request)
    {
        try
        {
            using var client = new TcpClient();

            await client.ConnectAsync(Host, Port);

            using NetworkStream stream = client.GetStream();

            var options = new JsonSerializerOptions
            {
                Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
            };

            string json = JsonSerializer.Serialize(request, options);

            Console.WriteLine($"Отправка запроса: {json}");

            byte[] requestBytes = Encoding.UTF8.GetBytes(json);

            await stream.WriteAsync(requestBytes);

            byte[] buffer = new byte[8192];

            int bytesRead = await stream.ReadAsync(buffer);

            if (bytesRead == 0)
            {
                Console.WriteLine("Сервер не вернул ответ.");
                return null;
            }

            string responseJson = Encoding.UTF8.GetString(
                buffer,
                0,
                bytesRead);

            Console.WriteLine($"Получен ответ: {responseJson}");

            return JsonSerializer.Deserialize<Response>(responseJson);
        }
        catch (SocketException)
        {
            Console.WriteLine("Ошибка: сервер недоступен.");
        }
        catch (IOException)
        {
            Console.WriteLine("Ошибка сетевого взаимодействия.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }

        return null;
    }
}