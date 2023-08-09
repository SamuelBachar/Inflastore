using Camera.MAUI;
using Neminaj.Models;
using Neminaj.Repositories;
using Neminaj.ViewsModels;

namespace Neminaj.Views;


public class TempCardData
{
    public ZXing.BarcodeFormat Format { get; set; }
    public string CardInfo { get; set; }

    public byte[] Image { get; set; }
}

public partial class AddCardView : ContentPage
{
    TempCardData TempCardData { get; set; } = new TempCardData();
    SavedCardDetailViewModel SavedCardViewModel { get; set; } = null;

    public AddCardView(SavedCardDetailViewModel savedCardViewModel)
    {
        InitializeComponent();

        BindingContext = savedCardViewModel;
        SavedCardViewModel = savedCardViewModel;

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

        cameraView.BarCodeDetectionFrameRate = 5;
        cameraView.BarCodeDetectionMaxThreads = 5;
        cameraView.ControlBarcodeResultDuplicate = true;
        cameraView.BarCodeDetectionEnabled = true;

        if (cameraView.MaxZoomFactor >= 2.5f)
            cameraView.ZoomFactor = 2.5f;
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

    private void BtnScan_Clicked(object sender, EventArgs e)
    {
        //myImage.Source = cameraView.GetSnapShot(Camera.MAUI.ImageFormat.PNG);
    }

    private void cameraView_BarcodeDetected(object sender, Camera.MAUI.ZXingHelper.BarcodeEventArgs args)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            //barcodeResult.Text = $"{args.Result[0].BarcodeFormat}: {args.Result[0].Text}";
            string text = $"{args.Result[0].BarcodeFormat}: {args.Result[0].Text}";
            lblCode.Text = text;

            TempCardData.Format = args.Result[0].BarcodeFormat;
            TempCardData.CardInfo = args.Result[0].Text;


            // todo handle situation when card is scaned but not known
            // for example show bar-code or qr code and ask for colour and name of card
            string fileNameResources = GetImageFromResource(TempCardData.CardInfo);

            if (fileNameResources == string.Empty)
            {
                // TODO create pop up with entry and color pick
            }

            if (fileNameResources != string.Empty)
            {
                this.CardImage.Source = ImageSource.FromFile(fileNameResources);
                TempCardData.Image = File.ReadAllBytes(fileNameResources);
                this.btnAddCard.IsVisible = true;
            }

        });
    }

    private async void btnAddCard_Clicked(object sender, EventArgs e)
    {
        SavedCard savedCard = new SavedCard
        {
            CardFormat = (int)TempCardData.Format,
            CardInfo = TempCardData.CardInfo,
            Image = TempCardData.Image
        };

        List<SavedCard> savedCards = new List<SavedCard>() { savedCard };

        if (! await SavedCardViewModel.SavedCardRepository.InsertNewCard(savedCards))
        {
            await DisplayAlert("Chyba ukladania karty", "Pri ukladaní karty nastala chyba: " + SQLConnection.StatusMessage, "Zavrieť");
        }
        else
        {
            await DisplayAlert("", "Karta uložená", "Zavrieť");
        }
    }

    private string GetImageFromResource(string cardInfo)
    {
        string imageName = string.Empty;

        if (cardInfo.StartsWith("6340095")) // Tesco
            imageName = "testo_card.png";
        else if (cardInfo.StartsWith("600211")) // Kaufland
            imageName = "kaufland_card.png";

        return imageName;
    }
}