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
                if (i != 2)
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

                for (int j = 0; j < functionResultsList[i].Count; j++)
                {
                    plt.Add.Marker(
                        functionResultsList[i][j].Item1,
                        functionResultsList[i][j].Item2,
                        MarkerShape.FilledCircle,
                        5,
                        color[i % color.Length]
                    );
                }
            }

            return plt;
        }

        public static void Main(String[] args)
        {
            List<Tuple<double, double>> lab = new List<Tuple<double, double>>()
            {
                new Tuple<double, double>(1.0, 2.4142),
                new Tuple<double, double>(1.9, 1.0818),
                new Tuple<double, double>(2.8, 0.50953),
                new Tuple<double, double>(3.7, 0.11836),
                new Tuple<double, double>(4.6, -0.24008),
                new Tuple<double, double>(5.5, -0.66818),
            };

            var firstDegree = ThirdLab.MinimalSqaresMethod(1, in lab);
            var secondDegree = ThirdLab.MinimalSqaresMethod(2, in lab);

            var plot = drawGraphic(
                [firstDegree, secondDegree, lab],
                [Color.FromHex("FF0000"), Color.FromHex("00FF00"), Color.FromHex("0000FF")],
                "Первая степень"
            );

            plot.SavePng("plot.png", 800, 600);
            Process.Start("xdg-open", "plot.png");


            Console.WriteLine(
                $"\n\nДля первой степени: {ThirdLab.sumOfSquareErrors(in lab, firstDegree)}"
            );
            Console.WriteLine(
                $"Для второй степени: {ThirdLab.sumOfSquareErrors(in lab, secondDegree)}"
            );
        }
    }
}
