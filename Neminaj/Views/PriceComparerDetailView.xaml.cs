using Neminaj.GlobalText;
using Neminaj.ViewsModels;

namespace Neminaj.Views;

public class CompanyItemsDetailData
{
    public string CompanyName { get; set; }
    public List<Tuple<string, float>> ListItemData { get; set; }
}

public class CompanyItemsDetailText
{
    public string CompanyName { get; set; }
    public List<string> ListItemsNames { get; set; }
}

public partial class PriceComparerDetailView : ContentPage
{
    private readonly PriceComparerDetailViewModel _priceComparerDetailViewModel;

    private List<CompanyItemsDetailText> _listWithoutDiscount;
    private List<CompanyItemsDetailText> _listWithDiscount;

    private List<CompanyItemsDetailData> _listWithoutDiscountExtra;
    private List<CompanyItemsDetailData> _listWithDiscountExtra;

    private Grid _gridWithDiscount { get; set; } = new Grid();
    private Grid _gridWithoutDiscount { get; set; } = new Grid();
    private ScrollView _scrollViewWithDiscount { get; set; }
    private ScrollView _scrollViewWithoutDiscount { get; set; }

    private bool AlreadyBuilded = false;

    public PriceComparerDetailView(PriceComparerDetailViewModel priceComparerDetailViewModel)
    {
        InitializeComponent();
        _priceComparerDetailViewModel = priceComparerDetailViewModel;
        BindingContext = _priceComparerDetailViewModel;
        //this.Loaded += async (s, e) => { await BuildPage(); };
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (!AlreadyBuilded)
        {
            BuildPage();
        }
    }

    private void BuildPage()
    {
        var groupsWithDiscount = _priceComparerDetailViewModel.ListCheapestItemsPerCompanies.Where(item => item.Discount).GroupBy(item => new { item.Company }).ToList();

        _listWithDiscount = new List<CompanyItemsDetailText>();
        _listWithDiscountExtra = new List<CompanyItemsDetailData>();

        foreach (var group in groupsWithDiscount)
        {
            _listWithDiscount.Add(new CompanyItemsDetailText
            {
                CompanyName = group.Key.Company.Name,
                ListItemsNames = group.Select(s => $"{s.ItemChoosen.FinalName}     {s.Price}").ToList(),
            });

            _listWithDiscountExtra.Add(new CompanyItemsDetailData
            {
                CompanyName = group.Key.Company.Name,
                //ListItemData = group.Select(s => new { s.ItemChoosen.FinalName, s.Price }).
                //AsEnumerable().
                //Select(d => new Tuple<string, float>(d.FinalName, d.Price)).ToList()
                ListItemData = group.Select(s => new Tuple<string, float>(s.ItemChoosen.FinalName, s.Price)).ToList()
            });
        }

        BuildGridView(_listWithDiscount, _listWithDiscountExtra, _gridWithDiscount, true);

        _scrollViewWithDiscount = new ScrollView
        {
            BackgroundColor = Colors.White,
            Content = _gridWithDiscount,
            HorizontalOptions = LayoutOptions.Center
        };

        var groupsWithoutDiscount = _priceComparerDetailViewModel.ListCheapestItemsPerCompanies.Where(item => !item.Discount).GroupBy(item => new { item.Company }).ToList();

        _listWithoutDiscount = new List<CompanyItemsDetailText>();
        _listWithoutDiscountExtra = new List<CompanyItemsDetailData>();

        foreach (var group in groupsWithoutDiscount)
        {
            _listWithoutDiscount.Add(new CompanyItemsDetailText
            {
                CompanyName = group.Key.Company.Name,
                ListItemsNames = group.Select(s => $"{s.ItemChoosen.FinalName}     {s.Price}").ToList(),
            });

            _listWithoutDiscountExtra.Add(new CompanyItemsDetailData
            {
                CompanyName = group.Key.Company.Name,
                //ListItemData = group.Select(s => new { s.ItemChoosen.FinalName, s.Price }).
                //AsEnumerable().
                //Select(d => new Tuple<string, float>(d.FinalName, d.Price)).ToList()

                ListItemData = group.Select(s => new Tuple<string, float>(s.ItemChoosen.FinalName, s.Price)).ToList()
            });
        }

        BuildGridView(_listWithoutDiscount, _listWithoutDiscountExtra, _gridWithoutDiscount, false);

        _scrollViewWithoutDiscount = new ScrollView
        {
            BackgroundColor = Colors.White,
            Content = _gridWithoutDiscount,
            HorizontalOptions = LayoutOptions.Center
        };

        this.Content = _scrollViewWithDiscount;
        AlreadyBuilded = true;
    }

    private void BuildGridView(List<CompanyItemsDetailText> listItemsPerCompanies, List<CompanyItemsDetailData> listItemsPerCompaniesData, Grid grid, bool discount)
    {
        int row = 0;
        grid.Padding = 10;
        grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
        grid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));

        Label lblTypeOfList = new Label
        {
            Text = discount ? "Nákup s kartou" : "Nákup bez karty",
            FontSize = 20,
            FontAttributes = FontAttributes.Bold,
            VerticalOptions = LayoutOptions.Center,
            HorizontalOptions = LayoutOptions.Center,
        };

        Grid.SetColumnSpan(lblTypeOfList, 2);
        Grid.SetRow(lblTypeOfList, row);
        grid.Add(lblTypeOfList);

        Button btnNextBack = new Button
        {
            Text = discount ? Texts.Next : Texts.Back,
            HorizontalOptions = LayoutOptions.Center,
            VerticalOptions = LayoutOptions.Center
        };

        btnNextBack.Clicked += BtnNextBack_Clicked;
        Grid.SetRow(btnNextBack, row);
        Grid.SetColumn(btnNextBack, 2);
        grid.Add(btnNextBack);

        //foreach (var item in listItemsPerCompanies)
        //{
        //    grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        //    row++;

        //    Label lblCompany = new Label
        //    {
        //        Text = $"\r\n{item.CompanyName}\r\n",
        //        FontSize = 14,
        //        FontAttributes = FontAttributes.Bold
        //    };

        //    Grid.SetRow(lblCompany, row);
        //    Grid.SetColumn(lblCompany, 0);
        //    grid.Add(lblCompany);

        //    grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        //    row++;

        //    Label itemsStringAgg = new Label
        //    {
        //        Text = item.ListItemsNames.Aggregate((i, j) => i + "\r\n" + j),
        //        LineBreakMode = LineBreakMode.WordWrap,
        //        HorizontalOptions = LayoutOptions.StartAndExpand,
        //        VerticalOptions = LayoutOptions.CenterAndExpand
        //    };

        //    Grid.SetColumnSpan(itemsStringAgg, 3);
        //    Grid.SetRow(itemsStringAgg, row);
        //    grid.Add(itemsStringAgg);
        //}

        foreach (var item in listItemsPerCompaniesData)
        {
            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            row++;

            Label lblCompany = new Label
            {
                Text = $"\r\n{item.CompanyName}\r\n",
                FontSize = 14,
                FontAttributes = FontAttributes.Bold
            };

            Grid.SetRow(lblCompany, row);
            Grid.SetColumn(lblCompany, 0);
            grid.Add(lblCompany);

            foreach (var data in item.ListItemData)
            {
                grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
                row++;

                Label itemName = new Label
                {
                    Text = data.Item1,
                    LineBreakMode = LineBreakMode.WordWrap,
                    HorizontalOptions = LayoutOptions.StartAndExpand,
                    VerticalOptions = LayoutOptions.CenterAndExpand
                };

                Grid.SetRow(itemName, row);
                Grid.SetColumn(itemName, 0);
                Grid.SetColumnSpan(itemName, 2);
                grid.Add(itemName);

                Label itemPrice = new Label
                {
                    Text = data.Item2.ToString("0.00") + " \u20AC",
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center
                };

                Grid.SetRow(itemPrice, row);
                Grid.SetColumn(itemPrice, 2);
                grid.Add(itemPrice);
            }
        }
    }

    private void BtnNextBack_Clicked(object sender, EventArgs e)
    {
        if (((Button)sender).Text == Texts.Next)
        {
            this.Content = _scrollViewWithoutDiscount;
        }

        if (((Button)sender).Text == Texts.Back)
        {
            this.Content = this._scrollViewWithDiscount;
        }
    }
}