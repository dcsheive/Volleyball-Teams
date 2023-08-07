using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Volleyball_Teams.Models;
using Volleyball_Teams.Services;

namespace Volleyball_Teams.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        ILogger<ItemsViewModel> logger;

        [ObservableProperty]
        private string? title;

        [ObservableProperty]
        private bool useRank;
        public SettingsViewModel(ILogger<ItemsViewModel> logger)
        {
            Title = "Settings";
            this.logger = logger;
        }

        [RelayCommand]
        private async void SaveRank()
        {
            Preferences.Set("UseRank", UseRank);
        }

        public async void OnAppearing()
        {
            UseRank = Preferences.Get("UseRank", false);
        }
    }
}
