using CommunityToolkit.Maui.Converters;
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

    private async void listCards_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        CardData cardData = ((CardData)e.CurrentSelection.First());

        await Shell.Current.GoToAsync(nameof(AddCardView),
        new Dictionary<string, object>
        {
            [nameof(SavedCardRepository)] = this._savedCardRepository,
            ["CardID"] = null,
            [nameof(CardData)] = cardData
        });
    }

    private async void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
    {

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