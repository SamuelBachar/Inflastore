using CommunityToolkit.Mvvm.ComponentModel;
using Neminaj.Repositoriesô;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neminaj.ViewsModels;

[QueryProperty(nameof(SavedCardRepository), nameof(SavedCardRepository))]
public partial class SavedCardViewModel : ObservableObject
{
    [ObservableProperty]
    public SavedCardRepository savedCardRepository;

    public SavedCardViewModel()
    {
            
    }
}
