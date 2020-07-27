using System;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using PortainerApiClient.Models;

namespace PortainerApiClient
{
    class Program
    {
        static void Main(string[] args)
        {
            HttpClient httpClient = new HttpClient { BaseAddress = new Uri("http://portainer:9000/") };

            Thread.Sleep(5000);

            Console.WriteLine("Calling Portainer API");

            HttpResponseMessage response = httpClient.PostAsync("api/auth", new StringContent("{ \"Username\": \"admin\", \"Password\": \"piDgeonsR4u!\" }")).Result;
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Authorization request failed. Status code: {response.StatusCode}, Reason: {response.ReasonPhrase}");
                return;
            }

            string jsonOutput = response.Content.ReadAsStringAsync().Result;
            Console.WriteLine($"JSON response: {jsonOutput}");

            if (!jsonOutput.StartsWith("{\"jwt\":\""))
            {
                Console.WriteLine("JWT token not found");
            }

            string jwt = jsonOutput[8..^3];
            Console.WriteLine($"JWT: {jwt}");

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", jwt);

            // Create local endpoint
            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("Name", "local-endpoint"),
                new KeyValuePair<string, string>("EndpointType", "1")
            });

            response = httpClient.PostAsync("api/endpoints", formContent).Result;
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Create endpoint request failed. Status code: {response.StatusCode}, Reason: {response.ReasonPhrase}");
                return;
            }

            jsonOutput = response.Content.ReadAsStringAsync().Result;
//            Console.WriteLine($"JSON response: {jsonOutput}");

            int index = jsonOutput.IndexOf("\"Id\":");
            string dockerEndpointId = jsonOutput.Substring(index + 5);
            dockerEndpointId = dockerEndpointId.Substring(0, dockerEndpointId.IndexOf(','));
            Console.WriteLine($"Endpoint ID: {dockerEndpointId}");

            var containers = ListContainers(httpClient, dockerEndpointId);

            // Kill the 'calculator-api' container
            Console.WriteLine("\nKilling the calculator-api container");
            string containerId = containers.Where(x => x.Image.Equals("calculator-api")).Select(x => x.Id).FirstOrDefault();

            response = httpClient.PostAsync($"api/endpoints/{dockerEndpointId}/docker/containers/{containerId}/kill", null).Result;
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Kill container request failed. Status code: {response.StatusCode}, Reason: {response.ReasonPhrase}");
                return;
            }

            Thread.Sleep(100);

            while (true)
            {
                ListContainers(httpClient, dockerEndpointId);
                Thread.Sleep(5000);
            }
        }

        private static IEnumerable<Container> ListContainers(HttpClient httpClient, string endpointId)
        {
            HttpResponseMessage response = httpClient.GetAsync($"api/endpoints/{endpointId}/docker/containers/json").Result;
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Container list request failed. Status code: {response.StatusCode}, Reason: {response.ReasonPhrase}");
                return null;
            }

            string jsonOutput = response.Content.ReadAsStringAsync().Result;
            var containers = JsonSerializer.Deserialize<List<Container>>(jsonOutput);

            Console.WriteLine("\nContainers:");
            Console.WriteLine("--------------");

            foreach (var container in containers)
            {
                Console.WriteLine($"ID: {container.Id}");
                Console.WriteLine($"Image: {container.Image}");
                Console.WriteLine($"State: {container.State}");
                Console.WriteLine($"Status: {container.Status}");
                Console.WriteLine("--------------");
            }

            return containers;
        }
    }
}
