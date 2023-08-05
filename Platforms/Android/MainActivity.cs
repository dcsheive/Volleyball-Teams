using Android;
using Android.App;
using Android.Content.PM;
using Android.OS;
using AndroidX.Core.Content;

namespace Volleyball_Teams
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    {

        protected override void OnCreate(Bundle? savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            if (ContextCompat.CheckSelfPermission(this, Manifest.Permission.WriteExternalStorage) != (int)Permission.Granted)
            {
                RequestPermissions(new string[] { Manifest.Permission.WriteExternalStorage, Manifest.Permission.ReadExternalStorage }, 0);
            }
        }
    }
}