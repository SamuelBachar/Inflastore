using Neminaj.Interfaces;
using Neminaj.Models;
using Neminaj.Utils;
using SharedTypesLibrary.DTOs.API;
using SharedTypesLibrary.Models.API;
using SharedTypesLibrary.Models.API.ServiceResponseModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Neminaj.Services;

public class RegisterService : IRegisterService
{
    public async Task<(UserRegisterDTO UserRegisterDTO, string ResultMessage)> RegisterHTTPS(UserRegisterRequest userRegisterRequest)
    {
        //HTTPS
        try
        {
            var httpClient = new HttpClientService().GetPlatformSpecificHttpClient();
            var baseUrl = DeviceInfo.Platform == DevicePlatform.Android
                ? "https://10.0.2.2:7279"
                : "https://localhost:7279";

            var responseTest = await httpClient.GetAsync($"{baseUrl}/WeatherForecast");
            var dataTest = await responseTest.Content.ReadAsStringAsync();

            var response = await httpClient.PostAsJsonAsync($"{baseUrl}/api/User/register", userRegisterRequest, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
            var serializedResponse = await response.Content.ReadFromJsonAsync<ServiceResponse<UserRegisterDTO>>();

            if (response.IsSuccessStatusCode)
            {
                return (new UserRegisterDTO { Email = serializedResponse.Data.Email }, serializedResponse.Message);
            }
            else if ((response.StatusCode == System.Net.HttpStatusCode.BadRequest) && (!serializedResponse.Success))
            {
                return (null, serializedResponse.Message);
            }
            else if ((response.StatusCode == System.Net.HttpStatusCode.BadRequest) && (serializedResponse.Data == null))
            {
                var responseString = await response.Content.ReadAsStringAsync();
                Dictionary<string, List<string>> dicGenericErrors = GenericHttpErrorReader.ExtractErrorsFromWebAPIResponse(responseString);

                var errorStr = string.Empty;

                foreach (var error in dicGenericErrors)
                {
                    foreach (var errorInfo in error.Value)
                        errorStr += errorInfo + "\r\n";
                }

                return (null, errorStr);
            }

            return (null, $"Neočakavaná odpoveď od servera, neznáma chyba");
        }
        catch (Exception ex)
        {
            return (null, $"Nastala chyba pri komunikácií so serverom. Chyba: {ex.Message}");
        }
    }
}
