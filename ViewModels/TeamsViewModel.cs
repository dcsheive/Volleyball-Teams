﻿using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using Volleyball_Teams.Models;
using Volleyball_Teams.Services;
using Volleyball_Teams.Util;
using Volleyball_Teams.Views;
using Xamarin.CommunityToolkit.ObjectModel;
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
            Teams = new ObservableRangeCollection<Team>();
            LeftTeams = new ObservableRangeCollection<Team>();
            RightTeams = new ObservableRangeCollection<Team>();
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
            HashSet<Player> players = new HashSet<Player>();
            foreach (Player player in LeftTeam)
            {
                players.Add(player);
            }
            foreach (Player player in RightTeam)
            {
                if (players.Contains(player))
                {
                    await Application.Current.MainPage.DisplayAlert("Failed", $"The teams selected have 1 or more mutual players.", "OK");
                    return;
                }
            }
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
            logger.LogDebug($"IsBusy = {IsBusy}");
            logger.LogDebug($"Run DoLoadTeams");
            List<Team> newTeams = new();
            try
            {
                Players = await playerStore.GetPlayersAsync();
                var teamdbs = await teamStore.GetTeamsAsync();
                int count = 0;
                foreach (var teamdb in teamdbs)
                {
                    if (teamdb.IsRandom) continue;
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
                    newTeams.Add(team);
                }
                await MainThread.InvokeOnMainThreadAsync(() => { Teams.ReplaceRange(newTeams); });
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

        private void SortByName() => MainThread.BeginInvokeOnMainThread(() => { Teams.ReplaceRange(Teams.ToList().OrderBy(p => p.Name)); });
        private void SortByWins() => MainThread.BeginInvokeOnMainThread(() => { Teams.ReplaceRange(Teams.ToList().OrderByDescending(p => p.NumWins)); });
        private void SortByRatio() => MainThread.BeginInvokeOnMainThread(() =>
        {
            Teams.ReplaceRange(Teams.ToList().OrderByDescending(p =>
            {
                if (p.NumLosses == 0) return p.NumWins;
                else if (p.NumWins == 0) return -1 * p.NumLosses;
                else return p.NumWins / p.NumLosses;
            }));
        });

        private void SortByLosses() => MainThread.BeginInvokeOnMainThread(() => { Teams.ReplaceRange(Teams.ToList().OrderByDescending(p => p.NumLosses)); });
        private void SortByPower() => MainThread.BeginInvokeOnMainThread(() => { Teams.ReplaceRange(Teams.ToList().OrderByDescending(p => p.Power)); });
    }
}