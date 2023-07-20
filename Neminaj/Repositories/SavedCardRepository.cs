using Neminaj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neminaj.Repositoriesô
{
    public class SavedCardRepository
    {

        public SavedCardRepository()
        {
                
        }

        public async Task<List<SavedCard>> GetAllSavedCards()
        {
            try
            {
                await SQLConnection.InitAsync();
                return await SQLConnection.m_ConnectionAsync.Table<SavedCard>().ToListAsync();
            }
            catch (Exception ex)
            {
                SQLConnection.StatusMessage = $"Failed to retrieve data from table SavedCard. {ex.Message}";
            }

            return new List<SavedCard>();
        }

        public async Task<bool> InsertNewCard(List<SavedCard> listSavedCards)
        {
            try
            {
                await SQLConnection.InitAsync();
                return await SQLConnection.m_ConnectionAsync.InsertAllAsync(listSavedCards) > 0;
            }
            catch (Exception ex)
            {
                SQLConnection.StatusMessage = $"Failed to insert data into table: SavedCard. {ex.Message}";
            }

            return false;
        }

        public async Task<bool> DeleteSavedCard(List<SavedCard> listSavedCards)
        {
            try
            {
                await SQLConnection.InitAsync();

                int cntOfDeleted = 0;

                foreach (SavedCard item in listSavedCards)
                    cntOfDeleted += await SQLConnection.m_ConnectionAsync.DeleteAsync(item);

                return cntOfDeleted > 0;
            }
            catch (Exception ex)
            {
                SQLConnection.StatusMessage = $"Failed to insert data into table: SavedCartItems. {ex.Message}";
            }

            return false;
        }

    }
}
