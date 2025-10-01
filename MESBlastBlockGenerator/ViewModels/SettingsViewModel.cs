using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MESBlastBlockGenerator.Helpers;
using MESBlastBlockGenerator.Models.Settings;
using System.ComponentModel.DataAnnotations;

namespace MESBlastBlockGenerator.ViewModels
{
    public partial class SettingsViewModel(AppSettings settings) : ObservableValidator
    {
        private readonly AppSettings _settings = settings;

        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [Url(ErrorMessage = "Неверный формат URL")]
        private string _endpointUrl = settings.SoapClient.EndpointUrl;

        [RelayCommand]
        private void SaveSettings()
        {
            SettingsManager.SaveAppSettings(_settings);
        }

        partial void OnEndpointUrlChanged(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _settings.SoapClient.EndpointUrl = value;
            }
        }
    }
}
