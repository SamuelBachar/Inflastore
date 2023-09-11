using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Core.Extensions;
using CommunityToolkit.Maui.Core.Views;
using CommunityToolkit.Maui.Views;
using Microsoft.Maui.Graphics;
using Neminaj.ViewsModels;
using System.IO;

namespace Neminaj.Views;

public class ColorValue
{
    public int Id { get; set; }
    public Color Color { get; set; }

    public string Name { get; set; }
}

public partial class NotKnownCardView : ContentPage
{
    NotKnownCardViewModel NotKnownCardViewModel { get; set; } = null;

    // Events
    public delegate void NotKnownCardView_BtnAddCard_Clicked(object sender, EventArgs e);
    public static event NotKnownCardView_BtnAddCard_Clicked On_NotKnownCardView_BtnAddCard_Clicked;

    public NotKnownCardView(NotKnownCardViewModel notKnownCardViewModel)
    {
        InitializeComponent();
        NotKnownCardViewModel = notKnownCardViewModel;
        this.BindingContext = NotKnownCardViewModel;
    }

    private async void btnSaveCard_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(CardName.Text))
        {
            CardName.Placeholder = "Zadajte názov karty";
            CardName.PlaceholderColor = Colors.OrangeRed;
        }
        else
        {
            NotKnownCardViewModel.ResultNotKnownCard.CardName = CardName.Text;
            NotKnownCardViewModel.ResultNotKnownCard.NotKnownCardColor = BorderPalleteColor.BackgroundColor.ToInt();

            // Make sure someone is listening to event
            if (On_NotKnownCardView_BtnAddCard_Clicked != null)
            {
                On_NotKnownCardView_BtnAddCard_Clicked(this, new EventArgs());
            }

            this.BorderPalleteColor.BackgroundColor = Colors.OrangeRed;
            this.SliderR.Value = 255;
            this.SliderG.Value = 165;
            this.SliderB.Value = 0;
            this.CardName.Text = string.Empty;
            await Shell.Current.GoToAsync("..");
        }
    }

    private void CardNamed_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (CardName.PlaceholderColor == Colors.OrangeRed)
        {
            CardName.TextColor = Colors.Black;
        }
    }

    private void SliderR_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        int value = (int)e.NewValue;
        txtR.Text = value.ToString();

        BorderPalleteColor.BackgroundColor = Color.FromRgb((int)SliderR.Value, (int)SliderG.Value, (int)SliderB.Value);
    }

    private void SliderG_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        int value = (int)e.NewValue;
        txtG.Text = value.ToString();

        BorderPalleteColor.BackgroundColor = Color.FromRgb((int)SliderR.Value, (int)SliderG.Value, (int)SliderB.Value);
    }

    private void SliderB_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        int value = (int)e.NewValue;
        txtB.Text = value.ToString();

        BorderPalleteColor.BackgroundColor = Color.FromRgb((int)SliderR.Value, (int)SliderG.Value, (int)SliderB.Value);
    }
}