﻿using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.ApplicationModel;
using Neminaj.Interfaces;
using Neminaj.Models;
using Neminaj.Repositories;
using Neminaj.Services;
using Neminaj.Views;
using SharedTypesLibrary.DTOs.API;
using SharedTypesLibrary.Models.API.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neminaj.ViewsModels;

[QueryProperty(nameof(NavigationShopRepository), nameof(NavigationShopRepository))]
[QueryProperty(nameof(ListCompaniesIds), nameof(ListCompaniesIds))]
[QueryProperty(nameof(CompanyRepository), nameof(CompanyRepository))]

public partial class NavigationShopViewModel : ObservableObject
{
    [ObservableProperty]
    private NavigationShopRepository navigationShopRepository;

    [ObservableProperty]
    private CompanyRepository companyRepository;

    [ObservableProperty]
    private List<int> listCompaniesIds;

    public IMap Map;
    public IGeolocation GeoLocation;
    public ISettingsService SettingsService;

    public NavigationShopViewModel(IMap map, ISettingsService settingsService, IGeolocation geoLocation)
    {
        this.Map = map;
        this.SettingsService = settingsService;
        this.GeoLocation = geoLocation;
    }

    public async Task<List<NavigationShopData>> GetAllNavigShopDataAsync()
    {
        return await NavigationShopRepository.GetAllNavigShopDataAsync();
    }

    public async Task<List<NavigationShopData>> GetChoosenCompaniesNavigShopsDataAsync()
    {
        return await NavigationShopRepository.GetSpecificCompaniesNavigShopsDataAsync(ListCompaniesIds);
    }

    public async Task<List<CompanyDTO>> GetChoosenCompaniesData()
    {
        return await CompanyRepository.GetSpecificCompaniesAsync(ListCompaniesIds);
    }

    public async Task OpenMapWithChoosenCompanies(FoundedShop foundedShop)
    {
        await Map.OpenAsync(
        foundedShop.Latitude,
        foundedShop.Longtitude, 

        new MapLaunchOptions
        {
            Name = $"{foundedShop.CompanyName} - {foundedShop.FullAddress}",
            NavigationMode = NavigationMode.Driving
        });
    }
}
