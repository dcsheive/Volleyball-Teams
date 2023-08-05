using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using Volleyball_Teams.Models;
using Volleyball_Teams.Services;
using Volleyball_Teams.Util;
using Volleyball_Teams.Views;

namespace Volleyball_Teams.ViewModels
{
    public partial class ItemsViewModel : ObservableObject
    {
        readonly IDataStore<Player> dataStore;
        ILogger<ItemsViewModel> logger;

        public ObservableCollection<Player> Players { get; set; }
        [ObservableProperty]
        private Player? selectedItem = default;

        [ObservableProperty]
        private string? title;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private int hereCount;
        public ItemsViewModel(IDataStore<Player> dataStore, ILogger<ItemsViewModel> logger)
        {
            this.dataStore = dataStore;
            this.logger = logger;
            Title = "Players";
            Players = new ObservableCollection<Player>();
            IsBusy = false;
        }



        [RelayCommand]
        private async void OnAddItem(object obj)
        {
            await Shell.Current.GoToAsync(nameof(NewItemPage));
        }

        [RelayCommand]
        private async void ItemSelectionChanged(object sender)
        {
            Player? item = sender as Player;
            if (item == null)
            {
                logger.LogWarning("item is null.");
                return;
            }
            await dataStore.UpdateItemAsync(item);
            logger.LogDebug($"item is {item.Name}, {item.IsHere}");
            UpdateHereCount();
        }


        [RelayCommand]
        private async void DeletePlayer(object sender)
        {
            Player? item = sender as Player;
            if (item == null)
            {
                logger.LogWarning("item is null.");
                return;
            }
            Players.Remove(item);
            UpdateHereCount();
            await dataStore.DeleteItemAsync(item);
        }


        [RelayCommand]
        private async Task LoadPlayers()
        {
            logger.LogDebug($"IsBusy={IsBusy}");
            try
            {
                Players.Clear();
                HereCount = 0;
                var items = await dataStore.GetItemsAsync(true);
                foreach (var item in items)
                {
                    Players.Add(item);
                    logger.LogDebug($"{item.Name}, {item.IsHere}");
                }
                UpdateHereCount();
            }
            catch (Exception ex)
            {
                logger.LogError("{ex}", ex);
            }
            finally
            {
                IsBusy = false;
                logger.LogDebug("Set IsBusy to false");
            }
        }

        async public void OnAppearing()
        {
            SelectedItem = null;
            await LoadPlayers();

        }

        void UpdateHereCount()
        {
            HereCount = Players.Count(p => p.IsHere);
        }
    }
}