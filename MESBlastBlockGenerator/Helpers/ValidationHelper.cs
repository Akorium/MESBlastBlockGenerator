using Avalonia.Controls;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace MESBlastBlockGenerator.Helpers
{
    public static class ValidationHelper
    {
        // Временное ограничение пока нет конкретных данных по максимальному возможному объёму блока
        private const int maxWellsCount = 5000;

        public static int ValidatePositiveInt(TextBox box, string fieldName)
        {
            if (!int.TryParse(box.Text, out int value) || value <= 0)
            {
                box.Classes.Add("invalid");
                throw new ValidationException($"{fieldName} должно быть целым положительным числом");
            }
            return value;
        }
        public static double ValidateDouble(TextBox box, string fieldName)
        {
            if (!double.TryParse(box.Text, out double value))
            {
                box.Classes.Add("invalid");
                throw new ValidationException($"{fieldName} имеет некорректное значение");
            }
            return value;
        }
        public static double ValidatePositiveDouble(TextBox box, string fieldName)
        {
            if (!double.TryParse(box.Text, out double value) || value <= 0)
            {
                box.Classes.Add("invalid");
                throw new ValidationException($"{fieldName} должно быть положительным числом");
            }
            return value;
        }
        public static string ValidateString(TextBox box, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(box.Text))
            {
                box.Classes.Add("invalid");
                throw new ValidationException($"{fieldName} имеет некорректное значение");
            }
            string value = box.Text;
            return value;
        }
        public static void ValidateWellsCount(TextBox[] textBoxes, int maxCol, int maxRow)
        {
            if (maxCol * maxRow > maxWellsCount)
            {
                foreach (var textBox in textBoxes)
                {
                    textBox.Classes.Add("invalid");
                }
                throw new ValidationException($"Превышено допустимое количество скважин: {maxWellsCount}");
            }
        }
        public static void ClearValidation(params Control[] controls)
        {
            foreach (var control in controls)
            {
                control.Classes.Remove("invalid");
            }
        }
    }
}
