using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Text;
using Volleyball_Teams.Models;
using Volleyball_Teams.Services;
using Volleyball_Teams.Util;
using Volleyball_Teams.Views;

namespace Volleyball_Teams.ViewModels
{
    public partial class TeamsViewModel : ObservableObject
    {
        ILogger<TeamsViewModel> logger;
        readonly IPlayerStore playerStore;
        readonly ITeamStore teamStore;
        readonly IGlobalVariables globalVariables;

        [ObservableProperty]
        private ObservableCollection<Team> teams;

        [ObservableProperty]
        private ObservableCollection<string> winTeams;

        [ObservableProperty]
        private ObservableCollection<string> loseTeams;

        public bool AtLeast2Teams
        {
            get
            {
                return Teams.Count > 1;
            }
        }

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
        private bool useRank;

        [ObservableProperty]
        private bool didNotFinishLoading;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string loadText = Constants.Loading.LoadingTeams;

        private Player playerDragged;
        private List<Player> Players;
        private bool disableSelect;

        public TeamsViewModel(IPlayerStore playerStore, ITeamStore teamStore, ILogger<TeamsViewModel> logger, IGlobalVariables globalVariables)
        {
            this.playerStore = playerStore;
            this.teamStore = teamStore;
            this.logger = logger;
            this.globalVariables = globalVariables;
            IsBusy = false;
            DidNotFinishLoading = true;
            Title = Constants.Title.Teams;
            Teams = new ObservableCollection<Team>();
            Players = new List<Player>();
            NumTeams = Settings.NumTeams;
            OnPropertyChanged(nameof(AtLeast2Teams));
        }

        public void OnAppearing()
        {
            IsLoading = false;
            DidNotFinishLoading = true;
            UseRank = Settings.UseRank;
            if (globalVariables.TeamID == 0)
            {
                if (Players.Count == 0) LoadPlayers();
            }
            else
            {
                LoadTeams();
            }
        }

        [RelayCommand]
        private void AddTeam()
        {
            Task.Run(() => DoAddTeam());
        }
        private void DoAddTeam()
        {
            if (NumTeams >= Players.Count) return;
            if (IsBusy) return;
            NumTeams++;
            Settings.NumTeams = NumTeams;
            LoadPlayers();
        }

        [RelayCommand]
        private void RemoveTeam()
        {
            Task.Run(() => DoRemoveTeam());
        }
        private void DoRemoveTeam()
        {
            if (NumTeams <= 1) return;
            if (IsBusy) return;
            NumTeams--;
            Settings.NumTeams = NumTeams;
            LoadPlayers();
        }

        [RelayCommand]
        public void ItemDragged(Player player)
        {
            logger.LogDebug($"ItemDragged : {player.Name}");
            player.IsBeingDragged = true;
            playerDragged = player;
        }

        [RelayCommand]
        private void SetOppositeTeam(string option)
        {
            if (disableSelect) return;
            if (WinningTeam == LosingTeam || string.IsNullOrEmpty(LosingTeam) || string.IsNullOrEmpty(WinningTeam))
            {
                if (option == "win")
                {
                    LosingTeam = LoseTeams.Where(e => e != WinningTeam).First();
                }
                else
                {
                    WinningTeam = WinTeams.Where(e => e != LosingTeam).First();
                }
            }
        }

        [RelayCommand]
        public void ItemDraggedOver(Player player)
        {
            logger.LogDebug($"ItemDraggedOver : {player?.Name}");
            if (player == playerDragged)
            {
                player.IsBeingDragged = false;
            }
            player.IsBeingDraggedOver = player != playerDragged;
        }

        [RelayCommand]
        public void ItemDropped(Player player)
        {
            try
            {
                var playerToMove = playerDragged;
                var playerToInsertBefore = player;
                if (playerToMove == null || playerToInsertBefore == null || playerToMove == playerToInsertBefore)
                    return;
                int insertAtIndex = playerToInsertBefore.Team.IndexOf(playerToInsertBefore);
                if (insertAtIndex >= 0 && insertAtIndex < Players.Count)
                {
                    playerToMove.Team.Remove(playerToMove);
                    playerToInsertBefore.Team.Insert(insertAtIndex, playerToMove);
                    CalcPowers();
                    if (playerToMove.Team.Count == 0)
                    {
                        Teams.Remove(playerToMove.Team);
                        NumTeams = Teams.Count;
                        NumberTeams();
                        ResetWinLoseTeams();
                    }
                    playerToMove.Team = playerToInsertBefore.Team;
                    playerToMove.IsBeingDragged = false;
                    playerToInsertBefore.IsBeingDraggedOver = false;
                }
                logger.LogDebug($"ItemDropped: [{playerToMove?.Name}] => [{playerToInsertBefore?.Name}], target index = [{insertAtIndex}]");
                var teams = Teams.ToList();
                Teams = new ObservableCollection<Team>(teams);
            }
            catch (Exception ex)
            {
                logger.LogError("{ex}", ex);
            }
        }

        [RelayCommand]
        public void LoadPlayers()
        {
            Task.Run(async () => await DoLoadPlayers());
        }
        private async Task DoLoadPlayers()
        {
            if (IsBusy) return;
            IsBusy = true;
            logger.LogDebug($"IsBusy = {IsBusy}");
            try
            {
                Players = await playerStore.GetPlayersHereAsync();
                if (UseRank)
                    SortWithRank();
                else
                    SortRandom();
                ResetWinLoseTeams();
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

        [RelayCommand]
        private void LoadTeams()
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
                Teams.Clear();
                Players = await playerStore.GetPlayersAsync();
                TeamDB teamdb = await teamStore.GetTeamAsync(globalVariables.TeamID);
                globalVariables.TeamID = 0;
                string[] teamsStr = teamdb.IDStr.Split('$');
                List<Team> teamsArr = new();
                int count = 0;
                for (int i = 0; i < teamsStr.Length; i++)
                {
                    if (string.IsNullOrEmpty(teamsStr[i])) { continue; }
                    teamsArr.Add(new Team(count++, GetPlayerListFromStr(teamsStr[i])));
                }
                NumTeams = teamsArr.Count;
                Teams = new ObservableCollection<Team>(teamsArr);
                CalcPowers();
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

        [RelayCommand]
        private async Task OpenSavedTeams(object obj)
        {
            IsLoading = true;
            await Shell.Current.GoToAsync($"{nameof(SavedTeamsPage)}");
            IsLoading = false;
        }

        [RelayCommand]
        private async Task SaveTeams()
        {
            StringBuilder sb = new StringBuilder();
            StringBuilder idstr = new StringBuilder();
            foreach (var team in Teams)
            {
                idstr.Append("$");
                sb.Append($"Team {team.NumberText}\n");
                sb.Append(string.Join("\n", team.Select(e => "\t-" + e.Name).ToList()));
                idstr.Append(string.Join(",", team.Select(x => x.Id).ToList()));
                sb.Append("\n");
            }
            TeamDB teamDB = new TeamDB();
            teamDB.IDStr = idstr.ToString();
            if (await teamStore.CheckIfExists(teamDB.IDStr))
            {
                await Application.Current.MainPage.DisplayAlert("Failed", "This team configuration already exists.", "OK");
                return;
            }
            await teamStore.AddTeamAsync(teamDB);
            string endStr = sb.ToString();
            logger.LogDebug(endStr);
            await Application.Current.MainPage.DisplayAlert("Confirmation", endStr, "OK");
        }

        [RelayCommand]
        private async Task Apply()
        {
            if (string.IsNullOrEmpty(WinningTeam) || string.IsNullOrEmpty(LosingTeam)) return;
            bool result = await Application.Current.MainPage.DisplayAlert("Confirmation", $"Team {WinningTeam} Wins\nTeam {LosingTeam} Loses", "OK", "Cancel");
            if (result)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendLine("Winners");
                foreach (var player in Teams[int.Parse(WinningTeam) - 1])
                {
                    sb.AppendLine($"\t-{player.Name}");
                    player.NumWins++;
                }
                sb.AppendLine("Losers");
                foreach (var player in Teams[int.Parse(LosingTeam) - 1])
                {
                    sb.AppendLine($"\t-{player.Name}");
                    player.NumLosses++;
                }
                await playerStore.UpdatePlayersAsync(Players);
                string endStr = sb.ToString();
                logger.LogDebug(endStr);
                await Application.Current.MainPage.DisplayAlert("Confirmation", endStr, "OK");
            }
        }

        private void NumberTeams()
        {
            int count = 0;
            foreach (Team team in Teams)
            {
                team.Number = count++;
            }
        }

        public void CalcPowers()
        {
            foreach (Team t in Teams)
            {
                int pow = 0;
                foreach (Player p in t)
                    pow += int.Parse(p.NumStars);
                t.Power = pow;
            }
        }

        private async void ResetWinLoseTeams()
        {
            OnPropertyChanged(nameof(AtLeast2Teams));
            if (NumTeams > 1)
            {
                disableSelect = true;
                WinTeams = new ObservableCollection<string>(Teams.Select(t => t.NumberText).ToList());
                LoseTeams = new ObservableCollection<string>(Teams.Select(t => t.NumberText).ToList());
                await Task.Delay(200);
                WinningTeam = WinTeams[0];
                LosingTeam = LoseTeams[1];
                disableSelect = false;
            }
        }

        private Team[] MakeTeams()
        {
            Teams.Clear();
            Constants.Shuffle(Players);
            if (NumTeams > Players.Count) NumTeams = Players.Count;
            if (NumTeams == 0 && Players.Count > 0)
            {
                NumTeams++;
                if (Players.Count > 1) NumTeams++;
            }
            Team[] teams = new Team[NumTeams];
            for (int i = 0; i < teams.Length; i++)
            {
                teams[i] = new Team(i, new List<Player>());
            }
            return teams;
        }

        private void SortRandom()
        {
            Team[] teams = MakeTeams();
            int counter = 0;
            foreach (var player in Players.ToList())
            {
                if (counter == NumTeams) counter = 0;
                Team team = teams[counter];
                team.Add(player);
                player.Team = team;
                counter++;
            }
            Teams = new ObservableCollection<Team>(teams);
        }

        private void SortWithRank()
        {
            Team[] teams = MakeTeams();
            foreach (var player in Players.ToList())
            {
                Team smallest = teams[0];
                for (int i = 1; i < teams.Length; i++)
                {
                    Team current = teams[i];
                    if (smallest.Power > current.Power) smallest = current;
                }
                smallest.Add(player);
                player.Team = smallest;
                smallest.Power += int.Parse(player.NumStars);
            }
            Teams = new ObservableCollection<Team>(teams);
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