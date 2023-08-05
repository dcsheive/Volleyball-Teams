using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Volleyball_Teams.Views;

namespace Volleyball_Teams.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [RelayCommand]
        private async void Login(object obj)
        {
            // Prefixing with `//` switches to a different navigation stack instead of pushing to the active one
            await Shell.Current.GoToAsync($"//{nameof(AboutPage)}");
        }
    }
}
