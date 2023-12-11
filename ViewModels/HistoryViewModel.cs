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
        readonly IGameStore gameStore;
        readonly IGlobalVariables globalVariables;

        [ObservableProperty]
        private ObservableCollection<Game> games;

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
        private string loadText = Constants.Loading.SelectGame;

        private List<Player> Players;
        private List<Team> Teams;

        public HistoryViewModel(IPlayerStore playerStore, ITeamStore teamStore, IGameStore gameStore, ILogger<HistoryViewModel> logger, IGlobalVariables globalVariables)
        {
            this.playerStore = playerStore;
            this.teamStore = teamStore;
            this.gameStore = gameStore;
            this.logger = logger;
            this.globalVariables = globalVariables;
            Title = Constants.Title.History;
            IsBusy = false;
            DidNotFinishLoading = true;
            Players = new List<Player>();
            Teams = new List<Team>();
            Games = new ObservableCollection<Game>();
        }

        public void OnAppearing()
        {
            IsLoading = false;
            DidNotFinishLoading = true;
            UseRank = Settings.UseRank;
            LoadGames();
        }

        [RelayCommand]
        private async Task SelectGame(Game tl)
        {
            IsLoading = true;
            LoadText = Constants.Loading.SelectGame;
            globalVariables.LeftTeam = tl.LeftTeam;
            globalVariables.RightTeam = tl.RightTeam;
            globalVariables.NewGame = true;
            if (!tl.HasWinner)
            {
                globalVariables.LeftScore = tl.LeftTeamScore;
                globalVariables.RightScore = tl.RightTeamScore;
            }
            else
            {
                globalVariables.LeftScore = 0;
                globalVariables.RightScore = 0;
            }
            logger.LogDebug($"Select Game ID = {tl.Id}");
            await Shell.Current.GoToAsync($"..");
            IsLoading = false;
        }

        [RelayCommand]
        private async Task DeleteGame(Game tl)
        {
            IsLoading = true;
            LoadText = Constants.Loading.DeleteGame;
            Games.Remove(tl);
            await gameStore.DeleteGameByIdAsync(tl.Id);
            IsLoading = false;
        }

        [RelayCommand]
        private async Task CloseTeams()
        {
            await Shell.Current.GoToAsync($"..");
        }

        [RelayCommand]
        public void LoadGames()
        {
            Task.Run(async () => await DoLoadGames());
        }
        private async Task DoLoadGames()
        {
            if (IsBusy) return;
            IsBusy = true;
            logger.LogDebug($"IsBusy = {IsBusy}");
            try
            {
                Players.Clear();
                Teams.Clear();
                Games.Clear();
                Players = await playerStore.GetPlayersAsync();
                List<TeamDB> teamdbs = await teamStore.GetTeamsAsync();
                int count = 0;
                foreach (var teamdb in teamdbs)
                {
                    string[] playerids = teamdb.PlayerIdStr.Split(",");
                    List<Player> teamplayers = new List<Player>();
                    foreach (var playerid in playerids)
                    {
                        Player player = Players.Where(p => p.Id == int.Parse(playerid)).FirstOrDefault();
                        if (player != null) teamplayers.Add(player);
                    }
                    Team team = new Team(count++, teamplayers);
                    team.NumLosses = teamdb.NumLosses;
                    team.NumWins = teamdb.NumWins;
                    team.Name = teamdb.Name;
                    team.Power = teamdb.Power;
                    team.Id = teamdb.Id;
                    Teams.Add(team);
                }
                List<GameDB> gamedbs = await gameStore.GetGamesAsync();
                gamedbs.Reverse();
                foreach (var gamedb in gamedbs)
                {
                    Game game = new Game();
                    game.Id = gamedb.Id;
                    game.LeftTeam = Teams.Where(t => t.Id == gamedb.LeftTeamId).FirstOrDefault();
                    game.RightTeam = Teams.Where(t => t.Id == gamedb.RightTeamId).FirstOrDefault();
                    game.LeftTeamScore = gamedb.LeftTeamScore;
                    game.RightTeamScore = gamedb.RightTeamScore;
                    game.LeftWins = gamedb.LeftWins;
                    game.HasWinner = gamedb.HasWinner;
                    if (game.HasWinner)
                    {
                        if (game.LeftWins) game.Winner = game.LeftTeam.NameDisplay;
                        else game.Winner = game.RightTeam.NameDisplay;
                    }
                    Games.Add(game);
                }

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