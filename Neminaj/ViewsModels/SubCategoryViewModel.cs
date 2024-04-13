using CommunityToolkit.Mvvm.ComponentModel;
using Neminaj.Repositories;
using SharedTypesLibrary.DTOs.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedTypesLibrary.Models.API.DatabaseModels;

namespace Neminaj.ViewsModels;

[QueryProperty(nameof(ItemRepo), nameof(ItemRepo))]
[QueryProperty(nameof(UnitRepo), nameof(UnitRepo))]
[QueryProperty(nameof(CartRepo), nameof(CartRepo))]
[QueryProperty(nameof(CategoryRepo), nameof(CategoryRepo))]
[QueryProperty(nameof(CompanyRepo), nameof(CompanyRepo))]
[QueryProperty(nameof(ItemPriceRepo), nameof(ItemPriceRepo))]
[QueryProperty(nameof(ChoosenCategory), nameof(ChoosenCategory))]

public partial class SubCategoryViewModel : ObservableObject
{
    [ObservableProperty]
    public ItemRepository itemRepo;

    [ObservableProperty]
    public UnitRepository unitRepo;

    [ObservableProperty]
    public SavedCartRepository cartRepo;

    [ObservableProperty]
    public CategoryRepository categoryRepo;

    [ObservableProperty]
    public CompanyRepository companyRepo;

    [ObservableProperty]
    public ItemPriceRepository itemPriceRepo;

    [ObservableProperty]
    public CategoryDTO choosenCategory;

    public SubCategoryViewModel()
    {

    }
}
