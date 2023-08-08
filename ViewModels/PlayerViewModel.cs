﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using Volleyball_Teams.Models;
using Volleyball_Teams.Services;
using Volleyball_Teams.Util;
using Volleyball_Teams.Views;

namespace Volleyball_Teams.ViewModels
{
    public partial class PlayersViewModel : ObservableObject
    {
        readonly IDataStore<Player> dataStore;
        ILogger<PlayersViewModel> logger;

        public ObservableCollection<Player> Players { get; set; }

        [ObservableProperty]
        private string? title;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private int hereCount;

        [ObservableProperty]
        private bool didNotFinishLoading;
        public PlayersViewModel(IDataStore<Player> dataStore, ILogger<PlayersViewModel> logger)
        {
            this.dataStore = dataStore;
            this.logger = logger;
            Title = "Players";
            Players = new ObservableCollection<Player>();
            IsBusy = false;
            DidNotFinishLoading = true;
        }



        [RelayCommand]
        private async void OnAddItem(object obj)
        {
            await Shell.Current.GoToAsync($"{nameof(NewPlayerPage)}?ID=");
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
        private async void EditItem(object obj)
        {
            Player p = (Player)obj;
            await Shell.Current.GoToAsync($"{nameof(NewPlayerPage)}?ID={p.Id}");
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
                DidNotFinishLoading = false;
                IsBusy = false;
                logger.LogDebug("Set IsBusy to false");
            }
        }

        async public void OnAppearing()
        {
            DidNotFinishLoading = true;
            await LoadPlayers();
        }

        void UpdateHereCount()
        {
            HereCount = Players.Count(p => p.IsHere);
        }
    }
}