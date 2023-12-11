using System;
using System.Collections.Generic;
using System.Text;

namespace Volleyball_Teams.Util
{
    public class Settings
    {
        const int theme = 0;
        const bool useRank = true;
        const bool useScore = false;
        const string sortBy = Constants.Settings.SortByName;
        const string teamsSortBy = Constants.Settings.SortByName;
        const int numTeams = 2;

        public static int Theme
        {
            get => Preferences.Get(nameof(Theme), theme);
            set => Preferences.Set(nameof(Theme), value);
        }

        public static bool UseRank
        {
            get => Preferences.Get(nameof(UseRank), useRank);
            set => Preferences.Set(nameof(UseRank), value);
        }

        public static bool UseScore
        {
            get => Preferences.Get(nameof(UseScore), useScore);
            set => Preferences.Set(nameof(UseScore), value);
        }

        public static string SortBy
        {
            get => Preferences.Get(nameof(SortBy), sortBy);
            set => Preferences.Set(nameof(SortBy), value);
        }

        public static string TeamsSortBy
        {
            get => Preferences.Get(nameof(TeamsSortBy), teamsSortBy);
            set => Preferences.Set(nameof(TeamsSortBy), value);
        }

        public static int NumTeams
        {
            get => Preferences.Get(nameof(NumTeams), numTeams);
            set => Preferences.Set(nameof(NumTeams), value);
        }

    }
}
