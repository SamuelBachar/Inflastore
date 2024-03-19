using Microsoft.Maui.Controls;
using Neminaj.Interfaces;
using Neminaj.Repositories;
using SharedTypesLibrary.Models.API.DatabaseModels;
using SharedTypesLibrary.DTOs.API;
using System;
using System.Collections.Generic;
using System.IO;
using Region = SharedTypesLibrary.Models.API.DatabaseModels.Region;
using Neminaj.Events;
using Neminaj.Services;


namespace Neminaj.Views;

public class CompanySettingModel
{
    public string CompanyName { get; set; }
    public int Id { get; set; }
    public bool IsChecked { get; set; } = true;
    public string ImageUrl { get; set; }
}

public partial class FirstStartUpView : ContentPage
{
    readonly ISettingsService _settingsService = null;
    readonly IConnectivity _connectivityService = null;

    List<CompanyDTO> _listCompany = new List<CompanyDTO>();
    List<CompanySettingModel> _listCompanySettings { get; set; } = new List<CompanySettingModel>();

    CompanyRepository _companyRepo = null;

    List<Region> _listRegions { get; set; } = null;
    Dictionary<int, List<District>> _dicDistrict { get; set; } = null;

    bool WasFirstStartUp = false;

    public FirstStartUpView(CompanyRepository companyRepository, ISettingsService settingsService, IConnectivity connectivityService)
    {
        InitializeComponent();

        this.Title = "Prvotné nastavenie";

        _companyRepo = companyRepository;
        _settingsService = settingsService;
        _connectivityService = connectivityService;

        this.Disappearing += async (s, e) => { await FirstStartUp_OnDisappearing(); };
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (_connectivityService.NetworkAccess == NetworkAccess.Internet)
        {
            if (await ISettingsService.ContainsStatic("SettingsAtLeastOnceSaved"))
            {
                await Shell.Current.GoToAsync("//CategoryPickerView");
            }
            else
            {
                await BuildPage();
            }
        }
        else
        {
            if (await this.DisplayAlert("Chyba", "Zariadenie nemá pripojenie k internetu\r\nNie je možné načítať položky.\r\nZopakovať načítanie položiek ?", "Zopakovať", "Zavrieť"))
            {
                await BuildPage();
            }
        }
    }

    private async Task BuildPage()
    {
        if (_connectivityService.NetworkAccess == NetworkAccess.Internet)
        {
            await this.SettingsContentView.InitContentView(_companyRepo, _settingsService);
        }
        else
        {
            if (await this.DisplayAlert("Chyba", "Zariadenie nemá pripojenie k internetu\r\nNie je možné načítať položky.\r\nZopakovať načítanie položiek ?", "Zopakovať", "Zavrieť"))
            {
                await BuildPage();
            }
        }
    }

    private async void BtnConfirmed_Clicked(object sender, EventArgs e)
    {
        WasFirstStartUp = true;
        await Shell.Current.GoToAsync("//CategoryPickerView");
    }

    private async Task FirstStartUp_OnDisappearing()
    {
        if (WasFirstStartUp)
        {
            if (!await ISettingsService.ContainsStatic("SettingsAtLeastOnceSaved"))
                await _settingsService.Save<bool>("SettingsAtLeastOnceSaved", true);
        }
    }

}
