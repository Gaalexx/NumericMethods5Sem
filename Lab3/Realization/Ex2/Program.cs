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
            String name = "",
            double specialX = double.NaN
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

            string[] rainbowColors =
            {
                "#FF0000",
                "#FF7F00",
                "#FFFF00",
                "#00FF00",
                "#0000FF",
                "#4B0082",
                "#9400D3",
            };

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

                // Draw segments between tuple points with different colors
                for (int seg = 0; seg < results.Count; seg++)
                {
                    double segStart = seg == 0 ? left : results[seg - 1].Item1;
                    double segEnd = results[seg].Item1;

                    List<double> segX = new List<double>();
                    List<double> segY = new List<double>();

                    for (int i = 0; i < x.Length; i++)
                    {
                        if (x[i] >= segStart && x[i] <= segEnd)
                        {
                            segX.Add(x[i]);
                            segY.Add(y[i]);
                        }
                    }

                    if (segX.Count > 0)
                    {
                        Color segColor = Color.FromHex(rainbowColors[seg % rainbowColors.Length]);
                        plt.Add.ScatterLine(segX.ToArray(), segY.ToArray(), segColor);
                    }

                    // Add bold point at tuple coordinate
                    var marker = plt.Add.Scatter(
                        new double[] { results[seg].Item1 },
                        new double[] { results[seg].Item2 }
                    );
                    marker.Color = Color.FromHex("#000000");
                    marker.MarkerSize = 10;
                }

                // Draw remaining segment after last tuple point
                if (results.Count > 0 && results[results.Count - 1].Item1 < right)
                {
                    List<double> segX = new List<double>();
                    List<double> segY = new List<double>();

                    for (int i = 0; i < x.Length; i++)
                    {
                        if (x[i] >= results[results.Count - 1].Item1)
                        {
                            segX.Add(x[i]);
                            segY.Add(y[i]);
                        }
                    }

                    if (segX.Count > 0)
                    {
                        Color segColor = Color.FromHex(
                            rainbowColors[results.Count % rainbowColors.Length]
                        );
                        plt.Add.ScatterLine(segX.ToArray(), segY.ToArray(), segColor);
                    }
                }

                // Add special point marker
                if (!double.IsNaN(specialX))
                {
                    double specialY = func[j](specialX, in results);
                    var specialMarker = plt.Add.Scatter(
                        new double[] { specialX },
                        new double[] { specialY }
                    );
                    specialMarker.Color = Color.FromHex("#FF0000");
                    specialMarker.MarkerSize = 12;
                }
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
                "Spline Polynomial",
                x
            );

            plot.SavePng("spline.png", 800, 600);
            Process.Start("xdg-open", "spline.png");
        }
    }
}
