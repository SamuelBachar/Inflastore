using CommunityToolkit.Maui.Converters;
using CommunityToolkit.Maui.Views;
using Neminaj.ContentViews;
using Neminaj.Models;
using Neminaj.Repositories;
using Neminaj.Utils;
using Neminaj.ViewsModels;
using SharedTypesLibrary.DTOs.API;
using SharedTypesLibrary.Models.API.DatabaseModels;
using SQLite;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Neminaj.Views;

public partial class ChooseCardView : ContentPage
{
    private ClubCardRepository _clubCardRepository { get; set; } = null;
    private SavedCardRepository _savedCardRepository { get; set; } = null;
    private List<CardData> _listCardDatas { get; set; } = new List<CardData>();

    byte[] _imageBufferOtherCard = null;

    ActivityIndicatorPopUp _popUpIndic = new ActivityIndicatorPopUp("Načítavam karty ...");

    private delegate Task PageBuilded_Event();
    private event PageBuilded_Event On_PageBuilded_Event;

    public ChooseCardView(ClubCardRepository clubCardRepository, SavedCardRepository savedCardRepository)
    {
        InitializeComponent();
        _clubCardRepository = clubCardRepository;
        _savedCardRepository = savedCardRepository;

        this.Loaded += async (s, e) => { await ShowPopupIndicator(); };
        this.Loaded += async (s, e) => { await BuildPage(); };
        this.On_PageBuilded_Event += async () => { await On_PageBuilded(); };
    }

    private async Task BuildPage()
    {
        //ActivityIndicatorPopUp popUpIndic = new ActivityIndicatorPopUp("Načítavam karty ...");
        //this.ShowPopup(popUpIndic);
        //popUpIndic.TurnOnActivityIndicator();

        _listCardDatas.Clear();

        List<ClubCardDTO> listClubCards = await _clubCardRepository.GetAllClubCards();

        List<ClubCardDTO> listClubCardsTemp = new List<ClubCardDTO>();

        // deep copy
        listClubCardsTemp.AddRange(listClubCards.Select(i => new ClubCardDTO()
        {
            Id = i.Id,
            Name = i.Name,
            Company_Id = i.Company_Id,
            Image = i.Image,
            Url = i.Url,
        }));

        ClubCardDTO clubCardOther = listClubCardsTemp.Where(card => card.Name == "Other").First();
        listClubCardsTemp.Remove(clubCardOther);
        listClubCardsTemp = listClubCardsTemp.OrderBy(company => company.Name).ToList();

        clubCardOther.Name = "Iná karta";
        listClubCardsTemp.Add(clubCardOther);

        foreach (ClubCardDTO clubCard in listClubCardsTemp)
        {
            _listCardDatas.Add
            (
                new CardData
                {
                    Name = clubCard.Name,
                    CardImage = clubCard.Image,
                    CardImageUrl = clubCard.Url,
                    IsKnownCard = clubCard.Name == "Iná karta" ? false : true
                }
            );
        }

        //using (Stream stream = EmbeddedResource.OpenEmbeddedImageStream("club_cards.png"))
        //{
        //    using (MemoryStream memoryStream = new MemoryStream())
        //    {
        //        stream.CopyTo(memoryStream);

        //        _imageBufferOtherCard = new byte[stream.Length];
        //        memoryStream.ToArray().CopyTo(_imageBufferOtherCard, 0);
        //    }
        //}

        //_listCardDatas.Add(new CardData
        //{
        //    Name = "Iná karta",
        //    CardImage = _imageBufferOtherCard,
        //    IsKnownCard = false
        //});

        if (On_PageBuilded_Event != null)
        {
            await On_PageBuilded_Event();
        }

        listCards.ItemsSource = new ObservableCollection<CardData>(_listCardDatas);
        //popUpIndic.TurnOffActivityIndicator();
    }

    private async Task ShowPopupIndicator()
    {
        await this.ShowPopupAsync(_popUpIndic);
    }

    private async Task On_PageBuilded()
    {
        await Task.Delay(1000);
        await _popUpIndic.CloseAsync();
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        CardData cardData = (CardData)(e.Parameter);

        await Shell.Current.GoToAsync(nameof(AddCardView),
        new Dictionary<string, object>
        {
            [nameof(SavedCardRepository)] = this._savedCardRepository,
            ["CardID"] = 0, // not used in this case
            [nameof(CardData)] = cardData
        });
    }

    private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {
        string text = ((SearchBar)sender).Text;

        if (text.Length >= 1)
        {
            List<CardData> cardDataList = await _clubCardRepository.SearchItems(_listCardDatas, text);
            listCards.ItemsSource = new ObservableCollection<CardData>(cardDataList);
            _clubCardRepository.ClearFilteredList();
        }

        if (text.Length == 0)
        {
            _clubCardRepository.ClearFilteredList();
            listCards.ItemsSource = new ObservableCollection<CardData>(_listCardDatas);
        }
    }
}