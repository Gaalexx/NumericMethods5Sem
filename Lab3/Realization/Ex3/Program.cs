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

        public static void Main(String[] args)
        {
            /* List<Tuple<double, double>> testLab3 = new List<Tuple<double, double>>()
            {
                new Tuple<double, double>(0, 0),
                new Tuple<double, double>(1.7, 1.3038),
                new Tuple<double, double>(3.4, 1.8439),
                new Tuple<double, double>(5.1, 2.2583),
                new Tuple<double, double>(6.8, 2.6077),
                new Tuple<double, double>(8.5, 2.9155),
            };

            var res1 = ThirdLab.MinimalSqaresMethod(1, in testLab3);

            for (int i = 0; i < res1.Count; i++)
            {
                Console.WriteLine($"x = {res1[i].Item1}         F{i} = {res1[i].Item2}");
            }

            var res2 = ThirdLab.MinimalSqaresMethod(2, in testLab3);

            Console.WriteLine("_________________________________________________");

            res2 = ThirdLab.MinimalSqaresMethod(2, in testLab3);
            for (int i = 0; i < res2.Count; i++)
            {
                Console.WriteLine($"x = {res2[i].Item1}         F{i} = {res2[i].Item2}");
            }

            var results = new List<List<Tuple<double, double>>>() { res1, res2, testLab3 };

            var graph = drawGraphic(
                in results,
                [new Color(200, 15, 150), new Color(15, 250, 100), new Color(50, 110, 170)]
            );
            graph.SavePng("plot.png", 800, 600);
            Process.Start("xdg-open", "plot.png");


            Console.WriteLine(ThirdLab.sumOfSquareErrors(in testLab3, in res1));
            Console.WriteLine(ThirdLab.sumOfSquareErrors(in testLab3, in res2)); */

            List<Tuple<double, double>> lab = new List<Tuple<double, double>>()
            {
                new Tuple<double, double>(1.0, 2.4142),
                new Tuple<double, double>(1.9, 1.0818),
                new Tuple<double, double>(2.8, 0.50953),
                new Tuple<double, double>(3.7, 0.11836),
                new Tuple<double, double>(4.6, -0.24008),
                new Tuple<double, double>(5.5, -0.66818),
            };

            var plot = drawGraphic(
                [ThirdLab.MinimalSqaresMethod(1, in lab), ThirdLab.MinimalSqaresMethod(2, in lab)],
                [new Color(200, 15, 150), new Color(15, 200, 150)],
                "Лабораторная работа №4"
            );

            plot.SavePng("plot.png", 800, 600);
            Process.Start("xdg-open", "plot.png");

            Console.WriteLine(
                $"Для первой степени: {ThirdLab.sumOfSquareErrors(in lab, ThirdLab.MinimalSqaresMethod(1, in lab))}"
            );
            Console.WriteLine(
                $"Для второй степени: {ThirdLab.sumOfSquareErrors(in lab, ThirdLab.MinimalSqaresMethod(2, in lab))}"
            );
        }
    }
}
