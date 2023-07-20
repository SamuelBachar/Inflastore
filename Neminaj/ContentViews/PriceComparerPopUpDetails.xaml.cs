using CommunityToolkit.Maui.Views;
using Neminaj.GlobalText;
using Neminaj.Views;
using System.Collections.ObjectModel;

namespace Neminaj.ContentViews;

public class CompanyItemsDetailText
{
    public string CompanyName { get; set; }
    public List<string> ListItemsNames { get; set; }
}

public partial class PriceComparerPopUpDetails : Popup
{

    public ObservableCollection<CompanyItemsDetailText> ObservableCompanyItemsDetailText { get; set; } = new ObservableCollection<CompanyItemsDetailText>();

    CompanyItemsDetailText withoutDiscount = new CompanyItemsDetailText();
    CompanyItemsDetailText withDiscount = new CompanyItemsDetailText();

    Border Border { get; set; }

    Grid GridWithDiscount { get; set; } = new Grid();
    Grid GridWithoutDiscount { get; set; } = new Grid();

    ScrollView ScrollViewWithDiscount { get; set; }
    ScrollView ScrollViewWithoutDiscount { get; set; }

    List<CompanyItemsDetailText> listWithoutDiscount;
    List<CompanyItemsDetailText> listWithDiscount;

    private void BuildGridView(List<CompanyItemsDetailText> listItemsPerCompanies, Grid grid, bool discount)
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

        foreach (var item in listItemsPerCompanies)
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

            grid.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
            row++;

            Label itemsStringAgg = new Label
            {
                Text = item.ListItemsNames.Aggregate((i, j) => i + "\r\n" + j),
                LineBreakMode = LineBreakMode.WordWrap,
                HorizontalOptions = LayoutOptions.StartAndExpand,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };

            Grid.SetColumnSpan(itemsStringAgg, 3);
            Grid.SetRow(itemsStringAgg, row);
            grid.Add(itemsStringAgg);
        }
    }

    public PriceComparerPopUpDetails(List<CheapestItemPerCompany> listCheapestItemsPerCompany)
    {
        InitializeComponent();

        var groupsWithDiscount = listCheapestItemsPerCompany.Where(item => item.Discount).GroupBy(item => new { item.Company }).ToList();

        listWithDiscount = new List<CompanyItemsDetailText>();

        foreach (var group in groupsWithDiscount)
        {
            listWithDiscount.Add(new CompanyItemsDetailText
            {
                CompanyName = group.Key.Company.Name,
                ListItemsNames = group.Select(s => s.ItemChoosen.FinalName).ToList(),
            });
        }

        BuildGridView(listWithDiscount, GridWithDiscount, true);

        ScrollViewWithDiscount = new ScrollView
        {
            BackgroundColor = Colors.White,
            Content = GridWithDiscount,
            HorizontalOptions = LayoutOptions.Center
        };

        var groupsWithoutDiscount = listCheapestItemsPerCompany.Where(item => !item.Discount).GroupBy(item => new { item.Company }).ToList();

        listWithoutDiscount = new List<CompanyItemsDetailText>();

        foreach (var group in groupsWithoutDiscount)
        {
            listWithoutDiscount.Add(new CompanyItemsDetailText
            {
                CompanyName = group.Key.Company.Name,
                ListItemsNames = group.Select(s => s.ItemChoosen.FinalName).ToList(),
            });
        }

        BuildGridView(listWithoutDiscount, GridWithoutDiscount, false);

        ScrollViewWithoutDiscount = new ScrollView
        {
            BackgroundColor = Colors.White,
            Content = GridWithoutDiscount,
            HorizontalOptions = LayoutOptions.Center
        };

        Border = new Border
        {
            BackgroundColor = Colors.White,
            Content = ScrollViewWithDiscount,
            MaximumHeightRequest = 600, // TODO treba podla obrazoky telefonu
            MaximumWidthRequest = 300
        };

        this.Content = Border;
    }

    private void BtnNextBack_Clicked(object sender, EventArgs e)
    {
        if (((Button)sender).Text == Texts.Next)
        {
            this.Border.Content = ScrollViewWithoutDiscount;
            this.Content = this.Border;
        }

        if (((Button)sender).Text == Texts.Back)
        {
            this.Border.Content = ScrollViewWithDiscount;
            this.Content = this.Border;
        }
    }
}