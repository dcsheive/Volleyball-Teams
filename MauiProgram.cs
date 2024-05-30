using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;
using Volleyball_Teams.Models;
using Volleyball_Teams.Services;
using Volleyball_Teams.ViewModels;
using Volleyball_Teams.Views;

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
			    .ConfigureMauiHandlers(handlers => handlers.AddHandler<Microsoft.Maui.Controls.Picker, Volleyball_Teams.Handlers.PickerHandler>())
			    .ConfigureMauiHandlers(handlers => handlers.AddHandler<Microsoft.Maui.Controls.Switch, Volleyball_Teams.Handlers.SwitchHandler>())
#endif
                .UseSentry(options =>
                {
                    options.Dsn = "https://6e1b57ec572513180fee1e7caf32a1f8@o4506345415245824.ingest.sentry.io/4506345416884224";
                    options.Debug = false;
                    options.TracesSampleRate = 1.0;
                    options.Release = AppInfo.PackageName + AppInfo.Current.VersionString;
                })
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
            builder.Services.AddSingleton<IGameStore, GameStore>();
            builder.Services.AddSingleton<IGlobalVariables, GlobalVariables>();
            builder.Services.AddScoped<PlayersViewModel>();
            builder.Services.AddScoped<PlayersPage>();
            builder.Services.AddScoped<GameViewModel>();
            builder.Services.AddScoped<GamePage>();
            builder.Services.AddScoped<RandomTeamsViewModel>();
            builder.Services.AddScoped<RandomTeamsPage>();
            builder.Services.AddScoped<TeamsViewModel>();
            builder.Services.AddScoped<TeamsPage>();
            builder.Services.AddScoped<HistoryViewModel>();
            builder.Services.AddScoped<HistoryPage>();
            builder.Services.AddScoped<NewPlayerViewModel>();
            builder.Services.AddScoped<NewPlayerPage>(); 
            builder.Services.AddScoped<NewTeamViewModel>();
            builder.Services.AddScoped<NewTeamPage>();
            builder.Services.AddScoped<SettingsViewModel>();
            builder.Services.AddScoped<SettingsPage>();

            return builder.Build();
        }
    }
}