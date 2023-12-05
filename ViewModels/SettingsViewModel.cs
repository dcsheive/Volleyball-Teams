using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Volleyball_Teams.Models;
using Volleyball_Teams.Services;
using Volleyball_Teams.Util;

namespace Volleyball_Teams.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        ILogger<SettingsViewModel> logger;
        readonly IPlayerStore playerStore;
        readonly ITeamStore teamStore;

        [ObservableProperty]
        private string? title;

        [ObservableProperty]
        private bool useRank;

        [ObservableProperty]
        private bool useScore;
        public SettingsViewModel(ILogger<SettingsViewModel> logger, IPlayerStore playerStore, ITeamStore teamStore)
        {
            Title = Constants.Title.Settings;
            this.logger = logger;
            this.playerStore = playerStore;
            this.teamStore = teamStore;
        }

        public void OnAppearing()
        {
            UseRank = Preferences.Get(Constants.Settings.UseRank, true);
            UseScore = Preferences.Get(Constants.Settings.UseScore, false);
        }

        [RelayCommand]
        private async Task SaveUseRank()
        {
            Preferences.Set(Constants.Settings.UseRank, UseRank);
        }

        [RelayCommand]
        private async Task SaveUseScore()
        {
            Preferences.Set(Constants.Settings.UseScore, UseScore);
            if (UseScore)
            {
                await playerStore.SetPlayerRanksByRatio();
            }
        }

        [RelayCommand]
        private async Task DeleteAll()
        {
            bool result = await Application.Current.MainPage.DisplayAlert("Confirmation", "Are you sure you want to delete all players?", "Yes", "No");
            if (result)
            {
                await teamStore.DeleteAllTeamsAsync();
                await playerStore.DeleteAllPlayersAsync();
            }

        }

        [RelayCommand]
        private async Task DeleteAllTeams()
        {
            bool result = await Application.Current.MainPage.DisplayAlert("Confirmation", "Are you sure you want to delete all saved teams?", "Yes", "No");
            if (result)
            {
                await teamStore.DeleteAllTeamsAsync();
            }

        }

        [RelayCommand]
        private async Task ZeroWins()
        {
            bool result = await Application.Current.MainPage.DisplayAlert("Confirmation", "Are you sure you want to reset all scores?", "Yes", "No");
            if (result)
            {
                var items = await playerStore.GetPlayersAsync();
                foreach(Player item in items)
                {
                    item.NumWins = 0;
                    item.NumLosses = 0;
                }
                await playerStore.UpdatePlayersAsync(items);
            }
        }
    }
}
