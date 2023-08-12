using CommunityToolkit.Maui.Views;
using Neminaj.Views;
using Neminaj.ViewsModels;

namespace Neminaj.ContentViews;

public class ColorValue
{
    public int Id { get; set; }
    public Color Color { get;set; }

    public string Name { get; set; }
}

public partial class NotKnownCardPopUp : Popup
{
    ResultNotKnownCard ResultData { get; set; } = new ResultNotKnownCard();

    List<ColorValue> ListColorValues { get; set; } = new List<ColorValue>();
    SavedCardDetailViewModel SavedCardDetailViewModel { get; set; } = null;

    public NotKnownCardPopUp(SavedCardDetailViewModel savedCardDetailViewModel, int windowWidth, int windowHeight, ResultNotKnownCard result)
    {
        InitializeComponent();

        SavedCardDetailViewModel = savedCardDetailViewModel;

        ListColorValues.Add(new ColorValue { Id = 0, Color = Colors.White, Name = "Biela" });
        ListColorValues.Add(new ColorValue { Id = 0, Color = Colors.IndianRed, Name = "Červená" });
        ListColorValues.Add(new ColorValue { Id = 0, Color = Colors.Black, Name = "Čierna" });
        ListColorValues.Add(new ColorValue { Id = 0, Color = Colors.Blue, Name = "Modrá" });
        ListColorValues.Add(new ColorValue { Id = 0, Color = Colors.BlueViolet, Name = "Oranžová" });
        ListColorValues.Add(new ColorValue { Id = 0, Color = Colors.Orange, Name = "Oranžová" });
        ListColorValues.Add(new ColorValue { Id = 0, Color = Colors.Orange, Name = "Oranžová" });
        ListColorValues.Add(new ColorValue { Id = 0, Color = Colors.Orange, Name = "Oranžová" });
        ListColorValues.Add(new ColorValue { Id = 0, Color = Colors.Orange, Name = "Oranžová" });
        ListColorValues.Add(new ColorValue { Id = 0, Color = Colors.Orange, Name = "Oranžová" });
        ListColorValues.Add(new ColorValue { Id = 0, Color = Colors.Orange, Name = "Oranžová" });
        ListColorValues.Add(new ColorValue { Id = 0, Color = Colors.Orange, Name = "Oranžová" });
        ListColorValues.Add(new ColorValue { Id = 0, Color = Colors.Orange, Name = "Oranžová" });
        ListColorValues.Add(new ColorValue { Id = 0, Color = Colors.Orange, Name = "Oranžová" });
        ListColorValues.Add(new ColorValue { Id = 0, Color = Colors.Orange, Name = "Oranžová" });
        ListColorValues.Add(new ColorValue { Id = 0, Color = Colors.Orange, Name = "Oranžová" });

        ResultData = result;

        MainBorder.HeightRequest = windowHeight * 0.75;
        MainBorder.WidthRequest = windowWidth * 0.75;
    }

    private void ColorPicker_PickedColorChanged(object sender, ColorPicker.Maui.PickedColorChangedEventArgs e)
    {
        ResultData.Color = e.NewPickedColorValue;
    }

    private void btnSaveCard_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(CardName.Text))
        {
            CardName.Placeholder = "Zadajte názov karty";
            CardName.PlaceholderColor = Colors.OrangeRed;
        }
        else
        {
            ResultData.CardName = CardName.Text;
            ResultData.Color = BorderPalleteColor.BackgroundColor;
            this.Close();
        }
    }

    private void CardNamed_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (CardName.PlaceholderColor == Colors.OrangeRed)
        {
            CardName.TextColor = Colors.Black;
        }
    }

    private void ColorPicker_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    private void SliderR_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        int value = (int)e.NewValue;
        txtR.Text = value.ToString();

        BorderPalleteColor.BackgroundColor = Color.FromRgb(int.Parse(txtR.Text), int.Parse(txtG.Text), int.Parse(txtB.Text));
        BorderPalleteColor.Color = Color.FromRgb(int.Parse(txtR.Text), int.Parse(txtG.Text), int.Parse(txtB.Text));
    }

    private void SliderG_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        int value = (int)e.NewValue;
        txtG.Text = value.ToString();

        BorderPalleteColor.BackgroundColor = Color.FromRgb(int.Parse(txtR.Text), int.Parse(txtG.Text), int.Parse(txtB.Text));
        BorderPalleteColor.Color = Color.FromRgb(int.Parse(txtR.Text), int.Parse(txtG.Text), int.Parse(txtB.Text));
    }

    private void SliderB_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        int value = (int)e.NewValue;
        txtB.Text = value.ToString();

        BorderPalleteColor.BackgroundColor = Color.FromRgb(int.Parse(txtR.Text), int.Parse(txtG.Text), int.Parse(txtB.Text));
        BorderPalleteColor.Color = Color.FromRgb(int.Parse(txtR.Text), int.Parse(txtG.Text), int.Parse(txtB.Text));
    }
}