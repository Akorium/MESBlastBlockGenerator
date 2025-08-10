using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using MESBlastBlockGenerator.DTO;
using MESBlastBlockGenerator.Helpers;
using MESBlastBlockGenerator.Models;
using NLog;
using System;
using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace MESBlastBlockGenerator
{
    public partial class MainWindow : Window
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly AppSettings appSettings = SettingsManager.LoadSettings();
        private InputParameters? inputs;

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

            MaxRowTextBox.Text = appSettings.MaxRow.ToString();
            MaxColTextBox.Text = appSettings.MaxCol.ToString();
            RotationAngleTextBox.Text = appSettings.RotationAngle.ToString();
            BaseXTextBox.Text = appSettings.BaseX.ToString();
            BaseYTextBox.Text = appSettings.BaseY.ToString();
            DistanceTextBox.Text = appSettings.Distance.ToString();
            PitNameTextBox.Text = appSettings.PitName;
            LevelTextBox.Text = appSettings.Level.ToString();
            BlockNumberTextBox.Text = appSettings.BlockNumber.ToString();
            DispersedChargeCheckBox.IsChecked = appSettings.DispersedCharge;
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
                logger.Debug($"Попытка генерации XML с maxRow = {inputs.MaxRow}, maxCol = {inputs.MaxCol}, baseX = {inputs.BaseX}, baseY = {inputs.BaseY}, distance = {inputs.Distance}, pitName = {inputs.PitName}, level = {inputs.Level}, blockNumber = {inputs.BlockNumber}, dispersedCharge = {inputs.DispersedCharge}");
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
            ValidationHelper.ClearValidation(MaxRowTextBox, MaxColTextBox, RotationAngleTextBox, BaseXTextBox, BaseYTextBox,
                DistanceTextBox, PitNameTextBox, LevelTextBox, BlockNumberTextBox);
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
                Distance = ValidationHelper.ValidateDouble(DistanceTextBox, FieldNames.Descriptions[Fields.Distance]),
                PitName = ValidationHelper.ValidateString(PitNameTextBox, FieldNames.Descriptions[Fields.PitName]),
                Level = ValidationHelper.ValidatePositiveInt(LevelTextBox, FieldNames.Descriptions[Fields.Level]),
                BlockNumber = ValidationHelper.ValidatePositiveInt(BlockNumberTextBox, FieldNames.Descriptions[Fields.BlockNumber]),
                DispersedCharge = DispersedChargeCheckBox.IsChecked ?? false
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
            appSettings.Distance = inputs.Distance;
            appSettings.PitName = inputs.PitName;
            appSettings.Level = inputs.Level;
            appSettings.BlockNumber = inputs.BlockNumber;
            appSettings.DispersedCharge = inputs.DispersedCharge;
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

        protected override void OnClosed(EventArgs e)
        {
            if (inputs != null)
                UpdateSettings(inputs);
            base.OnClosed(e);
        }
    }
}