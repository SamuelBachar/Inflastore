using Neminaj.Constants;
using Neminaj.Interfaces;
using Neminaj.Models;
using Neminaj.Utils;
using SharedTypesLibrary.DTOs.API;
using SharedTypesLibrary.Models.API.ServiceResponseModel;
using System.Net.Http.Json;
using System.Text.Json;

namespace Neminaj.Services;

public class LoginService : ILoginService
{
    private readonly IHttpClientFactory _httpClientFactory;
    public LoginService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<(UserLoginDTO UserInfo, string ResultMessage)> LoginHTTPS(string email, string passWord)
    {
        //HTTPS
        try
        {
            if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
            {
                var httpClient = _httpClientFactory.CreateClient(AppConstants.HttpsClientName);

                UserLoginRequest userLoginRequest = new UserLoginRequest { Email = email, Password = passWord };
                var response = await httpClient.PostAsJsonAsync($"/api/User/login", userLoginRequest, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
                var serializedResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<UserLoginDTO>>();

                if (response.IsSuccessStatusCode)
                {
                    return (new UserLoginDTO { Email = serializedResponse.Data.Email, JWT = serializedResponse.Data.JWT }, serializedResponse.Message);
                }
                else if ((response.StatusCode == System.Net.HttpStatusCode.BadRequest) && (!serializedResponse.Success))
                {
                    return (null, serializedResponse.Message);
                }
                else if ((response.StatusCode == System.Net.HttpStatusCode.BadRequest) && (serializedResponse.Data == null))
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    Dictionary<string, List<string>> dicGenericErrors = GenericHttpErrorReader.ExtractErrorsFromWebAPIResponse(responseString);

                    var temp = string.Empty;

                    foreach (var error in dicGenericErrors)
                    {
                        foreach (var errorInfo in error.Value)
                            temp += errorInfo + "\r\n";
                    }

                    return (null, temp);
                }

                return (null, ResultMessage: $"Neočakavaná odpoveď od servera, neznáma chyba");
            }
            else
            {
                return (null, ResultMessage: "Nie je možné sa prihlásiť\r\n. Zariadenie nemá pripojenie k internetu");
            }
        }
        catch (Exception ex)
        {
            return (null, $"Nastala chyba pri komunikácií so serverom. Chyba: {ex.Message}");
        }
    }
}
