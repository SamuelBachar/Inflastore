using Neminaj.Views;
using System.Diagnostics.Metrics;

namespace Neminaj.ContentViews;

public partial class NavigationBarMainPage : ContentView
{
	int CountValue = 0;

    //ImageButton BtnCart = null;

	public NavigationBarMainPage()
	{
        InitializeComponent();

        //BtnCart = new ImageButton
        //{
        //    ZIndex = 0,
        //    HorizontalOptions = LayoutOptions.End,
        //    VerticalOptions = LayoutOptions.Center,
        //    Aspect = Aspect.AspectFit,
        //    HeightRequest = 20,
        //    WidthRequest = 20,
        //    MaximumHeightRequest = 20,
        //    MaximumWidthRequest = 20,
        //    Source = ImageSource.FromFile("cart.svg")
        //};

        //Grid.SetRow(BtnCart, 0);
        //Grid.SetColumn(BtnCart, 1);

        //this.GridBar.Add(BtnCart);
    }

    public void IncreaseShoppingCartCounter()
    {
        //Grid.SetColumn(this.BtnCart, 1);

        CountValue++;

        if (CountValue >= 100)
        {
            this.CartCounter.FontSize = 11; 
            this.CartCounter.Text = "99+";
            this.CartCounter.Margin = new Thickness(0, 5, 14, 20);
        }
        else
        {
            if (CountValue >= 10)
            {
                this.CartCounter.Margin = new Thickness(0, 5, 16, 20);
            }

            this.CartCounter.FontSize = 13;
            this.CartCounter.Text = CountValue.ToString();
        }
    }

    public void DecreaseShoppingCartCounter()
    {
        CountValue--;

        if (CountValue - 1 < 0) // should not happen
			CountValue = 0;

        if (CountValue - 1 >= 0 || CountValue - 1 <= 99)
		{
            if (CountValue < 9)
            {
                this.CartCounter.Margin = new Thickness(0, 5, 19, 20);
            }

            if (CountValue >= 10)
            {
                this.CartCounter.Margin = new Thickness(0, 5, 16, 20);
            }

            this.CartCounter.Text = CountValue.ToString();
            this.CartCounter.FontSize = 13;
        }
    }

    public void SetShoppingCartCounter(int countValue)
    {
        CountValue = countValue;

        if (CountValue >= 100)
        {
            this.CartCounter.FontSize = 11;
            this.CartCounter.Text = "99+";
            this.CartCounter.Margin = new Thickness(0, 5, 14, 20);
        }
        else
        {
            if (CountValue < 9)
            {
                this.CartCounter.Margin = new Thickness(0, 5, 19, 20);
            }

            if (CountValue >= 10)
            {
                this.CartCounter.Margin = new Thickness(0, 5, 16, 20);
            }

            this.CartCounter.FontSize = 13;
            this.CartCounter.Text = CountValue.ToString();
        }
    }


    public ImageButton GetBtnCart()
	{
		return CartCicle;
	}

    public Grid GetBarGrid()
    {
        return GridBar;
    }
}