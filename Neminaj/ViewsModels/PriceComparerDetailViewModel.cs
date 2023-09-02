using CommunityToolkit.Mvvm.ComponentModel;
using Neminaj.Models;
using SharedTypesLibrary.DTOs.API;
using SharedTypesLibrary.Models.API.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neminaj.ViewsModels;

public class CheapestItemPerCompany
{
    public CompanyDTO Company { get; set; }
    public ItemChoosen ItemChoosen { get; set; }
    public bool Discount { get; set; }
}

[QueryProperty(nameof(ListCheapestItemsPerCompanies), nameof(ListCheapestItemsPerCompanies))]

public partial class PriceComparerDetailViewModel : ObservableObject
{
    [ObservableProperty]
    public List<CheapestItemPerCompany> listCheapestItemsPerCompanies;

    public PriceComparerDetailViewModel()
    {
        
    }
}
