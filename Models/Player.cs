using SQLite;
using System;
using System.ComponentModel;

namespace Volleyball_Teams.Models
{
    public class Player: INotifyPropertyChanged
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public bool IsHere { get; set; } = true;
        public string NumStars { get; set; } = "1";

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}