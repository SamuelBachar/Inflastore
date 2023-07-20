using Neminaj.Models;
using System.Collections.ObjectModel;
using Neminaj.GlobalEnums;
using Neminaj.Repositories;
using Neminaj.GlobalText;
using Neminaj.Events;
using System.Collections.Specialized;
using Neminaj.Interfaces;
using CommunityToolkit.Maui.Views;
using Neminaj.ContentViews;

namespace Neminaj.Views;

public class CheapestItemPerCompany
{
    public Company Company;
    public ItemChoosen ItemChoosen;
    public bool Discount;

    public CheapestItemPerCompany(Company company, ItemChoosen itemChoosen, bool discount)
    {
        Company = company;
        ItemChoosen = itemChoosen;
        Discount = discount;
    }
}

public class CustomCell : ViewCell
{

    Grid GridCellView = null;
    StackLayout StackLayout = null;
    Label LblName = null;
    Label LblPrice1 = null;
    Label LblPrice2 = null;
    Label LblPrice3 = null;

    public static readonly BindableProperty IdInListProperty =
    BindableProperty.Create("IdInList", typeof(int), typeof(CustomCell), -1);
    public static readonly BindableProperty FinalNameProperty =
        BindableProperty.Create("FinalName", typeof(string), typeof(CustomCell), "FinalName");
    public static readonly BindableProperty Price1Property =
        BindableProperty.Create("Price1", typeof(float?), typeof(CustomCell));
    public static readonly BindableProperty Price2Property =
        BindableProperty.Create("Price2", typeof(float?), typeof(CustomCell));
    public static readonly BindableProperty Price3Property =
        BindableProperty.Create("Price3", typeof(float?), typeof(CustomCell));
    public static readonly BindableProperty CntOfItemsProperty =
        BindableProperty.Create("CnfOfItems", typeof(int), typeof(CustomCell), 1);

    public int IdInList
    {
        get { return (int)GetValue(IdInListProperty); }
        set { SetValue(IdInListProperty, value); }
    }

    public string FinalName
    {
        get { return (string)GetValue(FinalNameProperty); }
        set { SetValue(FinalNameProperty, value); }
    }

    public float? Price1
    {
        get { return (float?)GetValue(Price1Property); }
        set { SetValue(Price1Property, value); }
    }

    public float? Price2
    {
        get { return (float?)GetValue(Price2Property); }
        set { SetValue(Price2Property, value); }
    }
    public float? Price3
    {
        get { return (float?)GetValue(Price3Property); }
        set { SetValue(Price3Property, value); }
    }

    public int CntOfItems
    {
        get { return (int)GetValue(CntOfItemsProperty); }
        set { SetValue(CntOfItemsProperty, value); }
    }

    public CustomCell(ViewElements pe, PageLayoutInfo.ViewLayoutInfo pageLayoutInfo)
    {
        GridCellView = new Grid();
        GridCellView.RowDefinitions.Add(new RowDefinition() { Height = GridLength.Auto });

        GridCellView.ColumnDefinitions.Add(new ColumnDefinition() { Width = pe.MainGrid.FirstColumnWidth });

        for (int i = 0; i < pageLayoutInfo.GridCntCols; i++)
        {
            GridCellView.ColumnDefinitions.Add(new ColumnDefinition() { Width = pe.MainGrid.AnotherColumnWidth });
        }

        StackLayout = new StackLayout();
        LblName = new Label();
        LblName.LineBreakMode = LineBreakMode.WordWrap;
        LblName.VerticalOptions = LayoutOptions.Center;
        LblName.HorizontalOptions = LayoutOptions.Start;
        StackLayout.Add(LblName);

        GridCellView.Add(StackLayout, column: 0);

        int colOffset = 1;

        for (int i = 0; i < pageLayoutInfo.GridCntCols; i++)
        {
            if (i == 0)
            {
                LblPrice1 = new Label();
                LblPrice1.VerticalOptions = LayoutOptions.Center;
                LblPrice1.HorizontalOptions = LayoutOptions.Center;
                GridCellView.Add(LblPrice1, colOffset);
            }
            else if (i == 1)
            {
                LblPrice2 = new Label();
                LblPrice2.VerticalOptions = LayoutOptions.Center;
                LblPrice2.HorizontalOptions = LayoutOptions.Center;
                GridCellView.Add(LblPrice2, colOffset);
            }
            else if (i == 2)
            {
                LblPrice3 = new Label();
                LblPrice3.VerticalOptions = LayoutOptions.Center;
                LblPrice3.HorizontalOptions = LayoutOptions.Center;
                GridCellView.Add(LblPrice3, colOffset);
            }

            colOffset++;
        }

        View = GridCellView;
    }

    protected override void OnBindingContextChanged()
    {
        base.OnBindingContextChanged();

        if (BindingContext != null)
        {
            LblName.Text = FinalName;

            if (LblPrice1 != null && Price1 != null)
                LblPrice1.Text = (CntOfItems * Math.Round(Price1.Value, 2)).ToString("0.00");

            if (LblPrice2 != null && Price2 != null)
                LblPrice2.Text = (CntOfItems * Math.Round(Price2.Value, 2)).ToString("0.00");

            if (LblPrice3 != null && Price3 != null)
                LblPrice3.Text = (CntOfItems * Math.Round(Price3.Value, 2)).ToString("0.00");
        }
    }
}

public class PageLayoutInfo
{
    public int CntOfViewsInPage { get; set; }

    /*
    Count of cols depends on count of companies + 1 column for item names
    Maximum count of cols per one grid is 4
    e.g. 1 grid: 3 customers + 1 item names
    e.g. 2 grid: 4 customer + 2 item names --> 1st grid : 3 customer + 1 item names, 2nd grid: 1 customer + 1 item names
 */
    public class ViewLayoutInfo
    {
        public int GridCntCols { get; set; }
        public int GridCntRows { get; set; }

        public List<Company> ListCompaniesInView { get; set; }
    }

    public List<ViewLayoutInfo> ListViewLayoutInfo { get; set; }
}

public class ViewElements
{
    public class ViewMainGrid
    {
        public Grid ViewGrid { get; set; }
        public int CountCols { get; set; }
        public const int CountRows = 5;
        public const int Padding = 5;
        public const double RowSpacing = 10.0d;

        public GridLength FirstRowHeight { get; set; } = GridLength.Auto; // Images of companies
        public GridLength SecondRowHeight { get; set; } = new GridLength(0.7, GridUnitType.Star);// list views
        public GridLength ThirdRowHeight { get; set; } = GridLength.Auto; // Spolu
        public GridLength FourthRowHeight { get; set; } = GridLength.Auto; // Spolu s kartou
        public GridLength FifthRowHeight { get; set; } = GridLength.Auto; // Navigate Buttons
        public GridLength SixthRowHeight { get; set; } = GridLength.Auto; // Kombinacia najlacnejsi obchod

        public GridLength SeventhRowHeight { get; set; } = GridLength.Auto; // Spat Porovnaj Dalsi


        public GridLength FirstColumnWidth { get; set; } = new GridLength(0.5, GridUnitType.Star);
        public GridLength AnotherColumnWidth { get; set; }
    }

    public ViewMainGrid MainGrid { get; set; }

    public Label LblMode { get; set; }
    public List<Image> ListCompaniesImages { get; set; }

    public class ListViewRecords
    {
        public ListView ListView { get; set; }
    }
    public ListViewRecords _ListViewRecords { get; set; }

    public Label SummaryLabelCheapestPrice { get; set; }

    public Label SummaryLabelCheapestPriceDiscount { get; set; }

    public Label SummaryLabelCheapestPriceValue { get; set; }

    public Label SummaryLabelCheapestPriceDiscountValue { get; set; }

    public List<Label> ListSummaryPriceLabels { get; set; }
    public List<Label> ListSummaryPriceDiscountLabels { get; set; }
    public List<Button> ListBtnNavigate { get; set; }

    public Button BtnNavigateAllShops { get; set; }

    public Button BtnBack { get; set; }
    public Button BtnNext { get; set; }

    public Button BtnCompare { get; set; }

    public Image ImageEmptyCart { get; set; }
}

public partial class PriceComparerView : ContentPage
{
    // Events
    private event EventHandler EventCompaniesListLoaded;
    private event EventHandler ViewsBuilded;

    // Views objects
    private ItemPicker MainPage { get; set; } = null;
    private SettingsView SettingsView { get; set; } = null;

    private PageLayoutInfo PageLayoutInfo { get; set; } = null;
    private List<ViewElements> ListViewElements { get; set; } = null;

    // Repositories objects
    private CompanyRepository CompanyRepository { get; set; } = null;

    private ItemPriceRepository ItemPriceRepo { get; set; } = null;

    private NavigationShopRepository NavigationShopRepo { get; set; } = null;

    private IGeolocation GeoLocation { get; set; } = null;

    // Lists
    public ObservableCollection<ItemChoosen> ItemsChoosed { get; set; } = new ObservableCollection<ItemChoosen>();

    private List<ItemChoosen> ItemsChoosedModified { get; set; } = new List<ItemChoosen>();

    private List<Company> ListComp = new List<Company>();

    ActivityIndicator ActivityIndicator { get; set; } = null;

    private List<CheapestItemPerCompany> ListCheapestItemsPerCompanies { get; set; } = new List<CheapestItemPerCompany>();

    // Mode
    private ComparerMode Mode { get; set; }

    // test
    DataTemplate customViewCell;
    int CurrentViewIndex = 0;
    VerticalStackLayout vertStackLayout = null;
    Label text = null;

    public PriceComparerView(ItemPicker mainPage, SettingsView settingsPage, CompanyRepository companyRepo, ItemPriceRepository itemPriceRepo, NavigationShopRepository navigationShopRepository, IGeolocation geoLocation)
    {
        InitializeComponent();

        MainPage = mainPage;
        SettingsView = settingsPage;
        CompanyRepository = companyRepo;
        ItemPriceRepo = itemPriceRepo;
        NavigationShopRepo = navigationShopRepository;
        GeoLocation = geoLocation;

        CreateActivityIndicator();
        TurnOnActivityIndicator();

        ItemsChoosed = MainPage.GetChoosenItems();
        ItemsChoosed.CollectionChanged += async (s, e) => { await ChoosenItems_Changed(s, e); };
        MainPage.OnObservableItemsChoosed_Swaped += OnObservableItemsChoosed_Swaped;

        CartView.OnItemCountOf_Changed += OnItemCountOf_Changed_Event;
        SettingsView.OnCheckBoxCompany_Changed += async (s, e) => { await OnCheckBoxCompany_Changed_Event(s, e); };

        this.EventCompaniesListLoaded += BuildPage;
        this.Loaded += async (s, e) => { await GetListCompanies(); };
        this.ViewsBuilded += async (s, e) => { await FillPrices(build: true); };
        this.ViewsBuilded += OnAppearing;
        this.Appearing += OnAppearing;
    }

    public void CreateActivityIndicator()
    {
        ActivityIndicator = new ActivityIndicator
        {
            IsRunning = false,
            Color = Colors.Orange
        };
    }

    public void TurnOnActivityIndicator()
    {
        if (!ActivityIndicator.IsRunning)
        {
            if (ActivityIndicator.Parent != null)
                vertStackLayout.Remove(ActivityIndicator);

            ActivityIndicator.IsRunning = true;
            ActivityIndicator.HorizontalOptions = LayoutOptions.Center;
            ActivityIndicator.VerticalOptions = LayoutOptions.Center;
            ActivityIndicator.HeightRequest = 200;
            ActivityIndicator.WidthRequest = 200;

            vertStackLayout = new VerticalStackLayout();
            text = new Label
            {
                Text = "Načítavam dáta",
                FontSize = 32,
                FontAttributes = FontAttributes.Bold,
                TextColor = Colors.DarkGrey,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.End,
            };

            vertStackLayout.HorizontalOptions = LayoutOptions.Center;
            vertStackLayout.VerticalOptions = LayoutOptions.Center;
            vertStackLayout.Add(ActivityIndicator);
            vertStackLayout.Add(text);

            Content = vertStackLayout;
        }
    }

    public void TurnOffActivityIndicator()
    {
        ActivityIndicator.IsRunning = false;
        Content = ListViewElements[CurrentViewIndex].MainGrid.ViewGrid;
    }

    private async Task OnCheckBoxCompany_Changed_Event(object sender, CompanyCheckBoxChanged_EventArgs args)
    {
        if (SettingsView != null)
        {
            this.ListComp = await CompanyRepository.GetAllCompaniesAsync();
            List<int> listIdsCheckedCompanies = SettingsView.GetListIdsCheckedCompanies();
            this.ListComp = this.ListComp.Where(com => listIdsCheckedCompanies.Contains(com.Id)).ToList();

            TurnOnActivityIndicator();
            this.BuildPage(this, new EventArgs());
            await FillPrices(build: true);
            TurnOffActivityIndicator();
        }
    }

    /* Function called after raising event CollectionChanged which is called after creating this class and user adds additional item into cart (or delete one) */
    private async Task ChoosenItems_Changed(object sender, NotifyCollectionChangedEventArgs args)
    {
        TurnOnActivityIndicator();

        if (args.Action == NotifyCollectionChangedAction.Add)
        {
            int newItemsStartIndex = args.NewStartingIndex;
            await this.FillPrices(added: true, newItemsStartIndex: newItemsStartIndex);
        }

        if (args.Action == NotifyCollectionChangedAction.Remove)
        {
            await this.FillPrices(added: false, deleted: true);
        }

        TurnOffActivityIndicator();
    }

    /* Function called after raising event OnObservableItemsChoosed_Swaped when user choosed saved cart from SQLite with it's items to work with */
    private async void OnObservableItemsChoosed_Swaped(object sender, EventArgs args)
    {
        ItemsChoosed.CollectionChanged -= async (s, e) => { await ChoosenItems_Changed(s, e); };
        ItemsChoosed = MainPage.GetChoosenItems();
        ItemsChoosedModified = ItemsChoosed.ToList();

        for (int i = 0; i < PageLayoutInfo.CntOfViewsInPage; i++)
            ListViewElements[i]._ListViewRecords.ListView.ItemsSource = ItemsChoosedModified;

        await FillPrices(build: true);

        ItemsChoosed.CollectionChanged += async (s, e) => { await ChoosenItems_Changed(s, e); };
    }

    private void OnItemCountOf_Changed_Event(object sender, ItemCountOf_Changed_EventArgs args)
    {
        TurnOnActivityIndicator();

        ItemsChoosedModified = ItemsChoosed.ToList();

        foreach (ViewElements viewElem in ListViewElements)
        {
            ItemChoosen item = viewElem._ListViewRecords.ListView.ItemsSource.Cast<ItemChoosen>()
                 .Where(item => item.IdInList == args.IndexInObservableCollection).FirstOrDefault();

            if (item != null)
            {

                item.FinalName = args.FinalName;
                item.CntOfItems = args.Count;


                viewElem._ListViewRecords.ListView.ItemsSource = viewElem._ListViewRecords.ListView.ItemsSource.Cast<ItemChoosen>().ToList();
            }
        }

        TurnOffActivityIndicator();
    }

    private void BuildPage(object sender, EventArgs e)
    {
        CurrentViewIndex = 0;

        PageLayoutInfo = new PageLayoutInfo();
        this.GetPageLayoutInfo(PageLayoutInfo);
        this.GetViewsLayoutInfo(PageLayoutInfo);

        ListViewElements = new List<ViewElements>();

        ItemsChoosedModified = ItemsChoosed.ToList();

        for (int i = 0; i < PageLayoutInfo.CntOfViewsInPage; i++)
        {
            ListViewElements.Add(new ViewElements());

            this.CreateViewMainGrid(ListViewElements[i], PageLayoutInfo.ListViewLayoutInfo[i]);
            this.CreateViewCompaniesImages(ListViewElements[i], PageLayoutInfo.ListViewLayoutInfo[i]);
            this.CreateViewCustomCellListView(ListViewElements[i], PageLayoutInfo.ListViewLayoutInfo[i]);
            this.CreateViewLabelsSummary(ListViewElements[i], PageLayoutInfo.ListViewLayoutInfo[i]);
            this.CreateViewNavigateButtons(ListViewElements[i], PageLayoutInfo.ListViewLayoutInfo[i]);
            this.CreateViewButtonsBackCompareNext(ListViewElements[i], PageLayoutInfo.ListViewLayoutInfo[i], PageLayoutInfo);
            ListViewElements[i]._ListViewRecords.ListView.ItemsSource = ItemsChoosedModified;
        }

        this.ViewsBuilded?.Invoke(this, new EventArgs());
    }

    private void OnAppearing(object sender, EventArgs args)
    {
        if (this.ItemsChoosedModified == null || (this.ItemsChoosedModified != null && this.ItemsChoosedModified.Count == 0))
        {
            if (PageLayoutInfo != null)
            {
                for (int i = 0; i < PageLayoutInfo.CntOfViewsInPage; i++)
                {
                    if (ListViewElements[i].ImageEmptyCart == null)
                    {
                        ListViewElements[i].ImageEmptyCart = new Image
                        {
                            Source = ImageSource.FromFile("cart_empty.svg"),
                            Aspect = Aspect.AspectFit,
                            HorizontalOptions = LayoutOptions.Center,
                            VerticalOptions = LayoutOptions.Center,
                        };

                        Grid.SetColumnSpan(ListViewElements[i].ImageEmptyCart, PageLayoutInfo.ListViewLayoutInfo[i].GridCntCols + 1);
                        Grid.SetRow(ListViewElements[i].ImageEmptyCart, 1);

                        ListViewElements[i].MainGrid.ViewGrid.Add(ListViewElements[i].ImageEmptyCart, row: 1);
                    }
                    else
                    {
                        ListViewElements[i].ImageEmptyCart.IsVisible = true;
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < PageLayoutInfo.CntOfViewsInPage; i++)
            {
                /* if (ListViewElements[i].ImageEmptyCart.Parent != null)
                     ListViewElements[i].MainGrid.ViewGrid.Remove(ListViewElements[i].ImageEmptyCart);

                 ListViewElements[i].MainGrid.ViewGrid.Add(ListViewElements[i]._ListViewRecords.ListView, column: 0, row: 1);*/

                if (ListViewElements[i].ImageEmptyCart != null)
                    ListViewElements[i].ImageEmptyCart.IsVisible = false;
            }
        }
    }

    private async Task FillPrices(bool build = false, bool added = false, bool deleted = false, int? newItemsStartIndex = null)
    {
        TurnOnActivityIndicator();

        ItemsChoosedModified = ItemsChoosed.ToList();
        List<ItemChoosen> listChoosenTemp = new List<ItemChoosen>();

        for (int i = 0; i < PageLayoutInfo.CntOfViewsInPage; i++)
        {

            if (build || deleted)
            {
                listChoosenTemp.AddRange(ItemsChoosedModified.Select(i => new ItemChoosen()
                {
                    Name = i.Name,
                    FinalName = i.FinalName,
                    Id = i.Id,
                    IdInList = i.IdInList,
                    CntOfItems = i.CntOfItems
                }));

                await this.FillPrices(ListViewElements[i], PageLayoutInfo.ListViewLayoutInfo[i], listChoosenTemp);
                ListViewElements[i]._ListViewRecords.ListView.ItemsSource = listChoosenTemp.ToList();
                listChoosenTemp.Clear();
            }

            if (added)
            {
                listChoosenTemp = ListViewElements[i]._ListViewRecords.ListView.ItemsSource.Cast<ItemChoosen>().ToList();

                // Deep copy added item / items to list of choosen items in view, later for new items prices will be filled
                listChoosenTemp.AddRange(
                    ItemsChoosedModified.GetRange(newItemsStartIndex.Value, ItemsChoosedModified.Count - newItemsStartIndex.Value).
                    Select(i => new ItemChoosen()
                    {
                        Name = i.Name,
                        FinalName = i.FinalName,
                        Id = i.Id,
                        IdInList = i.IdInList,
                        CntOfItems = i.CntOfItems
                    }));

                await this.FillPrices(ListViewElements[i], PageLayoutInfo.ListViewLayoutInfo[i], listChoosenTemp, newItemsStartIndex);
                ListViewElements[i]._ListViewRecords.ListView.ItemsSource = listChoosenTemp.ToList();
            }
        }

        TurnOffActivityIndicator();
        Content = ListViewElements[CurrentViewIndex].MainGrid.ViewGrid;
    }

    private async Task GetListCompanies()
    {
        this.ListComp = await CompanyRepository.GetAllCompaniesAsync();

        if (await ISettingsService.ContainsStatic("SettingsAtLeastOnceSaved"))
        {
            List<int> listIdsSavedAndChoosedCompanies = SettingsView.GetCheckedAndSavedCompaniesFromSettings(this.ListComp);
            this.ListComp = this.ListComp.Where(com => listIdsSavedAndChoosedCompanies.Contains(com.Id)).ToList();
        }

        this.EventCompaniesListLoaded?.Invoke(this, new EventArgs());
    }

    /*
     *  PageElements.MainGrid is de-facto main content of whole Page
     *  Therefore PageLayoutInfo is defined also by PageElements.MainGrid members
     */
    private void GetPageLayoutInfo(PageLayoutInfo pageLayoutInfo)
    {
        pageLayoutInfo.CntOfViewsInPage = (this.ListComp.Count / 3) + (this.ListComp.Count % 3 > 0 ? 1 : 0);
    }

    private void GetViewsLayoutInfo(PageLayoutInfo pageLayoutInfo)
    {
        int tempCount = this.ListComp.Count;
        pageLayoutInfo.ListViewLayoutInfo = new List<PageLayoutInfo.ViewLayoutInfo>();

        for (int i = 0; i < pageLayoutInfo.CntOfViewsInPage; i++)
        {
            if ((tempCount / 3) >= 1)
            {
                pageLayoutInfo.ListViewLayoutInfo.Add(new PageLayoutInfo.ViewLayoutInfo()
                {
                    GridCntCols = 3,
                    GridCntRows = ViewElements.ViewMainGrid.CountRows,
                    ListCompaniesInView = new List<Company>(ListComp.GetRange(i * 3, 3))
                });
            }
            else
            {
                pageLayoutInfo.ListViewLayoutInfo.Add(new PageLayoutInfo.ViewLayoutInfo()
                {
                    GridCntCols = (tempCount % 3),
                    GridCntRows = ViewElements.ViewMainGrid.CountRows,
                    ListCompaniesInView = new List<Company>(ListComp.GetRange(i * 3, (tempCount % 3)))
                });
            }

            tempCount -= 3;
        }
    }

    private void CreateViewMainGrid(ViewElements pe, PageLayoutInfo.ViewLayoutInfo viewLayoutInfo)
    {
        // 1. Create grid
        pe.MainGrid = new ViewElements.ViewMainGrid();

        pe.MainGrid.ViewGrid = new Grid
        {
            Padding = ViewElements.ViewMainGrid.Padding,
            RowSpacing = ViewElements.ViewMainGrid.RowSpacing
        };

        // 2. Set Row definitions
        pe.MainGrid.ViewGrid.RowDefinitions.Add(new RowDefinition() { Height = pe.MainGrid.FirstRowHeight });
        pe.MainGrid.ViewGrid.RowDefinitions.Add(new RowDefinition() { Height = pe.MainGrid.SecondRowHeight });
        pe.MainGrid.ViewGrid.RowDefinitions.Add(new RowDefinition() { Height = pe.MainGrid.ThirdRowHeight });
        pe.MainGrid.ViewGrid.RowDefinitions.Add(new RowDefinition() { Height = pe.MainGrid.FourthRowHeight });
        pe.MainGrid.ViewGrid.RowDefinitions.Add(new RowDefinition() { Height = pe.MainGrid.FifthRowHeight });
        pe.MainGrid.ViewGrid.RowDefinitions.Add(new RowDefinition() { Height = pe.MainGrid.SixthRowHeight });
        pe.MainGrid.ViewGrid.RowDefinitions.Add(new RowDefinition() { Height = pe.MainGrid.SeventhRowHeight });

        // 3. Set Column definitions
        pe.MainGrid.ViewGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = pe.MainGrid.FirstColumnWidth });

        pe.MainGrid.AnotherColumnWidth = viewLayoutInfo.GridCntCols == 1 ?
                                         GridLength.Star :
                                         viewLayoutInfo.GridCntCols == 2 ?
                                         new GridLength(0.50, GridUnitType.Star) :
                                         new GridLength(0.33, GridUnitType.Star);

        for (int i = 0; i < viewLayoutInfo.GridCntCols; i++)
        {
            pe.MainGrid.ViewGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = pe.MainGrid.AnotherColumnWidth });
        }
    }

    private void CreateViewCompaniesImages(ViewElements pe, PageLayoutInfo.ViewLayoutInfo viewLayoutInfo)
    {
        pe.ListCompaniesImages = new List<Image>();

        Grid tempGrid = new Grid();
        Grid.SetColumnSpan(tempGrid, viewLayoutInfo.GridCntCols + 1);
        tempGrid.AddRowDefinition(new RowDefinition(GridLength.Auto));

        for (int i = 0; i < viewLayoutInfo.GridCntCols + 1; i++)
        {
            if (i == 0)
                tempGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = pe.MainGrid.FirstColumnWidth });
            else
                tempGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = pe.MainGrid.AnotherColumnWidth });
        }

        Label lblItem = new Label()
        {
            Text = "Položka",
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        tempGrid.Add(lblItem, 0, 0);

        //pe.MainGrid.ViewGrid.Add(lblItem, 0, 0);

        int colOffset = 1;

        for (int i = 0; i < viewLayoutInfo.GridCntCols; i++)
        {
            Image temp = new Image
            {
                // workaround for not working commented code below
                Source = this.GetCorrectCompanyLogoFileName(ListComp.Where(com => com.Id == viewLayoutInfo.ListCompaniesInView[i].Id).First().Name),
                //Source = ImageSource.FromStream(() => new MemoryStream(listComp[helpCounter].Image)),

                WidthRequest = 50,
                HeightRequest = 50,
                Aspect = Aspect.AspectFit,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center
            };

            //pe.MainGrid.ViewGrid.Add(temp, colOffset, 0);
            tempGrid.Add(temp, column: colOffset, row: 0);

            pe.ListCompaniesImages.Add(temp); // todo probably not needed
            colOffset++;
        }

        pe.MainGrid.ViewGrid.Add(tempGrid, 0, 0);
    }

    private void CreateViewCustomCellListView(ViewElements pe, PageLayoutInfo.ViewLayoutInfo viewLayoutInfo)
    {
        customViewCell = new DataTemplate(() =>
        {
            CustomCell customCell = new CustomCell(pe, viewLayoutInfo);
            return customCell;
        });

        customViewCell.SetBinding(CustomCell.IdInListProperty, "IdInList");
        customViewCell.SetBinding(CustomCell.FinalNameProperty, "FinalName");
        customViewCell.SetBinding(CustomCell.Price1Property, "Price1");
        customViewCell.SetBinding(CustomCell.Price2Property, "Price2");
        customViewCell.SetBinding(CustomCell.Price3Property, "Price3");
        customViewCell.SetBinding(CustomCell.CntOfItemsProperty, "CntOfItems");

        pe._ListViewRecords = new ViewElements.ListViewRecords();
        pe._ListViewRecords.ListView = new ListView()
        {
            VerticalScrollBarVisibility = ScrollBarVisibility.Always,
            SelectionMode = ListViewSelectionMode.None,
            ItemTemplate = customViewCell,
            SeparatorColor = Colors.Black,
            SeparatorVisibility = SeparatorVisibility.Default
        };

        Grid.SetColumnSpan(pe._ListViewRecords.ListView, viewLayoutInfo.GridCntCols + 1);
        pe.MainGrid.ViewGrid.Add(pe._ListViewRecords.ListView, column: 0, row: 1);
    }

    private void CreateViewLabelsSummary(ViewElements pe, PageLayoutInfo.ViewLayoutInfo viewLayoutInfo)
    {
        Grid tempGrid = new Grid();
        Grid.SetColumnSpan(tempGrid, viewLayoutInfo.GridCntCols + 1);
        tempGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto)); // Spolu
        tempGrid.RowDefinitions.Add(new RowDefinition(GridLength.Auto)); // Spolu s kartou

        for (int i = 0; i < viewLayoutInfo.GridCntCols + 1; i++)
        {
            if (i == 0)
                tempGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = pe.MainGrid.FirstColumnWidth });
            else
                tempGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = pe.MainGrid.AnotherColumnWidth });
        }

        pe.ListSummaryPriceLabels = new List<Label>();
        pe.ListSummaryPriceDiscountLabels = new List<Label>();

        int row = 0;
        int collOfset = 1;

        for (int i = 0; i < 2; i++)
        {
            collOfset = 1;

            Label lblSummary = new Label();
            lblSummary.Text = row == 0 ? Texts.Summary : Texts.SummaryWithCard;
            lblSummary.FontAttributes = FontAttributes.Bold;
            lblSummary.HorizontalOptions = LayoutOptions.Start;
            lblSummary.VerticalOptions = LayoutOptions.Center;

            //pe.MainGrid.ViewGrid.Add(lblSummary, column: 0, row: row + i);
            tempGrid.Add(lblSummary, column: 0, row: row);

            for (int j = 0; j < viewLayoutInfo.GridCntCols; j++)
            {
                Label lblSummaryResult = new Label();
                lblSummaryResult.HorizontalOptions = LayoutOptions.Center;
                lblSummaryResult.VerticalOptions = LayoutOptions.Center;
                //pe.MainGrid.ViewGrid.Add(lblSummaryResult, column: collOfset, row: row + i);
                tempGrid.Add(lblSummaryResult, column: collOfset, row: row);

                if (i == 0)
                    pe.ListSummaryPriceLabels.Add(lblSummaryResult);
                else
                    pe.ListSummaryPriceDiscountLabels.Add(lblSummaryResult);

                collOfset++;
            }

            row++;
        }

        pe.MainGrid.ViewGrid.Add(tempGrid, column: 0, row: 2);

        Grid grid = new Grid
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto),
            },

            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(GridLength.Star),
            },

            RowSpacing = 5
        };

        Grid.SetColumnSpan(grid, viewLayoutInfo.GridCntCols + 1);

        Label lblShopCombination = new Label();
        lblShopCombination.Text = "Najlacnejší nákup (nákup vo viacerých obchodoch):";
        lblShopCombination.FontAttributes = FontAttributes.Bold;
        lblShopCombination.HorizontalOptions = LayoutOptions.Start;
        lblShopCombination.VerticalOptions = LayoutOptions.Center;
        Grid.SetColumnSpan(lblShopCombination, viewLayoutInfo.GridCntCols + 1);
        Grid.SetColumn(lblShopCombination, 0);
        Grid.SetRow(lblShopCombination, 0);
        grid.Add(lblShopCombination);

        pe.SummaryLabelCheapestPrice = new Label();
        pe.SummaryLabelCheapestPrice.Text = Texts.Summary;
        pe.SummaryLabelCheapestPrice.FontAttributes = FontAttributes.Bold;
        pe.SummaryLabelCheapestPrice.HorizontalOptions = LayoutOptions.Start;
        pe.SummaryLabelCheapestPrice.VerticalOptions = LayoutOptions.Center;
        Grid.SetColumn(pe.SummaryLabelCheapestPrice, 0);
        Grid.SetRow(pe.SummaryLabelCheapestPrice, 1);
        grid.Add(pe.SummaryLabelCheapestPrice);

        pe.SummaryLabelCheapestPriceValue = new Label();
        pe.SummaryLabelCheapestPriceValue.FontAttributes = FontAttributes.Bold;
        pe.SummaryLabelCheapestPriceValue.HorizontalOptions = LayoutOptions.Center;
        pe.SummaryLabelCheapestPriceValue.VerticalOptions = LayoutOptions.Center;
        Grid.SetColumn(pe.SummaryLabelCheapestPriceValue, 1);
        Grid.SetRow(pe.SummaryLabelCheapestPriceValue, 1);
        grid.Add(pe.SummaryLabelCheapestPriceValue);

        pe.SummaryLabelCheapestPriceDiscount = new Label();
        pe.SummaryLabelCheapestPriceDiscount.Text = Texts.SummaryWithCard;
        pe.SummaryLabelCheapestPriceDiscount.FontAttributes = FontAttributes.Bold;
        pe.SummaryLabelCheapestPriceDiscount.HorizontalOptions = LayoutOptions.Start;
        pe.SummaryLabelCheapestPriceDiscount.VerticalOptions = LayoutOptions.Center;
        Grid.SetColumn(pe.SummaryLabelCheapestPriceDiscount, 0);
        Grid.SetRow(pe.SummaryLabelCheapestPriceDiscount, 2);
        grid.Add(pe.SummaryLabelCheapestPriceDiscount);

        pe.SummaryLabelCheapestPriceDiscountValue = new Label();
        pe.SummaryLabelCheapestPriceDiscountValue.FontAttributes = FontAttributes.Bold;
        pe.SummaryLabelCheapestPriceDiscountValue.HorizontalOptions = LayoutOptions.Center;
        pe.SummaryLabelCheapestPriceDiscountValue.VerticalOptions = LayoutOptions.Center;
        Grid.SetColumn(pe.SummaryLabelCheapestPriceDiscountValue, 1);
        Grid.SetRow(pe.SummaryLabelCheapestPriceDiscountValue, 2);
        grid.Add(pe.SummaryLabelCheapestPriceDiscountValue);

        Button btnDetails = new Button();
        btnDetails.Text = Texts.Detail;
        btnDetails.HorizontalOptions = LayoutOptions.Center;
        btnDetails.VerticalOptions = LayoutOptions.Center;
        btnDetails.Clicked += async (s, e) => { await BtnDetails_Clicked(s, e); };
        Grid.SetColumn(btnDetails, 2);
        Grid.SetRow(btnDetails, 1);
        grid.Add(btnDetails);

        Button btnNavigateAllShops = new Button();
        btnNavigateAllShops.Text = Texts.Navigate;
        btnNavigateAllShops.HorizontalOptions = LayoutOptions.Center;
        btnNavigateAllShops.VerticalOptions = LayoutOptions.Center;
        btnNavigateAllShops.Clicked += async (s, e) => { await BtnNavigate_Clicked(s, e); };
        Grid.SetColumn(btnNavigateAllShops, 2);
        Grid.SetRow(btnNavigateAllShops, 2);
        grid.Add(btnNavigateAllShops);

        pe.BtnNavigateAllShops = btnNavigateAllShops;

        pe.MainGrid.ViewGrid.Add(grid, column: 0, row: 4);
    }

    private void CreateViewNavigateButtons(ViewElements pe, PageLayoutInfo.ViewLayoutInfo viewLayoutInfo)
    {
        int collOfset = 1;

        Grid tempGrid = new Grid();
        Grid.SetColumnSpan(tempGrid, viewLayoutInfo.GridCntCols + 1);
        tempGrid.AddRowDefinition(new RowDefinition(GridLength.Auto));

        for (int i = 0; i < viewLayoutInfo.GridCntCols + 1; i++)
        {
            if (i == 0)
                tempGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = pe.MainGrid.FirstColumnWidth });
            else
                tempGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = pe.MainGrid.AnotherColumnWidth });
        }

        pe.ListBtnNavigate = new List<Button>();

        for (int i = 0; i < viewLayoutInfo.GridCntCols; i++)
        {
            Button btnNavigate = new Button();
            btnNavigate.Text = Texts.Navigate;
            btnNavigate.HorizontalOptions = LayoutOptions.Center;
            btnNavigate.VerticalOptions = LayoutOptions.Center;
            btnNavigate.ClassId = viewLayoutInfo.ListCompaniesInView[i].Id.ToString();
            btnNavigate.Clicked += async (s, e) => { await BtnNavigate_Clicked(s, e); };
            //pe.MainGrid.ViewGrid.Add(btnNavigate, column: collOfset, row: 3);
            tempGrid.Add(btnNavigate, column: collOfset, row: 0);

            pe.ListBtnNavigate.Add(btnNavigate);

            collOfset++;
        }

        pe.MainGrid.ViewGrid.Add(tempGrid, column: 0, row: 3);
    }

    private async Task BtnNavigate_Clicked(object sender, EventArgs e)
    {
        Button btnTemp = ((Button)sender);

        List<int> listCompaniesIds = new List<int>();

        // Specific company choosen
        if (btnTemp.ClassId != null)
        {
            bool found = false;

            for (int i = 0; i < PageLayoutInfo.CntOfViewsInPage; i++)
            {
                if (found)
                    break;

                foreach (Button btn in ListViewElements[i].ListBtnNavigate)
                {
                    if (btnTemp.ClassId == btn.ClassId)
                    {
                        listCompaniesIds.Add(int.Parse(btnTemp.ClassId));
                        found = true;
                        break;
                    }
                }
            }
        }

        // All companies choosen (which have cheapest prices, do not necessary means all of them will be shown in map)
        if (listCompaniesIds.Count == 0)
        {
            for (int i = 0; i < PageLayoutInfo.CntOfViewsInPage; i++)
            {
                PageLayoutInfo.ListViewLayoutInfo[i].ListCompaniesInView.ForEach(comp => listCompaniesIds.Add(comp.Id));
            }
        }

        await Shell.Current.GoToAsync(nameof(NavigationView),
        new Dictionary<string, object>
        {
            [nameof(NavigationShopRepository)] = this.NavigationShopRepo,
            [nameof(CompanyRepository)] = this.CompanyRepository,
            //["SettingsService"] = this.SettingsView.GetSettingService(),
            //["GeoLocation"] = this.GeoLocation,
            ["ListCompaniesIds"] = listCompaniesIds
        });
    }

    private async Task BtnDetails_Clicked(object sender, EventArgs e)
    {
        await this.ShowPopupAsync(new PriceComparerPopUpDetails(ListCheapestItemsPerCompanies));
    }

    private void CreateViewButtonsBackCompareNext(ViewElements pe, PageLayoutInfo.ViewLayoutInfo viewLayoutInfo, PageLayoutInfo pageLayoutInfo)
    {
        pe.BtnBack = new Button();
        pe.BtnBack.Text = Texts.Back;
        pe.BtnBack.HorizontalOptions = LayoutOptions.Start;
        pe.BtnBack.VerticalOptions = LayoutOptions.Center;
        pe.BtnBack.IsEnabled = false;
        pe.BtnBack.Clicked += Button_Back_Clicked;
        Grid.SetColumnSpan(pe.BtnBack, viewLayoutInfo.GridCntCols + 1);
        pe.MainGrid.ViewGrid.Add(pe.BtnBack, column: 0, row: 6);

        pe.BtnCompare = new Button();
        pe.BtnCompare.Text = Texts.Compare;
        pe.BtnCompare.HorizontalOptions = LayoutOptions.Center;
        pe.BtnCompare.VerticalOptions = LayoutOptions.Center;
        pe.BtnCompare.Clicked += Button_Compare_Clicked;
        Grid.SetColumnSpan(pe.BtnCompare, viewLayoutInfo.GridCntCols + 1);
        pe.MainGrid.ViewGrid.Add(pe.BtnCompare, column: 0, row: 6);

        pe.BtnNext = new Button();
        pe.BtnNext.Text = Texts.Next;
        pe.BtnNext.HorizontalOptions = LayoutOptions.End;
        pe.BtnNext.VerticalOptions = LayoutOptions.Center;
        pe.BtnNext.IsEnabled = (pageLayoutInfo.CntOfViewsInPage > 1);
        pe.BtnNext.Clicked += Button_Next_Clicked;
        Grid.SetColumnSpan(pe.BtnNext, viewLayoutInfo.GridCntCols + 1);
        pe.MainGrid.ViewGrid.Add(pe.BtnNext, column: 0, row: 6);
    }

    private async Task FillPrices(ViewElements pe,
        PageLayoutInfo.ViewLayoutInfo viewLayoutInfo,
        List<ItemChoosen> itemsChoosen,
        int? newItemsStartIndex = null)
    {
        if (itemsChoosen.Count > 0)
        {
            List<string> listItemIds = new List<string>();
            List<string> listCompIds = viewLayoutInfo.ListCompaniesInView.Select(comp => comp.Id.ToString()).ToList();

            if (newItemsStartIndex != null)
                listItemIds = itemsChoosen.GetRange(newItemsStartIndex.Value, itemsChoosen.Count - newItemsStartIndex.Value).Select(item => item.Id.ToString()).ToList();
            else
                listItemIds = itemsChoosen.Select(item => item.Id.ToString()).ToList();

            string filter = $"Company_Id IN ({String.Join(",", listCompIds.ToArray())})";
            filter += $" AND Item_Id IN ({String.Join(",", listItemIds.ToArray())})";

            List<ItemPrice> listItemPrice = await ItemPriceRepo.GetPriceItemsFilteredAsync(filter);

            if (listItemPrice.Count > 0)
            {
                int helpCompCounter = 0;
                int startIndex = (newItemsStartIndex == null ? 0 : newItemsStartIndex.Value);
                int count = newItemsStartIndex == null ? itemsChoosen.Count : itemsChoosen.Count - newItemsStartIndex.Value;

                foreach (Company comp in viewLayoutInfo.ListCompaniesInView)
                {
                    List<ItemPrice> listItemPricePerCompany = listItemPrice.Where(itemPrice => itemPrice.Company_Id == comp.Id).ToList();

                    if (listItemPricePerCompany.Count > 0)
                    {
                        foreach (ItemChoosen item in itemsChoosen.GetRange(startIndex, count))
                        {
                            ItemPrice itemPriceTemp = listItemPricePerCompany.Where(itemPriceComp => itemPriceComp.Item_Id == item.Id).FirstOrDefault();

                            if (itemPriceTemp != null)
                            {
                                if (helpCompCounter == 0)
                                {
                                    item.Price1 = itemPriceTemp.Price.ToString("0.00");
                                    item.PriceDiscount1 = itemPriceTemp.PriceDiscount.ToString("0.00");
                                }

                                if (helpCompCounter == 1)
                                {
                                    item.Price2 = itemPriceTemp.Price.ToString("0.00");
                                    item.PriceDiscount2 = itemPriceTemp.PriceDiscount.ToString("0.00");
                                }

                                if (helpCompCounter == 2)
                                {
                                    item.Price3 = itemPriceTemp.Price.ToString("0.00");
                                    item.PriceDiscount3 = itemPriceTemp.PriceDiscount.ToString("0.00");
                                }
                            }
                        }
                    }

                    helpCompCounter++;
                }
            }
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

    private void Button_Compare_Clicked(object sender, EventArgs e)
    {
        for (int viewIndex = 0; viewIndex < PageLayoutInfo.CntOfViewsInPage; viewIndex++)
        {
            for (int colIndex = 0; colIndex < PageLayoutInfo.ListViewLayoutInfo[viewIndex].GridCntCols; colIndex++)
            {
                float? summary = 0;
                float? summaryDiscount = 0;

                if (colIndex == 0)
                {
                    summary = ListViewElements[viewIndex]._ListViewRecords.ListView.ItemsSource.Cast<ItemChoosen>().Where(price => price.Price1 != null).Select(price => price.CntOfItems * float.Parse(price.Price1)).Sum();
                    summaryDiscount = ListViewElements[viewIndex]._ListViewRecords.ListView.ItemsSource.Cast<ItemChoosen>().Where(price => price.PriceDiscount1 != null).Select(price => price.CntOfItems * float.Parse(price.PriceDiscount1)).Sum();
                    ListViewElements[viewIndex].ListSummaryPriceLabels[colIndex].Text = summary.Value.ToString("0.00");
                    ListViewElements[viewIndex].ListSummaryPriceDiscountLabels[colIndex].Text = summaryDiscount.Value.ToString("0.00");
                }
                else if (colIndex == 1)
                {
                    summary = ListViewElements[viewIndex]._ListViewRecords.ListView.ItemsSource.Cast<ItemChoosen>().Where(price => price.Price2 != null).Select(price => price.CntOfItems * float.Parse(price.Price2)).Sum();
                    summaryDiscount = ListViewElements[viewIndex]._ListViewRecords.ListView.ItemsSource.Cast<ItemChoosen>().Where(price => price.PriceDiscount2 != null).Select(price => price.CntOfItems * float.Parse(price.PriceDiscount2)).Sum();
                    ListViewElements[viewIndex].ListSummaryPriceLabels[colIndex].Text = summary.Value.ToString("0.00");
                    ListViewElements[viewIndex].ListSummaryPriceDiscountLabels[colIndex].Text = summaryDiscount.Value.ToString("0.00");
                }
                else if (colIndex == 2)
                {
                    summary = ListViewElements[viewIndex]._ListViewRecords.ListView.ItemsSource.Cast<ItemChoosen>().Where(price => price.Price3 != null).Select(price => price.CntOfItems * float.Parse(price.Price3)).Sum();
                    summaryDiscount = ListViewElements[viewIndex]._ListViewRecords.ListView.ItemsSource.Cast<ItemChoosen>().Where(price => price.PriceDiscount3 != null).Select(price => price.CntOfItems * float.Parse(price.PriceDiscount3)).Sum();
                    ListViewElements[viewIndex].ListSummaryPriceLabels[colIndex].Text = summary.Value.ToString("0.00");
                    ListViewElements[viewIndex].ListSummaryPriceDiscountLabels[colIndex].Text = summaryDiscount.Value.ToString("0.00");
                }
            }
        }

        // item list (count of items) is same in each view, so we can take list of items from current view
        float summaryPrice = 0;
        float summaryPriceDiscount = 0;
        ListCheapestItemsPerCompanies.Clear();


        Dictionary<int, List<ItemChoosen>> dicListItems = new Dictionary<int, List<ItemChoosen>>();

        for (int viewIndex = 0; viewIndex < PageLayoutInfo.CntOfViewsInPage; viewIndex++)
            dicListItems.Add(viewIndex, ListViewElements[viewIndex]._ListViewRecords.ListView.ItemsSource.Cast<ItemChoosen>().ToList());

        int listSize = dicListItems.First().Value.Count; // (same count of rows in all views)

        for (int currItemIndex = 0; currItemIndex < listSize; currItemIndex++) // for each item in all views
        {
            CheapestItemPerCompany cheapestItemWithoutDiscount = null;
            CheapestItemPerCompany cheapestItemWithDiscount = null;

            float smallestPriceInRow = 0; // smallest price in row for all grid views
            float smallestPriceDiscountInRow = 0; // smallest price with discount in row for all grid view

            for (int viewIndex = 0; viewIndex < PageLayoutInfo.CntOfViewsInPage; viewIndex++) // now go through all pages ( pages - each page contains grid view)
            {
                float smallestPriceInRowPerView = 0;
                float smallestPriceDiscountInRowPerView = 0;

                ItemChoosen item = dicListItems[viewIndex][currItemIndex]; // take not yet processed item within current page - grid view

                for (int colIndex = 0; colIndex < PageLayoutInfo.ListViewLayoutInfo[viewIndex].GridCntCols; colIndex++) // and go through all columns in page - grid view
                {
                    if (colIndex == 0)
                    {
                        if (item.Price1 != null && smallestPriceInRow == 0)
                        {
                            smallestPriceInRowPerView = (item.CntOfItems * float.Parse(item.Price1));
                            smallestPriceInRow = smallestPriceInRowPerView;

                            cheapestItemWithoutDiscount = new CheapestItemPerCompany(company: PageLayoutInfo.ListViewLayoutInfo[viewIndex].ListCompaniesInView[colIndex], itemChoosen: item, discount: false);

                        }
                        else if (item.Price1 != null && (item.CntOfItems * float.Parse(item.Price1)) < smallestPriceInRow)
                        {
                            smallestPriceInRowPerView = (item.CntOfItems * float.Parse(item.Price1));
                            smallestPriceInRow = smallestPriceInRowPerView;
                            cheapestItemWithoutDiscount = new CheapestItemPerCompany(company: PageLayoutInfo.ListViewLayoutInfo[viewIndex].ListCompaniesInView[colIndex], itemChoosen: item, discount: false);
                        }

                        if (item.PriceDiscount1 != null && smallestPriceDiscountInRow == 0)
                        {
                            smallestPriceDiscountInRowPerView = (item.CntOfItems * float.Parse(item.PriceDiscount1));
                            smallestPriceDiscountInRow = smallestPriceDiscountInRowPerView;
                            cheapestItemWithDiscount = new CheapestItemPerCompany(company: PageLayoutInfo.ListViewLayoutInfo[viewIndex].ListCompaniesInView[colIndex], itemChoosen: item, discount: true);
                        }
                        else if (item.PriceDiscount1 != null && (item.CntOfItems * float.Parse(item.PriceDiscount1)) < smallestPriceDiscountInRow)
                        {
                            smallestPriceDiscountInRowPerView = (item.CntOfItems * float.Parse(item.PriceDiscount1));
                            smallestPriceDiscountInRow = smallestPriceDiscountInRowPerView;
                            cheapestItemWithDiscount = new CheapestItemPerCompany(company: PageLayoutInfo.ListViewLayoutInfo[viewIndex].ListCompaniesInView[colIndex], itemChoosen: item, discount: true);
                        }
                    }

                    if (colIndex == 1)
                    {
                        if (item.Price2 != null && (item.CntOfItems * float.Parse(item.Price2)) < smallestPriceInRow)
                        {
                            smallestPriceInRowPerView = (item.CntOfItems * float.Parse(item.Price2));
                            smallestPriceInRow = smallestPriceInRowPerView;
                            cheapestItemWithoutDiscount = new CheapestItemPerCompany(company: PageLayoutInfo.ListViewLayoutInfo[viewIndex].ListCompaniesInView[colIndex], itemChoosen: item, discount: false);
                        }

                        if (item.PriceDiscount2 != null && (item.CntOfItems * float.Parse(item.PriceDiscount2)) < smallestPriceDiscountInRow)
                        {
                            smallestPriceDiscountInRowPerView = (item.CntOfItems * float.Parse(item.PriceDiscount2));
                            smallestPriceDiscountInRow = smallestPriceDiscountInRowPerView;
                            cheapestItemWithDiscount = new CheapestItemPerCompany(company: PageLayoutInfo.ListViewLayoutInfo[viewIndex].ListCompaniesInView[colIndex], itemChoosen: item, discount: true);
                        }
                    }

                    if (colIndex == 2)
                    {
                        if (item.Price3 != null && (item.CntOfItems * float.Parse(item.Price3)) < smallestPriceInRow)
                        {
                            smallestPriceInRowPerView = (item.CntOfItems * float.Parse(item.Price3));
                            smallestPriceInRow = smallestPriceInRowPerView;
                            cheapestItemWithoutDiscount = new CheapestItemPerCompany(company: PageLayoutInfo.ListViewLayoutInfo[viewIndex].ListCompaniesInView[colIndex], itemChoosen: item, discount: false);
                        }

                        if (item.PriceDiscount3 != null && (item.CntOfItems * float.Parse(item.PriceDiscount3)) < smallestPriceDiscountInRow)
                        {
                            smallestPriceDiscountInRowPerView = (item.CntOfItems * float.Parse(item.PriceDiscount3));
                            smallestPriceDiscountInRow = smallestPriceDiscountInRowPerView;
                            cheapestItemWithDiscount = new CheapestItemPerCompany(company: PageLayoutInfo.ListViewLayoutInfo[viewIndex].ListCompaniesInView[colIndex], itemChoosen: item, discount: true);
                        }
                    }
                }
            }

            ListCheapestItemsPerCompanies.Add(cheapestItemWithoutDiscount);
            ListCheapestItemsPerCompanies.Add(cheapestItemWithDiscount);
            summaryPrice += smallestPriceInRow;
            summaryPriceDiscount += smallestPriceDiscountInRow;
        }


        for (int viewIndex = 0; viewIndex < PageLayoutInfo.CntOfViewsInPage; viewIndex++)
        {
            ListViewElements[viewIndex].SummaryLabelCheapestPriceValue.Text = $"{summaryPrice.ToString("0.00")}";
            ListViewElements[viewIndex].SummaryLabelCheapestPriceDiscountValue.Text = $"{summaryPriceDiscount.ToString("0.00")}";
        }
    }

    private void Button_Next_Clicked(object sender, EventArgs e)
    {
        if ((CurrentViewIndex + 1) < PageLayoutInfo.CntOfViewsInPage)
        {
            CurrentViewIndex++;
        }

        if (CurrentViewIndex + 1 == PageLayoutInfo.CntOfViewsInPage)
        {
            ListViewElements[CurrentViewIndex].BtnNext.IsEnabled = false;
        }

        if (CurrentViewIndex > 0)
            ListViewElements[CurrentViewIndex].BtnBack.IsEnabled = true;


        this.Content = ListViewElements[CurrentViewIndex].MainGrid.ViewGrid;
    }

    private void Button_Back_Clicked(object sender, EventArgs e)
    {
        if (CurrentViewIndex - 1 >= 0)
        {
            ListViewElements[CurrentViewIndex].BtnNext.IsEnabled = true;

            if ((CurrentViewIndex - 1) == 0)
            {
                ListViewElements[CurrentViewIndex].BtnBack.IsEnabled = false;
            }

            CurrentViewIndex--;

            this.Content = ListViewElements[CurrentViewIndex].MainGrid.ViewGrid;
        }
    }
}