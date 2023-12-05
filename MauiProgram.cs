using CommunityToolkit.Maui;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using Microsoft.AppCenter;
using Microsoft.Extensions.Logging;
using Volleyball_Teams.Models;
using Volleyball_Teams.Services;
using Volleyball_Teams.ViewModels;
using Volleyball_Teams.Views;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace Volleyball_Teams
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
#if ANDROID
			    .ConfigureMauiHandlers(handlers => handlers.AddHandler<Microsoft.Maui.Controls.Entry, Volleyball_Teams.Handlers.EntryHandler>())
#endif
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("fa-regular-400.ttf", "FontAwesomeRegular");
                    fonts.AddFont("fa-solid-900.ttf", "FontAwesomeSolid");
                    fonts.AddFont("fa-brands-400.ttf", "FontAwesomeBrands");
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-SemiBold.ttf", "OpenSansSemiBold");
                });

#if DEBUG
		builder.Logging.AddDebug();
		builder.Logging.SetMinimumLevel(LogLevel.Debug);
#endif
            builder.Services.AddSingleton<IPlayerStore, PlayerStore>();
            builder.Services.AddSingleton<ITeamStore, TeamStore>();
            builder.Services.AddSingleton<IGlobalVariables, GlobalVariables>();
            builder.Services.AddScoped<PlayersViewModel>();
            builder.Services.AddScoped<PlayersPage>();
            builder.Services.AddScoped<TeamsViewModel>();
            builder.Services.AddScoped<TeamsPage>();
            builder.Services.AddScoped<SavedTeamsViewModel>();
            builder.Services.AddScoped<SavedTeamsPage>();
            builder.Services.AddScoped<NewPlayerViewModel>();
            builder.Services.AddScoped<NewPlayerPage>();
            builder.Services.AddScoped<SettingsViewModel>();
            builder.Services.AddScoped<SettingsPage>();
            AppCenter.Start("android=6099ddb4-fc7d-44ba-aa25-329316ae8dee;" +
                  "uwp={Your UWP App secret here};" +
                  "ios={Your iOS App secret here};" +
                  "macos={Your macOS App secret here};",
                  typeof(Analytics), typeof(Crashes));
            return builder.Build();
        }
    }
}