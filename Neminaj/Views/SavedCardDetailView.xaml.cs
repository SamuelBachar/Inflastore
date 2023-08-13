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

    public SavedCardDetailView(SavedCardDetailViewModel savedCardDetailViewModel)
    {
        InitializeComponent();
        BindingContext = savedCardDetailViewModel;
        SavedCardDetailViewModel = savedCardDetailViewModel;
        this.Loaded += async (s, e) => { await BuildPage(); };
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        // here SavedCardDetailViewModel and its properties are initialized
    }

    private async Task BuildPage()
    {
        SavedCard savedCard = await SavedCardDetailViewModel.GetSpecificCard(SavedCardDetailViewModel.CardID);
        this.CardCode.BarcodeFormat = (BarcodeFormat)savedCard.CardFormat;
        this.CardCode.Barcode = savedCard.CardCode;

        // todo otestovat ci potrebne ???
        this.CardCode.Margin = 5;

        this.lblCardCode.Text = savedCard.CardCode;
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