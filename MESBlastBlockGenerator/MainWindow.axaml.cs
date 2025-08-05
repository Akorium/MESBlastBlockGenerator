using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace MESBlastBlockGenerator
{
    public partial class MainWindow : Window
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public MainWindow()
        {
            InitializeComponent();

            var version = Assembly.GetEntryAssembly()?.GetName().Version;
            string title = $"MESBlastBlockGenerator v{version?.Major}.{version?.Minor}.{version?.Build}";
            this.Title = title;
            logger.Info($"{title} запущен");
        }

        private void OnGenerateClick(object sender, RoutedEventArgs e)
        {
            logger.Info("Инициализирована генерация XML");
            StatusText.Foreground = Brushes.Green;
            StatusText.Text = "";

            if (ValidateInputs(out int maxRow, out int maxCol, out double rotationAngleDegrees, out double baseX, out double baseY,
                              out double distance, out string pitName, out int level, out int blockNumber, out string errorMessage))
            {
                try
                {
                    logger.Debug($"Попытка генерации XML с maxRow = {maxRow}, maxCol = {maxCol}, baseX = {baseX}, baseY = {baseY}, distance = {distance}, pitName = {pitName}, level = {level}, blockNumber = {blockNumber}");
                    string xmlContent = GenerateXmlContent(maxRow, maxCol, rotationAngleDegrees, baseX, baseY, distance,
                                                         pitName, level, blockNumber);
                    XmlOutput.Text = xmlContent;
                    string successGenerationMessage = "XML успешно сгенерирован!";
                    StatusText.Text = successGenerationMessage;
                    logger.Info(successGenerationMessage);
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

        private async void OnCopyClick(object sender, RoutedEventArgs e)
        {
            logger.Info("Инициализировано копирование результата в буфер обмена");
            if (!string.IsNullOrWhiteSpace(XmlOutput.Text))
            {
                // Получаем TopLevel (обычно это MainWindow)
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

        private static string GenerateXmlContent(int maxRow, int maxCol, double rotationAngleDegrees, double baseX, double baseY, double distance,
                                       string pitName, int level, int blockNumber)
        {
            var envelope = new Envelope
            {
                Header = new Header(),
                Body = new Body
                {
                    SoapXmlRequest = new SoapXmlRequest
                    {
                        XmlRequest = new XmlRequest
                        {
                            Message = new Message
                            {
                                MesPmv = new MesPmv
                                {
                                    HolesInBlastProject = new HolesInBlastProject
                                    {
                                        Holes = GenerateHoles(maxRow, maxCol, rotationAngleDegrees, baseX, baseY, distance, pitName, level, blockNumber)
                                    }
                                }
                            }
                        }
                    }
                }
            };
            var xmlSerializer = new XmlSerializer(typeof(Envelope));
            var xmlWriter = new StringWriter();
            xmlSerializer.Serialize(xmlWriter, envelope);
            return xmlWriter.ToString();
        }

        private static List<Hole> GenerateHoles(int maxRow, int maxCol, double rotationAngleDegrees, double baseX, double baseY, double distance, string pitName, int level, int blockNumber)
        {
            string blastProjectId = Guid.NewGuid().ToString();
            double rotationAngleRad = rotationAngleDegrees * Math.PI / 180.0;
            double cosAngle = Math.Cos(rotationAngleRad);
            double sinAngle = Math.Sin(rotationAngleRad);

            var holes = new List<Hole>();
            for (int row = 0; row < maxRow; row++)
            {
                for (int col = 0; col < maxCol; col++)
                {
                    double x = baseX + (col) * distance;
                    double y = baseY + (row) * distance;

                    double relX = x - baseX;
                    double relY = y - baseY;

                    x = baseX + relX * cosAngle - relY * sinAngle;
                    y = baseY + relX * sinAngle + relY * cosAngle;

                    var hole = new Hole
                    {
                        HoleItem = new HoleItem
                        {
                            BlastProjectId = blastProjectId,
                            HoleId = Guid.NewGuid().ToString(),
                            HoleNumber = $"{row:D2}{col:D2}",
                            PitCode = pitName,
                            PitName = pitName,
                            LevelCode = $"{pitName}{level}",
                            LevelName = $"{level}",
                            BlockCode = $"{pitName}{level}-{blockNumber}",
                            BlockName = $"{level}-{blockNumber}",
                            BlockDrillingCode = $"{pitName}{pitName}{level}{level}-{blockNumber}Drill",
                            BlockDrillingName = $"{level}-{blockNumber}",
                            BlockBlastingCode = $"{pitName}{level}-{blockNumber}Blast",
                            BlockBlastingName = $"{level}-{blockNumber}",
                            X = x.ToString(CultureInfo.InvariantCulture),
                            Y = y.ToString(CultureInfo.InvariantCulture),
                            XFact = x.ToString(CultureInfo.InvariantCulture),
                            YFact = y.ToString(CultureInfo.InvariantCulture)
                        }
                    };
                    holes.Add(hole);
                }
            }
            return holes;
        }

        private bool ValidateInputs(out int maxRow, out int maxCol, out double rotationAngleDegrees, out double baseX, out double baseY,
                          out double distance, out string pitName, out int level, out int blockNumber, out string errorMessage)
        {
            // Инициализация out-параметров
            maxRow = maxCol = level = blockNumber = 0;
            rotationAngleDegrees = baseX = baseY = distance = 0;
            errorMessage = pitName = string.Empty;
            const int maxWells = 5000;


            // Сброс стилей ошибок
            MaxRowTextBox.Classes.Remove("invalid");
            MaxColTextBox.Classes.Remove("invalid");
            RotationAngleTextBox.Classes.Remove("invalid");
            BaseXTextBox.Classes.Remove("invalid");
            BaseYTextBox.Classes.Remove("invalid");
            DistanceTextBox.Classes.Remove("invalid");
            LevelTextBox.Classes.Remove("invalid");
            PitNameTextBox.Classes.Remove("invalid");
            BlockNumberTextBox.Classes.Remove("invalid");

            // Проверка отдельных полей
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

            // Проверка общего количества скважин
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