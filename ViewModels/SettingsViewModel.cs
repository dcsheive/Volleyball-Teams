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
        readonly IPlayerStore<Player>? dataStore;
        ILogger<PlayersViewModel> logger;

        [ObservableProperty]
        private string? title;

        [ObservableProperty]
        private bool useRank;

        [ObservableProperty]
        private bool useScore;
        public SettingsViewModel(ILogger<PlayersViewModel> logger, IPlayerStore<Player> dataStore)
        {
            Title = "Settings";
            this.logger = logger;
            this.dataStore = dataStore;
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
                await dataStore.SetPlayerRanksByRatio();
            }
        }

        [RelayCommand]
        private async Task DeleteAll()
        {
            bool result = await Application.Current.MainPage.DisplayAlert("Confirmation", "Are you sure you want to delete all player?", "Yes", "No");
            if (result)
                await dataStore.DeleteAllPlayersAsync();
        }

        [RelayCommand]
        private async Task ZeroWins()
        {
            bool result = await Application.Current.MainPage.DisplayAlert("Confirmation", "Are you sure you want to reset all scores?", "Yes", "No");
            if (result)
            {
                var items = await dataStore.GetPlayersAsync();
                foreach(var item in items)
                {
                    item.NumWins = 0;
                    item.NumLosses = 0;
                }
                await dataStore.UpdatePlayersAsync(items);
            }
        }

        public async void OnAppearing()
        {
            UseRank = Preferences.Get(Constants.Settings.UseRank, false);
            UseScore = Preferences.Get(Constants.Settings.UseScore, false);
        }
    }
}
