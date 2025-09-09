using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using MESBlastBlockGenerator.Services;

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
            var mainWindow = new MainWindow();
            var serializationService = new XmlSerializationService();
            var xmlGenerationService = new XmlGenerationService(serializationService);
            var soapClientService = new SoapClientService(serializationService);
            var viewModel = new MainWindowViewModel(xmlGenerationService, soapClientService, mainWindow);
            mainWindow.DataContext = viewModel;

            desktopLifetime.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }
}