using Avalonia;
using Material.Styles.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MESBlastBlockGenerator.Services;
using NLog;
using Avalonia.Controls;

namespace MESBlastBlockGenerator;

public partial class App : Application
{
    private readonly NLog.Logger _logger = LogManager.GetCurrentClassLogger();
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktopLifetime)
        {
            var mainWindow = new MainWindow();
            var xmlService = new XmlGenerationService();
            var viewModel = new MainWindowViewModel(xmlService, mainWindow);
            mainWindow.DataContext = viewModel;

            desktopLifetime.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}