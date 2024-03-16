using Microsoft.Maui.Platform;
using Neminaj.Models;
using SharedTypesLibrary.Models.API.DatabaseModels;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Timers;
using Timer = System.Timers.Timer;

namespace Neminaj.Repositories
{
    public class ParentRepository<T>
    {
        DateTime _dateLastUpdate = DateTime.Now;
        public List<T> FilteredItems = new List<T>();

        public ParentRepository()
        {
        }

        public bool GetUpdatedNeeded() 
        { 
            bool updatedNeeded = false;
            DateTime dateTime = DateTime.Now;

            if (dateTime >= _dateLastUpdate.AddHours(6))
            {
                _dateLastUpdate = DateTime.Now;
                updatedNeeded = true;
            }

            return updatedNeeded; 
        }

        public async Task<string> RemoveDiacritics(string text)
        {
            string result = string.Empty;

            await Task.Run(() =>
            {
                string formD = text.Normalize(NormalizationForm.FormD);
                StringBuilder sb = new StringBuilder();

                foreach (char ch in formD)
                {
                    UnicodeCategory uc = CharUnicodeInfo.GetUnicodeCategory(ch);
                    if (uc != UnicodeCategory.NonSpacingMark)
                    {
                        sb.Append(ch);
                    }
                }

                result = sb.ToString().Normalize(NormalizationForm.FormC);
            });

            return result;
        }

        public void ClearFilteredList()
        {
            FilteredItems.Clear();
        }
    }
}
