using Microsoft.Maui.Controls;
using Neminaj.Models;
using Neminaj.Repositoriesô;

namespace Neminaj.Views;

class ViewContent
{
    public Grid MainGrid { get; set; } = null;

    public Grid GridCards { get; set; } = null;

    public Button BtnScanCard { get; set; } = null;

    public Button BtnScanFromPicture { get; set; } = null;

    public List<SavedCard> ListCards { get; set; } = null;

    public List<Image> ListImages { get; set; } = null;

}

public partial class CardsView : ContentPage
{
    SavedCardRepository SavedCardRepo { get; set; } = null;

    ViewContent ViewContent { get; set; } = null;

    Image Image = new Image();
    Frame Frame = new Frame();

    public CardsView(SavedCardRepository savedCardRepository)
    {
        InitializeComponent();

        SavedCardRepo = savedCardRepository;
        ViewContent = new ViewContent();
        this.ViewContent.MainGrid = new Grid();
        this.ViewContent.GridCards = new Grid();

        this.Loaded += async (s, e) => { await BuildPage(); };
    }

    private async Task BuildPage()
    {
        // Get list of SavedCards for further use
        this.ViewContent.ListCards = await this.SavedCardRepo.GetAllSavedCards();

        // START Create Main Grid //

        //this.ViewContent.MainGrid = new Grid();
        this.ViewContent.MainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star)); // Row 0: Grid for cards
        this.ViewContent.MainGrid.RowDefinitions.Add(new RowDefinition(new GridLength(0.10, GridUnitType.Star))); // Row 1: Buttons Scan and Scan from picture

        this.ViewContent.MainGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star)); // Add column
        this.ViewContent.MainGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star)); // Add column

        // END Create Main Grid //

        // START Create Cards Grid //
        //this.ViewContent.GridCards = new Grid();

        // Create columns (fixed 2)
        this.ViewContent.GridCards.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        this.ViewContent.GridCards.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));

        // Create rows ( first fixed size of 5)
        for (int i = 0; i < 10; i += 2)
            this.ViewContent.GridCards.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

        // Check if 5 rows are enough for already saved cards, if not create additional rows
        if (this.ViewContent.GridCards.RowDefinitions.Count < this.ViewContent.ListCards.Count)
        {
            for (int i = 0; i < (this.ViewContent.ListCards.Count - this.ViewContent.GridCards.RowDefinitions.Count); i += 2)
                this.ViewContent.GridCards.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
        }

        // Create scroll view and add CardsGrid to scroll view
        ScrollView scrollView = new ScrollView();
        scrollView.VerticalScrollBarVisibility = ScrollBarVisibility.Always;
        scrollView.Content = this.ViewContent.GridCards;
        Grid.SetColumnSpan(scrollView, 2);
        // END Create Cards Grid //

        // START Add scroll view with Cards Grid into first Row of Main grid //
        this.ViewContent.MainGrid.Add(scrollView, column: 0, row: 0);
        // END Add scroll view with Cards Grid into first Row of Main grid //

        // START Create button Scan Card //

        Button btnScanCard = new Button();
        btnScanCard.Text = "Skenovať";
        btnScanCard.Clicked += async (s, e) => { await BtnScanCard_Clicked(s, e); };

        HorizontalStackLayout horizCol0 = new HorizontalStackLayout();
        horizCol0.HorizontalOptions = LayoutOptions.Center;
        horizCol0.Add(btnScanCard);

        this.ViewContent.MainGrid.Add(horizCol0, column: 0, row: 1);

        Button btnScanCardFromPicture = new Button();
        btnScanCardFromPicture.Text = "Načítať z obrázku";
        btnScanCardFromPicture.Clicked += async (s, e) => { await BtnScanCard_Clicked(s, e); };

        HorizontalStackLayout horizCol1 = new HorizontalStackLayout();
        horizCol1.HorizontalOptions = LayoutOptions.Center;
        horizCol1.Add(btnScanCardFromPicture);

        this.ViewContent.MainGrid.Add(horizCol1, column: 1, row: 1);

        // END Create button Scan from picture //

        this.ViewContent.ListImages = new List<Image>();

        for (int row = 0, cardCounter = 0; cardCounter < this.ViewContent.ListCards.Count; row++)
        {
            for (int col = 0; ((cardCounter < this.ViewContent.ListCards.Count) && (col < 2)); col++, cardCounter++)
            {
                var stream = new MemoryStream(this.ViewContent.ListCards[cardCounter].Image);
                {
                    Image img = new Image();
                    img.Source = ImageSource.FromStream(() => stream);
                    img.Aspect = Aspect.AspectFit;
                    img.HeightRequest = (this.Window.Height / 5);
                    img.WidthRequest = (this.Window.Width / 2);

                    this.ViewContent.ListImages.Add(img);

                    this.ViewContent.GridCards.Add(img, col, row);
                }
            }
        }

        this.Content = this.ViewContent.MainGrid;
    }

    private async Task BtnScanCard_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AddCardView),
        new Dictionary<string, object>
        {
            [nameof(SavedCardRepository)] = this.SavedCardRepo,
        });
    }
}