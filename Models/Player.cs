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
        public bool IsHere { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}