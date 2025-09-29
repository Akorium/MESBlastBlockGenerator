using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MESBlastBlockGenerator.Helpers;
using MESBlastBlockGenerator.Models;
using MESBlastBlockGenerator.Models.Settings;
using MESBlastBlockGenerator.Services;
using MESBlastBlockGenerator.Services.Interfaces;
using MESBlastBlockGenerator.Views;
using System.Reflection;

namespace MESBlastBlockGenerator.ViewModels
{
    public partial class NavigationViewModel : ObservableObject
    {
        private static readonly IXmlSerializationService _serializationService = new XmlSerializationService();
        private static readonly ICoordinateCalculatorService _coordinateCalculatorService = new CoordinateCalculatorService();
        private static readonly IXmlGenerationService _xmlGenerationService = new XmlGenerationService(_serializationService, _coordinateCalculatorService);
        private static readonly AppSettings _appSettings = SettingsManager.LoadAppSettings();
        private static readonly ISoapClientService _soapClientService = new SoapClientService(_serializationService, _appSettings.SoapClient.EndpointUrl);
        private static readonly InputParameters _inputParameters = SettingsManager.LoadUserInputs();
        [ObservableProperty]
        private UserControl _currentView = new MainView();

        [ObservableProperty]
        private bool _isSidePanelOpen = false;
        [ObservableProperty]
        private string _title = GetTitle();

        [RelayCommand]
        private void ToggleSidePanel()
        {
            IsSidePanelOpen = !IsSidePanelOpen;
        }

        [RelayCommand]
        private void NavigateToMESGenerator()
        {
            var mesGeneratorViewModel = new MESGeneratorViewModel(_xmlGenerationService, _soapClientService, _inputParameters);
            var mesGeneratorView = new MESGeneratorView { DataContext = mesGeneratorViewModel };

            CurrentView = mesGeneratorView;
            IsSidePanelOpen = false;
        }
        [RelayCommand]
        private void NavigateToGeomixGenerator()
        {
            var geomixGeneratorViewModel = new GeomixGeneratorViewModel(_xmlGenerationService, _inputParameters);
            var geomixGeneratorView = new GeomixGeneratorView { DataContext = geomixGeneratorViewModel };

            CurrentView = geomixGeneratorView;
            IsSidePanelOpen = false;
        }
        private static string GetTitle()
        {
            var version = Assembly.GetEntryAssembly()?.GetName().Version;
            return $"BlockGenerator v{version?.Major}.{version?.Minor}.{version?.Build}";
        }
    }
}