using System;
using System.Net.Mail;
using MyDataStructures;

namespace Program
{
    class Program
    {
        public static double LagranzhInterpolationPolynomial(
            double x,
            in List<Tuple<double, double>> functionResults
        )
        {
            double answer = 0;
            for (int i = 0; i < functionResults.Count; i++)
            {
                double li = 1;
                for (int j = 0; j < functionResults.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    li *=
                        (x - functionResults[j].Item1)
                        / (functionResults[i].Item1 - functionResults[j].Item1);
                }
                answer += functionResults[i].Item2 * li;
            }
            return answer;
        }

        public delegate double Polynomial(double x, in List<Tuple<double, double>> coefficients);
        public delegate double MyFunction(double x);

        private double Omega(double x, in List<Tuple<double, double>> functionResults, int n)
        {
            double omega = x - functionResults[0].Item1;
            for (int i = 1; i < n; i++)
            {
                omega *= x - functionResults[i].Item1;
            }
            return omega;
        }

        private double OmegaDerivative(double x, in List<Tuple<double, double>> functionResults)
        {
            double answer = 0;
            for (int i = 0; i < functionResults.Count; i++)
            {
                double add = 1;
                for (int j = 0; j < functionResults.Count; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    add *= x - functionResults[j].Item1;
                }
                answer += add;
            }
            return answer;
        }

        private static double _devDif(in List<Tuple<double, double>> functionResults, int l, int r)
        {
            if (l == r)
            {
                return functionResults[l].Item2;
            }
            else
            {
                return (
                        _devDif(in functionResults, l, r - 1)
                        - _devDif(in functionResults, l + 1, r)
                    ) / (functionResults[l].Item1 - functionResults[r].Item1);
            }
        }

        public static double DevidedDifference(in List<Tuple<double, double>> functionResults)
        {
            if (functionResults.Count == 2)
            {
                return (functionResults[0].Item2 - functionResults[1].Item2)
                    / (functionResults[0].Item1 - functionResults[1].Item1);
            }
            else
            {
                return (
                        _devDif(in functionResults, 0, functionResults.Count - 2)
                        - _devDif(in functionResults, 1, functionResults.Count - 1)
                    )
                    / (functionResults[0].Item1 - functionResults[functionResults.Count - 1].Item1);
            }
        }

        public static double NewtonInterpolationPolynomial(
            double x,
            in List<Tuple<double, double>> functionResults
        )
        {
            double answer = functionResults[0].Item2;
            for (int i = 1; i < functionResults.Count; i++)
            {
                double add = 1;
                for (int j = 0; j < i; j++)
                {
                    add *= x - functionResults[j].Item1;
                }
                add *= _devDif(in functionResults, 0, i);
                answer += add;
            }
            return answer;
        }

        public static double ErrorState(
            double x,
            Polynomial func,
            in List<Tuple<double, double>> functionResults,
            MyFunction func1
        )
        {
            return Math.Abs(func(x, in functionResults) - func1(x));
        }

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

        public static double SplinePolynomial(
            double x,
            in List<Tuple<double, double>> functionResults
        )
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

        public static void Main(String[] args)
        {
            List<Tuple<double, double>> functionResults = new List<Tuple<double, double>>()
            {
                new Tuple<double, double>(
                    Math.PI / 8,
                    Math.Cos(Math.PI / 8) / Math.Sin(Math.PI / 8)
                ),
                new Tuple<double, double>(
                    Math.PI / 4,
                    Math.Cos(Math.PI / 4) / Math.Sin(Math.PI / 4)
                ),
                new Tuple<double, double>(
                    3 * Math.PI / 8,
                    Math.Cos(3 * Math.PI / 8) / Math.Sin(3 * Math.PI / 8)
                ),
                new Tuple<double, double>(
                    Math.PI / 2,
                    Math.Cos(Math.PI / 2) / Math.Sin(Math.PI / 2)
                ),
            };

            //Console.WriteLine(LagranzhInterpolationPolynomial(Math.PI / 3, in functionResults));
            //Console.WriteLine(NewtonInterpolationPolynomial(Math.PI / 3, in functionResults));

            List<Tuple<double, double>> functionResults2 = new List<Tuple<double, double>>()
            {
                new Tuple<double, double>(
                    Math.PI / 8,
                    Math.Cos(Math.PI / 8) / Math.Sin(Math.PI / 8)
                ),
                new Tuple<double, double>(
                    5 * Math.PI / 16,
                    Math.Cos(5 * Math.PI / 16) / Math.Sin(5 * Math.PI / 16)
                ),
                new Tuple<double, double>(
                    3 * Math.PI / 8,
                    Math.Cos(3 * Math.PI / 8) / Math.Sin(3 * Math.PI / 8)
                ),
                new Tuple<double, double>(
                    Math.PI / 2,
                    Math.Cos(Math.PI / 2) / Math.Sin(Math.PI / 2)
                ),
            };
            /* Console.WriteLine(LagranzhInterpolationPolynomial(Math.PI / 3, in functionResults2));
            Console.WriteLine(1 / Math.Tan(Math.PI / 3)); */

            List<Tuple<double, double>> functionResults3 = new List<Tuple<double, double>>()
            {
                new Tuple<double, double>(1, 2.4142),
                new Tuple<double, double>(1.9, 1.0818),
                new Tuple<double, double>(2.8, 0.50953),
                new Tuple<double, double>(3.7, 0.11836),
                new Tuple<double, double>(4.6, -0.24008),
            };

            List<Tuple<double, double>> test = new List<Tuple<double, double>>()
            {
                new Tuple<double, double>(0, 0),
                new Tuple<double, double>(1, 1.8415),
                new Tuple<double, double>(2, 2.9093),
                new Tuple<double, double>(3, 3.1411),
                new Tuple<double, double>(4, 3.2432),
            };

            Console.WriteLine(SplinePolynomial(1.5, in test));
        }
    }
}
