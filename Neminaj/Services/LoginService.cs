using Neminaj.Interfaces;
using Neminaj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using static System.Net.WebRequestMethods;

namespace Neminaj.Services;

public class LoginService : ILoginService
{
    public async Task<(UserInfo UserInfo, string ResultMessage)> Login(string userName, string passWord)
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

    public async Task<(UserInfo UserInfo, string ResultMessage)> LoginHTTPS(string userName, string passWord)
    {
        //HTTPS
        var httpClient = new HttpClientService().GetPlatformSpecificHttpClient();
        var baseUrl = DeviceInfo.Platform == DevicePlatform.Android
            ? "https://10.0.2.2:7279"
            : "https://localhost:7279";

        UserLoginRequest userLoginRequest = new UserLoginRequest { Email = userName, Password = passWord };
        var response = await httpClient.PostAsJsonAsync($"{baseUrl}/api/User/login", userLoginRequest);

        if (response.IsSuccessStatusCode)
        {
            var data = await response.Content.ReadAsStringAsync();

        }
        else
        {
            // todo
        }


        return (null, $"Nastala chyba pri komunikácií so serverom. Chyba: \r\n: test");
    }
}
