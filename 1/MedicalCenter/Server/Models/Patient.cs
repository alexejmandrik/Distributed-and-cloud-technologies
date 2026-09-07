using System;
using System.Collections.Generic;
using System.Text;

namespace MedicalCenter.Server.Models;
    public class Patient
    {
        public int Id { get; set; }

        public string FullName { get; set; } = string.Empty;

        public DateTime BirthDate { get; set; }

        public string Phone { get; set; } = string.Empty;
    }
