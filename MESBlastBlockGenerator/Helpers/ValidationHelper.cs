using Avalonia.Controls;
using System.ComponentModel.DataAnnotations;

namespace MESBlastBlockGenerator.Helpers
{
    public static class ValidationHelper
    {
        // Временное ограничение пока нет конкретных данных по максимальному возможному объёму блока
        private const int maxWellsCount = 5000;

        public static bool ValidatePositiveInt(TextBox box, out int value, string fieldName)
        {
            if (!int.TryParse(box.Text, out value) || value <= 0)
            {
                box.Classes.Add("invalid");
                throw new ValidationException($"{fieldName} должно быть целым положительным числом");
            }
            return true;
        }
        public static bool ValidateDouble(TextBox box, out double value, string fieldName)
        {
            if (!double.TryParse(box.Text, out value))
            {
                box.Classes.Add("invalid");
                throw new ValidationException($"{fieldName} имеет некорректное значение");
            }
            return true;
        }
        public static bool ValidateString(TextBox box, out string value, string fieldName)
        {
            if (string.IsNullOrWhiteSpace(box.Text))
            {
                box.Classes.Add("invalid");
                throw new ValidationException($"{fieldName} имеет некорректное значение");
            }
            value = box.Text;
            return true;
        }
        public static bool ValidateWellsCount(TextBox[] textBoxes, int maxCol, int maxRow)
        {
            if (maxCol * maxRow > maxWellsCount)
            {
                foreach (var textBox in textBoxes)
                {
                    textBox.Classes.Add("invalid");
                }
                throw new ValidationException($"Превышено допустимое количество скважин: {maxWellsCount}");
            }
            return true;
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
