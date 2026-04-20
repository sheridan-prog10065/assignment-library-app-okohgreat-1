namespace LibraryAppInteractive;

public partial class App : Application
{
    public static IServiceProvider ServiceProvider { get; private set; }

    public App()
    {
        InitializeComponent();
        MainPage = new AppShell();
    }
    protected override void OnStart()
    {
        base.OnStart();
        ServiceProvider = MauiProgram.CreateMauiApp().Services;
    }
}