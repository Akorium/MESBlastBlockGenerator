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
    public partial class MainWindowViewModel : ObservableValidator
    {
        private readonly IXmlGenerationService _xmlGenerationService;
        private readonly AppSettings _appSettings = SettingsManager.LoadSettings();
        private static readonly NLog.Logger _logger = LogManager.GetCurrentClassLogger();
        private readonly Window _mainWindow;
        private readonly SnackbarHost _statusSnackbar;
        private readonly InputParameters _inputParameters = new();

        public MainWindowViewModel(IXmlGenerationService xmlGenerationService, Window mainWindow)
        {
            _xmlGenerationService = xmlGenerationService;
            _mainWindow = mainWindow;
            _statusSnackbar = mainWindow.Find<SnackbarHost>("StatusSnackbar");

            InitializeFromSettings(_appSettings);
        }

        #region Properties
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[1-9]\d*$", ErrorMessage = "Должно быть положительным целым числом")]
        private string _maxRow;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[1-9]\d*$", ErrorMessage = "Должно быть положительным целым числом")]
        private string _maxCol;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[0-9]+([.,][0-9]*)?$", ErrorMessage = "Должно быть положительным числом")]
        private string _rotationAngle;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^-?[0-9]+([.,][0-9]*)?$", ErrorMessage = "Должно быть числом")]
        private string _baseX;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^-?[0-9]+([.,][0-9]*)?$", ErrorMessage = "Должно быть числом")]
        private string _baseY;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[0-9]+([.,][0-9]*)?$", ErrorMessage = "Должно быть положительным числом")]
        private string _baseZ;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[1-9]\d*$", ErrorMessage = "Должно быть положительным целым числом")]
        private string _distance;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        private string _pitName;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[1-9]\d*$", ErrorMessage = "Должно быть положительным целым числом")]
        private string _level;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[1-9]\d*$", ErrorMessage = "Должно быть положительным целым числом")]
        private string _blockNumber;
        [ObservableProperty]
        private bool _dispersedCharge = false;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[0-9]+([.,][0-9]*)?$", ErrorMessage = "Должно быть положительным числом")]
        private string _mainChargeMass;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[0-9]+([.,][0-9]*)?$", ErrorMessage = "Должно быть положительным числом")]
        private string _secondaryChargeMass;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[0-9]+([.,][0-9]*)?$", ErrorMessage = "Должно быть положительным числом")]
        private string _designDepth;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[0-9]+([.,][0-9]*)?$", ErrorMessage = "Должно быть положительным числом")]
        private string _realDepth;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[0-9]+([.,][0-9]*)?$", ErrorMessage = "Должно быть положительным числом")]
        private string _stemmingLength;
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
                    $"stemmingLength = {_inputParameters.StemmingLength}");
                string xmlContent = await _xmlGenerationService.GenerateXmlContentAsync(_inputParameters);

                SettingsManager.UpdateAndSaveSettings(_appSettings, _inputParameters);
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

        private void InitializeFromSettings(AppSettings settings)
        {
            MaxRow = settings.MaxRow;
            MaxCol = settings.MaxCol;
            RotationAngle = settings.RotationAngle;
            BaseX = settings.BaseX;
            BaseY = settings.BaseY;
            BaseZ = settings.BaseZ;
            Distance = settings.Distance;
            PitName = settings.PitName;
            Level = settings.Level;
            BlockNumber = settings.BlockNumber;
            DesignDepth = settings.DesignDepth;
            RealDepth = settings.RealDepth;
            DispersedCharge = settings.DispersedCharge;
            MainChargeMass = settings.MainChargeMass;
            SecondaryChargeMass = settings.SecondaryChargeMass;
            StemmingLength = settings.StemmingLength;
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
            if (double.TryParse(value, out double result) && result > 0)
            {
                _inputParameters.RotationAngle = result;
            }
        }
        partial void OnBaseXChanged(string value)
        {
            if (double.TryParse(value, out double result))
            {
                _inputParameters.BaseX = result;
            }
        }
        partial void OnBaseYChanged(string value)
        {
            if (double.TryParse(value, out double result))
            {
                _inputParameters.BaseY = result;
            }
        }

        partial void OnBaseZChanged(string value)
        {
            if (double.TryParse(value, out double result) && result > 0)
            {
                _inputParameters.BaseZ = result;
            }
        }
        partial void OnDistanceChanged(string value)
        {
            if (int.TryParse(value, out int result) && result > 0)
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
            if (double.TryParse(value, out double result) && result > 0)
            {
                _inputParameters.MainChargeMass = result;
            }
        }
        partial void OnSecondaryChargeMassChanged(string value)
        {
            if (double.TryParse(value, out double result) && result > 0)
            {
                _inputParameters.SecondaryChargeMass = result;
            }
        }
        partial void OnDesignDepthChanged(string value)
        {
            if (double.TryParse(value, out double result) && result > 0)
            {
                _inputParameters.DesignDepth = result;
            }
        }
        partial void OnRealDepthChanged(string value)
        {
            if (double.TryParse(value, out double result) && result > 0)
            {
                _inputParameters.RealDepth = result;
            }
        }
        partial void OnStemmingLengthChanged(string value)
        {
            if (double.TryParse(value, out double result) && result > 0)
            {
                _inputParameters.StemmingLength = result;
            }
        }
    }
}
