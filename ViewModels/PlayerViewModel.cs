using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Sentry;
using System.Collections.ObjectModel;
using Volleyball_Teams.Models;
using Volleyball_Teams.Services;
using Volleyball_Teams.Util;
using Volleyball_Teams.Views;
using Constants = Volleyball_Teams.Util.Constants;

namespace Volleyball_Teams.ViewModels
{
    public partial class PlayersViewModel : ObservableObject
    {
        ILogger<PlayersViewModel> logger;
        readonly IPlayerStore playerStore;

        [ObservableProperty]
        private ObservableCollection<Player> players;

        [ObservableProperty]
        private string? title;

        [ObservableProperty]
        private string sortText;

        [ObservableProperty]
        private bool isAllHere;

        [ObservableProperty]
        private string hereText;

        [ObservableProperty]
        private bool useRank;

        [ObservableProperty]
        private int hereCount;

        [ObservableProperty]
        private bool didNotFinishLoading;

        private bool IsBusy;
        private bool DoNotRunHere;
        private bool DoNotRunAllHere;
        public PlayersViewModel(IPlayerStore playerStore, ILogger<PlayersViewModel> logger)
        {
            this.playerStore = playerStore;
            this.logger = logger;
            Title = Constants.Title.Players;
            IsBusy = false;
            IsAllHere = true;
            DoNotRunHere = false;
            DidNotFinishLoading = true;
            Players = new ObservableCollection<Player>();
            HereText = Constants.Settings.AllHere;
            SortText = Settings.SortBy;
            if (string.IsNullOrEmpty(SortText))
            {
                SortText = Constants.Settings.SortByName;
            }
        }

        public void OnAppearing()
        {
            SentrySdk.CaptureMessage("Players Appearing");
            UseRank = Settings.UseRank;
            DidNotFinishLoading = true;
            LoadPlayers();
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
            logger.LogDebug($"IsBusy={IsBusy}");
            try
            {
                if (SortText == Constants.Settings.SortByName)
                {
                    SortText = Constants.Settings.SortByRank;
                    logger.LogDebug($"Sort = {SortText}");
                    SortByRank();
                    Settings.SortBy = Constants.Settings.SortByRank;
                }
                else if (SortText == Constants.Settings.SortByRank)
                {
                    SortText = Constants.Settings.SortByWins;
                    logger.LogDebug($"Sort = {SortText}");
                    SortByWins();
                    Settings.SortBy = Constants.Settings.SortByWins;
                }
                else if (SortText == Constants.Settings.SortByWins)
                {
                    SortText = Constants.Settings.SortByRatio;
                    logger.LogDebug($"Sort = {SortText}");
                    SortByRatio();
                    Settings.SortBy = Constants.Settings.SortByRatio;
                }
                else if (SortText == Constants.Settings.SortByRatio)
                {
                    SortText = Constants.Settings.SortByLoss;
                    logger.LogDebug($"Sort = {SortText}");
                    SortByLosses();
                    Settings.SortBy = Constants.Settings.SortByLoss;
                }
                else
                {
                    SortText = Constants.Settings.SortByName;
                    logger.LogDebug($"Sort = {SortText}");
                    SortByName();
                    Settings.SortBy = Constants.Settings.SortByName;
                }
            }
            catch (Exception ex)
            {
                logger.LogError("{ex}", ex);
            }
            finally
            {
                IsBusy = false;
                logger.LogDebug($"IsBusy = {IsBusy}");
            }
        }

        [RelayCommand]
        private async Task Here(object sender)
        {
            if (DoNotRunHere) return;
            try
            {
                Player? player = sender as Player;
                if (player == null)
                {
                    logger.LogWarning("player is null.");
                    return;
                }
                await playerStore.UpdatePlayerAsync(player);
                logger.LogDebug($"player is {player.Name}, IsHere = {player.IsHere}");
                UpdateHereCount();
            }
            catch (Exception ex)
            {
                logger.LogError("{ex}", ex);
            }
        }

        [RelayCommand]
        private async Task EditItem(object obj)
        {
            Player p = (Player)obj;
            logger.LogDebug($"edit player is {p.Name}");
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
            logger.LogDebug($"IsBusy = {IsBusy}");
            logger.LogDebug($"Run DoLoadPlayers");
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
                logger.LogDebug($"Sort = {SortText}");
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
                logger.LogDebug($"IsBusy = {IsBusy}");
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
            logger.LogDebug($"Run DoHereAll()");
            logger.LogDebug($"IsBusy = {IsBusy}");
            logger.LogDebug($"DoNotRunHere = {DoNotRunHere}");
            try
            {
                List<Player> list = Players.ToList();
                if (IsAllHere) HereText = Constants.Settings.AllHere;
                else HereText = Constants.Settings.NoneHere;
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
                logger.LogDebug($"IsBusy = {IsBusy}");
                logger.LogDebug($"DoNotRunHere = {DoNotRunHere}");
            }
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