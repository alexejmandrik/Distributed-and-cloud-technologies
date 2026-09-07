namespace MedicalCenter.Server.Models;

public enum AppointmentStatus
{
    Scheduled,
    Cancelled,
    Completed
}

public class Appointment
{
    public int Id { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public int ServiceId { get; set; }

    public DateTime DateTime { get; set; }

    public AppointmentStatus Status { get; set; }
}