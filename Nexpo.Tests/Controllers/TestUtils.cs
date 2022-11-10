﻿using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json.Linq;
using Nexpo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace Nexpo.Tests.Controllers
{
    public static class TestUtils
    {
        public static async Task<HttpClient> Login(string user)
        {
            var application = new WebApplicationFactory<Program>();
            var client = application.CreateClient();
            var json = new JsonObject();

            switch (user)
            {
                case "admin":
                    json.Add("email", "admin@example.com");
                    json.Add("password", "password");
                    break;
                case "student1":
                    json.Add("email", "student1@example.com");
                    json.Add("password", "password");
                    break;
                case "student2":
                    json.Add("email", "student2@example.com");
                    json.Add("password", "password");
                    break;
                case "student3":
                    json.Add("email", "student3@example.com");
                    json.Add("password", "password");
                    break;
                case "company1":
                    json.Add("email", "rep1@company1.example.com");
                    json.Add("password", "password");
                    break;
                case "company1rep2":
                    json.Add("email", "rep2@company1.example.com");
                    json.Add("password", "password");
                    break;
                case "company2":
                    json.Add("email", "rep1@company2.example.com");
                    json.Add("password", "password");
                    break;
                case "company3":
                    json.Add("email", "rep1@company3.example.com");
                    json.Add("password", "password");
                    break;
                case "company4":
                    json.Add("email", "rep1@company4.example.com");
                    json.Add("password", "password");
                    break;
                default:
                    return null; 
            }

            var payload = new StringContent(json.ToString(), Encoding.UTF8, "application/json");
            var response = await client.PostAsync("/api/session/signin", payload);
            string token = new StreamReader(response.Content.ReadAsStream()).ReadToEnd();
            var parser = JObject.Parse(token);
            token = "Bearer " + parser.Value<String>("token");
            client.DefaultRequestHeaders.Add("Authorization", token);
            return client;
        } 
    }
}
