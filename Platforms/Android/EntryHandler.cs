﻿using Android.Graphics.Drawables;
using Android.Views.InputMethods;
using AndroidX.AppCompat.Widget;
using Microsoft.Maui.Controls.Compatibility.Platform.Android;
using Microsoft.Maui.Controls.PlatformConfiguration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Volleyball_Teams.Handlers
{
    public partial class EntryHandler : Microsoft.Maui.Handlers.EntryHandler
    {
        protected override void ConnectHandler(AppCompatEditText platformView)
        {
            base.ConnectHandler(platformView);
            platformView.FocusChange += PlatformView_FocusChange;
            platformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Colors.Gray.ToAndroid());
        }

        protected override void DisconnectHandler(AppCompatEditText platformView)
        {
            base.DisconnectHandler(platformView);
            platformView.FocusChange -= PlatformView_FocusChange;
        }

        private void PlatformView_FocusChange(object sender, Android.Views.View.FocusChangeEventArgs args)
        {
            var platformView = (AppCompatEditText)sender;
            if (args.HasFocus)
            {
                platformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Color.FromRgb(0x51, 0x2B, 0xD4).ToAndroid());
                InputMethodManager inputMethodManager = (InputMethodManager)global::Android.App.Application.Context.GetSystemService(global::Android.Content.Context.InputMethodService);
                inputMethodManager.ShowSoftInput(PlatformView, ShowFlags.Forced);
            }
            else
            {
                platformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Colors.Gray.ToAndroid());
                InputMethodManager inputMethodManager = (InputMethodManager)global::Android.App.Application.Context.GetSystemService(global::Android.Content.Context.InputMethodService);
                inputMethodManager.HideSoftInputFromWindow(PlatformView.WindowToken, HideSoftInputFlags.None);
            }
        }
    }
}
