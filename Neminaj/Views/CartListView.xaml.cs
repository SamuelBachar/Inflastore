﻿using Neminaj.ContentViews;
using Neminaj.Models;
using Neminaj.Repositories;
using SharedTypesLibrary.Models.API.DatabaseModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Neminaj.Views;

public partial class CartListView : ContentPage
{
    SavedCartRepository SavedCartRepo;

    ItemRepository ItemRepo;

    UnitRepository UnitRepo;

    List<SavedCart> ListSavedCarts;

    public List<ItemChoosen> ListCartItemChoosen { get; set; }

    PopUpActivityIndicator _popUpIndic = new PopUpActivityIndicator("Načítavam nákupné zoznamy ...");

    ItemPicker ItemPicker;

#if WINDOWS
    int _lastCheckedCardId = -1;
#endif

    public CartListView(SavedCartRepository savedCartRepository, ItemRepository itemRepository, UnitRepository unitRepository, ItemPicker mainPage)
    {
        InitializeComponent();
        this.BindingContext = this;

        SavedCartRepo = savedCartRepository;
        ItemRepo = itemRepository;
        ItemPicker = mainPage;
        UnitRepo = unitRepository;
        this.Appearing += (s, e) => { this.Content = _popUpIndic; };
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        ListSavedCarts = await SavedCartRepo.GetAllSavedCartsAsync();

        if (ListSavedCarts.Count == 0)
        {
            this.Content = new Label
            {
                Text = "Nemáte vytvorení žiaden nákupny zoznam\r\nZoznam si môžte vytvoriť vo výbere položiek pomocou nákupného košíka",
                VerticalOptions = LayoutOptions.Center,
                HorizontalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold,
                LineBreakMode = LineBreakMode.WordWrap,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center
            };
        }
        else
        {
#if WINDOWS
            this.listViewSavedCarts.ItemsSource = ListSavedCarts;
#else
            this.listViewSavedCarts.ItemsSource = ListSavedCarts.OrderBy(n => n.Name);
#endif
            this.Content = MainScrollView;
        }
    }

    private async void Button_Nahlad_Clicked(object sender, EventArgs e)
    {
        int cartId = int.Parse(((Button)(sender)).ClassId);
        ListCartItemChoosen = await GetItemsFromSavedCart(cartId);

        await Shell.Current.GoToAsync(nameof(SavedCartDetailView),
           new Dictionary<string, object>
           {
               ["ListCartItemChoosen"] = ListCartItemChoosen,
           });
    }

    private async void Delete_Clicked(object sender, EventArgs e)
    {
        bool answer = await DisplayAlert("Mazanie zoznamu", "Prajete si naozaj zmazať nákupný zoznam?", "Áno", "Nie");

        if (answer)
        {
            int cartId = int.Parse(((ImageButton)(sender)).ClassId);
            SavedCart savedCardToDelete = ListSavedCarts.Where(cart => cart.Id == cartId).First();
            List<SavedCartItem> listSavedCardItemsToDelete = await SavedCartRepo.GetAllSavedCartItemsAsync(cartId);

            if (!await SavedCartRepo.DeleteSavedCart(listSavedCardItemsToDelete, savedCardToDelete))
            {
                await DisplayAlert("Chyba pri mazaní", "Pri mazaní nastala chyba: " + SQLConnection.StatusMessage, "Zavrieť");
            }
            else
            {
                ListSavedCarts.Remove(savedCardToDelete);

#if WINDOWS
                this.listViewSavedCarts.ItemsSource = ListSavedCarts;
#else
                this.listViewSavedCarts.ItemsSource = ListSavedCarts.OrderBy(n => n.Name);
#endif
            }
        }
    }

    private async void PredvolitCheckBox_CheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        try
        {
            int cartId = int.Parse(((CheckBox)(sender)).ClassId);
#if WINDOWS
            if (cartId != _lastCheckedCardId)
            {
                _lastCheckedCardId = cartId;
#endif
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            { 

                bool IsCartPicked = false;

                foreach (SavedCart savedCart in ListSavedCarts)
                {
                    if (savedCart.Id == cartId)
                    {
                        savedCart.IsChecked = e.Value;
                        IsCartPicked = e.Value;
                    }
                    else
                    {
                        savedCart.IsChecked = false;
                    }
                }
#if WINDOWS
                this.listViewSavedCarts.ItemsSource = ListSavedCarts.ToList();
#else
                this.listViewSavedCarts.ItemsSource = ListSavedCarts.OrderBy(n => n.Name);
#endif

                if (IsCartPicked)
                {
                    // Get items from saved cart
                    List<SavedCartItem> savedCartItems = await SavedCartRepo.GetAllSavedCartItemsAsync(cartId);

                    // Get Name and Unit_Id
                    List<Item> listItemsFromCard = await ItemRepo.GetSpecificItemsAsync(savedCartItems.Select(savedCart => savedCart.Item_Id).ToList());

                    // Get unit Tag
                    List<Unit> listUnits = await UnitRepo.GetSpecificUnits(listItemsFromCard.Select(item => item.Unit_Id).ToList());

                    List<ItemChoosen> listItemsFromSavedCart = new List<ItemChoosen>();

                    int idInListHelp = 0;

                    foreach (SavedCartItem cartItem in savedCartItems)
                    {
                        Item item = listItemsFromCard.Where(item => item.Id == cartItem.Item_Id).First();
                        Unit unit = listUnits.Where(unit => item.Unit_Id == unit.Id).First();

                        string name = item.Name;
                        string finalName = $"{cartItem.CntOfItem}x {item.Name} {unit.Tag}";
                        string unitTag = unit.Tag;
                        int cntOfItem = cartItem.CntOfItem;
                        int idInList = idInListHelp;

                        listItemsFromSavedCart.Add(new ItemChoosen
                        {
                            Id = item.Id,
                            IdInList = idInListHelp,
                            Name = name,
                            FinalName = finalName,
                            CntOfItems = cartItem.CntOfItem,
                            UnitTag = unitTag
                        });

                        idInListHelp++;
                    }

                    ItemPicker.SetChoosenItems(listItemsFromSavedCart);
                    this.Content = MainScrollView;
                }
            }
            else
            {
                await this.DisplayAlert("Chyba", "Zariadenie nemá pripojenie k internetu\r\nNie je možné predvoliť nákupný zoznam", "Zavrieť");
            }
            #if WINDOWS
            }
            #endif
        }
        catch (Exception ex)
        {
            await this.DisplayAlert("Chyba", $"Chyba načítania zoznamu\r\nNačítanie zoznamu zlyhalo s chybou: {ex.Message}", "Zavrieť");
        }
    }

    private async Task<List<ItemChoosen>> GetItemsFromSavedCart(int cartId)
    {
        List<ItemChoosen> listItemsFromSavedCart = new List<ItemChoosen>();

        await Task.Run(async () =>
        {
            // Get items from saved cart
            List<SavedCartItem> savedCartItems = await SavedCartRepo.GetAllSavedCartItemsAsync(cartId);

            // Get Name and Unit_Id
            List<Item> listItemsFromCard = await ItemRepo.GetSpecificItemsAsync(savedCartItems.Select(savedCart => savedCart.Item_Id).ToList());

            // Get unit Tag
            List<Unit> listUnits = await UnitRepo.GetSpecificUnits(listItemsFromCard.Select(item => item.Unit_Id).ToList());

            int idInListHelp = 0;

            foreach (SavedCartItem cartItem in savedCartItems)
            {
                Item item = listItemsFromCard.Where(item => item.Id == cartItem.Item_Id).First();
                Unit unit = listUnits.Where(unit => item.Unit_Id == unit.Id).First();

                string name = item.Name;
                string finalName = $"{cartItem.CntOfItem}x {item.Name} {unit.Tag}";
                string unitTag = unit.Tag;
                int cntOfItem = cartItem.CntOfItem;
                int idInList = idInListHelp;

                listItemsFromSavedCart.Add(new ItemChoosen
                {
                    Id = item.Id,
                    IdInList = idInListHelp,
                    Name = name,
                    FinalName = finalName,
                    CntOfItems = cartItem.CntOfItem,
                    UnitTag = unitTag
                });

                idInListHelp++;
            }

        });

        return listItemsFromSavedCart;
    }
}