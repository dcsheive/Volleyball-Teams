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
    public partial class TeamsViewModel : ObservableObject
    {

        readonly IPlayerStore<Player> dataStore;
        ILogger<PlayersViewModel> logger;

        public ObservableCollection<Team> Teams { get; private set; }
        public ObservableCollection<string> WinTeams { get; private set; }
        public ObservableCollection<string> LoseTeams { get; private set; }
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

        private Player playerDragged;

        public bool AtLeast2Teams
        {
            get
            {
                return Teams.Count > 1;
            }
        }

        public TeamsViewModel(IPlayerStore<Player> dataStore, ILogger<PlayersViewModel> logger)
        {
            this.dataStore = dataStore;
            this.logger = logger;
            Title = "Teams";
            Teams = new ObservableCollection<Team>();
            Players = new List<Player>();
            IsBusy = false;
            NumTeams = Preferences.Get(Constants.Settings.NumTeams, 2);
            OnPropertyChanged(nameof(AtLeast2Teams));
            DidNotFinishLoading = true;
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
            IsBusy = true;
            NumTeams++;
            Preferences.Set(Constants.Settings.NumTeams, NumTeams);
            LoadPlayers();
            IsBusy = false;
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
            IsBusy = true;
            NumTeams--;
            Preferences.Set(Constants.Settings.NumTeams, NumTeams);
            LoadPlayers();
            IsBusy = false;
        }

        [RelayCommand]
        public void ItemDragged(Player player)
        {
            Debug.WriteLine($"ItemDragged : {player.Name}");
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
            Debug.WriteLine($"ItemDraggedOver : {player?.Name}");
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
                Debug.WriteLine($"ItemDropped: [{playerToMove?.Name}] => [{playerToInsertBefore?.Name}], target index = [{insertAtIndex}]");
                var teams = Teams.ToList();
                Teams = new ObservableCollection<Team>(teams);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
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

        [RelayCommand]
        public void LoadPlayers(string pageLoad = "false")
        {
            Task.Run(async () => await DoLoadPlayers(pageLoad));
        }

        private async Task DoLoadPlayers(string pageLoad)
        {
            if (IsBusy) return;
            IsBusy = true;
            logger.LogDebug($"IsBusy={IsBusy}");
            bool isPageLoad = Convert.ToBoolean(pageLoad);
            try
            {
                if (isPageLoad)
                {
                    Players.Clear();
                    var items = await dataStore.GetPlayersHereAsync();
                    foreach (var item in items)
                    {
                        Players.Add(item);
                        logger.LogDebug($"{item.Name}, {item.IsHere}");
                    }
                }
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
                await Task.Delay(500);
                IsBusy = false;
                IsRefreshing = false;
                logger.LogDebug("Set IsBusy to false");
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
                    sb.AppendLine($"\t{player.Name}");
                    player.NumWins++;
                }
                sb.AppendLine("Losers");
                foreach (var player in Teams[int.Parse(LosingTeam) - 1])
                {
                    sb.AppendLine($"\t{player.Name}");
                    player.NumLosses++;
                }
                await dataStore.UpdatePlayersAsync(Players);
                string endStr = sb.ToString();
                Debug.Print(endStr);
                await Application.Current.MainPage.DisplayAlert("Confirmation", endStr, "OK");
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
            foreach (var team in teams)
            {
                Teams.Add(team);
            }
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
            foreach (var team in teams)
            {
                Teams.Add(team);
            }
        }

        async public void OnAppearing()
        {
            DidNotFinishLoading = true;
            UseRank = Preferences.Get(Constants.Settings.UseRank, true);
            if (Players.Count == 0)
                LoadPlayers("True");
        }
    }
}