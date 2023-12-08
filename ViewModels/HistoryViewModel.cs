using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using Volleyball_Teams.Models;
using Volleyball_Teams.Services;
using Volleyball_Teams.Util;

namespace Volleyball_Teams.ViewModels
{
    public partial class HistoryViewModel : ObservableObject
    {

        ILogger<HistoryViewModel> logger;
        readonly IPlayerStore playerStore;
        readonly ITeamStore teamStore;
        readonly IGlobalVariables globalVariables;

        [ObservableProperty]
        private ObservableCollection<Game> games;

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

        [ObservableProperty]
        private bool isRefreshing;

        [ObservableProperty]
        private bool useRank;

        [ObservableProperty]
        private bool didNotFinishLoading;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string loadText = Constants.Loading.SelectTeam;

        private List<Player> Players;

        public HistoryViewModel(IPlayerStore playerStore, ITeamStore teamStore, ILogger<HistoryViewModel> logger, IGlobalVariables globalVariables)
        {
            this.playerStore = playerStore;
            this.teamStore = teamStore;
            this.logger = logger;
            this.globalVariables = globalVariables;
            Title = Constants.Title.History;
            IsBusy = false;
            DidNotFinishLoading = true;
            Players = new List<Player>();
            Games = new ObservableCollection<Game>();
        }

        public void OnAppearing()
        {
            IsLoading = false;
            DidNotFinishLoading = true;
            UseRank = Settings.UseRank;
            LoadTeams();
        }

        [RelayCommand]
        private async Task SelectTeam(Game tl)
        {
            IsLoading = true;
            LoadText = Constants.Loading.SelectTeam;
            logger.LogDebug($"Select Team ID = {tl.Id}");
            await Shell.Current.GoToAsync($"..");
            IsLoading = false;
        }

        [RelayCommand]
        private async Task DeleteTeam(Game tl)
        {
            IsLoading = true;
            LoadText = Constants.Loading.DeleteGame;
            Games.Remove(tl);
            await teamStore.DeleteTeamByIdAsync(tl.Id);
            IsLoading = false;
        }

        [RelayCommand]
        private async Task CloseTeams()
        {
            await Shell.Current.GoToAsync($"..");
        }

        [RelayCommand]
        public void LoadTeams()
        {
            Task.Run(async () => await DoLoadTeams());
        }
        private async Task DoLoadTeams()
        {
            if (IsBusy) return;
            IsBusy = true;
            logger.LogDebug($"IsBusy = {IsBusy}");
            try
            {
                Players = await playerStore.GetPlayersAsync();
                //List<TeamDB> teamsdb = await teamStore.GetTeamsAsync();
                //List<Game> tll = new();
                //SavedTeams.Clear();
                //foreach (var teamdb in teamsdb)
                //{
                //    string[] teamsStr = teamdb.IDStr.Split('$');
                //    List<Team> teamsArr = new();
                //    string name = teamdb.Name;
                //    int count = 0;
                //    for (int i = 0; i < teamsStr.Length; i++)
                //    {
                //        if (string.IsNullOrEmpty(teamsStr[i])) { continue; }
                //        teamsArr.Add(new Team(count++, name, GetPlayerListFromStr(teamsStr[i])));
                //    }
                //    CalcPowers(teamsArr);
                //    tll.Add(new Game(teamdb.Id, teamsArr));
                //}
                //SavedTeams = new ObservableCollection<Game>(tll);
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
    }
}