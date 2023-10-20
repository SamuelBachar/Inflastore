using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Controls;
using Neminaj.ContentViews;
using Neminaj.Models;
using Neminaj.Repositories;
using SharedTypesLibrary.DTOs.API;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Neminaj.Views;


public partial class CardsView : ContentPage
{
    SavedCardRepository _savedCardRepo { get; set; } = null;

    public delegate void PageBuilded(object sender, EventArgs e);
    public static event PageBuilded OnPageBuilded;

    public bool ShowPopUp = true;

    public List<TapGestureRecognizer> ListTapGestureRecognizers { get; set; } = new List<TapGestureRecognizer>();

    PopUpActivityIndicator _popUpIndic = new PopUpActivityIndicator("Načítavam uložené karty ...");

    private delegate Task PageBuilded_Event();
    private event PageBuilded_Event On_PageBuilded_Event;


    public CardsView(SavedCardRepository savedCardRepository)
    {
        InitializeComponent();
        _savedCardRepo = savedCardRepository;
        this.BindingContext = this;

        this.Appearing += (s, e) => { this.Content = _popUpIndic; };
        this.Loaded += async (s, e) => { await BuildPage(); };
        AddCardView.On_AddCardView_CardAdded += async (s, e) => { await ChangeListOfCards(); };
        SavedCardDetailView.On_SavedCardDetailView_DeleteCard += async (s, e) => { await ChangeListOfCards(); };
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
            listSavedCards.ForEach(card => card.UknownCardColor = Color.FromInt(card.UknownCardColorDB));

            this.listCards.ItemsSource = listSavedCards;
            this.Content = this.MainControlWrapper;
        }
    }

    private async Task ChangeListOfCards()
    {
        ShowPopUp = false;
        await BuildPage();
        ShowPopUp = true;
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

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(SavedCardDetailView),
        new Dictionary<string, object>
        {
            [nameof(SavedCardRepository)] = this._savedCardRepo,
            ["CardID"] = e.Parameter
        });
    }
}