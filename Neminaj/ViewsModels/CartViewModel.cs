using CommunityToolkit.Mvvm.ComponentModel;
using Neminaj.ContentViews;
using Neminaj.Models;
using Neminaj.Repositories;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neminaj.ViewsModels;

[QueryProperty(nameof(ObservableItemsChoosed), nameof(ObservableItemsChoosed))]
[QueryProperty(nameof(SavedCartRepository), nameof(SavedCartRepository))]
public partial class CartViewModel : ObservableObject
{
    [ObservableProperty]
    private ObservableCollection<ItemChoosen> observableItemsChoosed;

    [ObservableProperty]
    private SavedCartRepository savedCartRepository;

    public CartViewModel()
    {
    }

    public void DeleteChoosenItem(int idInList)
    {
        if (ObservableItemsChoosed.Where(item => item.IdInList == idInList).Count() > 0)
        {
            ObservableItemsChoosed.Remove(ObservableItemsChoosed.Where(item => item.IdInList == idInList).First());
        }
    }

    public void ChangeFinalNameOfItem(ItemChoosen item)
    {
        item.FinalName = $"{item.CntOfItems}x {item.Name}";
        //ObservableItemsChoosed[idInList].FinalName = $"{ObservableItemsChoosed[idInList].CntOfItems}x {ObservableItemsChoosed[idInList].Name}";
    }

    public ObservableCollection<ItemChoosen> GetItemChoosens()
    {
        return ObservableItemsChoosed;
    }

    public async Task<bool> InsertNewCart(SavedCart savedCart)
    {
        return await SavedCartRepository.InsertNewCart(savedCart);
    }

    public async Task<List<SavedCart>> GetAllSavedCartsAsync()
    {
        return await SavedCartRepository.GetAllSavedCartsAsync();
    }

    public async Task<bool> InsertNewCartItems(List<SavedCartItem> listSavedCartItems)
    {
        return await SavedCartRepository.InsertNewCartItems(listSavedCartItems);
    }
}
