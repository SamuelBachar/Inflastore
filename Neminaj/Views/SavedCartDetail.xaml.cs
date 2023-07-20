using Neminaj.ViewsModels;

namespace Neminaj.Views;

public partial class SavedCartDetail : ContentPage
{
    private SavedCartDetailViewModel ViewModel { get; set; } = null;
    public SavedCartDetail(SavedCartDetailViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
        ViewModel = viewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        listItemChoosen.ItemsSource = ViewModel.ListCartItemChoosen;
    }
}