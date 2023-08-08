using System.Diagnostics;
using Volleyball_Teams.Models;
using Volleyball_Teams.ViewModels;
using Volleyball_Teams.Views;

namespace Volleyball_Teams.Views
{
    public partial class PlayersPage : ContentPage
    {
        PlayersViewModel _viewModel;

        public PlayersPage(PlayersViewModel viewModel)
        {
            InitializeComponent();

            BindingContext = _viewModel = viewModel;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }
    }
}