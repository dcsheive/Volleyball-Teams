using Volleyball_Teams.ViewModels;

namespace Volleyball_Teams.Views
{
    public partial class SettingsPage : ContentPage
    {
        SettingsViewModel _viewModel;
        public SettingsPage(SettingsViewModel viewModel)
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