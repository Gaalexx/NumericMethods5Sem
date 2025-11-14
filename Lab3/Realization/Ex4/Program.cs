using System;
using System.Diagnostics;
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

        public static void Main(String[] args)
        {
            var lab4Test = new List<Tuple<double, double>>()
            {
                new Tuple<double, double>(0.0, 1.0),
                new Tuple<double, double>(0.1, 1.1052),
                new Tuple<double, double>(0.2, 1.2214),
                new Tuple<double, double>(0.3, 1.3499),
                new Tuple<double, double>(0.4, 1.4918),
            };

            var firstDerivate = FourthProgram.getFirstDerivate(0.2, in lab4Test);

            var secondDerivate = FourthProgram.getSecondDerivate(0.2, in lab4Test);

            System.Console.WriteLine(
                $"First derivate {firstDerivate}\nSecond derivate {secondDerivate}"
            );

            /* var graph = drawGraphic(in results, [new Color(200, 15, 150), new Color(15, 250, 100)]);
            graph.SavePng("plot.png", 800, 600);
            Process.Start("xdg-open", "plot.png"); */
        }
    }
}
