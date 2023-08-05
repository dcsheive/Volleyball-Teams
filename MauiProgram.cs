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
            builder.Services.AddSingleton<IDataStore<Player>, MockDataStore>();
            builder.Services.AddScoped<ItemsViewModel>();
            builder.Services.AddScoped<ItemsPage>();
            builder.Services.AddScoped<AboutViewModel>();
            builder.Services.AddScoped<AboutPage>();
            builder.Services.AddScoped<NewItemViewModel>();
            builder.Services.AddScoped<NewItemPage>();

            return builder.Build();
        }
    }
}