using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using System;
using System.ComponentModel;
using Volleyball_Teams.ViewModels;

namespace Volleyball_Teams.Views
{
    public partial class GamePage : ContentPage
    {
        GameViewModel _viewModel;

        public GamePage(GameViewModel viewModel)
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