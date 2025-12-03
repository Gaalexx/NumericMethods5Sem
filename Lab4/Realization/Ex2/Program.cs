using System;
using System.Diagnostics;
using ScottPlot;

namespace BoundaryValueProblem
{
    class Program
    {
        static void DrawAndSaveGraph(
            string title,
            string filename,
            double[] x,
            double[] yNumeric,
            double[] yExact
        )
        {
            Plot plt = new();
            plt.Title(title);
            plt.XLabel("x");
            plt.YLabel("y");

            var exactLine = plt.Add.ScatterLine(x, yExact);
            exactLine.Color = ScottPlot.Color.FromHex("#FF0000");
            exactLine.LegendText = "Точное решение";
            exactLine.LineWidth = 2;

            var numericLine = plt.Add.ScatterLine(x, yNumeric);
            numericLine.Color = ScottPlot.Color.FromHex("#0000FF");
            numericLine.LegendText = "Численное решение";
            numericLine.LineWidth = 2;
            numericLine.LinePattern = LinePattern.Dashed;

            plt.ShowLegend();
            //plt.Axes.SetLimitsY(-25, 25);
            plt.Axes.SetLimitsY(0, 5);
            plt.SavePng(filename, 800, 600);
        }

        static double ExactSolution(double x)
        {
            return -1 + 2.0 / x + (2.0 * (x + 1) / x) * Math.Log(Math.Abs(x + 1));
        }

        static double ExactDerivative(double x)
        {
            return -2.0 / (x * x)
                + 2.0 * Math.Log(Math.Abs(x + 1)) / x
                - 2.0 * (x + 1) / (x * x) * Math.Log(Math.Abs(x + 1))
                + 2.0 * (x + 1) / (x * (x + 1));
        }

        static double F(double x, double y, double yp)
        {
            return 2.0 * y / (x * x * (x + 1));
        }

        static void RungeKutta4(
            double x,
            double y,
            double yp,
            double h,
            out double yNext,
            out double ypNext
        )
        {
            double k1 = h * yp;
            double l1 = h * F(x, y, yp);

            double k2 = h * (yp + l1 / 2);
            double l2 = h * F(x + h / 2, y + k1 / 2, yp + l1 / 2);

            double k3 = h * (yp + l2 / 2);
            double l3 = h * F(x + h / 2, y + k2 / 2, yp + l2 / 2);

            double k4 = h * (yp + l3);
            double l4 = h * F(x + h, y + k3, yp + l3);

            yNext = y + (k1 + 2 * k2 + 2 * k3 + k4) / 6;
            ypNext = yp + (l1 + 2 * l2 + 2 * l3 + l4) / 6;
        }

        static (double[] x, double[] y) ShootingMethod(
            double x0,
            double xn,
            double y0,
            double ypn,
            int n,
            out double initialDerivative
        )
        {
            double h = (xn - x0) / n;

            double eta1 = 0.0;
            double eta2 = 1.0;

            double SolveWithEta(double eta)
            {
                double y = y0,
                    yp = eta,
                    x = x0;
                for (int i = 0; i < n; i++)
                {
                    RungeKutta4(x, y, yp, h, out y, out yp);
                    x += h;
                }
                return yp;
            }

            double phi1 = SolveWithEta(eta1) - ypn;
            double phi2 = SolveWithEta(eta2) - ypn;

            double tolerance = 1e-12;
            int maxIter = 10000;
            double eta = eta2;
            double phi = phi2;

            for (int iter = 0; iter < maxIter; iter++)
            {
                if (Math.Abs(phi) < tolerance)
                    break;

                double etaNew = eta - phi * (eta - eta1) / (phi - phi1);

                eta1 = eta;
                phi1 = phi;
                eta = etaNew;
                phi = SolveWithEta(eta) - ypn;
            }

            initialDerivative = eta;

            double[] xVals = new double[n + 1];
            double[] yVals = new double[n + 1];

            xVals[0] = x0;
            yVals[0] = y0;

            double yCurr = y0,
                ypCurr = eta;
            double xCurr = x0;

            for (int i = 1; i <= n; i++)
            {
                RungeKutta4(xCurr, yCurr, ypCurr, h, out yCurr, out ypCurr);
                xCurr += h;
                xVals[i] = xCurr;
                yVals[i] = yCurr;
            }

            return (xVals, yVals);
        }

        static (double[] x, double[] y) FiniteDifferenceMethod(
            double x0,
            double xn,
            double y0,
            double ypn,
            int n
        )
        {
            double h = (xn - x0) / n;

            double[] a = new double[n + 1];
            double[] b = new double[n + 1];
            double[] c = new double[n + 1];
            double[] d = new double[n + 1];

            double[] x = new double[n + 1];
            for (int i = 0; i <= n; i++)
                x[i] = x0 + i * h;

            b[0] = 1;
            c[0] = 0;
            d[0] = y0;
            a[0] = 0;

            for (int i = 1; i < n; i++)
            {
                double xi = x[i];
                double coef = 2.0 / (xi * xi * (xi + 1));

                a[i] = 1.0 / (h * h);
                b[i] = -2.0 / (h * h) - coef;
                c[i] = 1.0 / (h * h);
                d[i] = 0;
            }

            a[n] = -1.0 / h;
            b[n] = 1.0 / h;
            c[n] = 0;
            d[n] = ypn;

            double[] alpha = new double[n + 1];
            double[] beta = new double[n + 1];

            alpha[0] = -c[0] / b[0];
            beta[0] = d[0] / b[0];

            for (int i = 1; i <= n; i++)
            {
                double denominator = b[i] + a[i] * alpha[i - 1];
                alpha[i] = -c[i] / denominator;
                beta[i] = (d[i] - a[i] * beta[i - 1]) / denominator;
            }

            double[] y = new double[n + 1];
            y[n] = beta[n];

            for (int i = n - 1; i >= 0; i--)
            {
                y[i] = alpha[i] * y[i + 1] + beta[i];
            }

            return (x, y);
        }

        static double RungeRombergError(double yh, double y2h, int p)
        {
            return Math.Abs(yh - y2h) / (Math.Pow(2, p) - 1);
        }

        static void Main(string[] args)
        {
            double x0 = 1.0;
            double xn = 2.0;
            double y0 = 1 + 4 * Math.Log(2);
            double ypn = ExactDerivative(xn);

            int n1 = 10;
            int n2 = 20;

            Console.WriteLine("1. МЕТОД СТРЕЛЬБЫ");
            Console.WriteLine("=================\n");

            var (xShoot1, yShoot1) = ShootingMethod(x0, xn, y0, ypn, n1, out double eta1);
            var (xShoot2, yShoot2) = ShootingMethod(x0, xn, y0, ypn, n2, out double eta2);

            Console.WriteLine($"\nРешение на сетке n={n1}:");
            Console.WriteLine("┌──────────┬──────────────┬──────────────┬──────────────┐");
            Console.WriteLine("│    x     │   y_числ     │   y_точн     │   Погрешность│");
            Console.WriteLine("├──────────┼──────────────┼──────────────┼──────────────┤");

            double maxErrorShoot1 = 0;
            for (int i = 0; i <= n1; i += 2)
            {
                double exact = ExactSolution(xShoot1[i]);
                double error = Math.Abs(yShoot1[i] - exact);
                maxErrorShoot1 = Math.Max(maxErrorShoot1, error);

                Console.WriteLine(
                    $"│ {xShoot1[i], 8:F4} │ {yShoot1[i], 12:E6} │ {exact, 12:E6} │ {error, 12:E4} │"
                );
            }
            Console.WriteLine("└──────────┴──────────────┴──────────────┴──────────────┘");

            double rreShoot = RungeRombergError(yShoot2[n2], yShoot1[n1], 4);
            Console.WriteLine($"\nМаксимальная погрешность (точное решение): {maxErrorShoot1:E4}");
            Console.WriteLine($"Оценка погрешности по Рунге-Ромбергу: {rreShoot:E4}");

            Console.WriteLine("\n\n2. КОНЕЧНО-РАЗНОСТНЫЙ МЕТОД");
            Console.WriteLine("============================\n");

            var (xFd1, yFd1) = FiniteDifferenceMethod(x0, xn, y0, ypn, n1);
            var (xFd2, yFd2) = FiniteDifferenceMethod(x0, xn, y0, ypn, n2);

            Console.WriteLine($"Решение на сетке n={n1}:");
            Console.WriteLine("│    x     │   y_числ     │   y_точн     │   Погрешность│");

            double maxErrorFd1 = 0;
            for (int i = 0; i <= n1; i += 2)
            {
                double exact = ExactSolution(xFd1[i]);
                double error = Math.Abs(yFd1[i] - exact);
                maxErrorFd1 = Math.Max(maxErrorFd1, error);

                Console.WriteLine(
                    $"│ {xFd1[i], 8:F4} │ {yFd1[i], 12:E6} │ {exact, 12:E6} │ {error, 12:E4} │"
                );
            }

            double rreFd = RungeRombergError(yFd2[n2], yFd1[n1], 2);
            Console.WriteLine($"\nМаксимальная погрешность (точное решение): {maxErrorFd1:E4}");
            Console.WriteLine($"Оценка погрешности по Рунге-Ромбергу: {rreFd:E4}");

            Console.WriteLine("\n\n3. СРАВНЕНИЕ МЕТОДОВ");
            Console.WriteLine("====================\n");

            Console.WriteLine("│     Параметр       │   Метод стрельбы   │ Конечно-разностный │");
            Console.WriteLine(
                $"│ Макс. погрешность │ {maxErrorShoot1, 18:E4} │ {maxErrorFd1, 18:E4} │"
            );
            Console.WriteLine($"│ Оценка Р-Р        │ {rreShoot, 18:E4} │ {rreFd, 18:E4} │");


            if (maxErrorShoot1 < maxErrorFd1)
                Console.WriteLine("• Метод стрельбы показал более высокую точность.");
            else if (maxErrorShoot1 > maxErrorFd1)
                Console.WriteLine("• Конечно-разностный метод показал более высокую точность.");
            else
                Console.WriteLine("• Оба метода показали сравнимую точность.");

            Console.WriteLine(
                $"• Найденное начальное условие для метода стрельбы: y'(1) = {eta2:F8}"
            );
            Console.WriteLine(
                $"• Точное значение y'(1) из аналитического решения: {ExactDerivative(1.0):F8}"
            );
            Console.WriteLine(
                $"• Погрешность начального условия: {Math.Abs(eta2 - ExactDerivative(1.0)):E4}"
            );

            double[] xExact = new double[n1 + 1];
            double[] yExact = new double[n1 + 1];
            for (int i = 0; i <= n1; i++)
            {
                xExact[i] = xShoot1[i];
                yExact[i] = ExactSolution(xExact[i]);
            }

            DrawAndSaveGraph("Метод стрельбы", "shooting_method.png", xShoot1, yShoot1, yExact);
            DrawAndSaveGraph(
                "Конечно-разностный метод",
                "finite_difference_method.png",
                xFd1,
                yFd1,
                yExact
            );

            Console.WriteLine(
                "\nГрафики сохранены: shooting_method.png, finite_difference_method.png"
            );

            Process.Start(new ProcessStartInfo("shooting_method.png") { UseShellExecute = true });
            Process.Start(
                new ProcessStartInfo("finite_difference_method.png") { UseShellExecute = true }
            );
        }
    }
}
