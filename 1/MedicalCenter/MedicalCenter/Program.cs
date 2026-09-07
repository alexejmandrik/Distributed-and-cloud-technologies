using System.Text.Json;
using MedicalCenter.Server.Protocol;

namespace MedicalCenter.Client;

public class Program
{
    private static readonly Client client = new();

    public static async Task Main()
    {
        while (true)
        {
            Console.Clear();

            Console.WriteLine("=================================");
            Console.WriteLine("       МЕДИЦИНСКИЙ ЦЕНТР");
            Console.WriteLine("=================================");
            Console.WriteLine("1. Зарегистрировать пациента");
            Console.WriteLine("2. Записать пациента на прием");
            Console.WriteLine("3. Отменить запись");
            Console.WriteLine("4. Завершить прием");
            Console.WriteLine("0. Выход");
            Console.WriteLine("=================================");
            Console.Write("Выберите операцию: ");

            string? choice = Console.ReadLine();

            Console.Clear();

            switch (choice)
            {
                case "1":
                    await RegisterPatient();
                    break;

                case "2":
                    await BookAppointment();
                    break;

                case "3":
                    await CancelAppointment();
                    break;

                case "4":
                    await CompleteAppointment();
                    break;

                case "0":
                    return;

                default:
                    Console.WriteLine(
                        "Некорректная операция.");
                    break;
            }

            Console.WriteLine();
            Console.WriteLine("Нажмите Enter...");
            Console.ReadLine();
        }
    }





    private static async Task RegisterPatient()
    {
        Console.WriteLine("=== Регистрация пациента ===");

        Console.Write("ФИО: ");
        string? fullName = Console.ReadLine();

        Console.Write("Дата рождения (например, 10.05.2000): ");
        string? birthDate = Console.ReadLine();

        Console.Write("Телефон: ");
        string? phone = Console.ReadLine();

        var request = new Request
        {
            Operation = "RegisterPatient",
            Data = new Dictionary<string, string>
            {
                ["fullName"] = fullName ?? "",
                ["birthDate"] = birthDate ?? "",
                ["phone"] = phone ?? ""
            }
        };

        Response? response =
            await client.SendRequestAsync(request);

        PrintResponse(response);
    }



    private static async Task BookAppointment()
    {
        Console.WriteLine("=== Запись на прием ===");

        Console.Write("ID пациента: ");
        string? patientId = Console.ReadLine();

        Console.Write("ID врача: ");
        string? doctorId = Console.ReadLine();

        Console.Write("ID услуги: ");
        string? serviceId = Console.ReadLine();

        Console.Write("Дата и время приема: ");
        string? dateTime = Console.ReadLine();

        var request = new Request
        {
            Operation = "BookAppointment",
            Data = new Dictionary<string, string>
            {
                ["patientId"] = patientId ?? "",
                ["doctorId"] = doctorId ?? "",
                ["serviceId"] = serviceId ?? "",
                ["dateTime"] = dateTime ?? ""
            }
        };

        Response? response =
            await client.SendRequestAsync(request);

        PrintResponse(response);
    }





    private static async Task CancelAppointment()
    {
        Console.WriteLine("=== Отмена записи ===");

        Console.Write("ID записи: ");
        string? appointmentId = Console.ReadLine();

        var request = new Request
        {
            Operation = "CancelAppointment",
            Data = new Dictionary<string, string>
            {
                ["appointmentId"] = appointmentId ?? ""
            }
        };

        Response? response =
            await client.SendRequestAsync(request);

        PrintResponse(response);
    }




    private static async Task CompleteAppointment()
    {
        Console.WriteLine("=== Завершение приема ===");

        Console.Write("ID записи: ");
        string? appointmentId = Console.ReadLine();

        var request = new Request
        {
            Operation = "CompleteAppointment",
            Data = new Dictionary<string, string>
            {
                ["appointmentId"] = appointmentId ?? ""
            }
        };

        Response? response =
            await client.SendRequestAsync(request);

        PrintResponse(response);
    }




    private static void PrintResponse(Response? response)
    {
        if (response == null)
        {
            Console.WriteLine("Ответ от сервера отсутствует.");
            return;
        }

        Console.WriteLine();

        if (response.Success)
        {
            Console.WriteLine("УСПЕШНО");
            Console.WriteLine(response.Message);

            if (response.Data != null)
            {
                Console.WriteLine(response.Data);
            }
        }
        else
        {
            Console.WriteLine("ОШИБКА");
            Console.WriteLine(response.Message);

            if (!string.IsNullOrEmpty(response.Error))
            {
                Console.WriteLine(
                    $"Описание: {response.Error}");
            }
        }
    }
}

