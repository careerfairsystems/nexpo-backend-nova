using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Nexpo.Models;
using Nexpo.DTO;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Xunit;


namespace Nexpo.Tests.Controllers
{ 
    public class NotificationsControllerTests
    {

        [Fact]
        public async Task NotifyAllSuccess(){
            
            // Setup
            var client = await TestUtils.Login("admin");

            var dto = new NotificationDTO{
                Title = "Test",
                Message = "Test",
                Date = "2021-05-05"
            };

            var payload = new StringContent(dto.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/Notification/NotifyAll", payload);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }


        [Fact]
        public async Task NotifyAllUnauthorized(){
            
            // Setup
            var client = await TestUtils.Login("student");

            var dto = new NotificationDTO{
                Title = "Test",
                Message = "Test",
                Date = "2021-05-05"
            };

            var payload = new StringContent(dto.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PutAsync("/api/Notification/NotifyAll", payload);

            // Assert
            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }


        [Fact]
        public async Task GetAllSuccess(){
            
            // Setup
            var client = await TestUtils.Login("student");

            var dto = new NotificationDTO{
                Title = "Test",
                Message = "Test",
                Date = "2021-05-05"
            };

            //Get all notifications
            var payload = new StringContent(dto.ToString(), Encoding.UTF8, "application/json");
            await client.PutAsync("/api/Notification/NotifyAll", payload);

            var response = await client.GetAsync("/api/Notification/GetAll");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }

        [Fact]
        public async Task GetNLatestSuccess(){
                
            // Setup
            var client = await TestUtils.Login("student");

            var dto = new NotificationDTO{
                Title = "Test",
                Message = "Test",
                Date = "2021-05-05"
            };

            //make notification
            var payload = new StringContent(dto.ToString(), Encoding.UTF8, "application/json");
            await client.PutAsync("/api/Notification/NotifyAll", payload);

            //get notification
            var response = await client.GetAsync("/api/Notification/GetNLatest/1");
    
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetNLatestNotFound(){
                
            // Setup
            var client = await TestUtils.Login("student");

            var dto = new NotificationDTO{
                Title = "Test",
                Message = "Test",
                Date = "2021-05-05"
            };

            //make notification
            var payload = new StringContent(dto.ToString(), Encoding.UTF8, "application/json");
            await client.PutAsync("/api/Notification/NotifyAll", payload);
    
            //get notification
            var response = await client.GetAsync("/api/Notification/GetNLatest/100");
    
            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }


    }
}

