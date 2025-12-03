using System;
using System.Collections.Generic;
using System.Diagnostics;
using ScottPlot;

namespace Program
{
    class Program
    {
        public static Plot drawGraphic(
            in List<Tuple<string, List<double>>> results,
            double x0,
            double xn,
            double h
        )
        {
            if (results == null || results.Count == 0)
                throw new InvalidOperationException("В списке нет функций на построение графика.");

            Plot plt = new();
            plt.Title("первая лаба");
            plt.XLabel("X");
            plt.YLabel("Y");

            plt.Add.HorizontalLine(0, color: ScottPlot.Color.FromHex("#000000"), width: 2);

            var rainbowColors = new[]
            {
                ScottPlot.Color.FromHex("#FF0000"),
                ScottPlot.Color.FromHex("#FF7F00"),
                ScottPlot.Color.FromHex("#FFFF00"),
                ScottPlot.Color.FromHex("#00FF00"),
                ScottPlot.Color.FromHex("#0000FF"),
                ScottPlot.Color.FromHex("#4B0082"),
                ScottPlot.Color.FromHex("#9400D3"),
            };

            int points = (int)Math.Round((xn - x0) / h) + 1;

            for (int i = 0; i < results.Count; i++)
            {
                var ys = results[i].Item2;
                if (ys.Count < points)
                    throw new InvalidOperationException(
                        $"Для графика '{results[i].Item1}' недостаточно точек: {ys.Count} < {points}"
                    );

                double[] xs = new double[points];
                double[] yarr = new double[points];

                for (int j = 0; j < points; j++)
                {
                    xs[j] = x0 + j * h;
                    yarr[j] = ys[j];
                }

                var line = plt.Add.ScatterLine(xs, yarr);
                line.Color = rainbowColors[i % rainbowColors.Length];
                line.LegendText = results[i].Item1;
            }

            plt.ShowLegend();
            return plt;
        }

        public const double accuracy = 1e-12;

        static double ExactSolution(double x)
        {
            return (1 + Math.Exp((x * x) / 2.0)) / x;
        }

        static double ExactSolutionDerivative(double x)
        {
            double E = Math.Exp(x * x / 2.0);
            return -(1 + E) / (x * x) + E;
        }

        static void RHS(double x, double y, double yp, out double dydx, out double dypdx)
        {
            dydx = yp;
            dypdx = (x * (x * x - 1) * yp + (x * x + 1) * y) / (x * x);
        }

        static List<(double x, double y, double yp)> ExplicitEuler(
            double h,
            double x0,
            double xn,
            double y0,
            double yp0
        )
        {
            var result = new List<(double, double, double)>();
            int N = (int)Math.Round((xn - x0) / h);
            double x = x0,
                y = y0,
                yp = yp0;

            for (int k = 0; k <= N; k++)
            {
                result.Add((x, y, yp));
                if (k < N)
                {
                    RHS(x, y, yp, out double dydx, out double dypdx);
                    y += h * dydx;
                    yp += h * dypdx;
                    x += h;
                }
            }
            return result;
        }

        static List<(double x, double y, double yp)> EulerCauchy(
            double h,
            double x0,
            double xn,
            double y0,
            double yp0
        )
        {
            var result = new List<(double, double, double)>();
            int N = (int)Math.Round((xn - x0) / h);
            double x = x0,
                y = y0,
                yp = yp0;

            for (int k = 0; k <= N; k++)
            {
                result.Add((x, y, yp));
                if (k < N)
                {
                    RHS(x, y, yp, out double k1y, out double k1yp);
                    double y_pred = y + h * k1y;
                    double yp_pred = yp + h * k1yp;

                    RHS(x + h, y_pred, yp_pred, out double k2y, out double k2yp);
                    y += h * (k1y + k2y) / 2.0;
                    yp += h * (k1yp + k2yp) / 2.0;
                    x += h;
                }
            }
            return result;
        }

        static List<(double x, double y, double yp)> FirstImprovedEuler(
            double h,
            double x0,
            double xn,
            double y0,
            double yp0
        )
        {
            var result = new List<(double, double, double)>();
            int N = (int)Math.Round((xn - x0) / h);
            double x = x0,
                y = y0,
                yp = yp0;

            for (int k = 0; k <= N; k++)
            {
                result.Add((x, y, yp));
                if (k < N)
                {
                    RHS(x, y, yp, out double k1y, out double k1yp);

                    double y_mid = y + (h / 2.0) * k1y;
                    double yp_mid = yp + (h / 2.0) * k1yp;

                    RHS(x + h / 2.0, y_mid, yp_mid, out double k2y, out double k2yp);

                    y += h * k2y;
                    yp += h * k2yp;
                    x += h;
                }
            }
            return result;
        }

        static List<(double x, double y, double yp)> RungeKutta4(
            double h,
            double x0,
            double xn,
            double y0,
            double yp0
        )
        {
            var result = new List<(double, double, double)>();
            int N = (int)Math.Round((xn - x0) / h);
            double x = x0,
                y = y0,
                yp = yp0;

            for (int k = 0; k <= N; k++)
            {
                result.Add((x, y, yp));
                if (k < N)
                {
                    RHS(x, y, yp, out double k1y, out double k1yp);
                    RHS(
                        x + h / 2.0,
                        y + h * k1y / 2.0,
                        yp + h * k1yp / 2.0,
                        out double k2y,
                        out double k2yp
                    );
                    RHS(
                        x + h / 2.0,
                        y + h * k2y / 2.0,
                        yp + h * k2yp / 2.0,
                        out double k3y,
                        out double k3yp
                    );
                    RHS(x + h, y + h * k3y, yp + h * k3yp, out double k4y, out double k4yp);

                    y += h * (k1y + 2.0 * k2y + 2.0 * k3y + k4y) / 6.0;
                    yp += h * (k1yp + 2.0 * k2yp + 2.0 * k3yp + k4yp) / 6.0;
                    x += h;
                }
            }
            return result;
        }

        static List<(double x, double y, double yp)> Adams4(
            double h,
            double x0,
            double xn,
            double y0,
            double yp0
        )
        {
            var result = new List<(double, double, double)>();

            var rk4Result = RungeKutta4(h, x0, x0 + 3 * h, y0, yp0);

            for (int i = 0; i < rk4Result.Count; i++)
            {
                result.Add(rk4Result[i]);
            }

            if (result.Count < 4)
            {
                Console.WriteLine(
                    "Ошибка: Не удалось получить достаточное количество начальных точек для метода Адамса"
                );
                return result;
            }

            int lastIndex = 3;

            while (result[lastIndex].Item1 < xn - accuracy)
            {
                double x_k = result[lastIndex].Item1;
                double y_k = result[lastIndex].Item2;
                double yp_k = result[lastIndex].Item3;

                double x_k_minus1 = result[lastIndex - 1].Item1;
                double y_k_minus1 = result[lastIndex - 1].Item2;
                double yp_k_minus1 = result[lastIndex - 1].Item3;

                double x_k_minus2 = result[lastIndex - 2].Item1;
                double y_k_minus2 = result[lastIndex - 2].Item2;
                double yp_k_minus2 = result[lastIndex - 2].Item3;

                double x_k_minus3 = result[lastIndex - 3].Item1;
                double y_k_minus3 = result[lastIndex - 3].Item2;
                double yp_k_minus3 = result[lastIndex - 3].Item3;

                RHS(x_k, y_k, yp_k, out double f_k_y, out double f_k_yp);
                RHS(
                    x_k_minus1,
                    y_k_minus1,
                    yp_k_minus1,
                    out double f_k_minus1_y,
                    out double f_k_minus1_yp
                );
                RHS(
                    x_k_minus2,
                    y_k_minus2,
                    yp_k_minus2,
                    out double f_k_minus2_y,
                    out double f_k_minus2_yp
                );
                RHS(
                    x_k_minus3,
                    y_k_minus3,
                    yp_k_minus3,
                    out double f_k_minus3_y,
                    out double f_k_minus3_yp
                );

                double y_next =
                    y_k
                    + h
                        / 24.0
                        * (55 * f_k_y - 59 * f_k_minus1_y + 37 * f_k_minus2_y - 9 * f_k_minus3_y);

                double yp_next =
                    yp_k
                    + h
                        / 24.0
                        * (
                            55 * f_k_yp
                            - 59 * f_k_minus1_yp
                            + 37 * f_k_minus2_yp
                            - 9 * f_k_minus3_yp
                        );

                double x_next = x_k + h;

                result.Add((x_next, y_next, yp_next));
                lastIndex++;

                if (x_next > xn + accuracy)
                    break;
            }

            return result;
        }

        public static double RungeRombergMethod(
            double step1,
            double step2,
            double res1,
            double res2,
            double p
        )
        {
            return res1 + (res1 - res2) / (Math.Pow(step1 / step2, p) - 1);
        }

        static void RungeRombergAnalysis(double h, double x0, double xn, double y0, double yp0)
        {
            Console.WriteLine("\n=== Анализ погрешности методом Рунге-Ромберга ===");
            Console.WriteLine("Для метода Рунге-Кутты 4-го порядка (p=4):");
            Console.WriteLine($"Коэффициент уточнения: 2^p - 1 = {Math.Pow(2, 4) - 1}");
            Console.WriteLine();

            var sol_h = RungeKutta4(h, x0, xn, y0, yp0);
            var sol_h2 = RungeKutta4(h / 2.0, x0, xn, y0, yp0);

            Console.WriteLine(
                "{0,-6} {1,-12} {2,-12} {3,-12}",
                "x",
                "y(h)",
                "y(h/2)",
                "Оценка погр."
            );
            Console.WriteLine(new string('-', 60));

            for (int i = 0; i < sol_h.Count; i++)
            {
                double x = sol_h[i].x;
                double y_h = sol_h[i].y;
                double y_h2 = sol_h2[i * 2].y;

                double error_estimate = RungeRombergMethod(h, h / 2.0, y_h, y_h2, 4.0);

                Console.WriteLine(
                    "{0,-6:F1} {1,-12:E6} {2,-12:E6} {3,-12:E6}",
                    x,
                    y_h,
                    y_h2,
                    Math.Abs(y_h2 - error_estimate)
                );
            }
        }

        public static void Main(string[] args)
        {
            double h = 0.1;
            double x0 = 1.0,
                xn = 2.0;
            double y0 = ExactSolution(1.0);
            double yp0 = ExactSolutionDerivative(1.0);

            Console.WriteLine("Лабораторная работа: Численные методы решения задачи Коши");
            Console.WriteLine("==========================================================");
            Console.WriteLine();
            Console.WriteLine($"Начальные условия: y({x0}) = {y0:F6}, y'({x0}) = {yp0:F6}");
            Console.WriteLine($"Отрезок интегрирования: [{x0}, {xn}]");
            Console.WriteLine($"Шаг сетки: h = {h}");
            Console.WriteLine();

            var euler = ExplicitEuler(h, x0, xn, y0, yp0);
            var eulerCauchy = EulerCauchy(h, x0, xn, y0, yp0);
            var improvedEuler = FirstImprovedEuler(h, x0, xn, y0, yp0);
            var rk4 = RungeKutta4(h, x0, xn, y0, yp0);
            var adams = Adams4(h, x0, xn, y0, yp0);

            Console.WriteLine("Таблица результатов (значения y(x)):");
            Console.WriteLine(
                "{0,-6} {1,-12} {2,-12} {3,-12} {4,-12} {5,-12} {6,-12}",
                "x",
                "Точное",
                "Эйлер",
                "Эйл-Коши",
                "Улучш.Э",
                "РК4",
                "Адамс4"
            );
            Console.WriteLine(new string('-', 110));

            for (int i = 0; i < euler.Count; i++)
            {
                double x = euler[i].x;
                double exact = ExactSolution(x);

                Console.WriteLine(
                    "{0,-6:F1} {1,-12:F6} {2,-12:F6} {3,-12:F6} {4,-12:F6} {5,-12:F6} {6,-12:F6}",
                    x,
                    exact,
                    euler[i].y,
                    eulerCauchy[i].y,
                    improvedEuler[i].y,
                    rk4[i].y,
                    adams[i].y
                );
            }

            Console.WriteLine("\n\nТаблица абсолютных погрешностей:");
            Console.WriteLine(
                "{0,-6} {1,-12} {2,-12} {3,-12} {4,-12} {5,-12}",
                "x",
                "Эйлер",
                "Эйл-Коши",
                "Улучш.Э",
                "РК4",
                "Адамс4"
            );
            Console.WriteLine(new string('-', 100));

            for (int i = 0; i < euler.Count; i++)
            {
                double x = euler[i].x;
                double exact = ExactSolution(x);

                Console.WriteLine(
                    "{0,-6:F1} {1,-12:E6} {2,-12:E6} {3,-12:E6} {4,-12:E6} {5,-12:E6}",
                    x,
                    Math.Abs(euler[i].y - exact),
                    Math.Abs(eulerCauchy[i].y - exact),
                    Math.Abs(improvedEuler[i].y - exact),
                    Math.Abs(rk4[i].y - exact),
                    Math.Abs(adams[i].y - exact)
                );
            }

            double maxErrEuler = 0,
                maxErrEulerCauchy = 0,
                maxErrImproved = 0,
                maxErrRK4 = 0,
                maxErrAdams = 0;
            for (int i = 0; i < euler.Count; i++)
            {
                double exact = ExactSolution(euler[i].x);
                maxErrEuler = Math.Max(maxErrEuler, Math.Abs(euler[i].y - exact));
                maxErrEulerCauchy = Math.Max(maxErrEulerCauchy, Math.Abs(eulerCauchy[i].y - exact));
                maxErrImproved = Math.Max(maxErrImproved, Math.Abs(improvedEuler[i].y - exact));
                maxErrRK4 = Math.Max(maxErrRK4, Math.Abs(rk4[i].y - exact));
                maxErrAdams = Math.Max(maxErrAdams, Math.Abs(adams[i].y - exact));
            }

            Console.WriteLine("\nМаксимальные погрешности:");
            Console.WriteLine($"Эйлер: {maxErrEuler:E6}");
            Console.WriteLine($"Эйлер-Коши: {maxErrEulerCauchy:E6}");
            Console.WriteLine($"Улучшенный Эйлер: {maxErrImproved:E6}");
            Console.WriteLine($"РК4: {maxErrRK4:E6}");
            Console.WriteLine($"Адамс4: {maxErrAdams:E6}");

            RungeRombergAnalysis(h, x0, xn, y0, yp0);
            //RungeErrorEstimate(h, x0, xn, y0, yp0);

            // Построение графиков
            var graphData = new List<Tuple<string, List<double>>>();

            int points = (int)Math.Round((xn - x0) / h) + 1;

            var exactValues = new List<double>(points);
            var eulerValues = new List<double>(points);
            var eulerCauchyValues = new List<double>(points);
            var improvedEulerValues = new List<double>(points);
            var rk4Values = new List<double>(points);
            var adamsValues = new List<double>(points);

            for (int i = 0; i < points; i++)
            {
                double x = x0 + i * h;
                exactValues.Add(ExactSolution(x));
                eulerValues.Add(euler[i].y);
                eulerCauchyValues.Add(eulerCauchy[i].y);
                improvedEulerValues.Add(improvedEuler[i].y);
                rk4Values.Add(rk4[i].y);
                adamsValues.Add(adams[i].y);
            }

            graphData.Add(new Tuple<string, List<double>>("Точное", exactValues));
            graphData.Add(new Tuple<string, List<double>>("Эйлер", eulerValues));
            graphData.Add(new Tuple<string, List<double>>("Эйлер-Коши", eulerCauchyValues));
            graphData.Add(new Tuple<string, List<double>>("Улучшенный Эйлер", improvedEulerValues));
            graphData.Add(new Tuple<string, List<double>>("Рунге-Кутта 4", rk4Values));
            graphData.Add(new Tuple<string, List<double>>("Адамс 4", adamsValues));

            var plot = drawGraphic(graphData, x0, xn, h);
            string filename = "methods_comparison.png";
            plot.SavePng(filename, 1200, 800);
            Console.WriteLine($"\nГрафик сохранен в файл {filename}");

            try
            {
                Process.Start(new ProcessStartInfo(filename) { UseShellExecute = true });
            }
            catch
            {
                // если запуск файла не поддерживается в окружении — молча пропускаем
            }
        }
    }
}
