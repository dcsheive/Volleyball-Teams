using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Microsoft.Maui.Controls;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Volleyball_Teams.Models;
using Volleyball_Teams.Services;
using Volleyball_Teams.Util;

namespace Volleyball_Teams.ViewModels
{
    public partial class TeamsViewModel : ObservableObject
    {

        readonly IDataStore<Player> dataStore;
        ILogger<PlayersViewModel> logger;

        public ObservableCollection<Team> Teams { get; private set; }
        private List<Player> Players;

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

        public TeamsViewModel(IDataStore<Player> dataStore, ILogger<PlayersViewModel> logger)
        {
            this.dataStore = dataStore;
            this.logger = logger;
            Title = "Teams";
            Teams = new ObservableCollection<Team>();
            Players = new List<Player>();
            IsBusy = false;
            NumTeams = Preferences.Get(Constants.Settings.NumTeams, 2);
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
        private  void DoRemoveTeam()
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
        public void LoadPlayers(string pageLoad = "false")
        {
            Task.Run(async() => await DoLoadPlayers(pageLoad));
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
                    var items = await dataStore.GetItemsHereAsync(true);
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
            }
            catch (Exception ex)
            {
                logger.LogError("{ex}", ex);
            }
            finally
            {
                DidNotFinishLoading = false;
                IsBusy = false;
                IsRefreshing = false;
                logger.LogDebug("Set IsBusy to false");
            }
        }

        private void SortRandom()
        {
            Teams.Clear();
            Constants.Shuffle(Players);
            if (NumTeams > Players.Count) NumTeams = Players.Count;
            Team[] teams = new Team[NumTeams];
            for (int i = 0; i < teams.Length; i++)
            {
                teams[i] = new Team(i, new List<Player>());
            }
            int counter = 0;
            foreach (var player in Players.ToList())
            {
                if (counter == NumTeams) counter = 0;
                Team team = teams[counter];
                team.Add(player);
                counter++;
            }
            foreach (var team in teams)
            {
                Teams.Add(team);
            }
        }

        private void SortWithRank()
        {
            Teams.Clear();
            Constants.Shuffle(Players);
            if (NumTeams > Players.Count) NumTeams = Players.Count;
            Team[] teams = new Team[NumTeams];
            for (int i = 0; i < teams.Length; i++)
            {
                teams[i] = new Team(i, new List<Player>());
            }
            foreach (var player in Players.ToList())
            {
                Team smallest = teams[0];
                for (int i = 1; i < teams.Length; i++)
                {
                    Team current = teams[i];
                    if (smallest.Power > current.Power) smallest = current;
                }
                smallest.Add(player);
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
            UseRank = Preferences.Get(Constants.Settings.UseRank, false);
            LoadPlayers("True");
        }
    }
}