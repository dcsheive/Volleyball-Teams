using Microsoft.Maui.ApplicationModel;
using System;
using System.Collections.Generic;
using System.Text;
using Volleyball_Teams;
using Volleyball_Teams.Util;

namespace Volleyball_Teams.Util
{
    public static class TheTheme
    {
        public static void SetTheme()
        {
            switch (Settings.Theme)
            {
                case 0:
                    Application.Current.UserAppTheme = AppTheme.Unspecified;
                    break;
                case 1:
                    Application.Current.UserAppTheme = AppTheme.Light;
                    break;
                case 2:
                    Application.Current.UserAppTheme = AppTheme.Dark;
                    break;
            }
        }
    }
}
