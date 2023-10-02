using Neminaj.Models;
using Neminaj.Repositories;
using Neminaj.Views;
using Syncfusion.Maui.Core;
using System.Collections.ObjectModel;

namespace Neminaj.ContentViews;

public partial class CartCounterControlView : ContentView
{
    private static CartCounterControlView _instance = null;
    ObservableCollection<ItemChoosen> _observableItemsChoosed { get; set; } = new ObservableCollection<ItemChoosen>();

    SavedCartRepository _cartRepo = null;
    enum AnimationType
    {
        Out,
        In,
        Pulse
    }

    private static int CountValue { get; set; } = 0;
    public CartCounterControlView()
	{
		InitializeComponent();
	}

    public void Init(SavedCartRepository CartRepo, ObservableCollection<ItemChoosen> ObservableItemsChoosed)
    {
        _cartRepo = CartRepo;
        _observableItemsChoosed = ObservableItemsChoosed;
        SetShoppingCartCounter(_observableItemsChoosed.Count);
    }

    public async Task IncreaseShoppingCartCounter()
    {
        //Grid.SetColumn(this.BtnCart, 1);

        CountValue++;

        if (CountValue >= 100)
        {
            this.lblCartCount.Text = "99+";
        }
        else
        {
            this.lblCartCount.Text = CountValue.ToString();
        }

        await AnimateCartCounter(AnimationType.In);
    }

    public async Task DecreaseShoppingCartCounter()
    {
        CountValue--;

        if (CountValue - 1 < 0) // should not happen
            CountValue = 0;

        if (CountValue - 1 >= 0 || CountValue - 1 <= 99)
        {
            this.lblCartCount.Text = CountValue.ToString();
        }

        await AnimateCartCounter(AnimationType.Out);
    }

    public void SetShoppingCartCounter(int countValue)
    {
        CountValue = countValue;

        if (CountValue >= 100)
        {
            this.lblCartCount.Text = "99+";
        }
        else
        {
            this.lblCartCount.Text = CountValue.ToString();
        }
    }

    async Task Pulse()
    {
        await CartCounter.ScaleTo(1, 180);
        await CartCounter.ScaleTo(1.2, 180);
        await CartCounter.ScaleTo(1, 180);
        await CartCounter.ScaleTo(1.2, 180);
        await CartCounter.ScaleTo(1, 180);
    }

    private async Task AnimateCartCounter(AnimationType animationType)
    {
        switch (animationType)
        {
            case AnimationType.In:
                //if (Rotate)
                //{
                //    await CartCounter.RotateTo(360, length: 250);
                //    Rotate = false;
                //}
                //else
                //{
                //    await CartCounter.RotateTo(-360, length: 250);
                //    Rotate = true;
                //}

                await Pulse();
                break;
            case AnimationType.Out:
                //await CartCounter.ScaleTo(0);
                break;
            default:
                await Pulse();
                break;
        }
    }

    private async void CartCounter_TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        await Shell.Current.GoToAsync(nameof(CartView),
            new Dictionary<string, object>
            {
                ["ObservableItemsChoosed"] = _observableItemsChoosed,
                ["SavedCartRepository"] = _cartRepo
            });
    }
}