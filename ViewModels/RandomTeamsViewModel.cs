using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using System.Text;
using Volleyball_Teams.Models;
using Volleyball_Teams.Services;
using Volleyball_Teams.Util;
using Volleyball_Teams.Views;
using Xamarin.CommunityToolkit.ObjectModel;

namespace Volleyball_Teams.ViewModels
{
    public partial class RandomTeamsViewModel : ObservableObject
    {
        ILogger<RandomTeamsViewModel> logger;
        readonly IPlayerStore playerStore;
        readonly ITeamStore teamStore;
        readonly IGlobalVariables globalVariables;

        [ObservableProperty]
        private ObservableRangeCollection<Team> teams;

        [ObservableProperty]
        private ObservableRangeCollection<Team> leftTeams;

        [ObservableProperty]
        private ObservableRangeCollection<Team> rightTeams;

        public bool AtLeast2Teams
        {
            get => Teams.Count > 1;
        }

        [ObservableProperty]
        private Team rightTeam;

        [ObservableProperty]
        private Team leftTeam;

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

        public RandomTeamsViewModel(IPlayerStore playerStore, ITeamStore teamStore, ILogger<RandomTeamsViewModel> logger, IGlobalVariables globalVariables)
        {
            this.playerStore = playerStore;
            this.teamStore = teamStore;
            this.logger = logger;
            this.globalVariables = globalVariables;
            IsBusy = false;
            DidNotFinishLoading = true;
            Title = Constants.Title.RandomTeams;
            Teams = new ObservableRangeCollection<Team>();
            LeftTeams = new ObservableRangeCollection<Team>();
            RightTeams = new ObservableRangeCollection<Team>();
            Players = new List<Player>();
            NumTeams = Settings.NumTeams;
            OnPropertyChanged(nameof(AtLeast2Teams));
        }

        public void OnAppearing()
        {
            IsLoading = false;
            DidNotFinishLoading = true;
            UseRank = Settings.UseRank;
            if (Players.Count == 0) LoadPlayers();
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
            OnPropertyChanged(nameof(Teams));
            OnPropertyChanged(nameof(LeftTeams));
            OnPropertyChanged(nameof(RightTeams));
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
            OnPropertyChanged(nameof(Teams));
            OnPropertyChanged(nameof(LeftTeams));
            OnPropertyChanged(nameof(RightTeams));
        }

        [RelayCommand]
        private void SetOppositeTeam(string option)
        {
            if (disableSelect) return;
            if (LeftTeam == RightTeam || RightTeam == null || LeftTeam == null)
            {
                if (option == "left")
                {
                    RightTeam = RightTeams.Where(e => e != LeftTeam).First();
                }
                else
                {
                    LeftTeam = LeftTeams.Where(e => e != RightTeam).First();
                }
            }
        }

        [RelayCommand]
        public void ItemDragged(Player player)
        {
            logger.LogDebug($"ItemDragged : {player.Name}");
            player.IsBeingDragged = true;
            playerDragged = player;
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
                        ResetTeamOptions();
                    }
                    playerToMove.Team = playerToInsertBefore.Team;
                    playerToMove.IsBeingDragged = false;
                    playerToInsertBefore.IsBeingDraggedOver = false;
                }
                logger.LogDebug($"ItemDropped: [{playerToMove?.Name}] => [{playerToInsertBefore?.Name}], target index = [{insertAtIndex}]");
                var teams = Teams.ToList();
                Teams.ReplaceRange(teams);
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
                ResetTeamOptions();
                OnPropertyChanged(nameof(Teams));
                OnPropertyChanged(nameof(LeftTeams));
                OnPropertyChanged(nameof(RightTeams));
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
        private async Task Play()
        {
            if (LeftTeam == null || RightTeam == null) return;
            bool result = await Application.Current.MainPage.DisplayAlert("Confirmation", $"{LeftTeam.NameDisplay}\n\tVS\n{RightTeam.NameDisplay}", "OK", "Cancel");
            if (result)
            {
                globalVariables.LeftTeam = LeftTeam;
                globalVariables.RightTeam = RightTeam;
                globalVariables.LeftScore = 0;
                globalVariables.RightScore = 0;
                globalVariables.NewGame = true;
                await Shell.Current.GoToAsync($"///{nameof(GamePage)}");
            }
        }

        [RelayCommand]
        private async Task EditName(Team t)
        {
            string response = await Application.Current.MainPage.DisplayPromptAsync("Edit Team Name", "What should this team be called?", "OK", "Cancel", t.NameDisplay, -1, null, t.NameDisplay);
            if (string.IsNullOrEmpty(response)) return;
            t.Name = response;
            ResetTeamOptions();
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

        private void ResetTeamOptions()
        {
            OnPropertyChanged(nameof(AtLeast2Teams));
            if (NumTeams > 1)
            {
                disableSelect = true;
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    LeftTeams.ReplaceRange(Teams.ToList());
                    RightTeams.ReplaceRange(Teams.ToList());
                    UpdateTeamSelection();
                });
                disableSelect = false;
            }
        }

        private async Task UpdateTeamSelection()
        {
            LeftTeam = LeftTeams[0];
            RightTeam = RightTeams[1];
            OnPropertyChanged(nameof(LeftTeam));
            OnPropertyChanged(nameof(RightTeam));
        }

        private Team[] MakeTeams()
        {
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
            MainThread.BeginInvokeOnMainThread(() => { Teams.ReplaceRange(teams); });
            OnPropertyChanged(nameof(Teams));
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
            MainThread.BeginInvokeOnMainThread(() => { 
                Teams.ReplaceRange(teams); 
            });
            OnPropertyChanged(nameof(Teams));

        }
    }
}