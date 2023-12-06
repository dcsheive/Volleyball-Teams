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
    public partial class PickerHandler : Microsoft.Maui.Handlers.PickerHandler
    {

        protected override void ConnectHandler(MauiPicker platformView)
        {
            base.ConnectHandler(platformView);
            platformView.FocusChange += PlatformView_FocusChange;
            platformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Colors.Gray.ToAndroid());

        }

        protected override void DisconnectHandler(MauiPicker platformView)
        {
            base.DisconnectHandler(platformView);
            platformView.FocusChange -= PlatformView_FocusChange;
        }

        private void PlatformView_FocusChange(object sender, Android.Views.View.FocusChangeEventArgs args)
        {
            var platformView = (MauiPicker)sender;
            if (args.HasFocus)
            {
                platformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Color.FromRgb(0x51, 0x2B, 0xD4).ToAndroid());
            }
            else
            {
                platformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Colors.Gray.ToAndroid());
            }
        }
    }
}
