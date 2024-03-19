using Neminaj.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neminaj.Services;

public class SettingsService : ISettingsService
{
    public Task<T> Get<T>(string key, T defaultValue) // todo make async
    {
        var result = Preferences.Default.Get<T>(key, defaultValue);
        return Task.FromResult(result);
    }

    //public Task<T> GetStatic<T>(string key, T defaultValue) // todo make async
    //{
    //    var result = Preferences.Default.Get<T>(key, defaultValue);
    //    return Task.FromResult(result);
    //}

    public Task Save<T>(string key, T value) // todo make async
    { 
        Preferences.Default.Set<T>(key, value);
        return Task.CompletedTask;
    }

    //public static Task<bool> ContainsStatic(string key) // todo make async
    //{
    //    bool result = Preferences.Default.ContainsKey(key);
    //    return Task.FromResult(result);
    //}
}
