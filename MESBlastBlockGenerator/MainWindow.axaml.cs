using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using MESBlastBlockGenerator.DTO;
using MESBlastBlockGenerator.Helpers;
using MESBlastBlockGenerator.Models;
using NLog;
using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;

namespace MESBlastBlockGenerator
{
    public partial class MainWindow : Window
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly AppSettings appSettings = SettingsManager.LoadSettings();
        private InputParameters? inputs;
        private readonly CultureInfo culture = CultureInfo.CurrentCulture;

        public MainWindow()
        {
            InitializeComponent();          
            ConfigureUI(appSettings);
        }

        private void ConfigureUI(AppSettings appSettings)
        {
            var version = Assembly.GetEntryAssembly()?.GetName().Version;
            string title = $"MESBlastBlockGenerator v{version?.Major}.{version?.Minor}.{version?.Build}";
            this.Title = title;

            MaxRowTextBox.Text = appSettings.MaxRow.ToString(culture);
            MaxColTextBox.Text = appSettings.MaxCol.ToString(culture);
            RotationAngleTextBox.Text = appSettings.RotationAngle.ToString(culture);
            BaseXTextBox.Text = appSettings.BaseX.ToString(culture);
            BaseYTextBox.Text = appSettings.BaseY.ToString(culture);
            BaseZTextBox.Text = appSettings.BaseZ.ToString(culture);
            DistanceTextBox.Text = appSettings.Distance.ToString(culture);
            PitNameTextBox.Text = appSettings.PitName;
            LevelTextBox.Text = appSettings.Level.ToString(culture);
            BlockNumberTextBox.Text = appSettings.BlockNumber.ToString(culture);
            DesignDepthTextBox.Text = appSettings.DesignDepth.ToString(culture);
            RealDepthTextBox.Text = appSettings.RealDepth.ToString(culture);
            DispersedChargeCheckBox.IsChecked = appSettings.DispersedCharge;
            MainChargeMassTextBox.Text = appSettings.MainChargeMass.ToString(culture);
            SecondaryChargeMassTextBox.Text = appSettings.SecondaryChargeMass.ToString(culture);
            StemmingLengthTextBox.Text = appSettings.StemmingLength.ToString(culture);
            logger.Info($"{title} запущен");
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Сгенерировать XML"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnGenerateClick(object sender, RoutedEventArgs e)
        {
            logger.Info("Инициализирована генерация XML");
            ClearValidationErrors();
            StatusText.Foreground = Brushes.Green;
            StatusText.Text = "";

            try
            {
                inputs = ValidateInputs();
                logger.Debug($"Попытка генерации XML с maxRow = {inputs.MaxRow}, maxCol = {inputs.MaxCol}, baseX = {inputs.BaseX}, baseY = {inputs.BaseY}, " +
                    $"baseZ = {inputs.BaseZ}, distance = {inputs.Distance}, pitName = {inputs.PitName}, level = {inputs.Level}, blockNumber = {inputs.BlockNumber}, " +
                    $"dispersedCharge = {inputs.DispersedCharge}, mainChargeMass = {inputs.MainChargeMass}" +
                    $"designDepth = {inputs.DesignDepth}, realDepth = {inputs.RealDepth}, {(inputs.DispersedCharge ? $", secondaryChargeMass = {inputs.SecondaryChargeMass}, " : "")} " +
                    $"stemmingLength = {inputs.StemmingLength}");
                string xmlContent = XmlGenerationHelper.GenerateXmlContent(inputs);

                UpdateSettings(inputs);
                XmlOutput.Text = xmlContent;
                string successGenerationMessage = "XML успешно сгенерирован!";
                ShowSuccess(successGenerationMessage);
            }
            catch (ValidationException ex)
            {
                string validationError = $"Ошибка валидации: {ex.Message}";
                ShowError(validationError);
            }
            catch (Exception ex)
            {
                string generationError = $"Ошибка генерации: {ex.Message}";
                ShowError(generationError);
            }
        }

        private void ClearValidationErrors()
        {
            ValidationHelper.ClearValidation(MaxRowTextBox, MaxColTextBox, RotationAngleTextBox, BaseXTextBox, BaseYTextBox, BaseZTextBox,
                DistanceTextBox, PitNameTextBox, LevelTextBox, BlockNumberTextBox, DesignDepthTextBox,RealDepthTextBox, MainChargeMassTextBox, 
                SecondaryChargeMassTextBox, StemmingLengthTextBox);
        }

        private InputParameters ValidateInputs()
        {
            var inputs = new InputParameters
            {
                MaxRow = ValidationHelper.ValidatePositiveInt(MaxRowTextBox, FieldNames.Descriptions[Fields.MaxRow]),
                MaxCol = ValidationHelper.ValidatePositiveInt(MaxColTextBox, FieldNames.Descriptions[Fields.MaxCol]),
                RotationAngle = ValidationHelper.ValidateDouble(RotationAngleTextBox, FieldNames.Descriptions[Fields.RotationAngle]),
                BaseX = ValidationHelper.ValidateDouble(BaseXTextBox, FieldNames.Descriptions[Fields.BaseX]),
                BaseY = ValidationHelper.ValidateDouble(BaseYTextBox, FieldNames.Descriptions[Fields.BaseY]),
                BaseZ = ValidationHelper.ValidateDouble(BaseZTextBox, FieldNames.Descriptions[Fields.BaseZ]),
                Distance = ValidationHelper.ValidateDouble(DistanceTextBox, FieldNames.Descriptions[Fields.Distance]),
                PitName = ValidationHelper.ValidateString(PitNameTextBox, FieldNames.Descriptions[Fields.PitName]),
                Level = ValidationHelper.ValidatePositiveInt(LevelTextBox, FieldNames.Descriptions[Fields.Level]),
                BlockNumber = ValidationHelper.ValidatePositiveInt(BlockNumberTextBox, FieldNames.Descriptions[Fields.BlockNumber]),
                DesignDepth = ValidationHelper.ValidatePositiveDouble(DesignDepthTextBox, FieldNames.Descriptions[Fields.DesignDepth]),
                RealDepth = ValidationHelper.ValidatePositiveDouble(RealDepthTextBox, FieldNames.Descriptions[Fields.RealDepth]),
                DispersedCharge = DispersedChargeCheckBox.IsChecked ?? false,
                MainChargeMass = ValidationHelper.ValidatePositiveDouble(MainChargeMassTextBox, FieldNames.Descriptions[Fields.MainChargeMass]),
                SecondaryChargeMass = DispersedChargeCheckBox.IsChecked.GetValueOrDefault(false) ? ValidationHelper.ValidatePositiveDouble(SecondaryChargeMassTextBox, FieldNames.Descriptions[Fields.SecondaryChargeMass]) : appSettings.SecondaryChargeMass,
                StemmingLength = ValidationHelper.ValidatePositiveDouble(StemmingLengthTextBox, FieldNames.Descriptions[Fields.StemmingLength])
            };
            ValidationHelper.ValidateWellsCount([MaxRowTextBox, MaxColTextBox], inputs.MaxCol, inputs.MaxRow);
            return inputs;
        }

        private void UpdateSettings(InputParameters inputs)
        {
            appSettings.MaxRow = inputs.MaxRow;
            appSettings.MaxCol = inputs.MaxCol;
            appSettings.RotationAngle = inputs.RotationAngle;
            appSettings.BaseX = inputs.BaseX;
            appSettings.BaseY = inputs.BaseY;
            appSettings.BaseZ = inputs.BaseZ;
            appSettings.Distance = inputs.Distance;
            appSettings.PitName = inputs.PitName;
            appSettings.Level = inputs.Level;
            appSettings.BlockNumber = inputs.BlockNumber;
            appSettings.DesignDepth = inputs.DesignDepth;
            appSettings.RealDepth = inputs.RealDepth;
            appSettings.DispersedCharge = inputs.DispersedCharge;
            appSettings.MainChargeMass = inputs.MainChargeMass;
            if (inputs.DispersedCharge) 
                appSettings.SecondaryChargeMass = inputs.SecondaryChargeMass;
            appSettings.StemmingLength = inputs.StemmingLength;
            SettingsManager.SaveSettings(appSettings);
        }

        /// <summary>
        /// Обработчик нажатия кнопки "Копировать в буфер обмена"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void OnCopyClick(object sender, RoutedEventArgs e)
        {
            logger.Info("Инициализировано копирование результата в буфер обмена");
            if (!string.IsNullOrWhiteSpace(XmlOutput.Text))
            {
                var topLevel = TopLevel.GetTopLevel(this);

                if (topLevel != null && topLevel.Clipboard != null)
                {
                    try
                    {
                        await topLevel.Clipboard.SetTextAsync(XmlOutput.Text);
                        string copySuccess = "XML скопирован в буфер обмена!";
                        ShowSuccess(copySuccess);
                    }
                    catch (Exception ex)
                    {
                        string copyError = $"Ошибка копирования: {ex.Message}";
                        ShowError(copyError);
                    }
                }
            }
        }

        private void ShowSuccess(string message)
        {
            StatusText.Text = message;
            logger.Info(message);
            StatusText.Foreground = Brushes.Green;
        }
        private void ShowError(string generationError)
        {
            StatusText.Text = generationError;
            StatusText.Foreground = Brushes.Red;
            logger.Error(generationError);
        }
    }
}