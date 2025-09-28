using MESBlastBlockGenerator.Models;

namespace MESBlastBlockGenerator.Services.Interfaces
{
    public interface ICoordinateCalculatorService
    {
        (double cos, double sin) PrecalculateRotation(double rotationAngle);
        (double x, double y) CalculateCoords(InputParameters inputs, double cosAngle, double sinAngle, int row, int col);
    }
}
