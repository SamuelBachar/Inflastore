using CommunityToolkit.Maui.Converters;
using Neminaj.Models;
using Neminaj.Repositories;
using Neminaj.Utils;
using Neminaj.ViewsModels;
using SharedTypesLibrary.DTOs.API;
using SharedTypesLibrary.Models.API.DatabaseModels;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Neminaj.Views;

public partial class ChooseCardView : ContentPage
{
    private CompanyRepository _companyRepository { get; set; } = null;
    private SavedCardRepository _savedCardRepository { get; set; } = null;
    private List<CardData> _listCardDatas { get; set; } = new List<CardData>();

    byte[] _imageBufferOtherCard = null;
    public ChooseCardView(CompanyRepository companyRepository, SavedCardRepository savedCardRepository)
    {
        InitializeComponent();
        _companyRepository = companyRepository;
        _savedCardRepository = savedCardRepository;
        this.Loaded += async (s, e) => { await BuildPage(); };
    }

    private async Task BuildPage()
    {
            _listCardDatas.Clear();

            List<CompanyDTO> listCompanies = await _companyRepository.GetAllCompaniesAsync();
            listCompanies = listCompanies.OrderBy(company => company.Name).ToList();

            foreach (CompanyDTO company in listCompanies)
            {
                _listCardDatas.Add
                (
                    new CardData
                    {
                        Name = company.Name,
                        CardImage = company.Image,
                        IsKnownCard = true
                    }
                );
            }

            using (Stream stream = EmbeddedResource.OpenEmbeddedImageStream("club_cards.png"))
            {
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    stream.CopyTo(memoryStream);

                    _imageBufferOtherCard = new byte[stream.Length];
                    memoryStream.ToArray().CopyTo(_imageBufferOtherCard, 0);
                }
            }

            _listCardDatas.Add(new CardData
            {
                Name = "Iná karta",
                CardImage = _imageBufferOtherCard,
                IsKnownCard = false
            });

            listCards.ItemsSource = new ObservableCollection<CardData>(_listCardDatas);
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

    }
}