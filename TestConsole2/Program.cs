using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var httpClient = new HttpClient())
            {

                var baseUrl = "https://localhost:7279";

                // Test neovereny ucet
                //UserLoginRequest userLoginRequest = new UserLoginRequest { Email = "user@example1.com", Password = "string" };
                //var response = await httpClient.PostAsJsonAsync($"{baseUrl}/api/User/login", userLoginRequest, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                //ServiceResponse<UserLoginDTO>? serializedResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<UserLoginDTO>>();

                //Console.WriteLine("Test neovereneho uctu");

                //Console.WriteLine(serializedResponse.Message);
                //Console.WriteLine(serializedResponse.ExceptionMessage);
                //Console.WriteLine(serializedResponse.Success);

                // Test prihlasenie pomocou nie email formatu

                Console.WriteLine("Test zleho formatu emailu");

                UserLoginRequest userLoginRequest1 = new UserLoginRequest { Email = "userexample1.com", Password = "string" };
                var response = await httpClient.PostAsJsonAsync($"{baseUrl}/api/User/login", userLoginRequest1, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                var serializedResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<UserLoginDTO>>();

                var response1 = await response.Content.ReadAsStringAsync();
                Console.WriteLine(response1);

                Console.WriteLine(serializedResponse.Message);
                Console.WriteLine(serializedResponse.ExceptionMessage);
                Console.WriteLine(serializedResponse.Success);

                // Vysledok: nedojde sprava ani do Controllera tym padom treba sledovat 

                int a = 0;
            }
        }

        public static Dictionary<string, List<string>> ExtractErrorsFromWebAPIResponse(string body)
        {
            var response = new Dictionary<string, List<string>>();

            var jsonElement = JsonSerializer.Deserialize<JsonElement>(body);
            var errorsJsonElement = jsonElement.GetProperty("errors");

            foreach (var fieldWithErrors in errorsJsonElement.EnumerateObject())
            {
                var field = fieldWithErrors.Name;
                var errors = new List<string>();

                foreach (var errorKind in fieldWithErrors.Value.EnumerateArray())
                {
                    var error = errorKind.GetString();
                    errors.Add(error);
                }

                response.Add(field, errors);
            }

            return response;
        }
    }
}
