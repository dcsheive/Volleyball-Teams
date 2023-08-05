using Volleyball_Teams.Services;
using Volleyball_Teams.Views;

namespace Volleyball_Teams
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));

            MainPage = new AppShell();
        }
    }
}