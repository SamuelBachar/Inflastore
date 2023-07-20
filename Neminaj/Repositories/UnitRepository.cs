using Neminaj.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Neminaj.Repositories;

public class UnitRepository
{
    public UnitRepository()
    {

    }

    public async Task<List<Unit>> GetAllUnitsAsync()
    {
        try
        {
            await SQLConnection.InitAsync();
            return await SQLConnection.m_ConnectionAsync.Table<Unit>().ToListAsync();
        }
        catch (Exception ex)
        {
            SQLConnection.StatusMessage = $"Failed to retrieve data. {ex.Message}";
        }

        return new List<Unit>();
    }

    public async Task<List<Unit>> GetSpecificUnits(List<int> listUnitIds)
    {
        try
        {
            await SQLConnection.InitAsync();
            return await SQLConnection.m_ConnectionAsync.Table<Unit>().Where(unit => listUnitIds.Contains(unit.Id)).ToListAsync();
        }
        catch (Exception ex)
        {
            SQLConnection.StatusMessage = $"Failed to retrieve data. {ex.Message}";
        }

        return new List<Unit>();
    }
}
