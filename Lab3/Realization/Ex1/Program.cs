using System;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using MyDataStructures;
using ScottPlot;

namespace Program
{
    class Program
    {
        public delegate double f(double x, in List<Tuple<double, double>> res);

        public static Plot drawGraphic(
            double left,
            double right,
            double step,
            List<f> func,
            List<Tuple<double, double>> results,
            Color[] color,
            String name = ""
        )
        {
            Plot plt = new();
            if (name != "")
            {
                plt.Title(name);
            }
            plt.XLabel("X");
            plt.YLabel("Y");

            plt.Add.HorizontalLine(0, color: ScottPlot.Color.FromHex("#000000"), width: 2);

            int n = (int)((right - left) / step) + 1;
            for (int j = 0; j < func.Count; j++)
            {
                double[] x = new double[n];
                double[] y = new double[n];

                int index = 0;
                for (double i = left; i <= right; i += step)
                {
                    x[index] = i;
                    y[index] = func[j](i, in results);
                    index++;
                }

                plt.Add.ScatterLine(x, y, color[j]);
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

            var plot = drawGraphic(
                Math.PI / 8,
                Math.PI / 2,
                0.1,
                [FirstLab.LagranzhInterpolationPolynomial, FirstLab.NewtonInterpolationPolynomial],
                a,
                new Color[]
                {
                    ScottPlot.Color.FromHex("#FF0000"),
                    ScottPlot.Color.FromHex("#0000FF"),
                },
                "А"
            );

            plot.SavePng("plot.png", 800, 600);
            Process.Start("xdg-open", "plot.png");

            var plot1 = drawGraphic(
                Math.PI / 8,
                Math.PI / 2,
                0.1,
                [FirstLab.LagranzhInterpolationPolynomial, FirstLab.NewtonInterpolationPolynomial],
                b,
                new Color[]
                {
                    ScottPlot.Color.FromHex("#FF0000"),
                    ScottPlot.Color.FromHex("#0000FF"),
                },
                "Б"
            );

            plot1.SavePng("plot1.png", 800, 600);
            Process.Start("xdg-open", "plot1.png");

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
