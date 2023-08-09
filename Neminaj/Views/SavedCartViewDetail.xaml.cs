using Neminaj.ViewsModels;

namespace Neminaj.Views;

public partial class SavedCartDetailView : ContentPage
{
    private SavedCartDetailViewModel SavedCartDetailViewModel { get; set; } = null;
    public SavedCartDetailView(SavedCartDetailViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
        SavedCartDetailViewModel = viewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        listItemChoosen.ItemsSource = SavedCartDetailViewModel.ListCartItemChoosen;
    }
}