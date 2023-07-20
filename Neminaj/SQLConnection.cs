using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


using Neminaj.Models;

namespace Neminaj;

public class SQLConnection
{
    public static string m_DBPath { get; } = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "neminaj.db3");
    public static SQLiteAsyncConnection m_ConnectionAsync = null;
    public static SQLiteConnection m_ConnectionSync = null;

    public static string StatusMessage { get; set; }

    public SQLConnection()
    {
        InitAsync();
    }

    public static async Task InitAsync()
    {
        if (m_ConnectionAsync is not null)
            return;

        m_ConnectionAsync = new SQLiteAsyncConnection(SQLConnection.m_DBPath, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite);

        await m_ConnectionAsync.CreateTableAsync<Item>();
        await m_ConnectionAsync.CreateTableAsync<ItemPrice>();
        await m_ConnectionAsync.CreateTableAsync<Company>();
        await m_ConnectionAsync.CreateTableAsync<Unit>();
        await m_ConnectionAsync.CreateTableAsync<SavedCart>();
        await m_ConnectionAsync.CreateTableAsync<SavedCartItem>();
        await m_ConnectionAsync.CreateTableAsync<NavigationShopData>();
    }

    public static void Init()
    {
        if (m_ConnectionSync is not null)
            return;

        m_ConnectionSync = new SQLiteConnection(SQLConnection.m_DBPath, SQLiteOpenFlags.Create | SQLiteOpenFlags.ReadWrite);
        m_ConnectionSync.CreateTable<Item>();
        m_ConnectionSync.CreateTable<ItemPrice>();
        m_ConnectionSync.CreateTable<Company>();
        m_ConnectionSync.CreateTable<Unit>();
        m_ConnectionSync.CreateTable<SavedCart>();
        m_ConnectionSync.CreateTable<SavedCartItem>();
    }
}
