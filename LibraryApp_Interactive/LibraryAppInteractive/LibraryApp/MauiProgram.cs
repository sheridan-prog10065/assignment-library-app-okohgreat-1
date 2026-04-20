using System.Collections.Immutable;

namespace LibraryAppInteractive;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            })
            .Services
            .AddSingleton<Library>()
            .AddSingleton<LibraryBrowsePage>()
            .AddSingleton<LibraryAdminPage>()
            .AddSingleton<AppShell>();;
        
        return builder.Build();
    }
}