using Neminaj.Interfaces;
using Neminaj.Repositories;
using Region = SharedTypesLibrary.Models.API.DatabaseModels.Region;
using SharedTypesLibrary.Models.API.DatabaseModels;
using SharedTypesLibrary.DTOs.API;
using Neminaj.Views;
using System.Timers;
using Neminaj.Events;

namespace Neminaj.ContentViews;

public partial class SettingsContentView : ContentView
{
    public delegate void CheckBoxCompany_Changed(object sender, CompanyCheckBoxChanged_EventArgs e);
    public static event CheckBoxCompany_Changed OnCheckBoxCompanyContentView_Changed;

    private CompanyRepository _companyRepo { get; set; } = null;
    private ISettingsService _settingsService { get; set; } = null;

    List<CompanyDTO> _listCompany = new List<CompanyDTO>();
    List<CompanySettingModel> _listCompanySettings { get; set; } = new List<CompanySettingModel>();

    private List<Region> _listRegions { get; set; } = null;

    private  Dictionary<int, List<District>> _dicDistrict { get; set; } = null;

    System.Timers.Timer TimerSliderStoreDistance = null;

    double SliderValue = 0;

    bool WasRegionPickerLoaded = false;
    int FirstLoadedRegionPickerIndex = 0;

    public SettingsContentView()
	{
        InitializeComponent();

        TimerSliderStoreDistance = new System.Timers.Timer();
        TimerSliderStoreDistance.Elapsed += new System.Timers.ElapsedEventHandler(OnTimerSliderStoreDistance);
        TimerSliderStoreDistance.Interval = 3000;
        TimerSliderStoreDistance.Enabled = false;
    }

    public async Task InitContentView(CompanyRepository companyRepository, ISettingsService settingsService)
    {
        _companyRepo = companyRepository;
        _settingsService = settingsService;

        SetRegionList();
        this.RegionPicker.ItemsSource = _listRegions;
        this.RegionPicker.ItemDisplayBinding = new Binding("Name");

        SetDistrictList();
        this.DistrictPicker.ItemsSource = _dicDistrict[1];
        this.DistrictPicker.ItemDisplayBinding = new Binding("Name");

        await LoadCompaniesSettings();
        await LoadDistanceSetting();
        await LoadRegionSetting();
        await LoadDistrictSetting();
    }

    private async Task LoadCompaniesSettings()
    {
        List<CompanySettingModel> listStored = new List<CompanySettingModel>();
        _listCompany = await _companyRepo.GetAllCompaniesAsync();

        foreach (CompanyDTO comp in _listCompany)
        {
            // if Company was not before saved than it will be automatically checked ( used in compare )
            bool isChecked = await _settingsService.Get<bool>($"{comp.Name}_SettingsChkBox_Id_{comp.Id}", true);

            _listCompanySettings.Add
            (
                new CompanySettingModel { CompanyName = comp.Name, Id = comp.Id, ImageUrl = comp.Url, IsChecked = isChecked }
            );
        }

        this.listCompanySetting.ItemsSource = _listCompanySettings;
    }

    private async Task LoadDistanceSetting()
    {
        this.Slider.Value = await _settingsService.Get<double>(nameof(Slider), 10.0d);
    }

    private async void OnTimerSliderStoreDistance(object source, ElapsedEventArgs e)
    {
        await _settingsService.Save(nameof(Slider), Slider.Value);
        TimerSliderStoreDistance.Enabled = false;
    }

    private async Task LoadRegionSetting()
    {
        string strChoosenRegion = await _settingsService.Get<string>(nameof(Region), "Bratislavský");

        int indexRegion = this.RegionPicker.Items.IndexOf(strChoosenRegion);
        this.RegionPicker.SelectedIndex = indexRegion;
        FirstLoadedRegionPickerIndex = indexRegion;
        WasRegionPickerLoaded = true;
    }

    private async Task LoadDistrictSetting()
    {
        string strChoosenDistrict = await _settingsService.Get<string>(nameof(District), "Bratislava I");

        int indexDistrict = this.DistrictPicker.Items.IndexOf(strChoosenDistrict);
        this.DistrictPicker.SelectedIndex = indexDistrict;
    }

    private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        if (!TimerSliderStoreDistance.Enabled)
            TimerSliderStoreDistance.Enabled = true;
    }

    private void RegionPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        Picker regionPicker = (Picker)sender;

        int selectedIndex = regionPicker.SelectedIndex;

        if (selectedIndex != -1)
        {
            this.DistrictPicker.ItemsSource = _dicDistrict[selectedIndex];

            // Workaround with dealing not selected District
            // 1. Do not override user already stored District
            // 2. After choosing region set first possible District
            if (FirstLoadedRegionPickerIndex != selectedIndex && WasRegionPickerLoaded)
            {
                this.DistrictPicker.SelectedIndex = 0;
            }
        }
    }

    private async void DistrictPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        Picker districtPicker = (Picker)sender;

        int selectedIndex = districtPicker.SelectedIndex;

        if (selectedIndex != -1)
        {
            // Region Index Save
            if ((Region)this.RegionPicker.SelectedItem != null)
            {
                await _settingsService.Save(nameof(Region), ((Region)this.RegionPicker.SelectedItem).Name); 
            }

            District choosenDistrict = (District)this.DistrictPicker.SelectedItem;

            // District Index Save
            await _settingsService.Save(nameof(District), choosenDistrict.Name);
        }
    }

    private async void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        CheckBox checkBox = sender as CheckBox;

        if (checkBox.AutomationId != null)
        {
            CompanySettingModel compSetModel = _listCompanySettings.Where(compSet => compSet.Id == int.Parse(checkBox.AutomationId)).First();
            await _settingsService.Save($"{compSetModel.CompanyName}_SettingsChkBox_Id_{compSetModel.Id}", compSetModel.IsChecked);

            // Make sure someone is listening to event
            if (OnCheckBoxCompanyContentView_Changed != null)
            {
                CompanyCheckBoxChanged_EventArgs args = new CompanyCheckBoxChanged_EventArgs(compSetModel.Id, compSetModel.CompanyName);
                OnCheckBoxCompanyContentView_Changed(this, args);
            }
        }
    }

    private void SetRegionList()
    {
        _listRegions = new List<Region>();
        _listRegions.Add(new Region { Id = 0, Name = "Bratislavský" });
        _listRegions.Add(new Region { Id = 1, Name = "Trnavský" });
        _listRegions.Add(new Region { Id = 2, Name = "Trenčianský" });
        _listRegions.Add(new Region { Id = 3, Name = "Nitrianský" });
        _listRegions.Add(new Region { Id = 4, Name = "Žilinský" });
        _listRegions.Add(new Region { Id = 5, Name = "Banskobystrický" });
        _listRegions.Add(new Region { Id = 6, Name = "Prešovský" });
        _listRegions.Add(new Region { Id = 7, Name = "Košický" });
    }

    private void SetDistrictList()
    {
        _dicDistrict = new Dictionary<int, List<District>>();

        // Bratislavský
        _dicDistrict.Add
        (
            0,
            new List<District>()
            {
                new District { Id = 1, Name = "Bratislava I" },
                new District { Id = 2, Name = "Bratislava II"},
                new District { Id = 3, Name = "Bratislava III"},
                new District { Id = 4, Name = "Bratislava IV"},
                new District { Id = 5, Name = "Bratislava V"},
                new District { Id = 5, Name = "Malacky"},
                new District { Id = 7, Name = "Pezinok"},
                new District { Id = 8, Name = "Senec"},
            }
        );

        // Trnavský
        _dicDistrict.Add
        (
            1,
            new List<District>()
            {
                new District { Id = 9, Name = "Dunajská Streda" },
                new District { Id = 10, Name = "Galanta"},
                new District { Id = 11, Name = "Hlohovec"},
                new District { Id = 12, Name = "Piešťany"},
                new District { Id = 13, Name = "Senica"},
                new District { Id = 14, Name = "Skalica"},
                new District { Id = 15, Name = "Trnava"},
            }
        );

        // Trenčianský
        _dicDistrict.Add
        (
            2,
            new List<District>()
            {
                new District { Id = 16, Name = "Bánovce nad Bebravou" },
                new District { Id = 17, Name = "Ilava"},
                new District { Id = 18, Name = "Myjava"},
                new District { Id = 19, Name = "Nové Mesto nad Váhom"},
                new District { Id = 20, Name = "Partizánske"},
                new District { Id = 21, Name = "Považská Bystrica"},
                new District { Id = 22, Name = "Prievidza"},
                new District { Id = 23, Name = "Púchov"},
                new District { Id = 24, Name = "Trenčín"},
            }
        );

        // Nitrianský
        _dicDistrict.Add
        (
            3,
            new List<District>()
            {
                new District { Id = 1, Name = "Komárno" },
                new District { Id = 2, Name = "Levice"},
                new District { Id = 3, Name = "Nitra"},
                new District { Id = 4, Name = "Nové Zámky"},
                new District { Id = 5, Name = "Šaľa"},
                new District { Id = 6, Name = "Zlaté Moravce"},
            }
        );

        // Žilinský
        _dicDistrict.Add
        (
            4,
            new List<District>()
            {
                new District { Id = 1, Name = "Bytča" },
                new District { Id = 2, Name = "Čadca"},
                new District { Id = 3, Name = "Dolný Kubín"},
                new District { Id = 4, Name = "Kysucké Nové Mesto"},
                new District { Id = 5, Name = "Liptovský Mikuláš"},
                new District { Id = 6, Name = "Martin"},
                new District { Id = 7, Name = "Námestovo"},
                new District { Id = 8, Name = "Ružomberok"},
                new District { Id = 9, Name = "Turčianske Teplice"},
                new District { Id = 10, Name = "Tvrdošín"},
                new District { Id = 11, Name = "Žilina"}
            }
        );

        // Bánsko Bystrický
        _dicDistrict.Add
        (
            5,
            new List<District>()
            {
                new District { Id = 1, Name = "Banská Bystrica" },
                new District { Id = 2, Name = "Banská Štiavnica"},
                new District { Id = 3, Name = "Brezno"},
                new District { Id = 4, Name = "Detva"},
                new District { Id = 5, Name = "Krupina"},
                new District { Id = 6, Name = "Lučenec"},
                new District { Id = 7, Name = "Poltár"},
                new District { Id = 8, Name = "Revúca"},
                new District { Id = 9, Name = "Rimavská Sobota"},
                new District { Id = 10, Name = "Veľký Krtíš"},
                new District { Id = 11, Name = "Zvolen"},
                new District { Id = 12, Name = "Žarnovica"},
                new District { Id = 13, Name = "Žiar nad Hronom"}
            }
        );

        // Prešovský
        _dicDistrict.Add
        (
            6,
            new List<District>()
            {
                new District { Id = 1, Name = "Bardejov" },
                new District { Id = 2, Name = "Humenné"},
                new District { Id = 3, Name = "Kežmarok"},
                new District { Id = 4, Name = "Levoča"},
                new District { Id = 5, Name = "Medzilaborce"},
                new District { Id = 6, Name = "Poprad"},
                new District { Id = 7, Name = "Prešov"},
                new District { Id = 8, Name = "Sabinov"},
                new District { Id = 9, Name = "Snina"},
                new District { Id = 10, Name = "Stará Ľubovňa"},
                new District { Id = 11, Name = "Stropkov"},
                new District { Id = 12, Name = "Svidník"},
                new District { Id = 13, Name = "Vranov nad Topľou"}
            }
        );

        // Košický
        _dicDistrict.Add
        (
            7,
            new List<District>()
            {
                new District { Id = 1, Name = "Gelnica" },
                new District { Id = 2, Name = "Košice I"},
                new District { Id = 3, Name = "Košice II"},
                new District { Id = 4, Name = "Košice III"},
                new District { Id = 5, Name = "Košice IV"},
                new District { Id = 6, Name = "Košice - okolie"},
                new District { Id = 7, Name = "Michalovce"},
                new District { Id = 8, Name = "Rožňava"},
                new District { Id = 9, Name = "Sobrance"},
                new District { Id = 10, Name = "Spišská Nová Ve"},
                new District { Id = 11, Name = "Trebišov"},
            }
        );
    }
}