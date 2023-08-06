using Neminaj.Interfaces;
using Neminaj.Models;
using Neminaj.Utils;
using SharedTypesLibrary.DTOs.API;
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

public class LoginService : ILoginService
{
    public async Task<(UserLoginInfo UserInfo, string ResultMessage)> Login(string userName, string passWord)
    {
        //try
        //{
        //    if (Connectivity.Current.NetworkAccess == NetworkAccess.Internet)
        //    {
        //        UserInfo userInfo = new UserInfo();
        //        HttpClient httpClient = new HttpClient();
        //        string url = $"https://localhost:7279/api/User/login/{userName}/{passWord}";
        //        httpClient.BaseAddress = new Uri(url);

        //        HttpResponseMessage response = await httpClient.GetAsync(string.Empty);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            userInfo = await response.Content.ReadFromJsonAsync<UserInfo>();
        //            return (UserInfo: await Task.FromResult(userInfo), ResultMessage: "Úspešne prihlásenie");
        //        }
        //        else
        //        {
        //            return (UserInfo: null, "Chybné meno alebo heslo");
        //        }
        //    }
        //    else
        //    {
        //        return (UserInfo: null, ResultMessage: "Nie je možné sa prihlásiť\r\n. Zariadenie nemá pripojenie k internetu");
        //    }
        //}
        //catch (Exception ex)
        //{
        //    return (null, $"Nastala chyba pri komunikácií so serverom. Chyba: \r\n: {ex.Message}");
        //}

        //HTTP
        var httpClient = new HttpClient();
        var baseUrl = DeviceInfo.Platform == DevicePlatform.Android ?
            "http://10.0.2.2:5192"
            :
            "http://localhost:5912";

        var response = await httpClient.GetAsync($"{baseUrl}/WeatherForecast");

        var data = await response.Content.ReadAsStringAsync();

        return (null, $"Nastala chyba pri komunikácií so serverom. Chyba: \r\n: test");
    }

    public async Task<(UserLoginDTO UserInfo, string ResultMessage)> LoginHTTPS(string email, string passWord)
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

            UserLoginRequest userLoginRequest = new UserLoginRequest { Email = email, Password = passWord };
            var response = await httpClient.PostAsJsonAsync($"{baseUrl}/api/User/login", userLoginRequest, new JsonSerializerOptions() { PropertyNameCaseInsensitive = true });
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

            return (null, $"Neočakavaná odpoveď od servera, neznáma chyba");
        }
        catch (Exception ex)
        {
            return (null, $"Nastala chyba pri komunikácií so serverom. Chyba: {ex.Message}");
        }
    }
}
