using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Text;
using Volleyball_Teams.Models;
using Volleyball_Teams.Services;
using Volleyball_Teams.Util;

namespace Volleyball_Teams.ViewModels
{
    [QueryProperty(nameof(ID), "ID")]
    public partial class NewTeamViewModel : ObservableObject
    {
        public string ID { get; set; }

        ILogger<NewTeamViewModel> logger;
        readonly ITeamStore teamStore;
        readonly IPlayerStore playerStore;

        [ObservableProperty]
        private ObservableCollection<Player> players;

        [ObservableProperty]
        private string selectedStar;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private string? name;

        [ObservableProperty]
        private string? title;

        [ObservableProperty]
        private bool showDelete;

        [ObservableProperty]
        private int wins;

        [ObservableProperty]
        private int losses;

        [ObservableProperty]
        private int power;

        private int TeamCount;
        private TeamDB MyTeamDB = new TeamDB();
        public NewTeamViewModel(ITeamStore teamStore, IPlayerStore playerStore, ILogger<NewTeamViewModel> logger)
        {
            this.teamStore = teamStore;
            this.playerStore = playerStore;
            this.logger = logger;
            Title = Constants.Title.NewTeam;
        }
        async public void OnAppearing()
        {
            TeamCount = await teamStore.GetTeamsCountAsync();
            if (!string.IsNullOrEmpty(ID))
            {
                ShowDelete = true;
                await LoadPlayers();
                await GetTeam();
            }
            else
            {
                ShowDelete = false;
                await LoadPlayers();
                Name = $"Team {TeamCount + 1}";
                Wins = 0;
                Losses = 0;
            }
        }
        [RelayCommand]
        private void CalcPower()
        {
            int pow = 0;
            foreach( Player p in Players)
            {
                if (p.IsChecked) pow += int.Parse(p.NumStars);
            }
            Power = pow;
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
            await teamStore.DeleteTeamAsync(MyTeamDB);
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand(CanExecute = nameof(ValidateSave))]
        private async Task Save()
        {
            logger.LogDebug("Save: Name: {name}", Name);
            MyTeamDB.Name = Name;
            MyTeamDB.NumWins = Wins;
            MyTeamDB.NumLosses = Losses;
            MyTeamDB.Power = Power;
            List<Player> list = Players.Where(p => p.IsChecked).ToList();
            StringBuilder idstr = new StringBuilder();
            foreach (Player player in list)
            {
                idstr.Append(player.Id + ",");
            }
            MyTeamDB.PlayerIdStr = idstr.ToString().Substring(0, idstr.Length - 1);
            if (string.IsNullOrEmpty(ID))
            {
                if (await CheckDB())
                {
                    await Application.Current.MainPage.DisplayAlert("Failed", "This name has already been used.", "OK");
                    return;
                }
                _ = await teamStore.AddTeamAsync(MyTeamDB);
            }
            else
                await teamStore.UpdateTeamAsync(MyTeamDB);
            await Shell.Current.GoToAsync("..");
        }

        private async Task LoadPlayers()
        {
            Players = new ObservableCollection<Player>(await playerStore.GetPlayersAsync());
        }

        private async Task<bool> CheckDB()
        {
            TeamDB t = await teamStore.GetTeamByNameAsync(Name);
            if (t == null) return false;
            return true;
        }

        private bool ValidateSave()
        {
            var canExecute = !String.IsNullOrWhiteSpace(Name);
            logger.LogDebug("ValidateSave: {canExecute}", canExecute);
            return canExecute;
        }

        private async Task GetTeam()
        {
            TeamDB teamdb = await teamStore.GetTeamAsync(int.Parse(ID));            
            string[] playerids = teamdb.PlayerIdStr.Split(",");
            foreach (string playerid in playerids)
            {
                Player p = Players.Where(pl => pl.Id == int.Parse(playerid)).FirstOrDefault();
                p.IsChecked = true;
            }
            MyTeamDB = teamdb;
            Name = teamdb.Name;
            Wins = teamdb.NumWins;
            Losses = teamdb.NumLosses;
        }
    }
}
