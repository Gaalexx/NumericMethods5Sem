using System;
using MyDataStructures;

public class SecondLab
{
    struct ABCDCoefficients
    {
        public double[] a { get; private set; }
        public double[] b { get; private set; }
        public double[] c { get; private set; }
        public double[] d { get; private set; }
        public double[] borders { get; private set; }

        private double[] getCElements(in List<Tuple<double, double>> functionResults)
        {
            int n = functionResults.Count - 1;

            if (n < 1)
            {
                throw new ArgumentException("Для построения сплайна нужно как минимум 2 точки");
            }

            double[] h = new double[n + 1];

            for (int i = 1; i <= n; i++)
            {
                h[i] = functionResults[i].Item1 - functionResults[i - 1].Item1;
            }

            if (n == 1)
            {
                return new double[] { 0, 0 };
            }

            int systemSize = n - 1;
            Matrix<double> equation = new Matrix<double>(systemSize, systemSize);
            Matrix<double> b_vector = new Matrix<double>(systemSize, 1);

            for (int i = 1; i <= systemSize; i++)
            {
                int row = i - 1;

                if (i > 1)
                {
                    equation[row][row - 1] = h[i];
                }

                equation[row][row] = 2 * (h[i] + h[i + 1]);

                if (i < systemSize)
                {
                    equation[row][row + 1] = h[i + 1];
                }

                double f_diff_right =
                    (functionResults[i + 1].Item2 - functionResults[i].Item2) / h[i + 1];
                double f_diff_left =
                    (functionResults[i].Item2 - functionResults[i - 1].Item2) / h[i];
                b_vector[row][0] = 3 * (f_diff_right - f_diff_left);
            }

            var solution = Matrix<double>.LUSolutionMethod(equation, b_vector);

            double[] c_coeff = new double[n + 1];
            c_coeff[0] = 0;
            c_coeff[n] = 0;

            for (int i = 1; i <= systemSize; i++)
            {
                c_coeff[i] = solution[i - 1][0];
            }

            return c_coeff;
        }

        public ABCDCoefficients(in List<Tuple<double, double>> functionResults)
        {
            int n = functionResults.Count - 1;

            if (n < 1)
            {
                throw new ArgumentException("Для построения сплайна нужно как минимум 2 точки");
            }

            a = new double[n + 1];
            b = new double[n + 1];
            c = getCElements(in functionResults);
            d = new double[n + 1];
            borders = new double[functionResults.Count];

            double[] h = new double[n + 1];
            for (int i = 1; i <= n; i++)
            {
                h[i] = functionResults[i].Item1 - functionResults[i - 1].Item1;
            }

            for (int i = 1; i <= n; i++)
            {
                a[i] = functionResults[i - 1].Item2;

                if (i == n)
                {
                    b[i] =
                        (functionResults[i].Item2 - functionResults[i - 1].Item2) / h[i]
                        - 2.0 * h[i] * c[i - 1] / 3.0;
                    d[i] = -c[i - 1] / (3.0 * h[i]);
                }
                else
                {
                    b[i] =
                        (functionResults[i].Item2 - functionResults[i - 1].Item2) / h[i]
                        - h[i] * (c[i] + 2 * c[i - 1]) / 3.0;
                    d[i] = (c[i] - c[i - 1]) / (3.0 * h[i]);
                }
            }

            for (int i = 0; i < functionResults.Count; i++)
            {
                borders[i] = functionResults[i].Item1;
            }
        }

        public Tuple<int, int> getBorders(double x)
        {
            for (int i = 0; i < borders.Length - 1; i++)
            {
                if (borders[i] <= x && x <= borders[i + 1])
                {
                    return new Tuple<int, int>(i, i + 1);
                }
            }
            throw new Exception(
                $"Точка {x} вне границ интерполяции [{borders[0]}, {borders[borders.Length - 1]}]"
            );
        }
    }

    public static double SplinePolynomial(double x, in List<Tuple<double, double>> functionResults)
    {
        var coefficients = new ABCDCoefficients(in functionResults);

        #region debug
        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine("КОЭФФИЦИЕНТЫ КУБИЧЕСКИХ СПЛАЙНОВ");
        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine($"Количество интервалов: {coefficients.a.Length - 1}");
        Console.WriteLine();

        Console.WriteLine("№\tИнтервал\t\t\tКоэффициенты");
        Console.WriteLine("-".PadRight(80, '-'));

        for (int i = 1; i < coefficients.a.Length; i++)
        {
            Console.WriteLine(
                $"{i}\t[{functionResults[i - 1].Item1:F4}, {functionResults[i].Item1:F4}]\t"
                    + $"a={coefficients.a[i]:F6} | "
                    + $"b={coefficients.b[i]:F6} | "
                    + $"c={coefficients.c[i - 1]:F6} | "
                    + $"d={coefficients.d[i]:F6}"
            );
        }
        Console.WriteLine("=".PadRight(80, '='));
        Console.WriteLine();
        #endregion

        Tuple<int, int> borders = coefficients.getBorders(x);
        int intervalIndex = borders.Item1 + 1;

        double dx = x - functionResults[borders.Item1].Item1;
        double answer =
            coefficients.a[intervalIndex]
            + coefficients.b[intervalIndex] * dx
            + coefficients.c[intervalIndex - 1] * dx * dx
            + coefficients.d[intervalIndex] * dx * dx * dx;

        return answer;
    }
}
