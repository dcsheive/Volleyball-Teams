using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Volleyball_Teams.Util
{
    public static class Constants
    {
#if ANDROID
        public const string DatabaseFilename = $"{nameof(Volleyball_Teams)}v1.db3";
#else
        public const string DatabaseFilename = "SQLite.db3";
#endif

        public const SQLite.SQLiteOpenFlags Flags =
            // open the database in read/write mode
            SQLite.SQLiteOpenFlags.ReadWrite |
            // create the database if it doesn't exist
            SQLite.SQLiteOpenFlags.Create |
            // enable multi-threaded database access
            SQLite.SQLiteOpenFlags.SharedCache;

        public static string DatabasePath =>
             Path.Combine(FileSystem.AppDataDirectory, DatabaseFilename);

        public static Random rng = new Random();

        public static void Shuffle<T>(this IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
        public class Title
        {
            public const string NewPlayer = "New Player";
            public const string NewTeam = "New Team";
            public const string Players = "Players";
            public const string RandomTeams = "Random Teams";
            public const string Teams = "Teams";
            public const string Game = "Game";
            public const string History = "History";
            public const string Settings = "Settings";
        }

        public class Loading
        {
            public const string SelectGame = "Replaying...";
            public const string LoadingHistory = "Loading History...";
            public const string LoadingTeams = "Loading Teams...";
            public const string DeleteGame = "Deleting...";
            public const string GameOver = "Game Over.";
            public const string GameMessage = "No teams are selected.\nSelect teams or \nreplay a game from history.";
        }

        public class Settings
        {
            public const string SortByName = "Sort By: Name";
            public const string SortByRank = "Sort By: Rank";
            public const string SortByWins = "Sort By: Wins";
            public const string SortByRatio = "Sort By: Ratio";
            public const string SortByLoss = "Sort By: Losses";
            public const string SortByPower = "Sort By: Power";
            public const string AllHere = "All Here";
            public const string NoneHere = "None Here";
        }
    }
}
