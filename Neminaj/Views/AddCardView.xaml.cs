﻿using Camera.MAUI;
using CommunityToolkit.Maui.Views;
using Neminaj.ContentViews;
using Neminaj.Models;
using Neminaj.Repositories;
using Neminaj.ViewsModels;
using System.Dynamic;
using ZXing;

namespace Neminaj.Views;


public class TempCardData
{
    public ZXing.BarcodeFormat Format { get; set; }
    public string CardInfo { get; set; }

    public byte[] Image { get; set; }

    public bool IsKnownCard { get; set; } = false;
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
        cameraView.FlashMode = FlashMode.Auto;

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

    private void cameraView_BarcodeDetected(object sender, Camera.MAUI.ZXingHelper.BarcodeEventArgs args)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            TempCardData.IsKnownCard = true;

            string text = $"{args.Result[0].BarcodeFormat}: {args.Result[0].Text}";
            lblCode.Text = $"Skenovanie úspešné:\r\n{text}";

            TempCardData.Format = args.Result[0].BarcodeFormat;
            TempCardData.CardInfo = args.Result[0].Text;


            // todo handle situation when card is scaned but not known
            // for example show bar-code or qr code and ask for colour and name of cardcvvf
            string fileNameResources = GetImageFromResource(TempCardData.CardInfo);

            if (fileNameResources == string.Empty)
            {
                TempCardData.IsKnownCard = false;
            }

            if (fileNameResources != string.Empty)
            {
                this.CardImage.Source = ImageSource.FromFile(fileNameResources);
                TempCardData.Image = File.ReadAllBytes(fileNameResources);
            }

            this.btnAddCard.IsVisible = true;
        });
    }

    private async void btnAddCard_Clicked(object sender, EventArgs e)
    {
        SavedCard savedCard = new SavedCard();
        //savedCard.CardFormat = (int)TempCardData.Format;
        //savedCard.CardInfo = TempCardData.CardInfo;

        if (!TempCardData.IsKnownCard)
        {
            ResultNotKnownCard resultNotKnownCard = new ResultNotKnownCard();

            await Shell.Current.GoToAsync(nameof(NotKnownCardView),
            new Dictionary<string, object>
            {
                [nameof(ResultNotKnownCard)] = resultNotKnownCard,
            });

            savedCard.IsKnownCard = false;
            savedCard.UnknownCardName = resultNotKnownCard.CardName;
            savedCard.UknownCardColor = resultNotKnownCard.Color.ToInt();
        }
        else
        {
            savedCard.Image = TempCardData.Image;
        }

        List<SavedCard> savedCards = new List<SavedCard>() { savedCard };

        if (!await SavedCardViewModel.SavedCardRepository.InsertNewCard(savedCards))
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
        else if (cardInfo.StartsWith("99958")) // Coop-Jednota
            imageName = "coop_jednota_card.png";
        else if (cardInfo.StartsWith("994027")) // Billa
            imageName = "billa_card.png";

        return imageName;
    }
}