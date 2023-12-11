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
        readonly IGameStore gameStore;

        [ObservableProperty]
        private string? title;

        [ObservableProperty]
        private bool useRank;

        [ObservableProperty]
        private bool useScore;

        [ObservableProperty]
        private bool isLight;

        [ObservableProperty]
        private bool isDefault;

        [ObservableProperty]
        private bool isDark;
        public SettingsViewModel(ILogger<SettingsViewModel> logger, IPlayerStore playerStore, ITeamStore teamStore, IGameStore gameStore)
        {
            Title = Constants.Title.Settings;
            this.logger = logger;
            this.playerStore = playerStore;
            this.teamStore = teamStore;
            this.gameStore = gameStore;
        }

        public void OnAppearing()
        {
            UseRank = Settings.UseRank;
            UseScore = Settings.UseScore;
            switch (Settings.Theme)
            {
                case 0:
                    IsDefault = true;
                    IsLight = false;
                    IsDark = false;
                    break;
                case 1:
                    IsDefault = false;
                    IsLight = true;
                    IsDark = false;
                    break;
                case 2:
                    IsDefault = false;
                    IsLight = false;
                    IsDark = true;
                    break;
            }
        }

        public void SetTheme(RadioButton r)
        {
            switch (r.Value)
            {
                case "System":
                    if (r.IsChecked) Settings.Theme = 0;
                    IsDefault = r.IsChecked;
                    break;
                case "Light":
                    if (r.IsChecked) Settings.Theme = 1;
                    IsLight = r.IsChecked;
                    break;
                case "Dark":
                    if (r.IsChecked) Settings.Theme = 2;
                    IsDark = r.IsChecked;
                    break;
            }
            TheTheme.SetTheme();
        }

        [RelayCommand]
        private async Task SaveUseRank()
        {
            Settings.UseRank = UseRank;
        }

        [RelayCommand]
        private async Task SaveUseScore()
        {
            Settings.UseScore = UseScore;
            if (UseScore)
            {
                await playerStore.SetPlayerRanksByRatio();
            }
        }

        [RelayCommand]
        private async Task DeleteAll()
        {
            bool result = await Application.Current.MainPage.DisplayAlert("Confirmation", "Are you sure you want to delete all records?", "Yes", "No");
            if (result)
            {
                await gameStore.DeleteAllGamesAsync();
                await teamStore.DeleteAllTeamsAsync();
                await playerStore.DeleteAllPlayersAsync();
            }
        }

        [RelayCommand]
        private async Task ClearHistory()
        {
            bool result = await Application.Current.MainPage.DisplayAlert("Confirmation", "Are you sure you want to delete the game history?", "Yes", "No");
            if (result)
            {
                await gameStore.DeleteAllGamesAsync();
            }
        }

        [RelayCommand]
        private async Task ZeroScores()
        {
            bool result = await Application.Current.MainPage.DisplayAlert("Confirmation", "Are you sure you want to reset all scores?", "Yes", "No");
            if (result)
            {
                List<Player> players = await playerStore.GetPlayersAsync();
                foreach (Player player in players)
                {
                    player.NumWins = 0;
                    player.NumLosses = 0;
                }
                await playerStore.UpdatePlayersAsync(players);
                List<TeamDB> teams = await teamStore.GetTeamsAsync();
                foreach (TeamDB team in teams)
                {
                    team.NumWins = 0;
                    team.NumLosses = 0;
                }
                await teamStore.UpdateTeamsAsync(teams);
            }
        }
    }
}
