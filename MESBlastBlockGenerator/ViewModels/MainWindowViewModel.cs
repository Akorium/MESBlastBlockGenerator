using Avalonia.Controls;
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
        [Range(1, int.MaxValue, ErrorMessage = "Должно быть больше 0")]
        public int maxRow;
        [ObservableProperty]
        public int maxCol;
        [ObservableProperty]
        public double rotationAngle;
        [ObservableProperty]
        public double baseX;
        [ObservableProperty]
        public double baseY;
        [ObservableProperty]
        public double baseZ;
        [ObservableProperty]
        public double distance;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        public string pitName = string.Empty;
        [ObservableProperty]
        public int level;
        [ObservableProperty]
        public int blockNumber;
        [ObservableProperty]
        public bool dispersedCharge = false;
        [ObservableProperty]
        public double mainChargeMass;
        [ObservableProperty]
        public double secondaryChargeMass;
        [ObservableProperty]
        public double designDepth;
        [ObservableProperty]
        public double realDepth;
        [ObservableProperty]
        public double stemmingLength;
        [ObservableProperty]
        public TextDocument generatedXml = new();
        [ObservableProperty]
        public IBrush statusColor = Brushes.Green;
        [ObservableProperty]
        public string title = string.Empty;
        [ObservableProperty]
        public bool copyButtonEnabled = false;
        #endregion

        [RelayCommand]
        private async Task GenerateXml()
        {
            await GenerateXmlAsync();
        }

        [RelayCommand]
        private async Task CopyToClipboard()
        {
            await CopyToClipboardAsync();
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
            Title = GetTitle();
        }

        public async Task GenerateXmlAsync()
        {
            _logger.Info("Инициализирована генерация XML");
            ClearStatus();

            try
            {
                var inputs = GetInputs();
                _logger.Debug($"Попытка генерации XML с maxRow = {inputs.MaxRow}, maxCol = {inputs.MaxCol}, baseX = {inputs.BaseX}, baseY = {inputs.BaseY}, " +
                    $"baseZ = {inputs.BaseZ}, distance = {inputs.Distance}, pitName = {inputs.PitName}, level = {inputs.Level}, blockNumber = {inputs.BlockNumber}, " +
                    $"dispersedCharge = {inputs.DispersedCharge}, mainChargeMass = {inputs.MainChargeMass}" +
                    $"designDepth = {inputs.DesignDepth}, realDepth = {inputs.RealDepth}, {(inputs.DispersedCharge ? $", secondaryChargeMass = {inputs.SecondaryChargeMass}, " : "")} " +
                    $"stemmingLength = {inputs.StemmingLength}");
                string xmlContent = await _xmlGenerationService.GenerateXmlContentAsync(inputs);

                UpdateSettings(inputs);
                GeneratedXml = new TextDocument(xmlContent);
                ShowSuccess("XML успешно сгенерирован!");
                CopyButtonEnabled = true;
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка генерации: {ex.Message}");
            }
        }

        public async Task CopyToClipboardAsync()
        {
            _logger.Info("Инициализировано копирование результата в буфер обмена");
            if (!string.IsNullOrWhiteSpace(GeneratedXml.Text))
            {
                if (_mainWindow.Clipboard is { } clipboard)
                {
                    try
                    {
                        await clipboard.SetTextAsync(GeneratedXml.Text);
                        ShowSuccess("XML скопирован в буфер обмена!");
                    }
                    catch (Exception ex)
                    {
                        ShowError($"Ошибка копирования: {ex.Message}");
                    }
                }
            }
        }

        private InputParameters GetInputs()
        {
            var inputs = new InputParameters
            {
                MaxRow = MaxRow,
                MaxCol = MaxCol,
                RotationAngle = RotationAngle,
                BaseX = BaseX,
                BaseY = BaseY,
                BaseZ = BaseZ,
                Distance = Distance,
                PitName = PitName,
                Level = Level,
                BlockNumber = BlockNumber,
                DesignDepth = DesignDepth,
                RealDepth = RealDepth,
                DispersedCharge =  DispersedCharge,
                MainChargeMass = MainChargeMass,
                SecondaryChargeMass = SecondaryChargeMass,
                StemmingLength = StemmingLength
            };
            return inputs;
        }

        private void UpdateSettings(InputParameters inputs)
        {
            _appSettings.MaxRow = inputs.MaxRow;
            _appSettings.MaxCol = inputs.MaxCol;
            _appSettings.RotationAngle = inputs.RotationAngle;
            _appSettings.BaseX = inputs.BaseX;
            _appSettings.BaseY = inputs.BaseY;
            _appSettings.BaseZ = inputs.BaseZ;
            _appSettings.Distance = inputs.Distance;
            _appSettings.PitName = inputs.PitName;
            _appSettings.Level = inputs.Level;
            _appSettings.BlockNumber = inputs.BlockNumber;
            _appSettings.DesignDepth = inputs.DesignDepth;
            _appSettings.RealDepth = inputs.RealDepth;
            _appSettings.DispersedCharge = inputs.DispersedCharge;
            _appSettings.MainChargeMass = inputs.MainChargeMass;
            if (inputs.DispersedCharge)
                _appSettings.SecondaryChargeMass = inputs.SecondaryChargeMass;
            _appSettings.StemmingLength = inputs.StemmingLength;
            SettingsManager.SaveSettings(_appSettings);
        }

        private void ShowSuccess(string message)
        {
            SnackbarHost.Post(
            new SnackbarModel(
                message,
                TimeSpan.FromSeconds(5)),
            _statusSnackbar.HostName,
            DispatcherPriority.Normal);
            _logger.Info(message);
        }
        private void ShowError(string message)
        {
            StatusColor = Brushes.Red;
            SnackbarHost.Post(
            new SnackbarModel(
                message,
                TimeSpan.FromSeconds(5)),
            _statusSnackbar.HostName,
            DispatcherPriority.Normal);
            _logger.Info(message);
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
    }
}
