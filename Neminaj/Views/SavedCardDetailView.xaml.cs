using Camera.MAUI;
using Neminaj.Models;
using Neminaj.ViewsModels;
using ZXing;

namespace Neminaj.Views;

// todo do it based on SavedCartDetail.xaml and SavedCartDetail.xaml.cs and calling is in v CartListView.xaml.cs
// another example is in CardsView -> AddCardView and SavedCardViewModel 
// and do it based on SavedCard
public partial class SavedCardDetailView : ContentPage
{
    SavedCardDetailViewModel SavedCardDetailViewModel { get; set; } = null;
    SavedCard SavedCard { get; set; } = null;

    public SavedCardDetailView(SavedCardDetailViewModel savedCardDetailViewModel)
    {
        InitializeComponent();
        BindingContext = savedCardDetailViewModel;
        SavedCardDetailViewModel = savedCardDetailViewModel;
        this.Appearing += async (s, e) => { await BuildPage(); };
        this.Disappearing += async (s, e) => { await CleanWhenDisappearing(); };
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        // here SavedCardDetailViewModel and its properties are initialized
    }

    private async Task CleanWhenDisappearing()
    {
        await Shell.Current.GoToAsync("..");
    }

    private async Task BuildPage()
    {
        SavedCard = await SavedCardDetailViewModel.GetSpecificCard(SavedCardDetailViewModel.CardID);

        //        < VerticalStackLayout HorizontalOptions = "Center" VerticalOptions = "Center" >
        //    < cv:BarcodeImage x:Name = "BarcodeImage" WidthRequest = "400" HeightRequest = "400"
        //                     Aspect = "AspectFit" BarcodeForeground = "Black" BarcodeBackground = "White" BarcodeMargin = "5" />
        //    < Label x: Name = "lblCardCode" HorizontalOptions = "Center" VerticalOptions = "Center" />
        //</ VerticalStackLayout >

        VerticalStackLayout vertStack = new();

        BarcodeImage barCodeImage = new BarcodeImage();
        barCodeImage.WidthRequest = 400;
        barCodeImage.HeightRequest = 400;
        barCodeImage.Aspect = Aspect.AspectFit;
        barCodeImage.BarcodeBackground = Colors.White;
        barCodeImage.BarcodeForeground = Colors.Black;
        barCodeImage.BarcodeMargin = 5;

        barCodeImage.BarcodeFormat = (BarcodeFormat)SavedCard.CardFormat;
        barCodeImage.Barcode = SavedCard.CardCode;

        Label cardCodeTxt = new Label();
        cardCodeTxt.Text = SavedCard.CardCode;

        vertStack.Add(barCodeImage);
        vertStack.Add(cardCodeTxt);

        this.Content = vertStack;
    }

    private async void ToolbarItemDelete_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Mazanie karty", "Prajete si naozaj zmazať klubovú kartu?", "Áno", "Nie");

        if (answer)
        {
            if (!await SavedCardDetailViewModel.DeleteSavedCard(SavedCardDetailViewModel.CardID))
            {
                await DisplayAlert("Chyba pri mazaní", "Pri mazaní nastala chyba: " + SQLConnection.StatusMessage, "Zavrieť");
            }
            else
            {
                await DisplayAlert("", "Klubová karta zmazaná", "Zavrieť");
            }
        }
    }
}