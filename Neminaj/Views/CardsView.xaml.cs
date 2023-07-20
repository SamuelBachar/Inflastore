using Neminaj.Repositoriesô;

namespace Neminaj.Views;

public partial class CardsView : ContentPage
{
    SavedCardRepository SavedCardRepo { get; set; } = null;

    public CardsView(SavedCardRepository savedCardRepository)
    {
        SavedCardRepo = savedCardRepository;

        InitializeComponent();
    }

    private async void BtnAddCardOrPicture_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(AddCardView),
        new Dictionary<string, object>
        {
            [nameof(SavedCardRepository)] = this.SavedCardRepo,
        });
    }

    private void listCards_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {

    }
}