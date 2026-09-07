using System;
using System.Collections.Generic;
using System.Text;
using MedicalCenter.Server;
using MedicalCenter.Server.Protocol;

namespace UnitTest
{
    public class MedicalCenterTests
    {
        [Fact]
        public void RegisterPatient_ValidData_ReturnsSuccess()
        {
            var service = new MedicalCenterService();

            var request = new Request
            {
                Operation = "RegisterPatient",
                Data = new Dictionary<string, string>
                {
                    ["fullName"] = "Иванов Иван Иванович",
                    ["birthDate"] = "15.03.1995",
                    ["phone"] = "80291234567"
                }
            };

            var response = service.ProcessRequest(request);

            Assert.True(response.Success);
            Assert.Equal("Пациент успешно зарегистрирован", response.Message);
            Assert.NotNull(response.Data);
        }


        [Fact]
        public void UnknownOperation_ReturnsError()
        {
            var service = new MedicalCenterService();

            var request = new Request
            {
                Operation = "UnknownOperation",
                Data = new Dictionary<string, string>()
            };

            var response = service.ProcessRequest(request);

            Assert.False(response.Success);
            Assert.Equal("Неизвестная операция", response.Message);
            Assert.NotNull(response.Error);
        }


        [Fact]
        public void RegisterPatient_InvalidBirthDate_ReturnsError()
        {
            var service = new MedicalCenterService();

            var request = new Request
            {
                Operation = "RegisterPatient",
                Data = new Dictionary<string, string>
                {
                    ["fullName"] = "Иванов Иван Иванович",
                    ["birthDate"] = "abc",
                    ["phone"] = "80291234567"
                }
            };

            var response = service.ProcessRequest(request);

            Assert.False(response.Success);
            Assert.Equal("Некорректные входные данные", response.Message);
            Assert.NotNull(response.Error);
        }


        [Fact]
        public async Task Client_ServerUnavailable_ReturnsNull()
        {
            var client = new MedicalCenter.Client.Client();

            var request = new Request
            {
                Operation = "RegisterPatient",
                Data = new Dictionary<string, string>
                {
                    ["fullName"] = "Петров Петр Петрович",
                    ["birthDate"] = "10.05.1990",
                    ["phone"] = "80291112233"
                }
            };

            var response = await client.SendRequestAsync(request);

            Assert.Null(response);
        }
    }
}