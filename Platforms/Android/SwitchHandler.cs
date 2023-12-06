using Android.Graphics.Drawables;
using Android.Views.InputMethods;
using AndroidX.AppCompat.Widget;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls.PlatformConfiguration;
using Microsoft.Maui.Platform;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Volleyball_Teams.Handlers
{
    public partial class SwitchHandler : Microsoft.Maui.Handlers.SwitchHandler
    {

        protected override void ConnectHandler(SwitchCompat platformView)
        {
            base.ConnectHandler(platformView);
            platformView.TrackTintList = Android.Content.Res.ColorStateList.ValueOf(Colors.Gray.ToAndroid());

        }

        protected override void DisconnectHandler(SwitchCompat platformView)
        {
            base.DisconnectHandler(platformView);
        }
    }
}
