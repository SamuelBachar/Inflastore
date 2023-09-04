using CommunityToolkit.Maui.Converters;
using Microsoft.Maui.Controls;
using Neminaj.Models;
using Neminaj.Repositories;
using SharedTypesLibrary.DTOs.API;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Neminaj.Views;

public class CardData
{
    public string Name { get; set; }
    public byte[] CardImage { get; set; }

    public bool IsKnownCard { get; set; }
}

public partial class CardsView : ContentPage
{
    SavedCardRepository _savedCardRepo { get; set; } = null;

    public delegate void PageBuilded(object sender, EventArgs e);
    public static event PageBuilded OnPageBuilded;

    public CardsView(SavedCardRepository savedCardRepository)
    {
        InitializeComponent();
        _savedCardRepo = savedCardRepository;

        this.Loaded += async (s, e) => { await BuildPage(); };
        OnPageBuilded += AssignList;
        AddCardView.On_AddCardView_CardAdded += async (s, e) => { await ChangeListOfCards(); };
    }

    private async Task BuildPage()
    {
        List<SavedCard> listSavedCards = await _savedCardRepo.GetAllSavedCards();

        if (listSavedCards.Count == 0)
        {
            this.Content = new Label
            {
                Text = "Nemáte pridanú žiadnu kartu\r\nKartu si môžte pridať stlačením tlačidla v pravom hornom rohu",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
                LineBreakMode = LineBreakMode.WordWrap,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };
        }
        else
        {
            listSavedCards = listSavedCards.OrderBy(card => card.CardName).ToList();

            this.BindingContext = this;
            this.listCards.ItemsSource = listSavedCards;
            this.Content = this.MainScrollView;
        }
        // Make sure someone is listening to event
        if (OnPageBuilded != null)
        {
            OnPageBuilded(this, new EventArgs()); // toto bolo vela krat ked view bolo transient
        }
    }

    private async Task ChangeListOfCards()
    {
        await Task.Run(async () =>
        {
            await BuildPage();
        });
    }

    private void AssignList(object sender, EventArgs e)
    {

    }

    private async void listCards_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        SavedCard savedCard = ((SavedCard)e.CurrentSelection.First());

        await Shell.Current.GoToAsync(nameof(SavedCardDetailView),
        new Dictionary<string, object>
        {
            [nameof(SavedCardRepository)] = this._savedCardRepo,
            ["CardID"] = savedCard.Id
        });
    }

    private async void ToolbarCardAdd_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(ChooseCardView));
    }

    private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        string text = ((SearchBar)sender).Text;

        if (text.Length >= 1)
        {
            List<SavedCard> savedCardList = await _savedCardRepo.SearchItems(text);
            listCards.ItemsSource = new ObservableCollection<SavedCard>(savedCardList);
            _savedCardRepo.ClearFilteredList();
        }

        if (text.Length == 0)
        {
            _savedCardRepo.ClearFilteredList();
            List<SavedCard> savedCardList = await _savedCardRepo.GetAllSavedCards();
            listCards.ItemsSource = new ObservableCollection<SavedCard>(savedCardList);
        }
    }
}

///// <summary>
///// Converts the incoming value from <see cref="byte"/>[] and returns the object of a type <see cref="ImageSource"/> or vice versa.
///// </summary>
//public class ByteArrayToImageSourceConverter : BaseConverter<byte[]?, ImageSource?>
//{
//    /// <inheritdoc/>
//    public override ImageSource? DefaultConvertReturnValue { get; set; } = null;

//    /// <inheritdoc/>
//    public override byte[]? DefaultConvertBackReturnValue { get; set; } = null;

//    /// <summary>
//    /// Converts the incoming value from <see cref="byte"/>[] and returns the object of a type <see cref="ImageSource"/>.
//    /// </summary>
//    /// <param name="value">The value to convert.</param>
//    /// <param name="culture">The culture to use in the converter. This is not implemented.</param>
//    /// <returns>An object of type <see cref="ImageSource"/>.</returns>
//    [return: NotNullIfNotNull(nameof(value))]
//    public override ImageSource? ConvertFrom(byte[]? value, CultureInfo? culture = null)
//    {
//        if (value is null)
//        {
//            return null;
//        }

//        return ImageSource.FromStream(() => new MemoryStream(value));
//    }

//    /// <summary>
//    /// Converts the incoming value from <see cref="StreamImageSource"/> and returns a <see cref="byte"/>[].
//    /// </summary>
//    /// <param name="value">The value to convert.</param>
//    /// <param name="culture">The culture to use in the converter. This is not implemented.</param>
//    /// <returns>An object of type <see cref="ImageSource"/>.</returns>
//    public override byte[]? ConvertBackTo(ImageSource? value, CultureInfo? culture = null)
//    {
//        if (value is null)
//        {
//            return null;
//        }

//        if (value is not StreamImageSource streamImageSource)
//        {
//            throw new ArgumentException("Expected value to be of type StreamImageSource.", nameof(value));
//        }

//        var streamFromImageSource = streamImageSource.Stream(CancellationToken.None).GetAwaiter().GetResult();

//        if (streamFromImageSource is null)
//        {
//            return null;
//        }

//        using var memoryStream = new MemoryStream();
//        streamFromImageSource.CopyTo(memoryStream);

//        return memoryStream.ToArray();
//    }
//}