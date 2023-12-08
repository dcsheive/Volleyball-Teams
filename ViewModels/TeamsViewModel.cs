using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Sentry;
using System.Collections.ObjectModel;
using Volleyball_Teams.Models;
using Volleyball_Teams.Services;
using Volleyball_Teams.Util;
using Volleyball_Teams.Views;
using Constants = Volleyball_Teams.Util.Constants;

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
        private ObservableCollection<Team> leftTeams;

        [ObservableProperty]
        private ObservableCollection<Team> rightTeams;

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
        private string sortText;

        [ObservableProperty]
        private bool useRank;

        [ObservableProperty]
        private bool didNotFinishLoading;

        [ObservableProperty]
        private bool isLoading;

        [ObservableProperty]
        private string loadText = Constants.Loading.LoadingTeams;

        public int NumTeams
        {
            get => Teams.Count;
        }

        private bool IsBusy;
        private bool DoNotRunHere;
        private bool DoNotRunAllHere;
        private List<Player> Players;
        private bool disableSelect;
        public TeamsViewModel(IPlayerStore playerStore, ITeamStore teamStore, ILogger<TeamsViewModel> logger, IGlobalVariables globalVariables)
        {
            this.playerStore = playerStore;
            this.teamStore = teamStore;
            this.logger = logger;
            this.globalVariables = globalVariables;
            Title = Constants.Title.Teams;
            IsBusy = false;
            DidNotFinishLoading = true;
            Players = new List<Player>();
            Teams = new ObservableCollection<Team>();
            SortText = Settings.TeamsSortBy;
            if (string.IsNullOrEmpty(SortText))
            {
                SortText = Constants.Settings.SortByName;
            }
        }

        public void OnAppearing()
        {
            UseRank = Settings.UseRank;
            DidNotFinishLoading = true;
            LoadTeams();
        }

        [RelayCommand]
        private async Task OnAddItem(object obj)
        {
            await Shell.Current.GoToAsync($"{nameof(NewTeamPage)}?ID=");
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
        private async Task Play()
        {
            if (LeftTeam == null || RightTeam == null) return;
            bool result = await Application.Current.MainPage.DisplayAlert("Confirmation", $"{LeftTeam.NameDisplay}\n\tVS\n{RightTeam.NameDisplay}", "OK", "Cancel");
            if (result)
            {
                globalVariables.LeftTeam = LeftTeam;
                globalVariables.RightTeam = RightTeam;
                await Shell.Current.GoToAsync(nameof(GamePage));
            }
        }

        [RelayCommand]
        private void Sort()
        {
            Task.Run(() => DoSort());
        }
        private void DoSort()
        {
            if (IsBusy) return;
            IsBusy = true;
            logger.LogDebug($"IsBusy={IsBusy}");
            try
            {
                if (SortText == Constants.Settings.SortByName)
                {
                    SortText = Constants.Settings.SortByPower;
                    logger.LogDebug($"Sort = {SortText}");
                    SortByPower();
                    Settings.TeamsSortBy = Constants.Settings.SortByPower;
                }
                else if (SortText == Constants.Settings.SortByPower)
                {
                    SortText = Constants.Settings.SortByWins;
                    logger.LogDebug($"Sort = {SortText}");
                    SortByWins();
                    Settings.TeamsSortBy = Constants.Settings.SortByWins;
                }
                else if (SortText == Constants.Settings.SortByWins)
                {
                    SortText = Constants.Settings.SortByRatio;
                    logger.LogDebug($"Sort = {SortText}");
                    SortByRatio();
                    Settings.TeamsSortBy = Constants.Settings.SortByRatio;
                }
                else if (SortText == Constants.Settings.SortByRatio)
                {
                    SortText = Constants.Settings.SortByLoss;
                    logger.LogDebug($"Sort = {SortText}");
                    SortByLosses();
                    Settings.TeamsSortBy = Constants.Settings.SortByLoss;
                }
                else
                {
                    SortText = Constants.Settings.SortByName;
                    logger.LogDebug($"Sort = {SortText}");
                    SortByName();
                    Settings.TeamsSortBy = Constants.Settings.SortByName;
                }
            }
            catch (Exception ex)
            {
                logger.LogError("{ex}", ex);
            }
            finally
            {
                IsBusy = false;
                logger.LogDebug($"IsBusy = {IsBusy}");
            }
        }

        [RelayCommand]
        private async Task Edit(object obj)
        {
            Team p = (Team)obj;
            logger.LogDebug($"edit team is {p.Name}");
            await Shell.Current.GoToAsync($"{nameof(NewTeamPage)}?ID={p.Id}");
        }

        [RelayCommand]
        private void LoadTeams()
        {
            Task.Run(() => DoLoadTeams());
        }
        private async Task DoLoadTeams()
        {
            if (IsBusy) return;
            IsBusy = true;
            Teams.Clear();
            logger.LogDebug($"IsBusy = {IsBusy}");
            logger.LogDebug($"Run DoLoadTeams");
            try
            {
                Players.Clear();
                Players = await playerStore.GetPlayersAsync();
                var teamdbs = await teamStore.GetTeamsAsync();
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
                logger.LogDebug($"Sort = {SortText}");
                if (SortText == Constants.Settings.SortByName) { SortByName(); }
                else if (SortText == Constants.Settings.SortByPower) { SortByPower(); }
                else if (SortText == Constants.Settings.SortByWins) { SortByWins(); }
                else if (SortText == Constants.Settings.SortByRatio) { SortByRatio(); }
                else { SortByLosses(); }
                ResetTeamOptions();
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

        private async void ResetTeamOptions()
        {
            OnPropertyChanged(nameof(AtLeast2Teams));
            if (NumTeams > 1)
            {
                disableSelect = true;
                LeftTeams = new ObservableCollection<Team>(Teams.ToList());
                RightTeams = new ObservableCollection<Team>(Teams.ToList());
                await Task.Delay(200);
                LeftTeam = LeftTeams[0];
                RightTeam = RightTeams[1];
                disableSelect = false;
            }
        }

        private void SortByName() => Teams = new ObservableCollection<Team>(Teams.ToList().OrderBy(p => p.Name));
        private void SortByWins() => Teams = new ObservableCollection<Team>(Teams.ToList().OrderByDescending(p => p.NumWins));

        private void SortByRatio() => Teams = new ObservableCollection<Team>(Teams.ToList().OrderByDescending(p =>
        {
            if (p.NumLosses == 0) return p.NumWins;
            else if (p.NumWins == 0) return -1 * p.NumLosses;
            else return p.NumWins / p.NumLosses;
        }));

        private void SortByLosses() => Teams = new ObservableCollection<Team>(Teams.ToList().OrderByDescending(p => p.NumLosses));
        private void SortByPower() => Teams = new ObservableCollection<Team>(Teams.ToList().OrderByDescending(p => p.Power));
    }
}