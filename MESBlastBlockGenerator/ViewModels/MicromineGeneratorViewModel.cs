using Avalonia.Media;
using Avalonia.Threading;
using AvaloniaEdit.Document;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Material.Styles.Controls;
using Material.Styles.Models;
using MESBlastBlockGenerator.Enums;
using MESBlastBlockGenerator.Helpers;
using MESBlastBlockGenerator.Models;
using MESBlastBlockGenerator.Services.Interfaces;
using NLog;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;

namespace MESBlastBlockGenerator.ViewModels
{
    public partial class MicromineGeneratorViewModel(IGenerationService generationService, InputParameters inputParameters) : ObservableValidator
    {
        private readonly IGenerationService _generationService = generationService;
        private readonly InputParameters _inputParameters = inputParameters;
        private static readonly NLog.Logger _logger = LogManager.GetCurrentClassLogger();
        private static readonly CultureInfo _culture = CultureInfo.CurrentCulture;

        #region Properties
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[1-9]\d*$", ErrorMessage = "Целое положительное число")]
        private string _maxRow = inputParameters.MaxRow.ToString(_culture);
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[1-9]\d*$", ErrorMessage = "Целое положительное число")]
        private string _maxCol = inputParameters.MaxCol.ToString(_culture);
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[0-9]+([.,][0-9]*)?$", ErrorMessage = "Положительное число")]
        private string _rotationAngle = inputParameters.RotationAngle.ToString(_culture);
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^-?[0-9]+([.,][0-9]*)?$", ErrorMessage = "Число")]
        private string _baseX = inputParameters.BaseX.ToString(_culture);
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^-?[0-9]+([.,][0-9]*)?$", ErrorMessage = "Число")]
        private string _baseY = inputParameters.BaseY.ToString(_culture);
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[0-9]+([.,][0-9]*)?$", ErrorMessage = "Положительное число")]
        private string _baseZ = inputParameters.BaseZ.ToString(_culture);
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[0-9]+([.,][0-9]*)?$", ErrorMessage = "Положительное число")]
        private string _distance = inputParameters.Distance.ToString(_culture);
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        private string _pitName = inputParameters.PitName;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[1-9]\d*$", ErrorMessage = "Целое положительное число")]
        private string _level = inputParameters.Level.ToString(_culture);
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[1-9]\d*$", ErrorMessage = "Целое положительное число")]
        private string _blockNumber = inputParameters.BlockNumber.ToString(_culture);
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[0-9]+([.,][0-9]*)?$", ErrorMessage = "Положительное число")]
        private string _mainChargeMass = inputParameters.MainChargeMass.ToString(_culture);
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[0-9]+([.,][0-9]*)?$", ErrorMessage = "Положительное число")]
        private string _secondaryChargeMass = inputParameters.SecondaryChargeMass.ToString(_culture);
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[0-9]+([.,][0-9]*)?$", ErrorMessage = "Положительное число")]
        private string _designDepth = inputParameters.DesignDepth.ToString(_culture);
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[1-9]\d*$", ErrorMessage = "Целое положительное число")]
        private string _designDiameter = inputParameters.DesignDiameter.ToString(_culture);
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^[0-9]+([.,][0-9]*)?$", ErrorMessage = "Положительное число")]
        private string _stemmingLength = inputParameters.StemmingLength.ToString(_culture);
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        private string _explosiveName = inputParameters.ExplosiveName;
        [ObservableProperty]
        [NotifyDataErrorInfo]
        [Required(ErrorMessage = "Обязательно для заполнения")]
        [RegularExpression(@"^-?[0-9]+([.,][0-9]*)?$", ErrorMessage = "Число")]
        private string _explosiveDensity = inputParameters.ExplosiveDensity.ToString(_culture);
        [ObservableProperty]
        private ChargeType _chargeType = inputParameters.ChargeType;
        [ObservableProperty]
        private TextDocument _generatedCollar = new();
        [ObservableProperty]
        private TextDocument _generatedIntervals = new();
        [ObservableProperty]
        private IBrush _statusColor = Brushes.Green;
        [ObservableProperty]
        private bool _isCsvGenerated = false;
        public ChargeType[] ChargeTypeValues { get; } = (ChargeType[])Enum.GetValues(typeof(ChargeType));
        public bool IsNotSingleChargeSelected => ChargeType != ChargeType.Одиночный;
        #endregion

        [RelayCommand]
        private void GenerateCsv()
        {
            _logger.Info("Инициализирована генерация ПМВ Micromine");
            ClearStatus();

            try
            {
                ValidateAllProperties();

                if (HasErrors)
                {
                    ShowMessage("Пожалуйста, исправьте ошибки ввода", true);
                    return;
                }

                _logger.Debug($"Попытка генерации ПМВ Micromine с maxRow = {_inputParameters.MaxRow}, maxCol = {_inputParameters.MaxCol}, baseX = {_inputParameters.BaseX}, baseY = {_inputParameters.BaseY}, " +
                    $"baseZ = {_inputParameters.BaseZ}, distance = {_inputParameters.Distance}, pitName = {_inputParameters.PitName}, level = {_inputParameters.Level}, blockNumber = {_inputParameters.BlockNumber}, " +
                    $"mainChargeMass = {_inputParameters.MainChargeMass}" +
                    $"designDepth = {_inputParameters.DesignDepth}, designDiameter = {_inputParameters.DesignDiameter}, stemmingLength = {_inputParameters.StemmingLength}");
                (string blastHoles, string blastBlockPoints) = _generationService.GenerateMicromineMassExplosionProject(_inputParameters);

                SettingsManager.SaveUserInputs(_inputParameters);
                GeneratedCollar = new TextDocument(blastHoles);
                GeneratedIntervals = new TextDocument(blastBlockPoints);
                ShowMessage("ПМВ Micromine успешно сгенерирован!");
                IsCsvGenerated = true;
            }
            catch (Exception ex)
            {
                ShowMessage($"Ошибка генерации: {ex.Message}", true);
            }
        }

        [RelayCommand]
        private async Task SaveCsvToFileAsync()
        {
            try
            {
                _logger.Info("Инициализировано сохранение ПМВ Micromine в файл");

                var mainWindow = Avalonia.Application.Current.ApplicationLifetime as Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime;
                var window = mainWindow?.MainWindow;

                if (window == null)
                {
                    ShowMessage("Не удалось получить главное окно", true);
                    return;
                }

                var blastHolesFile = await window.StorageProvider.SaveFilePickerAsync(new Avalonia.Platform.Storage.FilePickerSaveOptions
                {
                    Title = "Сохранить CSV файл",
                    FileTypeChoices =
                    [
                        new Avalonia.Platform.Storage.FilePickerFileType("CSV файлы")
                        {
                            Patterns = ["*.csv"],
                            MimeTypes = ["application/csv", "text/csv"]
                        },
                        new Avalonia.Platform.Storage.FilePickerFileType("Все файлы")
                        {
                            Patterns = ["*"]
                        }
                    ],
                    SuggestedFileName = $"{_inputParameters.PitName}{_inputParameters.Level}-{_inputParameters.BlockNumber}-collar.csv"
                });

                if (blastHolesFile != null)
                {
                    using var stream = await blastHolesFile.OpenWriteAsync();
                    using var writer = new StreamWriter(stream);
                    await writer.WriteAsync(GeneratedCollar.Text);
                    await writer.FlushAsync();

                    var blastHolesFilePath = blastHolesFile.Path.LocalPath;
                    var directory = Path.GetDirectoryName(blastHolesFile.Path.LocalPath);
                    var intervalsFilePath = Path.Combine(directory, $"{_inputParameters.PitName}{_inputParameters.Level}-{_inputParameters.BlockNumber}-interval.csv");
                    await File.WriteAllTextAsync(intervalsFilePath, GeneratedIntervals.Text);

                    ShowMessage($"ПМВ Micromine успешно сохранен в файл: {blastHolesFilePath}");
                    _logger.Info($"ПМВ Micromine файл сохранен: {blastHolesFilePath}");


                }
                else
                {
                    ShowMessage("Сохранение отменено пользователем");
                }
            }
            catch (Exception ex)
            {
                ShowMessage($"Ошибка сохранения файла: {ex.Message}", true);
                _logger.Error(ex, "Ошибка сохранения CSV файла");
            }
        }

        private void ShowMessage(string message, bool isError = false)
        {
            if (isError)
            {
                StatusColor = Brushes.Red;
                _logger.Error(message);
                SnackbarHost.Post(
                    new SnackbarModel($"{message}", TimeSpan.FromSeconds(5)),
                null, DispatcherPriority.Normal);
            }
            else
            {
                _logger.Info(message);
                SnackbarHost.Post(
                    new SnackbarModel($"{message}", TimeSpan.FromSeconds(5)),
                null, DispatcherPriority.Normal);
            }
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
        partial void OnExplosiveNameChanged(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                _inputParameters.ExplosiveName = value;
            }
        }
        partial void OnExplosiveDensityChanged(string value)
        {
            if (double.TryParse(value.ToString(_culture), out double result) && result > 0)
            {
                _inputParameters.ExplosiveDensity = result;
            }
        }
        partial void OnChargeTypeChanged(ChargeType value)
        {
            _inputParameters.ChargeType = value;
            OnPropertyChanged(nameof(IsNotSingleChargeSelected));
        }
        #endregion
    }
}
