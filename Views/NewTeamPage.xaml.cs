using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Xaml;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Volleyball_Teams.Models;
using Volleyball_Teams.ViewModels;

namespace Volleyball_Teams.Views
{
    public partial class NewTeamPage : ContentPage
    {
        NewTeamViewModel _viewModel;
        public NewTeamPage(NewTeamViewModel viewModel)
        {
            InitializeComponent();
            if (viewModel == null) { throw new ArgumentNullException(nameof(viewModel)); }
            BindingContext = _viewModel = viewModel;
            Loaded += Page_Loaded;
            Unloaded += Page_Unloaded;

        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            _viewModel.OnAppearing();
        }

        private void Page_Loaded(object sender, EventArgs e)
        {
            NameEntry.IsEnabled = true;
            _ = Task.Delay(200).ContinueWith(t =>
            {
                NameEntry.Focus();
            });
        }

        private void Page_Unloaded(object sender, EventArgs e)
        {
            NameEntry.IsEnabled = false;
            NameEntry.IsEnabled = true;
        }
    }
}