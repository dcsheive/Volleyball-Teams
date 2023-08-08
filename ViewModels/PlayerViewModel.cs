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
    public partial class PlayersViewModel : ObservableObject
    {
        readonly IDataStore<Player> dataStore;
        ILogger<PlayersViewModel> logger;

        public ObservableCollection<Player> Players { get; set; }

        [ObservableProperty]
        private string? title;

        [ObservableProperty]
        private string sortText;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool isRefreshing;

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
            SortText = Preferences.Get(Constants.Settings.SortBy, Constants.Settings.SortByName);
            if (string.IsNullOrEmpty(SortText))
            {
                Preferences.Set(Constants.Settings.SortBy, Constants.Settings.SortByName);
                SortText = Constants.Settings.SortByName;
            }
        }

        [RelayCommand]
        private async Task OnAddItem(object obj)
        {
            await Shell.Current.GoToAsync($"{nameof(NewPlayerPage)}?ID=");
        }

        [RelayCommand]
        private void Sort()
        {
            Task.Run(() => DoSort());
        }
        private void DoSort()
        {
            if (IsBusy) return;
            IsBusy = true;
            if (SortText == Constants.Settings.SortByName)
            {
                SortText = Constants.Settings.SortByRank;
                SortByRank();
                Preferences.Set(Constants.Settings.SortBy, Constants.Settings.SortByRank);
            }
            else if (SortText == Constants.Settings.SortByRank)
            {
                SortText = Constants.Settings.SortByDate;
                SortByDate();
                Preferences.Set(Constants.Settings.SortBy, Constants.Settings.SortByDate);
            }
            else
            {
                SortText = Constants.Settings.SortByName;
                SortByName();
                Preferences.Set(Constants.Settings.SortBy, Constants.Settings.SortByName);
            }
            IsBusy = false;
        }

        private void SortByName() => Players = new ObservableCollection<Player>(Players.ToList().OrderBy(p => p.Name));
        private void SortByRank() => Players = new ObservableCollection<Player>(Players.ToList().OrderByDescending(p => p.NumStars));
        private void SortByDate() => Players = new ObservableCollection<Player>(Players.ToList().OrderBy(p => p.Id));

        [RelayCommand]
        private async Task ItemSelectionChanged(object sender)
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
        private async Task EditItem(object obj)
        {
            Player p = (Player)obj;
            await Shell.Current.GoToAsync($"{nameof(NewPlayerPage)}?ID={p.Id}");
        }

        [RelayCommand]
        private void LoadPlayers()
        {
            Task.Run(() => DoLoadPlayers());
        }
        private async Task DoLoadPlayers()
        {
            if (IsBusy) return;
            IsBusy = true;
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
                if (SortText == Constants.Settings.SortByName) SortByName();
                else if (SortText == Constants.Settings.SortByDate) SortByDate();
                else SortByRank();
            }
            catch (Exception ex)
            {
                logger.LogError("{ex}", ex);
            }
            finally
            {
                DidNotFinishLoading = false;
                IsBusy = false;
                IsRefreshing = false;
                logger.LogDebug("Set IsBusy to false");
            }
        }


        async public void OnAppearing()
        {
            DidNotFinishLoading = true;
            LoadPlayers();
        }

        void UpdateHereCount()
        {
            HereCount = Players.Count(p => p.IsHere);
        }
    }
}