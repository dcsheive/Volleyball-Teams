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
    public partial class GameViewModel : ObservableObject
    {
        ILogger<GameViewModel> logger;
        readonly IPlayerStore playerStore;
        readonly ITeamStore teamStore;
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
        private string loadText = Constants.Loading.LoadingTeams;

        public GameViewModel(IPlayerStore playerStore, ITeamStore teamStore, ILogger<GameViewModel> logger, IGlobalVariables globalVariables)
        {
            this.playerStore = playerStore;
            this.teamStore = teamStore;
            this.logger = logger;
            this.globalVariables = globalVariables;
            IsBusy = false;
            DidNotFinishLoading = true;
            Title = Constants.Title.Game;
            LeftScore = 0;
            RightScore = 0;
            LeftTeam = globalVariables.LeftTeam;
            RightTeam = globalVariables.RightTeam;
        }

        public void OnAppearing()
        {
            IsLoading = false;
            DidNotFinishLoading = true;
            UseRank = Settings.UseRank;
        }

        [RelayCommand]
        private void MinusLeftScore()
        {
            if (LeftScore > 0)
                LeftScore--;
        }

        [RelayCommand]
        private void AddLeftScore()
        {
            if (LeftScore == 20)
            {
                WinGame();
            }
            LeftScore++;
        }

        [RelayCommand]
        private void MinusRightScore()
        {
            if (RightScore > 0)
                RightScore--;
        }

        [RelayCommand]
        private void AddRightScore()
        {
            if (RightScore == 20)
            {
                WinGame();
            }
            RightScore++;
        }

        private void WinGame()
        {

        }

        [RelayCommand]
        private async Task OpenHistory(object obj)
        {
            IsLoading = true;
            await Shell.Current.GoToAsync($"{nameof(HistoryPage)}");
            IsLoading = false;
        }

        [RelayCommand]
        private async Task SaveTeams()
        {            
            await Application.Current.MainPage.DisplayAlert("Confirmation", "", "OK");
        }
    }
}