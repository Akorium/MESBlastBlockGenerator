using MESBlastBlockGenerator.Models;
using MESBlastBlockGenerator.Services.Interfaces;
using System;

namespace MESBlastBlockGenerator.Services
{
    /// <summary>
    /// This service is responsible for calculating coordinates based on rotation and distance.
    /// </summary>
    public class CoordinateCalculatorService : ICoordinateCalculatorService
    {
        /// <summary>
        /// Precalculates the rotation angle into cosine and sine values.
        /// </summary>
        /// <param name="rotationAngle">The angle of rotation in degrees.</param>
        /// <returns>A tuple containing the cosine and sine values of the rotation angle.</returns>
        public (double cos, double sin) PrecalculateRotation(double rotationAngle)
        {
            var angleRad = rotationAngle * Math.PI / 180.0;
            return (Math.Cos(angleRad), Math.Sin(angleRad));
        }

        /// <summary>
        /// Calculates the coordinates of a point based on the base coordinates, distance, rotation angle, and row/column.
        /// </summary>
        /// <param name="inputs">The input parameters for the calculation.</param>
        /// <param name="cosAngle">The cosine value of the rotation angle.</param>
        /// <param name="sinAngle">The sine value of the rotation angle.</param>
        /// <param name="row">The row number.</param>
        /// <param name="col">The column number.</param>
        /// <returns>A tuple containing the x and y coordinates of the point.</returns>
        public (double x, double y) CalculateCoords(InputParameters inputs, double cosAngle, double sinAngle, int row, int col)
        {
            double x = inputs.BaseX + col * inputs.Distance;
            double y = inputs.BaseY - row * inputs.Distance;

            double relX = x - inputs.BaseX;
            double relY = y - inputs.BaseY;

            x = inputs.BaseX + relX * cosAngle - relY * sinAngle;
            y = inputs.BaseY - relX * sinAngle + relY * cosAngle;

            return (x, y);
        }
    }
}
