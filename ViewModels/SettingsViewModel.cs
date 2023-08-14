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
        readonly IDataStore<Player>? dataStore;
        ILogger<PlayersViewModel> logger;

        [ObservableProperty]
        private string? title;

        [ObservableProperty]
        private bool useRank;
        public SettingsViewModel(ILogger<PlayersViewModel> logger, IDataStore<Player> dataStore)
        {
            Title = "Settings";
            this.logger = logger;
            this.dataStore = dataStore;
        }

        [RelayCommand]
        private async Task SaveRank()
        {
            Preferences.Set(Constants.Settings.UseRank, UseRank);
        }

        [RelayCommand]
        private async Task DeleteAll()
        {
            bool result = await Application.Current.MainPage.DisplayAlert("Confirmation", "Are you sure you want to delete all player?", "Yes", "No");
            if (result)
                await dataStore.DeleteAllItemsAsync();
        }

        public async void OnAppearing()
        {
            UseRank = Preferences.Get(Constants.Settings.UseRank, false);
        }
    }
}
