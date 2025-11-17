using System;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using MyDataStructures;
using ScottPlot;

namespace Program
{
    class Program
    {
        public static Plot drawGraphic(
            in List<List<Tuple<double, double>>> functionResultsList,
            Color[] color,
            String name = ""
        )
        {
            if (functionResultsList.Count <= 0)
            {
                throw new InvalidOperationException("В списке нет функций на построение графика.");
            }
            if (color.Length != functionResultsList.Count)
            {
                throw new Exception(
                    "Количество цветов в массиве и количество списков результатов не равно"
                );
            }

            Plot plt = new();
            if (name != "")
            {
                plt.Title(name);
            }
            plt.XLabel("X");
            plt.YLabel("Y");

            plt.Add.HorizontalLine(0, color: ScottPlot.Color.FromHex("#000000"), width: 2);

            for (int i = 0; i < functionResultsList.Count; i++)
            {
                double[] x = new double[functionResultsList[i].Count];
                double[] y = new double[functionResultsList[i].Count];

                for (int j = 0; j < x.Length; j++)
                {
                    x[j] = functionResultsList[i][j].Item1;
                    y[j] = functionResultsList[i][j].Item2;
                }

                plt.Add.ScatterLine(x, y, color[i % color.Length]);
            }

            return plt;
        }

        public static double cot(double x)
        {
            return 1 / Math.Tan(x);
        }

        public static void Main(String[] args)
        {
            double x = Math.PI / 3;

            List<Tuple<double, double>> a = new List<Tuple<double, double>>()
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

            List<Tuple<double, double>> b = new List<Tuple<double, double>>()
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

            Console.WriteLine($"A - Лагранж: {FirstLab.LagranzhInterpolationPolynomial(x, in a)}");
            Console.WriteLine(
                $"A - Лагранж (погрешность): {FirstLab.ErrorState(x, FirstLab.LagranzhInterpolationPolynomial, in a, cot)}"
            );
            Console.WriteLine($"A - Ньютон: {FirstLab.NewtonInterpolationPolynomial(x, in a)}");
            Console.WriteLine(
                $"A - Ньютон (погрешность): {FirstLab.ErrorState(x, FirstLab.NewtonInterpolationPolynomial, in a, cot)}"
            );

            Console.WriteLine($"Б - Лагранж: {FirstLab.LagranzhInterpolationPolynomial(x, in b)}");
            Console.WriteLine(
                $"Б - Лагранж (погрешность): {FirstLab.ErrorState(x, FirstLab.LagranzhInterpolationPolynomial, in b, cot)}"
            );
            Console.WriteLine($"Б - Ньютон: {FirstLab.NewtonInterpolationPolynomial(x, in b)}");
            Console.WriteLine(
                $"Б - Ньютон (погрешность): {FirstLab.ErrorState(x, FirstLab.NewtonInterpolationPolynomial, in b, cot)}"
            );
        }
    }
}
