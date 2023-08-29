﻿namespace InflaStoreWebAPI.Services.UnitsService;
public class NavigationShopData : IUnitsService
{
    private readonly DataContext _context;

    public NavigationShopData(DataContext context)
    {
        _context = context;
    }

    public async Task<List<Unit>> GetAllUnitsAsync()
    {
        return await _context.Units.ToListAsync();
    }

    public async Task<List<Unit>> GetSpecificUnitsAsync(List<int> listIds)
    {
        return await _context.Units.Where(unit => listIds.Contains(unit.Id)).ToListAsync();
    }
}