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
            Routing.RegisterRoute(nameof(NewPlayerPage), typeof(NewPlayerPage));
            CurrentAppShell = this;
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