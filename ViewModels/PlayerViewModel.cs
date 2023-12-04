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
        readonly IPlayerStore playerStore;
        ILogger<PlayersViewModel> logger;

        public ObservableCollection<Player> Players { get; set; }

        [ObservableProperty]
        private string? title;

        [ObservableProperty]
        private string sortText;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool isAllHere;

        [ObservableProperty]
        private string hereText;

        [ObservableProperty]
        private bool isRefreshing;

        [ObservableProperty]
        private bool useRank;

        [ObservableProperty]
        private int hereCount;

        [ObservableProperty]
        private bool didNotFinishLoading;

        private bool DoNotRunHere;
        private bool DoNotRunAllHere;
        public PlayersViewModel(IPlayerStore playerStore, ILogger<PlayersViewModel> logger)
        {
            this.playerStore = playerStore;
            this.logger = logger;
            Title = "Players";
            Players = new ObservableCollection<Player>();
            IsBusy = false;
            DidNotFinishLoading = true;
            IsAllHere = true;
            DoNotRunHere = false;
            HereText = Constants.Settings.AllHere;
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
                SortText = Constants.Settings.SortByWins;
                SortByWins();
                Preferences.Set(Constants.Settings.SortBy, Constants.Settings.SortByWins);
            }
            else if (SortText == Constants.Settings.SortByWins)
            {
                SortText = Constants.Settings.SortByRatio;
                SortByRatio();
                Preferences.Set(Constants.Settings.SortBy, Constants.Settings.SortByRatio);
            }
            else if (SortText == Constants.Settings.SortByRatio)
            {
                SortText = Constants.Settings.SortByLoss;
                SortByLosses();
                Preferences.Set(Constants.Settings.SortBy, Constants.Settings.SortByLoss);
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
        private void SortByRank() => Players = new ObservableCollection<Player>(Players.ToList().OrderByDescending(p => p.NumStarsDisplay));
        private void SortByWins() => Players = new ObservableCollection<Player>(Players.ToList().OrderByDescending(p => p.NumWins));
        private void SortByRatio() => Players = new ObservableCollection<Player>(Players.ToList().OrderByDescending(p =>
        {
            if (p.NumLosses == 0) return p.NumWins;
            else if (p.NumWins == 0) return -1 * p.NumLosses;
            else return p.NumWins / p.NumLosses;
        }));
        private void SortByLosses() => Players = new ObservableCollection<Player>(Players.ToList().OrderByDescending(p => p.NumLosses));

        [RelayCommand]
        private async Task Here(object sender)
        {
            if (DoNotRunHere) return;
            Player? item = sender as Player;
            if (item == null)
            {
                logger.LogWarning("item is null.");
                return;
            }
            await playerStore.UpdatePlayerAsync(item);
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
                var items = await playerStore.GetPlayersAsync();
                foreach (var item in items)
                {
                    Players.Add(item);
                    logger.LogDebug($"{item.Name}, {item.IsHere}");
                }
                UpdateHereCount();
                if (SortText == Constants.Settings.SortByName) { SortByName(); }
                else if (SortText == Constants.Settings.SortByRank) { SortByRank(); }
                else if (SortText == Constants.Settings.SortByWins) { SortByWins(); }
                else if (SortText == Constants.Settings.SortByRatio) { SortByRatio(); }
                else { SortByLosses(); }
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

        [RelayCommand]
        private void HereAll()
        {
            Task.Run(() => DoHereAll());
        }
        private async Task DoHereAll()
        {
            if (DoNotRunAllHere) return;
            if (IsBusy) return;
            IsBusy = true;
            DoNotRunHere = true;
            List<Player> list = Players.ToList();
            if (IsAllHere) HereText = Constants.Settings.AllHere;
            else HereText = Constants.Settings.NoneHere;
            try
            {
                foreach (Player p in list)
                {
                    p.IsHere = IsAllHere;
                }
                await playerStore.UpdatePlayersAsync(list);
                UpdateHereCount();
            }
            catch (Exception ex)
            {
                logger.LogError("{ex}", ex);
            }
            finally
            {
                IsBusy = false;
                DoNotRunHere = false;
            }

        }

        async public void OnAppearing()
        {
            UseRank = Preferences.Get(Constants.Settings.UseRank, true);
            DidNotFinishLoading = true;
            LoadPlayers();
        }

        void UpdateHereCount()
        {
            DoNotRunAllHere = true;
            HereCount = Players.Count(p => p.IsHere);
            if (HereCount == 0) { IsAllHere = false; HereText = Constants.Settings.NoneHere; }
            if (HereCount == Players.Count) { IsAllHere = true; HereText = Constants.Settings.AllHere; }
            DoNotRunAllHere = false;
        }
    }
}