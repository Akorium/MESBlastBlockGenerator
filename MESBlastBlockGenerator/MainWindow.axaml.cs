using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using NLog;
using System;
using System.Reflection;

namespace MESBlastBlockGenerator
{
    public partial class MainWindow : Window
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly BlastBlockGenerator _generator = new();

        // Временное ограничение пока нет конкретных данных по максимальному возможному объёму блока
        private const int maxWells = 5000;

        public MainWindow()
        {
            InitializeComponent();

            var version = Assembly.GetEntryAssembly()?.GetName().Version;
            string title = $"MESBlastBlockGenerator v{version?.Major}.{version?.Minor}.{version?.Build}";
            this.Title = title;
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
            StatusText.Foreground = Brushes.Green;
            StatusText.Text = "";

            bool dispersedCharge = DispersedChargeCheckBox.IsChecked ?? false;

            if (ValidateInputs(out int maxRow, out int maxCol, out double rotationAngleDegrees, out double baseX, out double baseY,
                              out double distance, out string pitName, out int level, out int blockNumber, out string errorMessage))
            {
                try
                {
                    logger.Debug($"Попытка генерации XML с maxRow = {maxRow}, maxCol = {maxCol}, baseX = {baseX}, baseY = {baseY}, distance = {distance}, pitName = {pitName}, level = {level}, blockNumber = {blockNumber}, dispersedCharge = {dispersedCharge}");
                    string xmlContent = BlastBlockGenerator.GenerateXmlContent(maxRow, maxCol, rotationAngleDegrees, baseX, baseY, distance,
                                                         pitName, level, blockNumber, dispersedCharge);

                    string successGenerationMessage = "XML успешно сгенерирован!";
                    logger.Info(successGenerationMessage);
                    StatusText.Text = successGenerationMessage;
                    XmlOutput.Text = xmlContent;
                }
                catch (Exception ex)
                {
                    string generationError = $"Ошибка генерации: {ex.Message}";
                    StatusText.Text = generationError;
                    StatusText.Foreground = Brushes.Red;
                    logger.Error(generationError);
                }
            }
            else
            {
                StatusText.Text = errorMessage;
                StatusText.Foreground = Brushes.Red;
                logger.Error(errorMessage);
            }
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
                        StatusText.Text = "XML скопирован в буфер обмена!";
                        StatusText.Foreground = Brushes.Green;
                    }
                    catch (Exception ex)
                    {
                        string copyError = $"Ошибка копирования: {ex.Message}";
                        StatusText.Text = copyError;
                        StatusText.Foreground = Brushes.Red;
                        logger.Error(copyError);
                    }
                }
            }
        }

        /// <summary>
        /// Валидация входных параметров
        /// </summary>
        /// <param name="maxRow">Количество рядов скважин</param>
        /// <param name="maxCol">Количество столбцов скважин</param>
        /// <param name="rotationAngleDegrees">Угол поворота сетки скважин</param>
        /// <param name="baseX">X первой скважины блока</param>
        /// <param name="baseY">Y первой скважины блока</param>
        /// <param name="distance">Расстояние между скважинами</param>
        /// <param name="pitName">Название карьера</param>
        /// <param name="level">Уровень проекта</param>
        /// <param name="blockNumber">Порядковый номер блока</param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        private bool ValidateInputs(out int maxRow, out int maxCol, out double rotationAngleDegrees, out double baseX, out double baseY,
                          out double distance, out string pitName, out int level, out int blockNumber, out string errorMessage)
        {
            maxRow = maxCol = level = blockNumber = 0;
            rotationAngleDegrees = baseX = baseY = distance = 0;
            errorMessage = pitName = string.Empty;

            MaxRowTextBox.Classes.Remove("invalid");
            MaxColTextBox.Classes.Remove("invalid");
            RotationAngleTextBox.Classes.Remove("invalid");
            BaseXTextBox.Classes.Remove("invalid");
            BaseYTextBox.Classes.Remove("invalid");
            DistanceTextBox.Classes.Remove("invalid");
            LevelTextBox.Classes.Remove("invalid");
            PitNameTextBox.Classes.Remove("invalid");
            BlockNumberTextBox.Classes.Remove("invalid");

            if (!int.TryParse(MaxRowTextBox.Text, out maxRow) || maxRow <= 0)
            {
                MaxRowTextBox.Classes.Add("invalid");
                errorMessage = "Количество рядов должно быть положительным числом";
                return false;
            }

            if (!int.TryParse(MaxColTextBox.Text, out maxCol) || maxCol <= 0)
            {
                MaxColTextBox.Classes.Add("invalid");
                errorMessage = "Количество столбцов должно быть положительным числом";
                return false;
            }

            if (!double.TryParse(RotationAngleTextBox.Text, out rotationAngleDegrees))
            {
                RotationAngleTextBox.Classes.Add("invalid");
                errorMessage = "Некорректное значение угла поворота сетки";
                return false;
            }

            if (!double.TryParse(BaseXTextBox.Text, out baseX))
            {
                BaseXTextBox.Classes.Add("invalid");
                errorMessage = "Некорректное значение координаты X";
                return false;
            }

            if (!double.TryParse(BaseYTextBox.Text, out baseY))
            {
                BaseYTextBox.Classes.Add("invalid");
                errorMessage = "Некорректное значение координаты Y";
                return false;
            }

            if (!double.TryParse(DistanceTextBox.Text, out distance) || distance <= 0)
            {
                DistanceTextBox.Classes.Add("invalid");
                errorMessage = "Расстояние между скважинами должно быть положительным числом";
                return false;
            }

            if (string.IsNullOrWhiteSpace(PitNameTextBox.Text))
            {
                PitNameTextBox.Classes.Add("invalid");
                errorMessage = "Название карьера не может быть пустым";
                return false;
            }
            pitName = PitNameTextBox.Text;

            if (!int.TryParse(LevelTextBox.Text, out level))
            {
                LevelTextBox.Classes.Add("invalid");
                errorMessage = "Некорректное значение уровня";
                return false;
            }

            if (!int.TryParse(BlockNumberTextBox.Text, out blockNumber))
            {
                BlockNumberTextBox.Classes.Add("invalid");
                errorMessage = "Некорректное значение номера блока";
                return false;
            }
            
            int totalWells = maxCol * maxRow;
            if (totalWells > maxWells)
            {
                MaxRowTextBox.Classes.Add("invalid");
                MaxColTextBox.Classes.Add("invalid");
                errorMessage = $"Общее количество скважин ({totalWells}) превышает максимально допустимое ({maxWells})";
                return false;
            }

            return true;
        }
    }
}