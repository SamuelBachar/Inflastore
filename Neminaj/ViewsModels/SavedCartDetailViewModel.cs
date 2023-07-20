using CommunityToolkit.Mvvm.ComponentModel;
using Neminaj.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neminaj.ViewsModels;

[QueryProperty("ListCartItemChoosen", "ListCartItemChoosen")]
public partial class SavedCartDetailViewModel : ObservableObject
{
    [ObservableProperty]
    public List<ItemChoosen> listCartItemChoosen;

    public SavedCartDetailViewModel()
    {
            
    }
}