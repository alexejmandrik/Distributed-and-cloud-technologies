using MedicalCenter.Server.Models;
using MedicalCenter.Server.Protocol;

namespace MedicalCenter.Server;

public class MedicalCenterService
{
    private readonly List<Patient> patients = new();
    private readonly List<Doctor> doctors = new();
    private readonly List<Service> services = new();
    private readonly List<Appointment> appointments = new();
    private readonly List<Payment> payments = new();

    private int nextPatientId = 1;
    private int nextAppointmentId = 1;

    public MedicalCenterService()
    {
        doctors.Add(new Doctor
        {
            Id = 1,
            FullName = "Петров Петр Петрович",
            Specialization = "Терапевт"
        });

        doctors.Add(new Doctor
        {
            Id = 2,
            FullName = "Сидорова Анна Сергеевна",
            Specialization = "Кардиолог"
        });

        services.Add(new Service
        {
            Id = 1,
            Name = "Первичный прием",
            Price = 50
        });

        services.Add(new Service
        {
            Id = 2,
            Name = "Консультация специалиста",
            Price = 80
        });
    }

    public Response ProcessRequest(Request request)
    {
        try
        {
            return request.Operation switch
            {
                "RegisterPatient" => RegisterPatient(request),

                "BookAppointment" => BookAppointment(request),

                "CancelAppointment" => CancelAppointment(request),

                "CompleteAppointment" => CompleteAppointment(request),

                _ => new Response
                {
                    Success = false,
                    Message = "Неизвестная операция",
                    Error = $"Операция '{request.Operation}' не поддерживается"
                }
            };
        }
        catch (Exception ex)
        {
            return new Response
            {
                Success = false,
                Message = "Ошибка обработки запроса",
                Error = ex.Message
            };
        }
    }


    private Response RegisterPatient(Request request)
    {
        if (!request.Data.TryGetValue("fullName", out var fullName) ||
            string.IsNullOrWhiteSpace(fullName))
        {
            return new Response
            {
                Success = false,
                Message = "Некорректные входные данные",
                Error = "Необходимо указать ФИО пациента"
            };
        }

        if (!request.Data.TryGetValue("birthDate", out var birthDateString) ||
            !DateTime.TryParse(birthDateString, out var birthDate))
        {
            return new Response
            {
                Success = false,
                Message = "Некорректные входные данные",
                Error = "Некорректная дата рождения"
            };
        }

        if (!request.Data.TryGetValue("phone", out var phone) ||
            string.IsNullOrWhiteSpace(phone))
        {
            return new Response
            {
                Success = false,
                Message = "Некорректные входные данные",
                Error = "Необходимо указать номер телефона"
            };
        }

        var patient = new Patient
        {
            Id = nextPatientId++,
            FullName = fullName,
            BirthDate = birthDate,
            Phone = phone
        };

        patients.Add(patient);

        Console.WriteLine(
            $"[EVENT] PatientRegistered: пациент #{patient.Id} зарегистрирован");

        return new Response
        {
            Success = true,
            Message = "Пациент успешно зарегистрирован",
            Data = patient
        };
    }



    private Response BookAppointment(Request request)
    {
        if (!request.Data.TryGetValue("patientId", out var patientIdString) ||
            !int.TryParse(patientIdString, out var patientId))
        {
            return new Response
            {
                Success = false,
                Message = "Некорректные входные данные",
                Error = "Некорректный идентификатор пациента"
            };
        }

        if (!request.Data.TryGetValue("doctorId", out var doctorIdString) ||
            !int.TryParse(doctorIdString, out var doctorId))
        {
            return new Response
            {
                Success = false,
                Message = "Некорректные входные данные",
                Error = "Некорректный идентификатор врача"
            };
        }

        if (!request.Data.TryGetValue("serviceId", out var serviceIdString) ||
            !int.TryParse(serviceIdString, out var serviceId))
        {
            return new Response
            {
                Success = false,
                Message = "Некорректные входные данные",
                Error = "Некорректный идентификатор услуги"
            };
        }

        if (!request.Data.TryGetValue("dateTime", out var dateTimeString) ||
            !DateTime.TryParse(dateTimeString, out var dateTime))
        {
            return new Response
            {
                Success = false,
                Message = "Некорректные входные данные",
                Error = "Некорректная дата приема"
            };
        }

        var patient = patients.FirstOrDefault(x => x.Id == patientId);

        if (patient == null)
        {
            return new Response
            {
                Success = false,
                Message = "Объект не найден",
                Error = "Пациент не найден"
            };
        }

        var doctor = doctors.FirstOrDefault(x => x.Id == doctorId);

        if (doctor == null)
        {
            return new Response
            {
                Success = false,
                Message = "Объект не найден",
                Error = "Врач не найден"
            };
        }

        var service = services.FirstOrDefault(x => x.Id == serviceId);

        if (service == null)
        {
            return new Response
            {
                Success = false,
                Message = "Объект не найден",
                Error = "Услуга не найдена"
            };
        }

        var appointment = new Appointment
        {
            Id = nextAppointmentId++,
            PatientId = patientId,
            DoctorId = doctorId,
            ServiceId = serviceId,
            DateTime = dateTime,
            Status = AppointmentStatus.Scheduled
        };

        appointments.Add(appointment);

        Console.WriteLine(
            $"[EVENT] AppointmentCreated: запись #{appointment.Id} создана");

        return new Response
        {
            Success = true,
            Message = "Запись на прием успешно создана",
            Data = appointment
        };
    }



    private Response CancelAppointment(Request request)
    {
        if (!request.Data.TryGetValue("appointmentId", out var idString) ||
            !int.TryParse(idString, out var appointmentId))
        {
            return new Response
            {
                Success = false,
                Message = "Некорректные входные данные",
                Error = "Некорректный идентификатор записи"
            };
        }

        var appointment = appointments
            .FirstOrDefault(x => x.Id == appointmentId);

        if (appointment == null)
        {
            return new Response
            {
                Success = false,
                Message = "Объект не найден",
                Error = "Запись на прием не найдена"
            };
        }

        if (appointment.Status != AppointmentStatus.Scheduled)
        {
            return new Response
            {
                Success = false,
                Message = "Операция невозможна",
                Error = "Можно отменить только активную запись"
            };
        }

        appointment.Status = AppointmentStatus.Cancelled;

        Console.WriteLine(
            $"[EVENT] AppointmentCancelled: запись #{appointment.Id} отменена");

        return new Response
        {
            Success = true,
            Message = "Запись успешно отменена",
            Data = appointment
        };
    }




    private Response CompleteAppointment(Request request)
    {
        if (!request.Data.TryGetValue("appointmentId", out var idString) ||
            !int.TryParse(idString, out var appointmentId))
        {
            return new Response
            {
                Success = false,
                Message = "Некорректные входные данные",
                Error = "Некорректный идентификатор записи"
            };
        }

        var appointment = appointments
            .FirstOrDefault(x => x.Id == appointmentId);

        if (appointment == null)
        {
            return new Response
            {
                Success = false,
                Message = "Объект не найден",
                Error = "Запись на прием не найдена"
            };
        }

        if (appointment.Status != AppointmentStatus.Scheduled)
        {
            return new Response
            {
                Success = false,
                Message = "Операция невозможна",
                Error = "Можно завершить только активную запись"
            };
        }

        appointment.Status = AppointmentStatus.Completed;

        var service = services
            .First(x => x.Id == appointment.ServiceId);

        var payment = new Payment
        {
            Id = payments.Count + 1,
            AppointmentId = appointment.Id,
            Amount = service.Price,
            PaymentDate = DateTime.Now,
            IsPaid = true
        };

        payments.Add(payment);

        Console.WriteLine(
            $"[EVENT] AppointmentCompleted: запись #{appointment.Id} завершена");

        return new Response
        {
            Success = true,
            Message = "Прием успешно завершен",
            Data = new
            {
                Appointment = appointment,
                Payment = payment
            }
        };
    }
}