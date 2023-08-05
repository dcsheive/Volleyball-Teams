using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using Volleyball_Teams.Models;
using Volleyball_Teams.Services;

namespace Volleyball_Teams.ViewModels
{
    public partial class NewItemViewModel : ObservableObject
    {
        readonly IDataStore<Player>? dataStore;
        ILogger<NewItemViewModel> logger;

        public NewItemViewModel(IDataStore<Player> dataStore, ILogger<NewItemViewModel> logger)
        {
            if (dataStore == null) { throw new ArgumentNullException(nameof(dataStore)); }
            this.dataStore = dataStore;
            this.logger = logger;
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private string? name;

        [RelayCommand]
        private async void Cancel()
        {
            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand(CanExecute = nameof(ValidateSave))]
        private async void Save()
        {
            logger.LogDebug("Save: Name: {name}", Name);
            Player newItem = new()
            {
                Name = Name,
                IsHere = true
            };

            _ = await dataStore.AddItemAsync(newItem);
            Name = string.Empty;

            // This will pop the current page off the navigation stack
            await Shell.Current.GoToAsync("..");
        }

        private bool ValidateSave()
        {
            var canExecute = !String.IsNullOrWhiteSpace(Name);
            logger.LogDebug("ValidateSave: {canExecute}", canExecute);
            return canExecute;
        }
    }
}
