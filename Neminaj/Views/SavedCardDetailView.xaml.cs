using Camera.MAUI;
using Neminaj.Models;
using Neminaj.ViewsModels;
using Syncfusion.Maui.Barcode;
using ZXing;

namespace Neminaj.Views;

// todo do it based on SavedCartDetail.xaml and SavedCartDetail.xaml.cs and calling is in v CartListView.xaml.cs
// another example is in CardsView -> AddCardView and SavedCardViewModel 
// and do it based on SavedCard
public partial class SavedCardDetailView : ContentPage
{
    SavedCardDetailViewModel _savedCardDetailViewModel { get; set; } = null;
    SavedCard SavedCard { get; set; } = null;

    // Events
    public delegate void SavedCardDetail_DeleteCard(object sender, EventArgs e);
    public static event SavedCardDetail_DeleteCard On_SavedCardDetailView_DeleteCard;

    public SavedCardDetailView(SavedCardDetailViewModel savedCardDetailViewModel)
    {
        InitializeComponent();
        BindingContext = savedCardDetailViewModel;
        _savedCardDetailViewModel = savedCardDetailViewModel;
        this.Appearing += async (s, e) => { await BuildPage(); };
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        // here SavedCardDetailViewModel and its properties are initialized
    }

    private async Task BuildPage()
    {
        SavedCard = await _savedCardDetailViewModel.GetSpecificCard(_savedCardDetailViewModel.CardID);

        if (SavedCard.CardFormat == (int)ZXing.BarcodeFormat.CODE_128)
            sfBarcodeGen.Symbology = new Code128();
        else if (SavedCard.CardFormat == (int)ZXing.BarcodeFormat.EAN_13)
            sfBarcodeGen.Symbology = new EAN13();
        else if (SavedCard.CardFormat == (int)ZXing.BarcodeFormat.EAN_8)
            sfBarcodeGen.Symbology = new EAN8();
        else if (SavedCard.CardFormat == (int)ZXing.BarcodeFormat.UPC_A)
            sfBarcodeGen.Symbology = new UPCA();
        else if (SavedCard.CardFormat == (int)ZXing.BarcodeFormat.UPC_E)
            sfBarcodeGen.Symbology = new UPCE();
        else if (SavedCard.CardFormat == (int)ZXing.BarcodeFormat.CODE_39)
            sfBarcodeGen.Symbology = new Code39();
        else if (SavedCard.CardFormat == (int)ZXing.BarcodeFormat.CODE_93)
            sfBarcodeGen.Symbology = new Code93();
        else if (SavedCard.CardFormat == (int)ZXing.BarcodeFormat.CODABAR)
            sfBarcodeGen.Symbology = new Codabar();
        else if (SavedCard.CardFormat == (int)ZXing.BarcodeFormat.DATA_MATRIX)
        {
            sfBarcodeGen.Symbology = new DataMatrix();
            sfBarcodeGen.WidthRequest = 300;
            sfBarcodeGen.HeightRequest= 300;
        }
        else if (SavedCard.CardFormat == (int)ZXing.BarcodeFormat.QR_CODE)
        {
            sfBarcodeGen.Symbology = new QRCode();
            sfBarcodeGen.WidthRequest = 300;
            sfBarcodeGen.HeightRequest = 300;
        }

        sfBarcodeGen.Value = SavedCard.CardCode;
        sfBarcodeGen.ShowText = true;
    }

    private async void ToolbarItemDelete_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Mazanie karty", "Prajete si naozaj zmazať klubovú kartu?", "Áno", "Nie");

        if (answer)
        {
            if (!await _savedCardDetailViewModel.DeleteSavedCard(_savedCardDetailViewModel.CardID))
            {
                await DisplayAlert("Chyba pri mazaní", "Pri mazaní nastala chyba: " + SQLConnection.StatusMessage, "Zavrieť");
            }
            else
            {
                // Make sure someone is listening to event
                if (On_SavedCardDetailView_DeleteCard != null)
                {
                    On_SavedCardDetailView_DeleteCard(this, new EventArgs());
                }

                await DisplayAlert("", "Klubová karta zmazaná", "Zavrieť");
            }
        }
    }
}