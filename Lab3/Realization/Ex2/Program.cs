using System;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
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

        public static void Main(String[] args)
        {
            List<Tuple<double, double>> functionResults = new List<Tuple<double, double>>()
            {
                new Tuple<double, double>(1.0, 2.4142),
                new Tuple<double, double>(1.9, 1.0818),
                new Tuple<double, double>(2.8, 0.50953),
                new Tuple<double, double>(3.7, 0.11836),
                new Tuple<double, double>(4.6, -0.24008),
            };
            double x = 2.66666667;

            Console.WriteLine(
                $"Значение в точке {x} = {SecondLab.SplinePolynomial(x, in functionResults)}"
            );

            var plot = drawGraphic(
                1.0,
                4.6,
                0.1,
                new List<f>() { SecondLab.SplinePolynomial },
                functionResults,
                new Color[] { ScottPlot.Color.FromHex("#0000FF") },
                "Spline Polynomial"
            );

            plot.SavePng("spline.png", 800, 600);
            Process.Start("xdg-open", "spline.png");
        }
    }
}
