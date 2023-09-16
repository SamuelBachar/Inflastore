using CommunityToolkit.Maui.Views;
using Neminaj.ContentViews;
using Neminaj.Interfaces;
using SharedTypesLibrary.DTOs.API;
using SharedTypesLibrary.Models.API;

namespace Neminaj.Views;

public partial class RegisterView : ContentPage
{
    readonly IRegisterService _registerService = null;

    List<SharedTypesLibrary.Models.API.DatabaseModels.Region> ListRegions { get; set; } = null;

    UserRegisterRequest UserRegisterRequest { get; set; } = null;

    public RegisterView(IRegisterService registerService)
    {
        InitializeComponent();

        _registerService = registerService;

        ListRegions = new List<SharedTypesLibrary.Models.API.DatabaseModels.Region>();
        ListRegions.Add(new SharedTypesLibrary.Models.API.DatabaseModels.Region { Id = 1, Name = "Bratislavský"});
        ListRegions.Add(new SharedTypesLibrary.Models.API.DatabaseModels.Region { Id = 2, Name = "Trnavský" });
        ListRegions.Add(new SharedTypesLibrary.Models.API.DatabaseModels.Region { Id = 3, Name = "Trenčianský" });
        ListRegions.Add(new SharedTypesLibrary.Models.API.DatabaseModels.Region { Id = 4, Name = "Nitrianský" });
        ListRegions.Add(new SharedTypesLibrary.Models.API.DatabaseModels.Region { Id = 5, Name = "Žilinský" });
        ListRegions.Add(new SharedTypesLibrary.Models.API.DatabaseModels.Region { Id = 6, Name = "Banskobystrický" });
        ListRegions.Add(new SharedTypesLibrary.Models.API.DatabaseModels.Region { Id = 7, Name = "Prešovský" });
        ListRegions.Add(new SharedTypesLibrary.Models.API.DatabaseModels.Region { Id = 8, Name = "Košický" });

        RegionPicker.ItemsSource = ListRegions;
        RegionPicker.ItemDisplayBinding = new Binding("Name");

        UserRegisterRequest = new UserRegisterRequest();
    }

    private async void BtnRegisterHttps_Clicked(object sender, EventArgs e)
    {
        bool badEnty = false;

        if (string.IsNullOrWhiteSpace(EntryEmail.Text))
        {
            EntryEmail.Placeholder = "Zadajte e-mail";
            EntryEmail.PlaceholderColor = Colors.OrangeRed;

            badEnty = true;
        }

        if (string.IsNullOrWhiteSpace(EntryPassword.Text))
        {
            EntryPassword.IsPassword = false;
            EntryPassword.Placeholder = "Zadajte heslo";
            EntryPassword.PlaceholderColor = Colors.OrangeRed;

            badEnty = true;
        }

        if (string.IsNullOrWhiteSpace(EntryPasswordConfirm.Text))
        {
            EntryPasswordConfirm.IsPassword = false;
            EntryPasswordConfirm.Placeholder = "Zadajte potvrdzujúce heslo";
            EntryPasswordConfirm.PlaceholderColor = Colors.OrangeRed;

            badEnty = true;
        }

        if (RegionPicker.SelectedIndex == -1)
        {
            RegionPicker.Title = "Zvoľte kraj kvôli presným cenám";
            RegionPicker.TitleColor = Colors.OrangeRed;
            badEnty = true;
        }

        if (badEnty)
            return;

        UserRegisterRequest.Email = EntryEmail.Text;
        UserRegisterRequest.Password = EntryPassword.Text;
        UserRegisterRequest.ConfirmPassword = EntryPasswordConfirm.Text;
        UserRegisterRequest.Region_Id = ((SharedTypesLibrary.Models.API.DatabaseModels.Region)RegionPicker.ItemsSource[RegionPicker.SelectedIndex]).Id;

        ActivityIndicatorPopUp popUpIndic = new ActivityIndicatorPopUp("Registrujem ...");
        this.ShowPopupAsync(popUpIndic);
        popUpIndic.TurnOnActivityIndicator();

        (UserRegisterDTO UserRegisterDTO, string Message) response = await _registerService.RegisterHTTPS(UserRegisterRequest);

        popUpIndic.TurnOffActivityIndicator();

        if (response.UserRegisterDTO != null)
        {
            await DisplayAlert("Úspešná registrácia", response.Message, "Zavrieť");
        }
        else
        {
            await DisplayAlert("Registrácia chyba", response.Message, "Zavrieť");
        }
    }


    private void EntryPasswordConfirm_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!EntryPasswordConfirm.IsPassword && EntryPasswordConfirm.PlaceholderColor == Colors.OrangeRed)
        {
            EntryPasswordConfirm.IsPassword = true;
            EntryPasswordConfirm.TextColor = Colors.Black;
        }
    }

    private void EntryPassword_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (!EntryPassword.IsPassword && EntryPassword.PlaceholderColor == Colors.OrangeRed)
        {
            EntryPassword.IsPassword = true;
            EntryPassword.TextColor = Colors.Black;
        }
    }

    private void EntryEmail_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (EntryEmail.PlaceholderColor == Colors.OrangeRed)
        {
            EntryEmail.TextColor = Colors.Black;
        }
    }

    private void RegionPicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        Picker picker = (Picker)sender;

        int selectedIndex = picker.SelectedIndex;

        if (selectedIndex != -1)
        {
            UserRegisterRequest.Region_Id = ((SharedTypesLibrary.Models.API.DatabaseModels.Region)picker.ItemsSource[selectedIndex]).Id;
        }
    }
}