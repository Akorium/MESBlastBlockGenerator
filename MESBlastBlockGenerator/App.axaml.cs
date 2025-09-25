using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MESBlastBlockGenerator.Services;
using MESBlastBlockGenerator.ViewModels;


namespace MESBlastBlockGenerator;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            var navigationViewModel = new NavigationViewModel();
            var mainWindow = new MainWindow { DataContext = navigationViewModel };

            desktopLifetime.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}