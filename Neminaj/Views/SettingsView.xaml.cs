using CommunityToolkit.Maui.Views;
using Neminaj.ContentViews;
using Neminaj.Events;
using Neminaj.GlobalEnums;
using Neminaj.GlobalText;
using Neminaj.Interfaces;
using Neminaj.Models;
using Neminaj.Repositories;
using SharedTypesLibrary.DTOs.API;
using SharedTypesLibrary.Models.API.DatabaseModels;

namespace Neminaj.Views;

public partial class SettingsView : ContentPage
{
    // Events
    public delegate void CheckBoxCompany_Changed(object sender, CompanyCheckBoxChanged_EventArgs e);
    public static event CheckBoxCompany_Changed OnCheckBoxCompany_Changed;

    private CompanyRepository _companyRepo { get; set; } = null;
    private ISettingsService _settingsService { get; set; } = null;

    private IConnectivity _connectivityService { get; set; } = null;


    PopUpActivityIndicator _popUpIndic = new PopUpActivityIndicator("Načítavam nastavenia ...");

    private bool PageBuilded = false;

    public SettingsView(CompanyRepository companyRepository, ISettingsService settingsService, IConnectivity connectivityService)
    {
        InitializeComponent();
        _companyRepo = companyRepository;
        _settingsService = settingsService;
        _connectivityService = connectivityService;

        SettingsContentView.OnCheckBoxCompanyContentView_Changed += OnCheckBoxCompanyContentView_Changed_Event;

        this.Disappearing += async (s, e) => { await SettingsView_OnDisappearing(); };
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        await CheckBuildAndBuildIfNeeded();
    }

    private void OnCheckBoxCompanyContentView_Changed_Event(object sender, CompanyCheckBoxChanged_EventArgs args)
    {
        // Make sure someone is listening to event
        if (OnCheckBoxCompany_Changed != null)
        {
            CompanyCheckBoxChanged_EventArgs argsForwarded = new CompanyCheckBoxChanged_EventArgs(args.CompanyId, args.Name);
            OnCheckBoxCompany_Changed(this, argsForwarded);
        }
    }

    private async Task SettingsView_OnDisappearing()
    {
        if (PageBuilded)
        {
            if (!await ISettingsService.ContainsStatic("SettingsAtLeastOnceSaved"))
                await _settingsService.Save("SettingsAtLeastOnceSaved", true);
        }
    }

    public static List<int> GetCheckedAndSavedCompaniesFromSettings(List<CompanyDTO> listCompanies)
    {
        List<int> listCompaniesIdsChoosed = new List<int>();

        foreach (CompanyDTO com in listCompanies)
        {
            if (ISettingsService.GetStatic($"{com.Name}_SettingsChkBox_Id_{com.Id}", true).Result)
                listCompaniesIdsChoosed.Add(com.Id);
        }

        return listCompaniesIdsChoosed;
    }

    public async Task CheckBuildAndBuildIfNeeded()
    {
        if (_connectivityService.NetworkAccess == NetworkAccess.Internet && !PageBuilded)
        {
            this.Content = this._popUpIndic;
            await BuildPage();
            this.Content = this.MainControlWrapper;
        }
        else if (!PageBuilded)
        {
            await this.DisplayAlert("Chyba", "Zariadenie nemá pripojenie k internetu\r\nNie je možné načítať položky", "Zavrieť");
        }
    }

    public async Task BuildPage()
    {
        await this.SettingsContentView.InitContentView(_companyRepo, _settingsService);
        PageBuilded = true;
    }

    public async Task<List<int>> GetListIdsCheckedCompanies()
    {
        List<int> listCheckedCompanies = new List<int>();

        List<CompanyDTO> listComp = await _companyRepo.GetAllCompaniesAsync();

        foreach (CompanyDTO comp in listComp)
        {
            // if Company was not stored as saved than
            if (await _settingsService.Get<bool>($"{comp.Name}_SettingsChkBox_Id_{comp.Id}", false))
            {
                listCheckedCompanies.Add(comp.Id);
            }
        }

        return listCheckedCompanies;
    }
}
