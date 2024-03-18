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
    ItemRepository _itemRepo = null;
    UnitRepository _unitRepo = null;
    SavedCartRepository _cartRepo = null;
    CategoryRepository _categoryRepository = null;
    ItemPriceRepository _itemPriceRepository = null;

    List<Region> _listRegions { get; set; } = null;
    Dictionary<int, List<District>> _dicDistrict { get; set; } = null;

    bool WasFirstStartUp = false;

    public FirstStartUpView(CompanyRepository companyRepository, ISettingsService settingsService, IConnectivity connectivityService
                            //ItemRepository itemRepo,
                            //UnitRepository unitRepo,
                            //SavedCartRepository cartRepo,
                            //CategoryRepository categoryRepository,
                            //ItemPriceRepository itemPriceRepository
                            )
    {
        InitializeComponent();

        this.Title = "Prvotné nastavenie";

        _companyRepo = companyRepository;
        _settingsService = settingsService;
        _connectivityService = connectivityService;

        SetRegionList();
        this.RegionPicker.ItemsSource = _listRegions;
        this.RegionPicker.ItemDisplayBinding = new Binding("Name");

        SetDistrictList();
        this.DistrictPicker.ItemsSource = _dicDistrict[1];
        this.DistrictPicker.ItemDisplayBinding = new Binding("Name");

        this.Disappearing += async (s, e) => { await FirstStartUp_OnDisappearing(); };
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (await ISettingsService.ContainsStatic("SettingsAtLeastOnceSaved"))
        {
            await Shell.Current.GoToAsync("//CategoryPickerView");
        }
        else
        {
            _listCompany = await _companyRepo.GetAllCompaniesAsync();
            _listCompany.ForEach(comp => _listCompanySettings.Add
            (
                new CompanySettingModel { CompanyName = comp.Name, Id = comp.Id, ImageUrl = comp.Url, IsChecked = true }
            ));

            this.listCompanySetting.ItemsSource = _listCompanySettings;
        }
    }

    private async void BtnConfirmed_Clicked(object sender, EventArgs e)
    {
        bool badEnty = false;

        if (RegionPicker.SelectedIndex == -1)
        {
            RegionPicker.Title = "Zvoľte kraj kvôli presným cenám";
            RegionPicker.TitleColor = Colors.OrangeRed;
            badEnty = true;
        }

        if (DistrictPicker.SelectedIndex == -1)
        {
            DistrictPicker.Title = "Zvoľte okres kvôli presným cenám";
            DistrictPicker.TitleColor = Colors.OrangeRed;
            badEnty = true;
        }

        await Task.Run(async () =>
        {
            if (!badEnty)
            {
                WasFirstStartUp = true;
                await Shell.Current.GoToAsync("//CategoryPickerView");
            }
        });
    }

    private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        double value = (double)e.NewValue;

        if (value.ToString().Contains("."))
            LabelKm.Text = value.ToString().Substring(0, value.ToString().IndexOf(".") + 2);

        if (value.ToString().Contains(","))
            LabelKm.Text = value.ToString().Substring(0, value.ToString().IndexOf(",") + 2);
    }

    private void RegionPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        Picker picker = (Picker)sender;

        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            this.DistrictPicker.ItemsSource = _dicDistrict[selectedIndex];
        }
    }

    private void DistrictPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
    }

    private void CheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        CheckBox checkBox = sender as CheckBox;

        if (checkBox.AutomationId != null)
        {
            CompanySettingModel compSetModel = _listCompanySettings.Where(compSet => compSet.Id == int.Parse(checkBox.AutomationId)).First();
        }
    }

    private async Task FirstStartUp_OnDisappearing()
    {
        if (WasFirstStartUp)
        {
            await _settingsService.Save(nameof(Slider), Slider.Value);

            foreach (CompanySettingModel compSetModel in _listCompanySettings)
            {
                await _settingsService.Save($"{compSetModel.CompanyName}_SettingsChkBox_Id_{compSetModel.Id}", compSetModel.IsChecked);
            }

            if (!await ISettingsService.ContainsStatic("SettingsAtLeastOnceSaved"))
                await _settingsService.Save("SettingsAtLeastOnceSaved", true);

            District choosenDistrict = (District)this.DistrictPicker.SelectedItem;

            await _settingsService.Save(nameof(District), choosenDistrict.Id);
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