using SQLite;
using Volleyball_Teams.Services;
using Volleyball_Teams.Util;
using Volleyball_Teams.Views;

namespace Volleyball_Teams
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            TheTheme.SetTheme();
            MainPage = new AppShell();
        }

        protected override void OnSleep()
        {
            TheTheme.SetTheme();
            RequestedThemeChanged -= App_SetRequestedTheme;
        }

        protected override void OnResume()
        {
            TheTheme.SetTheme();
            RequestedThemeChanged += App_SetRequestedTheme;
        }

        private void App_SetRequestedTheme(object sender, AppThemeChangedEventArgs e)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                TheTheme.SetTheme();
            });
        }
    }
}