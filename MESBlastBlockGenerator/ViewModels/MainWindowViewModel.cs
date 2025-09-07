using Avalonia.Controls;
using Avalonia.Input.Platform;
using Avalonia.Media;
using Avalonia.Threading;
using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Styles.Controls;
using Material.Styles.Models;
using MESBlastBlockGenerator.Helpers;
using MESBlastBlockGenerator.Models;
using MESBlastBlockGenerator.Services.Interfaces;
using NLog;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Threading.Tasks;

namespace MESBlastBlockGenerator
{
    public partial class MainWindowViewModel(IXmlGenerationService xmlGenerationService, Window mainWindow) : ObservableValidator
    {
        private readonly IXmlGenerationService _xmlGenerationService = xmlGenerationService;
        private static readonly NLog.Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly Window _mainWindow = mainWindow;
        private readonly SnackbarHost _statusSnackbar = mainWindow.Find<SnackbarHost>("StatusSnackbar");
        private static readonly InputParameters _inputParameters = SettingsManager.LoadSavedInputs();
        private static readonly CultureInfo _culture = CultureInfo.CurrentCulture;

        #region Properties
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[1-9]\d*$", ErrorMessage = "Целое положительное число")]
        private string _maxRow = _inputParameters.MaxRow.ToString(_culture);
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[1-9]\d*$", ErrorMessage = "Целое положительное число")]
        private string _maxCol = _inputParameters.MaxCol.ToString(_culture);
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[0-9]+([.,][0-9]*)?$", ErrorMessage = "Положительное число")]
        private string _rotationAngle = _inputParameters.RotationAngle.ToString(_culture);
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^-?[0-9]+([.,][0-9]*)?$", ErrorMessage = "Число")]
        private string _baseX = _inputParameters.BaseX.ToString(_culture);
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^-?[0-9]+([.,][0-9]*)?$", ErrorMessage = "Число")]
        private string _baseY = _inputParameters.BaseY.ToString(_culture);
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[0-9]+([.,][0-9]*)?$", ErrorMessage = "Положительное число")]
        private string _baseZ = _inputParameters.BaseZ.ToString(_culture);
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[0-9]+([.,][0-9]*)?$", ErrorMessage = "Положительное число")]
        private string _distance = _inputParameters.Distance.ToString(_culture);
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        private string _pitName = _inputParameters.PitName;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[1-9]\d*$", ErrorMessage = "Целое положительное число")]
        private string _level = _inputParameters.Level.ToString(_culture);
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[1-9]\d*$", ErrorMessage = "Целое положительное число")]
        private string _blockNumber = _inputParameters.BlockNumber.ToString(_culture);
        [ObservableProperty]
        private bool _dispersedCharge = _inputParameters.DispersedCharge;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[0-9]+([.,][0-9]*)?$", ErrorMessage = "Положительное число")]
        private string _mainChargeMass = _inputParameters.MainChargeMass.ToString(_culture);
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[0-9]+([.,][0-9]*)?$", ErrorMessage = "Положительное число")]
        private string _secondaryChargeMass = _inputParameters.SecondaryChargeMass.ToString(_culture);
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[0-9]+([.,][0-9]*)?$", ErrorMessage = "Положительное число")]
        private string _designDepth = _inputParameters.DesignDepth.ToString(_culture);
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[0-9]+([.,][0-9]*)?$", ErrorMessage = "Положительное число")]
        private string _realDepth = _inputParameters.RealDepth.ToString(_culture);
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[1-9]\d*$", ErrorMessage = "Целое положительное число")]
        private string _designDiameter = _inputParameters.DesignDiameter.ToString(_culture);
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[1-9]\d*$", ErrorMessage = "Целое положительное число")]
        private string _realDiameter = _inputParameters.RealDiameter.ToString(_culture);
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[0-9]+([.,][0-9]*)?$", ErrorMessage = "Положительное число")]
        private string _coordinatesDeviation = _inputParameters.CoordinatesDeviation.ToString(_culture);
        [ObservableProperty]
        private bool _isDrilling = _inputParameters.IsDrilling;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[0-9]+([.,][0-9]*)?$", ErrorMessage = "Положительное число")]
        private string _stemmingLength = _inputParameters.StemmingLength.ToString(_culture);
        [ObservableProperty]
        private TextDocument _generatedXml = new();
        [ObservableProperty]
        private IBrush _statusColor = Brushes.Green;
        [ObservableProperty]
        private string _title = GetTitle();
        [ObservableProperty]
        private bool _copyButtonEnabled = false;
        #endregion

        [RelayCommand]
        private async Task GenerateXmlAsync()
        {
            _logger.Info("Инициализирована генерация XML");
            ClearStatus();

            try
            {
                ValidateAllProperties();

                if (HasErrors)
                {
                    ShowMessage("Пожалуйста, исправьте ошибки ввода", true);
                    return;
                }

                _logger.Debug($"Попытка генерации XML с maxRow = {_inputParameters.MaxRow}, maxCol = {_inputParameters.MaxCol}, baseX = {_inputParameters.BaseX}, baseY = {_inputParameters.BaseY}, " +
                    $"baseZ = {_inputParameters.BaseZ}, distance = {_inputParameters.Distance}, pitName = {_inputParameters.PitName}, level = {_inputParameters.Level}, blockNumber = {_inputParameters.BlockNumber}, " +
                    $"dispersedCharge = {_inputParameters.DispersedCharge}, mainChargeMass = {_inputParameters.MainChargeMass}" +
                    $"designDepth = {_inputParameters.DesignDepth}, realDepth = {_inputParameters.RealDepth}, {(_inputParameters.DispersedCharge ? $", secondaryChargeMass = {_inputParameters.SecondaryChargeMass}, " : "")} " +
                    $"designDiameter = {_inputParameters.DesignDiameter}, realDiameter = {_inputParameters.RealDiameter}, stemmingLength = {_inputParameters.StemmingLength}");
                string xmlContent = await _xmlGenerationService.GenerateXmlContentAsync(_inputParameters);

                SettingsManager.SaveInputs(_inputParameters);
                GeneratedXml = new TextDocument(xmlContent);
                ShowMessage("XML успешно сгенерирован!");
                CopyButtonEnabled = true;
            }
            catch (Exception ex)
            {
                ShowMessage($"Ошибка генерации: {ex.Message}", true);
            }
        }

        [RelayCommand]
        private async Task CopyToClipboardAsync()
        {
            _logger.Info("Инициализировано копирование результата в буфер обмена");
            if (!string.IsNullOrWhiteSpace(GeneratedXml.Text))
            {
                if (_mainWindow.Clipboard is IClipboard clipboard)
                {
                    try
                    {
                        await clipboard.SetTextAsync(GeneratedXml.Text);
                        ShowMessage("XML скопирован в буфер обмена!");
                    }
                    catch (Exception ex)
                    {
                        ShowMessage($"Ошибка копирования: {ex.Message}", true);
                    }
                }
            }
        }

        private void ShowMessage(string message, bool isError = false)
        {
            if (isError)
            {
                StatusColor = Brushes.Red;
                _logger.Error(message);
            }
            else
            {
                _logger.Info(message);
            }
            SnackbarHost.Post(
                new SnackbarModel(
                    message,
                    TimeSpan.FromSeconds(5)),
                _statusSnackbar.HostName,
                DispatcherPriority.Normal);
        }
        private static string GetTitle()
        {
            var version = Assembly.GetEntryAssembly()?.GetName().Version;
            return $"MESBlastBlockGenerator v{version?.Major}.{version?.Minor}.{version?.Build}";
        }
        private void ClearStatus()
        {
            StatusColor = Brushes.Green;
        }

        #region OnPropertyNameChanged
        partial void OnMaxRowChanged(string value)
        {
            if (int.TryParse(value, out int result) && result > 0)
            {
                _inputParameters.MaxRow = result;
            }
        }
        partial void OnMaxColChanged(string value)
        {
            if (int.TryParse(value, out int result) && result > 0)
            {
                _inputParameters.MaxCol = result;
            }
        }
        partial void OnRotationAngleChanged(string value)
        {
            if (double.TryParse(value.ToString(_culture), out double result) && result > 0)
            {
                _inputParameters.RotationAngle = result;
            }
        }
        partial void OnBaseXChanged(string value)
        {
            if (double.TryParse(value.ToString(_culture), out double result))
            {
                _inputParameters.BaseX = result;
            }
        }
        partial void OnBaseYChanged(string value)
        {
            if (double.TryParse(value.ToString(_culture), out double result))
            {
                _inputParameters.BaseY = result;
            }
        }

        partial void OnBaseZChanged(string value)
        {
            if (double.TryParse(value.ToString(_culture), out double result) && result > 0)
            {
                _inputParameters.BaseZ = result;
            }
        }
        partial void OnDistanceChanged(string value)
        {
            if (double.TryParse(value.ToString(_culture), out double result) && result > 0)
            {
                _inputParameters.Distance = result;
            }
        }
        partial void OnPitNameChanged(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _inputParameters.PitName = value;
            }
        }
        partial void OnLevelChanged(string value)
        {
            if (int.TryParse(value, out int result) && result > 0)
            {
                _inputParameters.Level = result;
            }
        }
        partial void OnBlockNumberChanged(string value)
        {
            if (int.TryParse(value, out int result) && result > 0)
            {
                _inputParameters.BlockNumber = result;
            }
        }
        partial void OnMainChargeMassChanged(string value)
        {
            if (double.TryParse(value.ToString(_culture), out double result) && result > 0)
            {
                _inputParameters.MainChargeMass = result;
            }
        }
        partial void OnSecondaryChargeMassChanged(string value)
        {
            if (double.TryParse(value.ToString(_culture), out double result) && result > 0)
            {
                _inputParameters.SecondaryChargeMass = result;
            }
        }
        partial void OnDesignDepthChanged(string value)
        {
            if (double.TryParse(value.ToString(_culture), out double result) && result > 0)
            {
                _inputParameters.DesignDepth = result;
            }
        }
        partial void OnRealDepthChanged(string value)
        {
            if (double.TryParse(value.ToString(_culture), out double result) && result > 0)
            {
                _inputParameters.RealDepth = result;
            }
        }
        partial void OnDispersedChargeChanged(bool value)
        {
            _inputParameters.DispersedCharge = value;
        }
        partial void OnStemmingLengthChanged(string value)
        {
            if (double.TryParse(value.ToString(_culture), out double result) && result > 0)
            {
                _inputParameters.StemmingLength = result;
            }
        }
        partial void OnDesignDiameterChanged(string value)
        {
            if (int.TryParse(value, out int result) && result > 0)
            {
                _inputParameters.DesignDiameter = result;
            }
        }
        partial void OnRealDiameterChanged(string value)
        {
            if (int.TryParse(value, out int result) && result > 0)
            {
                _inputParameters.RealDiameter = result;
            }
        }
        partial void OnIsDrillingChanged(bool value)
        {
            _inputParameters.IsDrilling = value;
        }
        partial void OnCoordinatesDeviationChanged(string value)
        {
            if (double.TryParse(value.ToString(_culture), out double result) && result > 0)
            {
                _inputParameters.CoordinatesDeviation = result;
            }
        }
        #endregion
    }
}
