﻿using Neminaj.Events;
using Neminaj.GlobalEnums;
using Neminaj.GlobalText;
using Neminaj.Interfaces;
using Neminaj.Models;
using Neminaj.Repositories;

namespace Neminaj.Views;

public class CompanyLoaded
{
    public int Id { get; set; }

    public string Name { get; set; }

    public ImageSource Image { get; set; }
}

public class CompanyCheckboxesState
{
    public string Id { get; set; }
    public string Name { get; set; }

    public bool IsChecked { get; set; }
}

public partial class SettingsView : ContentPage
{
    // Events
    public delegate void CheckBoxCompany_Changed(object sender, CompanyCheckBoxChanged_EventArgs e);
    public static event CheckBoxCompany_Changed OnCheckBoxCompany_Changed;

    private CompanyRepository CompanyRepo { get; set; } = null;
    private ISettingsService SettingsService { get; set; } = null;

    private List<Company> ListComp { get; set; } = null;
    private List<CompanyCheckboxesState> ListCompanyCheckboxesState = new();

    private Slider slider = null;
    private Label LabelKm = null;

    public SettingsView(CompanyRepository companyRepository, ISettingsService settingsService)
    {
        InitializeComponent();
        CompanyRepo = companyRepository;
        SettingsService = settingsService;

        this.Loaded += async (s, e) => { await BuildPage(); };
        this.Disappearing += async (s, e) => { await SettingsView_OnDisappearing(); };
    }

    public ISettingsService GetSettingService()
    {
        return this.SettingsService;
    }

    private async Task SettingsView_OnDisappearing()
    {
        await SettingsService.Save(nameof(slider), slider.Value);

        foreach (CompanyCheckboxesState compChkBoxState in ListCompanyCheckboxesState)
        {
            await SettingsService.Save($"{compChkBoxState.Name}_SettingsChkBox_Id_{compChkBoxState.Id}", compChkBoxState.IsChecked);
        }

        if (!await ISettingsService.ContainsStatic("SettingsAtLeastOnceSaved"))
            await SettingsService.Save("SettingsAtLeastOnceSaved", true);
    }

    public static List<int> GetCheckedAndSavedCompaniesFromSettings(List<Company> listCompanies)
    {
        List<int> listCompaniesIdsChoosed = new List<int>();

        foreach (Company com in listCompanies)
        {
            if (ISettingsService.GetStatic($"{com.Name}_SettingsChkBox_Id_{com.Id}", true).Result)
                listCompaniesIdsChoosed.Add(com.Id);
        }

        return listCompaniesIdsChoosed;
    }

    private void Slider_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        double value = (double)e.NewValue;
        LabelKm.Text = value.ToString().Substring(0, value.ToString().IndexOf(".") > 0 ? value.ToString().IndexOf(".") + 2 : value.ToString().Length);
    }

    public async Task BuildPage()
    {
        //this.Title = "Nastavenia";

        // START: Create MAIN GRID of View ///
        Grid gridMain = new Grid
        {
            // Fix column definition
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star),
            },
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto), // for Grid Customers
                new RowDefinition(GridLength.Auto), // for Price Comparer mode
                new RowDefinition(GridLength.Auto), // for Distance (km) within which to search shops navigation
            }
        };

        gridMain.Margin = 10;
        gridMain.RowSpacing = 10;
        gridMain.VerticalOptions = LayoutOptions.Start;
        // END: Create MAIN GRID of View ///

        int maxCompaniesPerRow = 3;
        int numberOfRows = 0;
        int numberOfColumns = 6;
        ListComp = await CompanyRepo.GetAllCompaniesAsync();

        Grid gridCustomers = new Grid
        {
            // Fix column definition
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star), // chkBox
                new ColumnDefinition(GridLength.Star), // Image
                new ColumnDefinition(GridLength.Star), // chkBox
                new ColumnDefinition(GridLength.Star), // Image
                new ColumnDefinition(GridLength.Star), // chkBox
                new ColumnDefinition(GridLength.Star), // Image
            }
        };

        gridCustomers.ColumnSpacing = 5;
        gridCustomers.RowSpacing = 20;
        gridCustomers.HorizontalOptions = LayoutOptions.StartAndExpand;
        gridCustomers.VerticalOptions = LayoutOptions.Start;


        // START: add label Výber obchodov

        // Add one row for text
        gridCustomers.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

        Label labelChooseShops = new Label
        {
            Text = "Výber obchodov",
            FontSize = 14,
            FontAttributes = FontAttributes.Bold
        };

        Grid.SetColumnSpan(labelChooseShops, 6);
        gridCustomers.Add(labelChooseShops, 0, 0);
        // END: add label Výber obchodov


        // Add grid rows
        numberOfRows += (ListComp.Count / maxCompaniesPerRow) + (ListComp.Count % maxCompaniesPerRow > 0 ? 1 : 0);

        for (int i = 0; i < numberOfRows; i++)
        {
            gridCustomers.RowDefinitions.Add(new RowDefinition() { Height = 50 });
        }

        // Add chkBoxes and Images
        int helpCounter = 0;
        bool breakLoop = false;
        int rowOffset = 1; // 1 because of one row for text
        ListCompanyCheckboxesState.Clear(); // clear Company check boxes state list

        for (int row = rowOffset; row < (numberOfRows + rowOffset); row++)
        {
            // One row == max 3 companies (combination chkBox and picture)
            for (int col = 0; col < numberOfColumns; col += 2)
            {
                CheckBox chkBox = new CheckBox
                {
                    AutomationId = ListComp[helpCounter].Id.ToString(), // Used to distinguish companies (selected and unselected companies)
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                };

                // load saved status of choosing company
                chkBox.IsChecked = await SettingsService.Get<bool>($"{ListComp[helpCounter].Name}_SettingsChkBox_Id_{ListComp[helpCounter].Id}", true);
                chkBox.CheckedChanged += CompanyCheckBoxChanged;
                ListCompanyCheckboxesState.Add(new CompanyCheckboxesState
                {
                    Id = chkBox.AutomationId,
                    Name = ListComp[helpCounter].Name,
                    IsChecked = chkBox.IsChecked
                });

                gridCustomers.Add(chkBox, col, row);

                gridCustomers.Add(new Image
                {
                    // workaround for not working commented code below
                    Source = this.GetCorrectCompanyLogoFileName(ListComp[helpCounter].Name),
                    //Source = ImageSource.FromStream(() => new MemoryStream(listComp[helpCounter].Image)),

                    WidthRequest = 50,
                    HeightRequest = 50,
                    //Aspect = Aspect.AspectFit,
                    HorizontalOptions = LayoutOptions.EndAndExpand

                }, col + 1, row);

                // Check if all companieos were already processed
                helpCounter++;
                if (helpCounter == ListComp.Count)
                {
                    breakLoop = true;
                    break;
                }
            }

            if (breakLoop)
                break;
        }

        // START: Create Frame for Grid Customers //
        Frame frameCustomers = new Frame();
        frameCustomers.Content = gridCustomers;
        frameCustomers.Content.HorizontalOptions = LayoutOptions.StartAndExpand;
        frameCustomers.Content.VerticalOptions = LayoutOptions.Start;

        Grid.SetRow(frameCustomers, 0);
        Grid.SetColumnSpan(frameCustomers, 6);
        gridMain.Add(frameCustomers);

        // END: Create Frame for Grid Customers //

        // START: Frame Distance //

        Grid gridDistance = new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(), // Slider
                new ColumnDefinition(), // To fit parent grid layout
                new ColumnDefinition(), // To fit parent grid layout
                new ColumnDefinition(), // To fit parent grid layout
                new ColumnDefinition(), // To fit parent grid layout
                new ColumnDefinition() // To fit parent grid layout
            },
            RowDefinitions =
            {
                new RowDefinition(), // Text
                new RowDefinition(), // Slider and Text
            },
            RowSpacing = 20
        };

        Label labelDistance = new Label
        {
            Text = "Hladať obchody v okruhu km",
            FontAttributes = FontAttributes.Bold,
            FontSize = 14
        };

        Grid.SetColumnSpan(labelDistance, 6);
        gridDistance.Add(labelDistance, 0, 0);

        /* 
            !!!!!!
            be aware label needs to be created before Slider because of ValueChanged event which is raised when value of
            slider is changed due to taking already saved value of slider from Application Preferences (store used for settings saving)
            !!!!!!
        */

        LabelKm = new Label
        {
            Text = "10.0",
            FontAttributes = FontAttributes.Bold
        };
        gridDistance.Add(LabelKm, 6, 1);

        slider = new Slider
        {
            Maximum = 100.0d,
            VerticalOptions = LayoutOptions.Start,
            Value = 10.0d,
        };

        slider.ValueChanged += this.Slider_ValueChanged;
        slider.Value = await SettingsService.Get<double>(nameof(slider), 10.0d);
        Grid.SetColumnSpan(slider, 5);
        gridDistance.Add(slider, 0, 1);

        Frame frameDistance = new Frame();
        frameDistance.Content = gridDistance;

        Grid.SetRow(frameDistance, 2);
        Grid.SetColumnSpan(frameDistance, 6);
        gridMain.Add(frameDistance);
        // END: Frame Distance //

        // Create ScrollView
        ScrollView scrollView = new ScrollView
        {
            VerticalScrollBarVisibility = ScrollBarVisibility.Always,
            Orientation = ScrollOrientation.Vertical
        };

        scrollView.Content = gridMain;

        // Set grid and its childrens as main content
        Content = scrollView;

    }

    private void CompanyCheckBoxChanged(object sender, EventArgs e)
    {
        CheckBox checkBox = sender as CheckBox;
        CompanyCheckboxesState chkState = ListCompanyCheckboxesState.Where(chkState => chkState.Id == checkBox.AutomationId).First();
        chkState.IsChecked = checkBox.IsChecked;

        // Make sure someone is listening to event
        if (OnCheckBoxCompany_Changed != null)
        {
            CompanyCheckBoxChanged_EventArgs args = new CompanyCheckBoxChanged_EventArgs(int.Parse(chkState.Id), chkState.Name);
            OnCheckBoxCompany_Changed(this, args);
        }
    }

    private string GetCorrectCompanyLogoFileName(string arg_Name)
    {
        string retVal = "";

        if (arg_Name == "Billa")
            retVal = "billa_logo.png";
        else if (arg_Name == "Lidl")
            retVal = "lidl_logo.png";
        else if (arg_Name == "Tesco")
            retVal = "tesco_logo1.png";
        else if (arg_Name == "Kaufland")
            retVal = "kaufland_logo.png";
        else if (arg_Name == "Test")
            retVal = "dotnet_bot.svg";
        else if (arg_Name == "Test1")
            retVal = "dotnet_bot.svg";
        else if (arg_Name == "Test2")
            retVal = "dotnet_bot.svg";
        else if (arg_Name == "Test3")
            retVal = "dotnet_bot.svg";

        return retVal;
    }

    public List<int> GetListIdsCheckedCompanies()
    {
        return ListCompanyCheckboxesState.Where(state => state.IsChecked).Select(state => int.Parse(state.Id)).ToList();
    }
}