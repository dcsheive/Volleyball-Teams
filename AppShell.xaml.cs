using System.Diagnostics;
using Volleyball_Teams.ViewModels;
using Volleyball_Teams.Views;

namespace Volleyball_Teams
{
    public partial class AppShell : Shell
    {
        public static AppShell? CurrentAppShell { get; private set; } = default!;

        public AppShell()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
            CurrentAppShell = this;
        }

        /// <summary>
        /// Logout
        /// </summary>
        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            Debug.WriteLine("AppShell: Logout");

            await Current.GoToAsync("//LoginPage");
        }

        protected override void OnNavigating(ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);

            if (args.Current != null)
            {
                Debug.WriteLine($"AppShell: source={args.Current.Location}, target={args.Target.Location}");
            }
        }
    }
}