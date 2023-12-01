using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.ObjectModel;
using Volleyball_Teams.Models;
using Volleyball_Teams.Services;

namespace Volleyball_Teams.ViewModels
{
    [QueryProperty(nameof(ID), "ID")]
    public partial class NewPlayerViewModel : ObservableObject
    {
        public string ID { get; set; }

        readonly IPlayerStore<Player>? dataStore;
        ILogger<NewPlayerViewModel> logger;
        public ObservableCollection<string> Stars { get; set; }
        public string SelectedStar { get; set; }
        public Player MyPlayer { get; set; }
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private string? name;

        [ObservableProperty]
        private bool showDelete;

        [ObservableProperty]
        private int wins;

        [ObservableProperty]
        private int losses;
        public NewPlayerViewModel(IPlayerStore<Player> dataStore, ILogger<NewPlayerViewModel> logger)
        {
            if (dataStore == null) { throw new ArgumentNullException(nameof(dataStore)); }
            this.dataStore = dataStore;
            this.logger = logger;
            Stars = new ObservableCollection<string>() { "1", "2", "3", "4", "5" };
        }

        [RelayCommand]
        private async Task Cancel()
        {
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand]
        private void MinusWins()
        {
            if (Wins > 0)
                Wins--;
        }

        [RelayCommand]
        private void AddWins()
        {   
            Wins++;
        }

        [RelayCommand]
        private void MinusLosses()
        {
            if (Losses > 0)
                Losses--;
        }

        [RelayCommand]
        private void AddLosses()
        {
            Losses++;
        }

        [RelayCommand]
        private async Task Delete()
        {
            await dataStore.DeletePlayerAsync(MyPlayer);
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand(CanExecute = nameof(ValidateSave))]
        private async Task Save()
        {
            logger.LogDebug("Save: Name: {name}", Name);
            MyPlayer.Name = Name;
            MyPlayer.NumStars = SelectedStar;
            MyPlayer.NumWins = Wins;
            MyPlayer.NumLosses = Losses;
            if (string.IsNullOrEmpty(ID))
            {
                if (await CheckDB())
                {
                    await Application.Current.MainPage.DisplayAlert("Name Found", "This name has already been used.", "OK");
                    return;
                }
                _ = await dataStore.AddPlayerAsync(MyPlayer);
            }
            else
                await dataStore.UpdatePlayerAsync(MyPlayer);
            await dataStore.SetPlayerRanksByRatio();
            await Shell.Current.GoToAsync("..");
        }

        private async Task<bool> CheckDB()
        {
            Player p = await dataStore.GetPlayerByNameAsync(Name);
            if (p == null) return false;
            return true;
        }

        private bool ValidateSave()
        {
            var canExecute = !String.IsNullOrWhiteSpace(Name);
            logger.LogDebug("ValidateSave: {canExecute}", canExecute);
            return canExecute;
        }

        private async Task GetPlayer()
        {
            MyPlayer = await dataStore.GetPlayerAsync(int.Parse(ID));
            Name = MyPlayer.Name;
            SelectedStar = MyPlayer.NumStars;
            Wins = MyPlayer.NumWins;
            Losses = MyPlayer.NumLosses;
        }

        async public void OnAppearing()
        {
            if (!string.IsNullOrEmpty(ID))
            {
                ShowDelete = true;
                await GetPlayer();
            }
            else
            {
                ShowDelete = false;
                MyPlayer = new Player();
                Name = string.Empty;
                SelectedStar = "3";
                Wins = 0;
                Losses = 0;
            }
        }
    }
}
