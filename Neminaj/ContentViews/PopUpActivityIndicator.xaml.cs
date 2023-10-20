namespace Neminaj.ContentViews;

public partial class PopUpActivityIndicator : ContentView
{
	public PopUpActivityIndicator(string text)
	{
		InitializeComponent();

		this.ActivityIndicator.IsRunning = true;
		this.lblPopUp.Text = text;
	}
}