using CommunityToolkit.Maui.Views;
using Neminaj.ViewsModels;

namespace Neminaj.Views;

public class ColorValue
{
    public int Id { get; set; }
    public Color Color { get; set; }

    public string Name { get; set; }
}

public partial class NotKnownCardView : ContentPage
{
    List<ColorValue> ListColorValues { get; set; } = new List<ColorValue>();

    NotKnownCardViewModel NotKnownCardViewModel { get; set; } = null;

    public NotKnownCardView(NotKnownCardViewModel notKnownCardViewModel)
    {
        InitializeComponent();

        NotKnownCardViewModel = notKnownCardViewModel;

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

    }

    //private void ColorPicker_PickedColorChanged(object sender, ColorPicker.Maui.PickedColorChangedEventArgs e)
    //{
    //    NotKnownCardViewModel.ResultNotKnownCard.Color = e.NewPickedColorValue;
    //}

    private void btnSaveCard_Clicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(CardName.Text))
        {
            CardName.Placeholder = "Zadajte názov karty";
            CardName.PlaceholderColor = Colors.OrangeRed;
        }
        else
        {
            NotKnownCardViewModel.ResultNotKnownCard.CardName = CardName.Text;
            NotKnownCardViewModel.ResultNotKnownCard.Color = BorderPalleteColor.BackgroundColor;
        }
    }

    private void CardNamed_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (CardName.PlaceholderColor == Colors.OrangeRed)
        {
            CardName.TextColor = Colors.Black;
        }

        lblCardName.Text = CardName.Text;
    }

    private void ColorPicker_SelectedIndexChanged(object sender, EventArgs e)
    {

    }

    private void SliderR_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        int value = (int)e.NewValue;
        txtR.Text = value.ToString();

        BorderPalleteColor.BackgroundColor = Color.FromRgb((int)SliderR.Value, (int)SliderG.Value, (int)SliderB.Value);
        //BorderPalleteColor.BackgroundColor = Color.FromRgb(int.Parse(txtR.Text), int.Parse(txtG.Text), int.Parse(txtB.Text));
        //BorderPalleteColor.Color = Color.FromRgb(int.Parse(txtR.Text), int.Parse(txtG.Text), int.Parse(txtB.Text));
    }

    private void SliderG_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        int value = (int)e.NewValue;
        txtG.Text = value.ToString();

        //BorderPalleteColor.BackgroundColor = Color.FromRgb(int.Parse(txtR.Text), int.Parse(txtG.Text), int.Parse(txtB.Text));
        BorderPalleteColor.BackgroundColor = Color.FromRgb((int)SliderR.Value, (int)SliderG.Value, (int)SliderB.Value);
        //BorderPalleteColor.Color = Color.FromRgb(int.Parse(txtR.Text), int.Parse(txtG.Text), int.Parse(txtB.Text));
    }

    private void SliderB_ValueChanged(object sender, ValueChangedEventArgs e)
    {
        int value = (int)e.NewValue;
        txtB.Text = value.ToString();

        //BorderPalleteColor.BackgroundColor = Color.FromRgb(int.Parse(txtR.Text), int.Parse(txtG.Text), int.Parse(txtB.Text));
        BorderPalleteColor.BackgroundColor = Color.FromRgb((int)SliderR.Value, (int)SliderG.Value, (int)SliderB.Value);
        //BorderPalleteColor.Color = Color.FromRgb(int.Parse(txtR.Text), int.Parse(txtG.Text), int.Parse(txtB.Text));
    }
}