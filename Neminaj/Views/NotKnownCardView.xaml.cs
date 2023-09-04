using CommunityToolkit.Maui.Views;
using Neminaj.ViewsModels;

namespace Neminaj.Views;

public class ColorValue
{
    public int Id { get; set; }
    public Color Color { get; set; }

    public string Name { get; set; }
}


// Todo take drawned image and store it as byte arrya into saved card  mct:DrawinView ??
// https://youtu.be/OB65n17bR98?t=164
// https://youtu.be/OB65n17bR98
public partial class NotKnownCardView : ContentPage
{
    NotKnownCardViewModel NotKnownCardViewModel { get; set; } = null;

    bool FontForCardNameSet { get; set; } = false;

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
            NotKnownCardViewModel.ResultNotKnownCard.Color = BorderPalleteColor.BackgroundColor;

            // Make sure someone is listening to event
            if (On_NotKnownCardView_BtnAddCard_Clicked != null)
            {
                On_NotKnownCardView_BtnAddCard_Clicked(this, new EventArgs());
            }

            this.lblCardName.Text = string.Empty;
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

        if (!FontForCardNameSet)
        {
            lblCardName.FontSize = BorderPalleteColor.Width / 12;

            FontForCardNameSet = true;
        }

        lblCardName.Text = CardName.Text;
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