using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Text;
using System.Xml.Linq;
using Volleyball_Teams.Models;
using Volleyball_Teams.Services;
using Volleyball_Teams.Util;
using Volleyball_Teams.Views;

namespace Volleyball_Teams.ViewModels
{
    public partial class GameViewModel : ObservableObject
    {
        ILogger<GameViewModel> logger;
        readonly IPlayerStore playerStore;
        readonly ITeamStore teamStore;
        readonly IGameStore gameStore;
        readonly IGlobalVariables globalVariables;

        [ObservableProperty]
        private Team leftTeam;

        [ObservableProperty]
        private Team rightTeam;

        [ObservableProperty]
        private int leftScore;

        [ObservableProperty]
        private int rightScore;

        [ObservableProperty]
        private string? title;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private bool useRank;

        [ObservableProperty]
        private bool didNotFinishLoading;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private bool showLoader;

        [ObservableProperty]
        private string loadText = Constants.Loading.GameMessage;

        private bool IsGameOver;
        private string GameOverMessage;
        public GameViewModel(IPlayerStore playerStore, ITeamStore teamStore, IGameStore gameStore, ILogger<GameViewModel> logger, IGlobalVariables globalVariables)
        {
            this.playerStore = playerStore;
            this.teamStore = teamStore;
            this.gameStore = gameStore;
            this.logger = logger;
            this.globalVariables = globalVariables;
            IsBusy = false;
            DidNotFinishLoading = true;
            Title = Constants.Title.Game;
            LeftScore = 0;
            RightScore = 0;
        }

        public void OnAppearing()
        {
            UseRank = Settings.UseRank;
            if (IsGameOver)
            {
                if (!globalVariables.NewGame)
                {
                    IsLoading = true;
                    ShowLoader = false;
                    LoadText = GameOverMessage;
                    return;
                }
                else
                {
                    IsLoading = false;
                    LoadGame();
                }
            }
            else
            {
                IsLoading = false;
                LoadGame();
            }
        }

        private void LoadGame()
        {
            globalVariables.NewGame = false;
            LeftTeam = globalVariables.LeftTeam;
            RightTeam = globalVariables.RightTeam;
            LeftScore = globalVariables.LeftScore;
            RightScore = globalVariables.RightScore;
            IsGameOver = false;
            if (LeftTeam == null || RightTeam == null)
            {
                IsLoading = true;
                ShowLoader = false;
                LoadText = Constants.Loading.GameMessage;
            }
            else
            {
                IsLoading = false;
                LeftTeam = globalVariables.LeftTeam;
                RightTeam = globalVariables.RightTeam;
            }
        }

        [RelayCommand]
        private void MinusLeftScore()
        {
            if (IsGameOver) return;
            if (LeftScore > 0)
                LeftScore--;
        }

        [RelayCommand]
        private async Task AddLeftScore()
        {
            if (IsGameOver) return;
            if (LeftScore == 20)
            {
                bool confirmed = await ConfirmWin(true);
                if (confirmed)
                {
                    LeftScore++;
                    await EndGame(true);
                }
                return;
            }
            LeftScore++;
        }

        [RelayCommand]
        private void MinusRightScore()
        {
            if (IsGameOver) return;
            if (RightScore > 0)
                RightScore--;
        }

        [RelayCommand]
        private async Task AddRightScore()
        {
            if (IsGameOver) return;
            if (RightScore == 20)
            {
                bool confirmed = await ConfirmWin(false);
                if (confirmed)
                {
                    RightScore++;
                    await EndGame(false);
                }
                return;

            }
            RightScore++;
        }

        [RelayCommand]
        private async Task OpenHistory(object obj)
        {
            LoadText = Constants.Loading.LoadingHistory;
            ShowLoader = true;
            IsLoading = true;
            await Shell.Current.GoToAsync($"{nameof(HistoryPage)}");
            IsLoading = false;
        }

        [RelayCommand]
        private async Task SaveTeams()
        {
            await Application.Current.MainPage.DisplayAlert("Confirmation", "", "OK");
        }

        private async Task<bool> ConfirmWin(bool leftWins)
        {
            Team winner;
            if (leftWins) winner = LeftTeam; else winner = RightTeam;
            bool result = await Application.Current.MainPage.DisplayAlert("Confirmation", $"{winner.NameDisplay} will win the game.", "OK", "Cancel");
            return result;
        }

        [RelayCommand]
        private async Task SaveGame()
        {
            bool result = await Application.Current.MainPage.DisplayAlert("Confirmation", $"Save this game for later?", "OK", "Cancel");
            if (result)
            {
                await EndGame(false, false);
            }

        }

        [RelayCommand]
        private async Task Reset()
        {
            bool result = await Application.Current.MainPage.DisplayAlert("Confirmation", $"Reset this game?", "OK", "Cancel");
            if (result)
            {
                IsLoading = false;
                IsGameOver = false;
                LeftScore = 0;
                RightScore = 0;
            }
        }

        private TeamDB MakeTeamDB(Team team)
        {
            TeamDB db = new TeamDB();
            db.Name = team.Name;
            db.NumWins = team.NumWins;
            db.NumLosses = team.NumLosses;
            db.Power = team.Power;
            db.IsRandom = true;
            StringBuilder idstr = new StringBuilder();
            foreach (Player player in team)
            {
                idstr.Append(player.Id + ",");
            }
            db.PlayerIdStr = idstr.ToString().Substring(0, idstr.Length - 1);
            return db;
        }

        private async Task EndGame(bool leftWins, bool hasWinner = true)
        {
            IsGameOver = true;
            if (LeftTeam.Id == 0)
            {
                TeamDB left = MakeTeamDB(LeftTeam);
                left.Name = "Team " + Guid.NewGuid().ToString().Substring(0, 6);
                await teamStore.AddTeamAsync(left);
                left = await teamStore.GetTeamByNameAsync(left.Name);
                LeftTeam.Id = left.Id;
                TeamDB right = MakeTeamDB(RightTeam);
                right.Name = "Team " + Guid.NewGuid().ToString().Substring(0, 6);
                await teamStore.AddTeamAsync(right);
                right = await teamStore.GetTeamByNameAsync(right.Name);
                RightTeam.Id = right.Id;
            }
            if (hasWinner) await UpdateScores(leftWins);
            await EnterGameIntoDB(leftWins, hasWinner);
            string loadstr = Constants.Loading.GameOver;
            if (hasWinner)
            {
                if (leftWins) loadstr += "\n" + LeftTeam.NameDisplay + " Wins!" + Constants.Loading.GameOverMessage;
                else loadstr += "\n" + RightTeam.NameDisplay + " Wins!" + Constants.Loading.GameOverMessage;
            }
            GameOverMessage = loadstr;
            LoadText = loadstr;
            ShowLoader = false;
            IsLoading = true;
        }

        private async Task UpdateScores(bool leftWins)
        {
            Team winner;
            Team loser;
            if (leftWins) { winner = LeftTeam; loser = RightTeam; }
            else { winner = RightTeam; loser = LeftTeam; }
            foreach (Player p in winner)
            {
                p.NumWins++;
            }
            foreach (Player p in loser)
            {
                p.NumLosses++;
            }
            TeamDB winnerDB = await teamStore.GetTeamAsync(winner.Id);
            TeamDB loserDB = await teamStore.GetTeamAsync(loser.Id);
            winnerDB.NumWins += 1;
            loserDB.NumLosses += 1;
            List<TeamDB> teams = new List<TeamDB> { winnerDB, loserDB };
            await teamStore.UpdateTeamsAsync(teams);
            await playerStore.UpdatePlayersAsync(winner);
            await playerStore.UpdatePlayersAsync(loser);
        }

        private async Task EnterGameIntoDB(bool leftWins, bool hasWinner)
        {
            GameDB g = new GameDB();
            g.LeftTeamId = LeftTeam.Id;
            g.RightTeamId = RightTeam.Id;
            g.LeftTeamScore = LeftScore;
            g.RightTeamScore = RightScore;
            g.HasWinner = hasWinner;
            g.LeftWins = leftWins;
            await gameStore.AddGameAsync(g);
        }
    }
}