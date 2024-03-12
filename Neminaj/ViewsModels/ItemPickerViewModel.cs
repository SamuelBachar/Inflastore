using CommunityToolkit.Mvvm.ComponentModel;
using Neminaj.Repositories;
using SharedTypesLibrary.DTOs.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neminaj.ViewsModels;

[QueryProperty(nameof(Category), nameof(Category))]
[QueryProperty(nameof(ItemRepo), nameof(ItemRepo))]
[QueryProperty(nameof(UnitRepo), nameof(UnitRepo))]
[QueryProperty(nameof(CartRepo), nameof(CartRepo))]
[QueryProperty(nameof(CompanyRepo), nameof(CompanyRepo))]
[QueryProperty(nameof(ItemPriceRepo), nameof(ItemPriceRepo))]

public partial class ItemPickerViewModel : ObservableObject
{
    [ObservableProperty]
    public CategoryDTO category;

    [ObservableProperty]
    public ItemRepository itemRepo;

    [ObservableProperty]
    public UnitRepository unitRepo;

    [ObservableProperty]
    public SavedCartRepository cartRepo;

    [ObservableProperty]
    public CompanyRepository companyRepo;

    [ObservableProperty]
    public ItemPriceRepository itemPriceRepo;

    public ItemPickerViewModel()
    {
        
    }
}
