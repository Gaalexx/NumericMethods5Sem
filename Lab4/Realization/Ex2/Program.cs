using System;
using ScottPlot;

namespace BoundaryValueProblem
{
    class Program
    {
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
            double k1y = yp;
            double k1yp = F(x, y, yp);

            double k2y = yp + h * k1yp / 2;
            double k2yp = F(x + h / 2, y + h * k1y / 2, yp + h * k1yp / 2);

            double k3y = yp + h * k2yp / 2;
            double k3yp = F(x + h / 2, y + h * k2y / 2, yp + h * k2yp / 2);

            double k4y = yp + h * k3yp;
            double k4yp = F(x + h, y + h * k3y, yp + h * k3yp);

            yNext = y + h * (k1y + 2 * k2y + 2 * k3y + k4y) / 6;
            ypNext = yp + h * (k1yp + 2 * k2yp + 2 * k3yp + k4yp) / 6;
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
            int maxIter = 100;
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
            double ypn = -1 + 3 * Math.Log(2);

            int n1 = 10;
            int n2 = 20;

            Console.WriteLine("1. МЕТОД СТРЕЛЬБЫ");
            Console.WriteLine("=================\n");

            var (xShoot1, yShoot1) = ShootingMethod(x0, xn, y0, ypn, n1, out double eta1);
            var (xShoot2, yShoot2) = ShootingMethod(x0, xn, y0, ypn, n2, out double eta2);

            Console.WriteLine($"Найденное начальное условие y'({x0}) = {eta1:F8} (для n={n1})");
            Console.WriteLine($"Найденное начальное условие y'({x0}) = {eta2:F8} (для n={n2})");

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
            Console.WriteLine("┌──────────┬──────────────┬──────────────┬──────────────┐");
            Console.WriteLine("│    x     │   y_числ     │   y_точн     │   Погрешность│");
            Console.WriteLine("├──────────┼──────────────┼──────────────┼──────────────┤");

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
            Console.WriteLine("└──────────┴──────────────┴──────────────┴──────────────┘");

            double rreFd = RungeRombergError(yFd2[n2], yFd1[n1], 2);
            Console.WriteLine($"\nМаксимальная погрешность (точное решение): {maxErrorFd1:E4}");
            Console.WriteLine($"Оценка погрешности по Рунге-Ромбергу: {rreFd:E4}");

            Console.WriteLine("\n\n3. СРАВНЕНИЕ МЕТОДОВ");
            Console.WriteLine("====================\n");

            Console.WriteLine("┌────────────────────┬────────────────────┬────────────────────┐");
            Console.WriteLine("│     Параметр       │   Метод стрельбы   │ Конечно-разностный │");
            Console.WriteLine("├────────────────────┼────────────────────┼────────────────────┤");
            Console.WriteLine(
                $"│ Макс. погрешность │ {maxErrorShoot1, 18:E4} │ {maxErrorFd1, 18:E4} │"
            );
            Console.WriteLine($"│ Оценка Рунге-     │ {rreShoot, 18:E4} │ {rreFd, 18:E4} │");
            Console.WriteLine("│ Ромберга          │                    │                    │");
            Console.WriteLine("└────────────────────┴────────────────────┴────────────────────┘");

            // Анализ результатов
            Console.WriteLine("\n4. ВЫВОДЫ");
            Console.WriteLine("==========\n");

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

            Console.WriteLine("\nДля завершения нажмите любую клавишу...");
            Console.ReadKey();
        }
    }
}
