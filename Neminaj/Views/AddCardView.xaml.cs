using Camera.MAUI;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Storage;
using Neminaj.ContentViews;
using Neminaj.Models;
using Neminaj.Repositories;
using Neminaj.Utils;
using Neminaj.ViewsModels;
using System.Dynamic;
using System.Reflection;
using ZXing;

namespace Neminaj.Views;


public class TempCardData
{
    public ZXing.BarcodeFormat Format { get; set; }
    public string CardCode { get; set; }

    public byte[] Image { get; set; }

    public bool IsKnownCard { get; set; } = false;

    public string CardName { get; set; }
}

public partial class AddCardView : ContentPage
{
    // Events
    public delegate void AddCardView_CardAdded(object sender, EventArgs e);
    public static event AddCardView_CardAdded On_AddCardView_CardAdded;

    TempCardData TempCardData { get; set; } = new TempCardData();
    SavedCardDetailViewModel SavedCardViewModel { get; set; } = null;

    ResultNotKnownCard ResultNotKnownCard { get; set; } = new ResultNotKnownCard();

    public AddCardView(SavedCardDetailViewModel savedCardViewModel)
    {
        InitializeComponent();

        BindingContext = savedCardViewModel;
        SavedCardViewModel = savedCardViewModel;

        NotKnownCardView.On_NotKnownCardView_BtnAddCard_Clicked += On_NotKnownCardView_BtnAddCard_Clicked;

        cameraView.BarCodeOptions = new()
        {
            PossibleFormats =
            {
                ZXing.BarcodeFormat.QR_CODE,
                ZXing.BarcodeFormat.All_1D,
                ZXing.BarcodeFormat.MAXICODE,
                ZXing.BarcodeFormat.RSS_14
            },
            AutoRotate = true,
            TryHarder = true,
            TryInverted = true,
            ReadMultipleCodes = false
        };

        cameraView.BarCodeDetectionFrameRate = 20;
        cameraView.BarCodeDetectionMaxThreads = 10;
        cameraView.ControlBarcodeResultDuplicate = true;
        cameraView.BarCodeDetectionEnabled = true;
        cameraView.FlashMode = FlashMode.Auto;

        if (cameraView.MaxZoomFactor >= 2.5f)
            cameraView.ZoomFactor = 2.5f;

        this.Appearing += AddCardView_Appearing;
    }

    private void AddCardView_Appearing(object sender, EventArgs e)
    {
        this.btnAddCard.IsVisible = false;
        this.lblCode.Text = string.Empty;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        // az tu su nainicializovane hodnoty pre SavedCardViewModel
    }

    private void cameraView_CamerasLoaded(object sender, EventArgs e)
    {
        cameraView.Camera = cameraView.Cameras.First();

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            // todo skontrolovat ci na samsungu ma problem zo startovanim kamery
            await cameraView.StopCameraAsync(); // malo by to tu byt lebo je na nejakych zariadeniach problem ked sa rovno zavola Start
            await cameraView.StartCameraAsync();
        });
    }

    private void cameraView_BarcodeDetected(object sender, Camera.MAUI.ZXingHelper.BarcodeEventArgs args)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            string text = $"{args.Result[0].BarcodeFormat}: {args.Result[0].Text}";
            lblCode.Text = $"Skenovanie úspešné:\r\n{text}";

            TempCardData.Format = args.Result[0].BarcodeFormat;
            TempCardData.CardCode = args.Result[0].Text;
            TempCardData.Image = SavedCardViewModel.CardData.CardImage;
            TempCardData.CardName = SavedCardViewModel.CardData.Name;
            TempCardData.IsKnownCard = SavedCardViewModel.CardData.IsKnownCard;

            this.btnAddCard.IsVisible = true;
        });
    }

    private async void btnAddCard_Clicked(object sender, EventArgs e)
    {
        SavedCard savedCard = new SavedCard();
        savedCard.CardFormat = (int)TempCardData.Format;
        savedCard.CardCode = TempCardData.CardCode;
        savedCard.IsKnownCard = TempCardData.IsKnownCard;

        if (!TempCardData.IsKnownCard)
        {
            await Shell.Current.GoToAsync(nameof(NotKnownCardView),
            new Dictionary<string, object>
            {
                [nameof(ResultNotKnownCard)] = ResultNotKnownCard,
            });
        }
        else
        {
            savedCard.Image = TempCardData.Image;
            savedCard.CardName = TempCardData.CardName;
            await InsertNewCard(savedCard);
        }
    }

    private async void On_NotKnownCardView_BtnAddCard_Clicked(object sender, EventArgs e)
    {
        SavedCard savedCard = new SavedCard();
        savedCard.CardFormat = (int)TempCardData.Format;
        savedCard.CardCode = TempCardData.CardCode;

        savedCard.CardName = ResultNotKnownCard.CardName;
        savedCard.IsKnownCard = false;
        savedCard.UknownCardColorDB = ResultNotKnownCard.NotKnownCardColor;

        await InsertNewCard(savedCard);
    }

    private async Task InsertNewCard(SavedCard savedCard)
    {
        if (!await SavedCardViewModel.InsertNewCard(savedCard))
        {
            await DisplayAlert("Chyba ukladania karty", "Pri ukladaní karty nastala chyba: " + SQLConnection.StatusMessage, "Zavrieť");
        }
        else
        {
            // Make sure someone is listening to event
            if (On_AddCardView_CardAdded != null)
            {
                On_AddCardView_CardAdded(this, new EventArgs()); // toto bolo vela krat ked view bolo transient
            }

            await DisplayAlert("", "Karta uložená", "Zavrieť");
        }

        TempCardData.IsKnownCard = false;
        TempCardData.CardCode = string.Empty;
        TempCardData.CardName = string.Empty;
        TempCardData.Format = 0x00U;
        TempCardData.Image = null;

        btnAddCard.IsVisible = false;
        lblCode.Text = string.Empty;
        //this.CardImage.Source = null;

        await Shell.Current.GoToAsync("..");
    }

    private (string CardName, string PictureName) GetImageFromResource(string cardInfo)
    {
        string imageName = string.Empty;
        string cardName = string.Empty;

        if (cardInfo.StartsWith("6340095")) // Tesco
        {
            imageName = "testo_card.png";
            cardName = "Tesco";
        }
        else if (cardInfo.StartsWith("600211")) // Kaufland
        {
            imageName = "kaufland_card.png";
            cardName = "Kaufland";
        }
        else if (cardInfo.StartsWith("99958")) // Coop-Jednota
        {
            imageName = "coop_jednota_card.png";
            cardName = "COOP Jednota";
        }
        else if (cardInfo.StartsWith("994027")) // Billa
        {
            imageName = "billa_card.png";
            cardName = "Billa";
        }

        return (cardName, imageName);
    }
}