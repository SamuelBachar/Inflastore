// See https://aka.ms/new-console-template for more information
using SharedTypesLibrary.DTOs.API;
using SharedTypesLibrary.Models.API;
using SharedTypesLibrary.Models.API.ServiceResponseModel;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;
using System.Text.Json;

Console.WriteLine("Hello, World!");

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

    var responseDic = new Dictionary<string, List<string>>();

    var jsonElement = JsonSerializer.Deserialize<JsonElement>(response1);
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

        responseDic.Add(field, errors);
    }

    // Vysledok: nedojde sprava ani do Controllera tym padom treba sledovat 

    foreach (var fieldWithErrors in responseDic)
    {
        Console.WriteLine($"-{fieldWithErrors.Key}");

        foreach (var error in fieldWithErrors.Value)
        {
            Console.WriteLine($"{error}");
        }
    }
}

class UserLoginRequestTest
{
    [Required, EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required]
    public string Password { get; set; } = string.Empty;
};
