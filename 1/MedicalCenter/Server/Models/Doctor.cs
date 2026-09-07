using System;
using System.Collections.Generic;
using System.Text;


namespace MedicalCenter.Server.Models;
public class Doctor
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public string Specialization { get; set; } = string.Empty;
    }
