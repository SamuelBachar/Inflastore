using CommunityToolkit.Maui.Views;

namespace Neminaj.ContentViews;

public partial class ActivityIndicatorPopUp : Popup
{
	string _text = string.Empty;
	public ActivityIndicatorPopUp(string text)
	{
		InitializeComponent();
		_text = text;

        this.Text.Text = _text;
        this.Indicator.IsRunning = true;
    }

    public void TurnOnActivityIndicator()
    {
		this.Text.Text = _text;
		this.Indicator.IsRunning = true;
    }

    public void TurnOffActivityIndicator()
	{
        this.Indicator.IsRunning = false;
		//this.Close();
    }
}