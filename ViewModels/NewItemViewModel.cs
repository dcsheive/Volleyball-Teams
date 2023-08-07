using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using Volleyball_Teams.Models;
using Volleyball_Teams.Services;

namespace Volleyball_Teams.ViewModels
{
    [QueryProperty(nameof(ID), "ID")]
    public partial class NewItemViewModel : ObservableObject
    {
        public string ID { get; set; }

        readonly IDataStore<Player>? dataStore;
        ILogger<NewItemViewModel> logger;
        public ObservableCollection<string> Stars { get; set; }
        public string SelectedStar { get; set; }
        public Player MyPlayer { get; set; }
        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        private string? name;

        [ObservableProperty]
        private bool showDelete;

        public NewItemViewModel(IDataStore<Player> dataStore, ILogger<NewItemViewModel> logger)
        {
            if (dataStore == null) { throw new ArgumentNullException(nameof(dataStore)); }
            this.dataStore = dataStore;
            this.logger = logger;
            Stars = new ObservableCollection<string>();
            Stars.Add("1");
            Stars.Add("2");
            Stars.Add("3");
            Stars.Add("4");
            Stars.Add("5");
        }

        [RelayCommand]
        private async void Cancel()
        {
            await Shell.Current.GoToAsync("..");
        }


        [RelayCommand]
        private async void Delete()
        {
            await dataStore.DeleteItemAsync(MyPlayer);
            await Shell.Current.GoToAsync("..");
        }

        [RelayCommand(CanExecute = nameof(ValidateSave))]
        private async void Save()
        {
            logger.LogDebug("Save: Name: {name}", Name);
            MyPlayer.Name = Name;
            MyPlayer.NumStars = SelectedStar;
            if (string.IsNullOrEmpty(ID))
                _ = await dataStore.AddItemAsync(MyPlayer);
            else
                await dataStore.UpdateItemAsync(MyPlayer);
            await Shell.Current.GoToAsync("..");
        }

        private bool ValidateSave()
        {
            var canExecute = !String.IsNullOrWhiteSpace(Name);
            logger.LogDebug("ValidateSave: {canExecute}", canExecute);
            return canExecute;
        }

        private async Task GetPlayer()
        {
            MyPlayer = await dataStore.GetItemAsync(int.Parse(ID));
            Name = MyPlayer.Name;
            SelectedStar = MyPlayer.NumStars;
        }

        async public void OnAppearing()
        {
            if (!string.IsNullOrEmpty(ID))
            {
                ShowDelete = true;
                await GetPlayer();
            }
            else
            {
                ShowDelete = false;
                MyPlayer = new Player();
                Name = string.Empty;
                SelectedStar = "1";
            }
        }
    }
}
