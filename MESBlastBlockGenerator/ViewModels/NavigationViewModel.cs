using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MESBlastBlockGenerator.Services;
using MESBlastBlockGenerator.Services.Interfaces;
using MESBlastBlockGenerator.Views;
using System.Reflection;

namespace MESBlastBlockGenerator.ViewModels
{
    public partial class NavigationViewModel : ObservableObject
    {
        private static readonly IXmlSerializationService _serializationService = new XmlSerializationService();
        private static readonly IXmlGenerationService _xmlGenerationService = new XmlGenerationService(_serializationService);
        private static readonly ISoapClientService _soapClientService = new SoapClientService(_serializationService);
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
            var mesGeneratorViewModel = new MESGeneratorViewModel(_xmlGenerationService, _soapClientService);
            var mesGeneratorView = new MESGeneratorView { DataContext = mesGeneratorViewModel };

            CurrentView = mesGeneratorView;
            IsSidePanelOpen = false;
        }
        [RelayCommand]
        private void NavigateToGeomixGenerator()
        {
            var geomixGeneratorViewModel = new GeomixGeneratorViewModel(_xmlGenerationService);
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