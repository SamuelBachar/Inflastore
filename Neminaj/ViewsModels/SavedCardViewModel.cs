using CommunityToolkit.Mvvm.ComponentModel;
using Neminaj.Models;
using Neminaj.Repositories;
using Neminaj.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neminaj.ViewsModels;

[QueryProperty(nameof(SavedCardRepository), nameof(SavedCardRepository))] // potrebne pri SavedCardDetailView, AddCardView

[QueryProperty(nameof(CardID), nameof(CardID))] // potrebne pri SavedCardDetailView

[QueryProperty(nameof(CardData), nameof(CardData))]

public partial class SavedCardDetailViewModel : ObservableObject
{
    [ObservableProperty]
    public SavedCardRepository savedCardRepository;

    [ObservableProperty]
    public CardData cardData;

    [ObservableProperty]
    public int cardID;

    public SavedCardDetailViewModel()
    {
            
    }

    public async Task<bool> DeleteSavedCard(int cardId)
    {
        return await SavedCardRepository.DeleteSavedCard(cardId);
    }

    public async Task<SavedCard> GetSpecificCard(int cardId)
    {
        return await SavedCardRepository.GetSpecificCard(cardId);
    }

    public async Task<bool> InsertNewCard(List<SavedCard> listSavedCards)
    {
        return await SavedCardRepository.InsertNewCard(listSavedCards);
    }

    public async Task<bool> InsertNewCard(SavedCard savedCard)
    {
        return await SavedCardRepository.InsertNewCard(savedCard);
    }
}
