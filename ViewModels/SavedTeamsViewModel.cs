using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using System.Windows.Input;
using Volleyball_Teams.Models;
using Volleyball_Teams.Services;
using Volleyball_Teams.Util;
using Volleyball_Teams.Views;

namespace Volleyball_Teams.ViewModels
{
    public partial class SavedTeamsViewModel : ObservableObject
    {

        readonly IPlayerStore playerStore;
        readonly ITeamStore teamStore;
        readonly IGlobalVariables globalVariables;
        ILogger<SavedTeamsViewModel> logger;

        private List<Player> Players;

        [ObservableProperty]
        private string losingTeam;
        [ObservableProperty]
        private string winningTeam;

        [ObservableProperty]
        private string? title;

        [ObservableProperty]
        private int numTeams;

        [ObservableProperty]
        private bool isBusy;

        private bool disableSelect;

        [ObservableProperty]
        private bool isRefreshing;

        [ObservableProperty]
        private bool useRank;

        [ObservableProperty]
        private bool didNotFinishLoading;

        public ObservableCollection<TeamList> SavedTeams { get; set; }

        public SavedTeamsViewModel(IPlayerStore playerStore, ITeamStore teamStore, ILogger<SavedTeamsViewModel> logger, IGlobalVariables globalVariables)
        {
            this.playerStore = playerStore;
            this.teamStore = teamStore;
            this.logger = logger;
            this.globalVariables = globalVariables;
            Title = "Saved Teams";
            SavedTeams = new();
            Players = new List<Player>();
            IsBusy = false;
            DidNotFinishLoading = true;
        }

        [RelayCommand]
        private async Task SelectTeam(TeamList tl)
        {
            globalVariables.TeamID = tl.Id;
            await Shell.Current.GoToAsync($"..");
        }

        [RelayCommand]
        private async Task DeleteTeam(TeamList tl)
        {
            SavedTeams.Remove(tl);
            await teamStore.DeleteTeamByIdAsync(tl.Id);
        }

        [RelayCommand]
        private async Task CloseTeams()
        {
            await Shell.Current.GoToAsync($"..");
        }

        async public void OnAppearing()
        {
            DidNotFinishLoading = true;
            UseRank = Preferences.Get(Constants.Settings.UseRank, true);
            LoadTeams();
        }

        public void CalcPowers(List<Team> teams)
        {
            foreach (Team t in teams)
            {
                int pow = 0;
                foreach (Player p in t)
                    pow += int.Parse(p.NumStars);
                t.Power = pow;
            }
        }

        public void LoadTeams()
        {
            Task.Run(async () => await DoLoadTeams());
        }

        private async Task DoLoadTeams()
        {
            Players = await playerStore.GetPlayersAsync();
            List<TeamDB> teamsdb = await teamStore.GetTeamsAsync();
            List<TeamList> tll = new();
            SavedTeams.Clear();
            foreach (var teamdb in teamsdb)
            {
                string[] teamsStr = teamdb.IDStr.Split('$');
                List<Team> teamsArr = new();
                int count = 0;
                for (int i = 0; i < teamsStr.Length; i++)
                {
                    if (string.IsNullOrEmpty(teamsStr[i])) { continue; }
                    teamsArr.Add(new Team(count++, GetPlayerListFromStr(teamsStr[i])));
                }
                CalcPowers(teamsArr);
                tll.Add(new TeamList(teamdb.Id, teamsArr));
            }
            SavedTeams = new ObservableCollection<TeamList>(tll);
        }

        private List<Player> GetPlayerListFromStr(string playerStr)
        {
            List<Player> players = new List<Player>();
            string[] playerArr = playerStr.Split(",");
            foreach (string id in playerArr)
            {
                int idInt = int.Parse(id);
                players.Add(Players.Where(player => player.Id == idInt).First());
            }
            return players;
        }
    }
}