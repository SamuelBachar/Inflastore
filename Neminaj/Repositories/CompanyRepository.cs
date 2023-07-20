using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Neminaj.Models;

namespace Neminaj.Repositories;

public class CompanyRepository
{
    public CompanyRepository()
    {

    }

    public async Task<List<Company>> GetAllCompaniesAsync()
    {
        try
        {
            await SQLConnection.InitAsync();
            return await SQLConnection.m_ConnectionAsync.Table<Company>().ToListAsync();
        }
        catch (Exception ex)
        {
            SQLConnection.StatusMessage = $"Failed to retrieve data. {ex.Message}";
        }

        return new List<Company>();
    }

    public List<Company> GetAllCompanies()
    {
        try
        {
            SQLConnection.Init();
            return SQLConnection.m_ConnectionSync.Table<Company>().ToList();
        }
        catch (Exception ex)
        {
            SQLConnection.StatusMessage = $"Failed to retrieve data. {ex.Message}";
        }

        return new List<Company>();
    }

    public async Task<List<Company>> GetSpecificCompaniesAsync(List<int> listCompaniesIds)
    {
        try
        {
            await SQLConnection.InitAsync();
            return await SQLConnection.m_ConnectionAsync.Table<Company>().Where(comp => listCompaniesIds.Contains(comp.Id)).ToListAsync();
        }
        catch (Exception ex)
        {
            SQLConnection.StatusMessage = $"Failed to retrieve data. {ex.Message}";
        }

        return new List<Company>();
    }
}
