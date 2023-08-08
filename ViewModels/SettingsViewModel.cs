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
        ILogger<PlayersViewModel> logger;

        [ObservableProperty]
        private string? title;

        [ObservableProperty]
        private bool useRank;
        public SettingsViewModel(ILogger<PlayersViewModel> logger)
        {
            Title = "Settings";
            this.logger = logger;
        }

        [RelayCommand]
        private async void SaveRank()
        {
            Preferences.Set(Constants.Settings.UseRank, UseRank);
        }

        public async void OnAppearing()
        {
            UseRank = Preferences.Get(Constants.Settings.UseRank, false);
        }
    }
}
