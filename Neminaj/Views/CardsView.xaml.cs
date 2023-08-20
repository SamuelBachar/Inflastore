using Microsoft.Maui.Controls;
using Neminaj.Models;
using Neminaj.Repositories;

namespace Neminaj.Views;

class ViewContent
{
    public Grid MainGrid { get; set; } = null;

    public Grid GridCards { get; set; } = null;

    public Button BtnScanCard { get; set; } = null;

    public Button BtnScanFromPicture { get; set; } = null;

    public List<SavedCard> ListCards { get; set; } = null;

    public List<Image> ListImages { get; set; } = null;

    public List<Frame> ListFrames { get; set; } = null;

    public List<TapGestureRecognizer> ListTapGestureRecognizers {get; set;} = null;

}

public partial class CardsView : ContentPage
{
    SavedCardRepository SavedCardRepo { get; set; } = null;

    ViewContent ViewContent { get; set; } = null;

    public CardsView(SavedCardRepository savedCardRepository)
    {
        InitializeComponent();
        SavedCardRepo = savedCardRepository;

        this.ViewContent = new ViewContent();
        this.ViewContent.MainGrid = new Grid();
        this.ViewContent.GridCards = new Grid();

        this.Loaded += async (s, e) => { await BuildPage(); };
        AddCardView.On_AddCardView_CardAdded += async (s, e) => { await FillGridWithCards(false); };
    }

    private async Task BuildPage()
    {
        // Get list of SavedCards for further use
        this.ViewContent.ListCards = await this.SavedCardRepo.GetAllSavedCards();

        // START Create Main Grid //

        this.ViewContent.MainGrid.ColumnSpacing = 10;
        this.ViewContent.MainGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star)); // Row 0: Grid for cards
        this.ViewContent.MainGrid.RowDefinitions.Add(new RowDefinition(new GridLength(0.10, GridUnitType.Star))); // Row 1: Buttons Scan and Scan from picture

        this.ViewContent.MainGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star)); // Add column
        this.ViewContent.MainGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star)); // Add column

        // END Create Main Grid //

        // START Create Cards Grid //
        this.ViewContent.GridCards.ColumnSpacing = 10;
        this.ViewContent.GridCards.RowSpacing = 10;

        // Create columns (fixed 2)
        this.ViewContent.GridCards.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));
        this.ViewContent.GridCards.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Auto));

        // Create rows ( first fixed size of 5) - this is here due to proportial layout of screen withing grid
        for (int i = 0; i < 10; i += 2)
            this.ViewContent.GridCards.RowDefinitions.Add(new RowDefinition(GridLength.Auto));

        // Check if 5 rows are enough for already saved cards, if not create additional rows
        if (this.ViewContent.GridCards.RowDefinitions.Count < (this.ViewContent.ListCards.Count / 2) + (this.ViewContent.ListCards.Count % 2))
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
        btnScanCard.MaximumHeightRequest = btnScanCard.FontSize;
        btnScanCard.MaximumWidthRequest = btnScanCard.Text.Length * btnScanCard.FontSize + 10;
        btnScanCard.Clicked += async (s, e) => { await BtnScanCard_Clicked(s, e); };

        this.ViewContent.MainGrid.Add(btnScanCard, column: 0, row: 1);

        Button btnScanCardFromPicture = new Button();
        btnScanCardFromPicture.Text = "Načítať z obrázku";
        btnScanCardFromPicture.MaximumHeightRequest = btnScanCard.FontSize;
        btnScanCardFromPicture.MaximumWidthRequest = btnScanCardFromPicture.Text.Length * btnScanCardFromPicture.FontSize + 10;
        btnScanCardFromPicture.Clicked += async (s, e) => { await BtnScanCard_Clicked(s, e); };

        this.ViewContent.MainGrid.Add(btnScanCardFromPicture, column: 1, row: 1);

        // END Create button Scan from picture //

        this.ViewContent.ListImages = new List<Image>();
        this.ViewContent.ListFrames = new List<Frame>();
        this.ViewContent.ListTapGestureRecognizers = new List<TapGestureRecognizer>();

        await this.FillGridWithCards(true);

        this.Content = this.ViewContent.MainGrid;
    }

    private async Task FillGridWithCards(bool firstBuild = false)
    {
        this.ViewContent.ListTapGestureRecognizers.Clear();

        await Task.Run(async () =>
        {
            this.ViewContent.ListCards = await this.SavedCardRepo.GetAllSavedCards();

            if (!firstBuild)
            {
                // Check if 5 rows are enough for already saved cards, if not create additional rows
                if (this.ViewContent.GridCards.RowDefinitions.Count < (this.ViewContent.ListCards.Count / 2) + (this.ViewContent.ListCards.Count % 2))
                {
                    for (int i = 0; i < (this.ViewContent.ListCards.Count - this.ViewContent.GridCards.RowDefinitions.Count); i += 2)
                        this.ViewContent.GridCards.RowDefinitions.Add(new RowDefinition(GridLength.Auto));
                }

                foreach (Image img in this.ViewContent.ListImages)
                    this.ViewContent.GridCards.Remove(img);

                foreach (Frame frame in this.ViewContent.ListFrames)
                    this.ViewContent.GridCards.Remove(frame);

                this.ViewContent.ListImages.Clear();
                this.ViewContent.ListFrames.Clear();
            }

            for (int row = 0, cardCounter = 0; cardCounter < this.ViewContent.ListCards.Count; row++)
            {
                for (int col = 0; ((cardCounter < this.ViewContent.ListCards.Count) && (col < 2)); col++, cardCounter++)
                {
                    TapGestureRecognizer tappedEvent = new TapGestureRecognizer();
                    tappedEvent.Tapped += CardsView_Tapped;
                    tappedEvent.CommandParameter = this.ViewContent.ListCards[cardCounter].Id;

                    if (this.ViewContent.ListCards[cardCounter].IsKnownCard)
                    {
                        Image img = new Image();

                        var stream = new MemoryStream(this.ViewContent.ListCards[cardCounter].Image);
                        {
                            img.Source = ImageSource.FromStream(() => stream);
                            img.Aspect = Aspect.AspectFit;
                            img.VerticalOptions = LayoutOptions.Center;
                            img.HorizontalOptions = LayoutOptions.Center;
                            img.HeightRequest = (this.Window.Height / 5);
                            img.WidthRequest = (this.Window.Width / 2) - 5;
                            img.GestureRecognizers.Add(tappedEvent);
                            this.ViewContent.GridCards.Add(img, col, row);
                            this.ViewContent.ListImages.Add(img);
                        }
                    }
                    else
                    {
                        Frame frame = new Frame();
                        frame.VerticalOptions = LayoutOptions.Center;
                        frame.HorizontalOptions = LayoutOptions.Center;
                        frame.HeightRequest = (this.Window.Height / 5);
                        frame.WidthRequest = (this.Window.Width / 2) - 5;
                        frame.BackgroundColor = Color.FromInt(this.ViewContent.ListCards[cardCounter].UknownCardColor);
                        frame.BorderColor = Colors.Black;
                        frame.GestureRecognizers.Add(tappedEvent);

                        Label lbl = new Label();
                        lbl.VerticalOptions = LayoutOptions.Center;
                        lbl.HorizontalOptions = LayoutOptions.Center;
                        lbl.Text = this.ViewContent.ListCards[cardCounter].CardName;
                        lbl.LineBreakMode = LineBreakMode.WordWrap;
                        lbl.FontSize = frame.WidthRequest / 12;

                        frame.Content = lbl;
                        this.ViewContent.ListFrames.Add(frame);

                        this.ViewContent.GridCards.Add(frame, col, row);
                    }

                    this.ViewContent.ListTapGestureRecognizers.Add(tappedEvent);
                }
            }
        });
    }

    private async void CardsView_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SavedCardDetailView),
        new Dictionary<string, object>
        {
            [nameof(SavedCardRepository)] = this.SavedCardRepo,
            ["CardID"] = e.Parameter
        });
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