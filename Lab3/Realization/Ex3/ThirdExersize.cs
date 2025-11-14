using System;
using MyDataStructures;

public class ThirdLab
{
    public static List<Tuple<double, double>> MinimalSqaresMethod(
        int polynomialDegree,
        in List<Tuple<double, double>> functionResults
    )
    {
        int coefficientsCount = polynomialDegree + 1;
        var system = new Matrix<double>(coefficientsCount, coefficientsCount);
        var b = new Matrix<double>(coefficientsCount, 1);

        int maxPower = 2 * polynomialDegree;
        double[] powerSums = new double[maxPower + 1];

        for (int power = 0; power <= maxPower; power++)
        {
            double sum = 0;
            for (int j = 0; j < functionResults.Count; j++)
            {
                sum += Math.Pow(functionResults[j].Item1, power);
            }
            powerSums[power] = sum;
        }

        for (int row = 0; row < coefficientsCount; row++)
        {
            for (int col = 0; col < coefficientsCount; col++)
            {
                system[row][col] = powerSums[row + col];
            }
        }

        for (int row = 0; row < coefficientsCount; row++)
        {
            double sum = 0;
            for (int j = 0; j < functionResults.Count; j++)
            {
                sum += functionResults[j].Item2 * Math.Pow(functionResults[j].Item1, row);
            }
            b[row][0] = sum;
        }

        var coefficients = Matrix<double>.LUSolutionMethod(system, b);

        var approximatedValues = new List<Tuple<double, double>>();
        for (int i = 0; i < functionResults.Count; i++)
        {
            double x = functionResults[i].Item1;
            double value = 0;
            for (int j = 0; j < coefficientsCount; j++)
            {
                value += coefficients[j][0] * Math.Pow(x, j);
            }
            approximatedValues.Add(new Tuple<double, double>(x, value));
        }

        return approximatedValues;
    }

    public static double sumOfSquareErrors(
        in List<Tuple<double, double>> XY,
        in List<Tuple<double, double>> XF
    )
    {
        double result = 0;
        for (int i = 0; i < XY.Count; i++)
        {
            result += Math.Pow(Math.Abs(XY[i].Item2 - XF[i].Item2), 2);
        }
        return result;
    }
}
