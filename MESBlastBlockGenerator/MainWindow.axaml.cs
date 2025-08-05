using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using NLog;
using System;
using System.Globalization;
using System.Reflection;

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

        private static string GenerateHole(int row, int col, string x, string y,
                                 string blastProjectId, string pitName,
                                 int level, int blockNumber)
        {
            string holeNum = $"{row:D2}{col:D2}";
            string holeId = Guid.NewGuid().ToString();

            return $@"    <hole>
      <holeitem blast_project_Id=""{blastProjectId}"" hole_id=""{holeId}"" 
                hole_number=""{holeNum}"" hole_type_code=""Explosive"" 
                hole_material=""Взрывные скважины ВСДП"" hole_material_code=""1078066"" 
                pit_code=""{pitName}"" pit_name=""{pitName}"" level_code=""{pitName}{level}"" level_name=""{level}"" 
                block_code=""{pitName}{level}-{blockNumber}"" block_name=""{level}-{blockNumber}"" 
                blockDrilling_code=""{pitName}{pitName}{level}{level}-{blockNumber}Drill"" blockDrilling_name=""{level}-{blockNumber}"" 
                blockBlasting_code=""{pitName}{level}-{blockNumber}Blast"" blockBlasting_name=""{level}-{blockNumber}"" 
                PlannedSubdrill=""1"" ExplosiveRatioByWell=""1.252"" depth_plan=""9.5"" 
                depth_plan_eom_id=""006"" depth_plan_eom=""м"" depth_fact=""7"" 
                depth_fact_eom_id=""018"" depth_fact_eom=""пог. м"" diameter_plan=""233"" 
                diameter_eom_id=""004"" diameter_eom=""см"" diameter_fact=""233"" 
                diameter_fact_eom_id=""003"" diameter_fact_eom=""мм"" 
                x=""{x}"" y=""{y}"" z=""980.66"" x_fact=""{x}"" y_fact=""{y}"" z_fact=""980.66"" 
                isDrilling=""true"" isDefective=""false"" isDelete=""false""/>
      <planChargeMaterials>
        <material material_code=""1025160"" material_shortname=""Вещество взрывчатое Березит Э-70"" 
                  QuantityCartridgePacked=""0"" amount_eom=""кг"" is_explosive=""true"" 
                  material_density=""1200"" cup_density=""0"">
          <amounts><amount value=""500"" priority=""1""/><amount value=""600"" priority=""2""/></amounts>
        </material>
        <material material_code=""798031"" material_shortname=""Шашка-детонатор литая ПТ-П-750"" 
                  QuantityCartridgePacked=""0"" amount_eom=""кг"" is_explosive=""false"" 
                  material_density=""1200"" cup_density="""">
          <amounts><amount value=""0.75"" priority=""1""/></amounts>
        </material>
      </planChargeMaterials>
      <stemming_length_plan value=""4.59""/>
    </hole>";
        }

        private static string GenerateXmlContent(int maxRow, int maxCol, double rotationAngleDegrees, double baseX, double baseY, double distance,
                                       string pitName, int level, int blockNumber)
        {
            using var writer = new System.IO.StringWriter();
            writer.Write(@"<?xml version=""1.0""?>
<x:Envelope xmlns:x=""http://schemas.xmlsoap.org/soap/envelope/"" xmlns:tem=""http://tempuri.org/"">
  <x:Header/>
  <x:Body>
    <tem:SoapXmlRequest>
      <tem:xmlRequest>
        <tem:Message><![CDATA[<?xml version=""1.0"" encoding=""utf-8""?>
<mes_pmv messageid=""1022a282f6afb23b0f3b"" systemid=""MES"" businessid="""">
  <holes_in_blast_project>
");

            string blastProjectId = Guid.NewGuid().ToString();
            double rotationAngleRad = rotationAngleDegrees * Math.PI / 180.0;
            double cosAngle = Math.Cos(rotationAngleRad);
            double sinAngle = Math.Sin(rotationAngleRad);

            for (int row = 0; row < maxRow; row++)
            {
                for (int col = 0; col < maxCol; col++)
                {
                    double x = baseX + (col) * distance;
                    double y = baseY + (row) * distance;

                    // Применяем поворот относительно (baseX, baseY)
                    double relX = x - baseX;
                    double relY = y - baseY;

                    x = baseX + relX * cosAngle - relY * sinAngle;
                    y = baseY + relX * sinAngle + relY * cosAngle;

                    writer.Write("\n" + GenerateHole(row, col, x.ToString(CultureInfo.InvariantCulture), y.ToString(CultureInfo.InvariantCulture), blastProjectId, pitName, level, blockNumber));
                }
            }

            writer.Write(@"
  </holes_in_blast_project>
</mes_pmv>]]></tem:Message>
      </tem:xmlRequest>
    </tem:SoapXmlRequest>
  </x:Body>
</x:Envelope>");

            return writer.ToString();
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