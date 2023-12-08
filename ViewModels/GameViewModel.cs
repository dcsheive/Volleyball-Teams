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

        private bool IsGameOver;
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
        }

        public void OnAppearing()
        {
            IsLoading = false;
            DidNotFinishLoading = true;
            UseRank = Settings.UseRank;
            LeftTeam = globalVariables.LeftTeam;
            RightTeam = globalVariables.RightTeam;
            IsGameOver = false;
            if (LeftTeam == null || RightTeam == null)
            {
                IsLoading = true;
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

        private async Task EndGame(bool leftWins)
        {
            
        }
    }
}