using System;
using System.Collections.Generic;
using System.Diagnostics;
using ScottPlot;

namespace Program
{
    class Program
    {
        public static Plot drawGraphic(
            in List<Tuple<String, List<double>>> results,
            double x0,
            double xn,
            double h
        )
        {
            if (results.Count <= 0)
            {
                throw new InvalidOperationException("В списке нет функций на построение графика.");
            }

            Plot plt = new();
            plt.Title("первая лаба");
            plt.XLabel("X");
            plt.YLabel("Y");

            plt.Add.HorizontalLine(0, color: ScottPlot.Color.FromHex("#000000"), width: 2);

            var rainbowColors = new[] {
                ScottPlot.Color.FromHex("#FF0000"),
                ScottPlot.Color.FromHex("#FF7F00"),
                ScottPlot.Color.FromHex("#FFFF00"),
                ScottPlot.Color.FromHex("#00FF00"),
                ScottPlot.Color.FromHex("#0000FF"),
                ScottPlot.Color.FromHex("#4B0082"),
                ScottPlot.Color.FromHex("#9400D3")
            };

            for (int i = 0; i < results.Count; i++)
            {
                int begin = 0;
                int end = (int)((xn - x0) / h);

                double[] x = new double[end];
                double[] y = new double[end];

                while (begin < end)
                {
                    x[begin] = x0 + begin * h;
                    y[begin] = results[i].Item2[begin];
                    begin++;
                }

                var line = plt.Add.ScatterLine(x, y);
                line.Color = rainbowColors[i % rainbowColors.Length];
                line.LegendText = results[i].Item1;
            }

            plt.ShowLegend();

            return plt;
        }

        public const double accuracy = 0.000001;

        static double ExactSolution(double x)
        {
            return (1 + Math.Exp(Math.Pow(x, -2))) / (x * x);
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
            double x = x0,
                y = y0,
                yp = yp0;

            while (x <= xn + accuracy)
            {
                result.Add((x, y, yp));
                RHS(x, y, yp, out double dydx, out double dypdx);
                y += h * dydx;
                yp += h * dypdx;
                x += h;
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
            double x = x0,
                y = y0,
                yp = yp0;

            while (x <= xn + accuracy)
            {
                result.Add((x, y, yp));

                RHS(x, y, yp, out double k1y, out double k1yp);
                double y_pred = y + h * k1y;
                double yp_pred = yp + h * k1yp;

                RHS(x + h, y_pred, yp_pred, out double k2y, out double k2yp);
                y += h * (k1y + k2y) / 2;
                yp += h * (k1yp + k2yp) / 2;
                x += h;
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
            double x = x0,
                y = y0,
                yp = yp0;

            while (x <= xn + accuracy)
            {
                result.Add((x, y, yp));

                RHS(x, y, yp, out double k1y, out double k1yp);

                double y_mid = y + (h / 2) * k1y;
                double yp_mid = yp + (h / 2) * k1yp;

                RHS(x + h / 2, y_mid, yp_mid, out double k2y, out double k2yp);

                y += h * k2y;
                yp += h * k2yp;
                x += h;
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
            double x = x0,
                y = y0,
                yp = yp0;

            while (x <= xn + accuracy)
            {
                result.Add((x, y, yp));

                RHS(x, y, yp, out double k1y, out double k1yp);
                RHS(x + h / 2, y + h * k1y / 2, yp + h * k1yp / 2, out double k2y, out double k2yp);
                RHS(x + h / 2, y + h * k2y / 2, yp + h * k2yp / 2, out double k3y, out double k3yp);
                RHS(x + h, y + h * k3y, yp + h * k3yp, out double k4y, out double k4yp);

                y += h * (k1y + 2 * k2y + 2 * k3y + k4y) / 6;
                yp += h * (k1yp + 2 * k2yp + 2 * k3yp + k4yp) / 6;
                x += h;
            }
            return result;
        }

        static void RungeRombergAnalysis(double h, double x0, double xn, double y0, double yp0)
        {
            Console.WriteLine("\n=== Анализ погрешности методом Рунге-Ромберга ===");
            Console.WriteLine("Для метода Рунге-Кутты 4-го порядка (p=4):");
            Console.WriteLine($"Коэффициент уточнения: 2^p - 1 = {Math.Pow(2, 4) - 1}");
            Console.WriteLine();

            var sol_h = RungeKutta4(h, x0, xn, y0, yp0);
            var sol_h2 = RungeKutta4(h / 2, x0, xn, y0, yp0);
            var sol_h4 = RungeKutta4(h / 4, x0, xn, y0, yp0);

            Console.WriteLine(
                "{0,-6} {1,-12} {2,-12} {3,-12} {4,-12}",
                "x",
                "y(h)",
                "y(h/2)",
                "y(h/4)",
                "Оценка погр."
            );
            Console.WriteLine(new string('-', 60));

            for (int i = 0; i < sol_h.Count; i++)
            {
                double x = sol_h[i].x;
                double y_h = sol_h[i].y;
                double y_h2 = sol_h2[i * 2].y;
                double y_h4 = sol_h4[i * 4].y;

                // Оценка погрешности по Рунге-Ромбергу
                double error_estimate = Math.Abs(y_h2 - y_h) / 15;

                Console.WriteLine(
                    "{0,-6:F1} {1,-12:E6} {2,-12:E6} {3,-12:E6} {4,-12:E6}",
                    x,
                    y_h,
                    y_h2,
                    y_h4,
                    error_estimate
                );
            }

            // Вычисляем порядок метода
            //Console.WriteLine("\nОценка фактического порядка точности:");
            double error_h = 0,
                error_h2 = 0;
            for (int i = 0; i < sol_h.Count; i++)
            {
                double exact = ExactSolution(sol_h[i].x);
                error_h = Math.Max(error_h, Math.Abs(sol_h[i].y - exact));
                error_h2 = Math.Max(error_h2, Math.Abs(sol_h2[i * 2].y - exact));
            }

            /* double p_estimated = Math.Log(error_h / error_h2) / Math.Log(2);
            Console.WriteLine($"Макс. погрешность при h: {error_h:E6}");
            Console.WriteLine($"Макс. погрешность при h/2: {error_h2:E6}");
            Console.WriteLine($"Оцененный порядок метода: {p_estimated:F2}"); */
        }

        // Метод Рунге для оценки погрешности (упрощенный)
        static void RungeErrorEstimate(double h, double x0, double xn, double y0, double yp0)
        {
            Console.WriteLine("\n=== Оценка погрешности методом Рунге ===");

            var sol1 = RungeKutta4(h, x0, xn, y0, yp0);
            var sol2 = RungeKutta4(h / 2, x0, xn, y0, yp0);

            Console.WriteLine(
                "{0,-6} {1,-15} {2,-15} {3,-15}",
                "x",
                "Абс. погр. (h)",
                "Абс. погр. (h/2)",
                "Оценка Рунге"
            );
            Console.WriteLine(new string('-', 60));

            double max_error_h = 0,
                max_error_h2 = 0,
                max_runge_estimate = 0;

            for (int i = 0; i < sol1.Count; i++)
            {
                double x = sol1[i].x;
                double exact = ExactSolution(x);
                double error_h = Math.Abs(sol1[i].y - exact);
                double error_h2 = Math.Abs(sol2[i * 2].y - exact);
                double runge_estimate = Math.Abs(sol2[i * 2].y - sol1[i].y) / 15;

                max_error_h = Math.Max(max_error_h, error_h);
                max_error_h2 = Math.Max(max_error_h2, error_h2);
                max_runge_estimate = Math.Max(max_runge_estimate, runge_estimate);

                Console.WriteLine(
                    "{0,-6:F1} {1,-15:E6} {2,-15:E6} {3,-15:E6}",
                    x,
                    error_h,
                    error_h2,
                    runge_estimate
                );
            }

            /* Console.WriteLine("\nМаксимальные значения:");
            Console.WriteLine($"Макс. абс. погр. (h): {max_error_h:E6}");
            Console.WriteLine($"Макс. абс. погр. (h/2): {max_error_h2:E6}");
            Console.WriteLine($"Макс. оценка Рунге: {max_runge_estimate:E6}");
            Console.WriteLine(
                $"Отношение погрешностей: {max_error_h / max_error_h2:F2} (ожидается ~16 для p=4)"
            ); */
        }

        public static void Main(string[] args)
        {
            double h = 0.1;
            double x0 = 1,
                xn = 2;
            double y0 = 1 + Math.Exp(1);
            double yp0 = 2 * Math.Exp(1) - 1;

            Console.WriteLine("Лабораторная работа: Численные методы решения задачи Коши");
            Console.WriteLine("==========================================================");
            Console.WriteLine();
            Console.WriteLine($"Начальные условия: y({x0}) = {y0:F6}, y'({x0}) = {yp0:F6}");
            Console.WriteLine($"Отрезок интегрирования: [{x0}, {xn}]");
            Console.WriteLine($"Шаг сетки: h = {h}");
            Console.WriteLine();

            // Вычисление всеми методами
            var euler = ExplicitEuler(h, x0, xn, y0, yp0);
            var eulerCauchy = EulerCauchy(h, x0, xn, y0, yp0);
            var improvedEuler = FirstImprovedEuler(h, x0, xn, y0, yp0);
            var rk4 = RungeKutta4(h, x0, xn, y0, yp0);

            Console.WriteLine("Таблица результатов (значения y(x)):");
            Console.WriteLine(
                "{0,-6} {1,-12} {2,-12} {3,-12} {4,-12} {5,-12}",
                "x",
                "Точное",
                "Эйлер",
                "Эйл-Коши",
                "Улучш.Э",
                "РК4"
            );
            Console.WriteLine(new string('-', 100));

            for (int i = 0; i < euler.Count; i++)
            {
                double x = euler[i].x;
                double exact = ExactSolution(x);

                Console.WriteLine(
                    "{0,-6:F1} {1,-12:F6} {2,-12:F6} {3,-12:F6} {4,-12:F6} {5,-12:F6}",
                    x,
                    exact,
                    euler[i].y,
                    eulerCauchy[i].y,
                    improvedEuler[i].y,
                    rk4[i].y
                );
            }

            Console.WriteLine("\n\nТаблица абсолютных погрешностей:");
            Console.WriteLine(
                "{0,-6} {1,-12} {2,-12} {3,-12} {4,-12}",
                "x",
                "Эйлер",
                "Эйл-Коши",
                "Улучш.Э",
                "РК4"
            );
            Console.WriteLine(new string('-', 90));

            for (int i = 0; i < euler.Count; i++)
            {
                double x = euler[i].x;
                double exact = ExactSolution(x);

                Console.WriteLine(
                    "{0,-6:F1} {1,-12:E2} {2,-12:E2} {3,-12:E2} {4,-12:E2}",
                    x,
                    Math.Abs(euler[i].y - exact),
                    Math.Abs(eulerCauchy[i].y - exact),
                    Math.Abs(improvedEuler[i].y - exact),
                    Math.Abs(rk4[i].y - exact)
                );
            }

            double maxErrEuler = 0,
                maxErrEulerCauchy = 0,
                maxErrImproved = 0;
            double maxErrRK4 = 0;

            for (int i = 0; i < euler.Count; i++)
            {
                double exact = ExactSolution(euler[i].x);
                maxErrEuler = Math.Max(maxErrEuler, Math.Abs(euler[i].y - exact));
                maxErrEulerCauchy = Math.Max(maxErrEulerCauchy, Math.Abs(eulerCauchy[i].y - exact));
                maxErrImproved = Math.Max(maxErrImproved, Math.Abs(improvedEuler[i].y - exact));
                maxErrRK4 = Math.Max(maxErrRK4, Math.Abs(rk4[i].y - exact));
            }

            RungeRombergAnalysis(h, x0, xn, y0, yp0);
            RungeErrorEstimate(h, x0, xn, y0, yp0);

            // Построение графиков
            var graphData = new List<Tuple<String, List<double>>>();
            
            var exactValues = new List<double>();
            for (int i = 0; i < euler.Count; i++)
                exactValues.Add(ExactSolution(euler[i].x));
            graphData.Add(new Tuple<String, List<double>>("Точное", exactValues));

            var eulerValues = new List<double>();
            for (int i = 0; i < euler.Count; i++)
                eulerValues.Add(euler[i].y);
            graphData.Add(new Tuple<String, List<double>>("Эйлер", eulerValues));

            var eulerCauchyValues = new List<double>();
            for (int i = 0; i < eulerCauchy.Count; i++)
                eulerCauchyValues.Add(eulerCauchy[i].y);
            graphData.Add(new Tuple<String, List<double>>("Эйлер-Коши", eulerCauchyValues));

            var improvedEulerValues = new List<double>();
            for (int i = 0; i < improvedEuler.Count; i++)
                improvedEulerValues.Add(improvedEuler[i].y);
            graphData.Add(new Tuple<String, List<double>>("Улучшенный Эйлер", improvedEulerValues));

            var rk4Values = new List<double>();
            for (int i = 0; i < rk4.Count; i++)
                rk4Values.Add(rk4[i].y);
            graphData.Add(new Tuple<String, List<double>>("Рунге-Кутта 4", rk4Values));

            var plot = drawGraphic(graphData, x0, xn, h);
            string filename = "methods_comparison.png";
            plot.SavePng(filename, 1200, 800);
            Console.WriteLine($"\nГрафик сохранен в файл {filename}");
            
            Process.Start(new ProcessStartInfo(filename) { UseShellExecute = true });
        }
    }
}
