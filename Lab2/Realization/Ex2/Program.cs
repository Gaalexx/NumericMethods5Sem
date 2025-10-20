using System;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using MyDataStructures;
using ScottPlot;
using ScottPlot.Interactivity.UserActionResponses;

namespace Program
{
    class Program
    {
        const int MAX_ITERATIONS = 1000000;

        public delegate double FunctionType(List<double> xi);

        public static double f1(List<double> xi)
        {
            return xi[0] - Math.Cos(xi[1]) - 1; //-1 чтобы функция имела вид f1(x1, x2) = 0;
        }

        public static double f1X1(List<double> xi)
        {
            return Math.Cos(xi[1]) + 1;
        }

        public static double f1X2(List<double> xi)
        {
            return Math.Acos(xi[0] - 1);
        }

        public static double f2X1(List<double> xi)
        {
            return Math.Pow(xi[1] - 1, 10) - 1;
        }

        public static double f2X2(List<double> xi)
        {
            return 1 + Math.Log10(xi[0] + 1);
        }

        public static double f2(List<double> xi)
        {
            return xi[1] - Math.Log10(xi[0] + 1) - 1; //то же самое
        }

        public static double df1dx1(List<double> xi)
        {
            return 1;
        }

        public static double df1dx2(List<double> xi)
        {
            return Math.Sin(xi[1]);
        }

        public static double df2dx1(List<double> xi)
        {
            return -1.0 / (Math.Log(10) * (xi[0] + 1));
        }

        public static double df2dx2(List<double> xi)
        {
            return 1;
        }

        public static double phi1(List<double> xi)
        {
            return Math.Cos(xi[1]) + 1;
        }

        public static double phi2(List<double> xi)
        {
            return Math.Log10(xi[0] + 1) + 1;
        }

        public static Plot drawPlot(FunctionType cb1, FunctionType cb2)
        {
            Plot plot = new();

            double[] x1 = new double[50];
            double[] y1 = new double[50];

            double[] x2 = new double[50];
            double[] y2 = new double[50];

            plot.Add.HorizontalLine(0, color: ScottPlot.Color.FromHex("#000000"), width: 2);
            plot.Add.VerticalLine(0, color: ScottPlot.Color.FromHex("#000000"), width: 2);

            for (int i = 0; i < x1.Length; i++)
            {
                x1[i] = i * 0.1 - 2;
                y1[i] = cb1(new List<double> { y1[i], x1[i] });
            }

            for (int i = 0; i < x2.Length; i++)
            {
                x2[i] = i * 0.1;
                y2[i] = cb2(new List<double> { x2[i], y2[i] });
            }

            plot.Axes.SetLimits(-2.5, 2.5, 0, 2.5);
            plot.Add.ScatterLine(x1, y1);
            plot.Add.ScatterLine(y2, x2);

            return plot;
        }

        public static bool lessThanEpsilon(ref List<double> iA, ref List<double> iAp, double eps)
        {
            if (iA.Count != iAp.Count)
            {
                throw new Exception("Размер векторов должен быть одинаковым!");
            }
            for (int i = 0; i < iA.Count; i++)
            {
                if (Math.Abs(iA[i] - iAp[i]) >= eps)
                {
                    return false;
                }
            }
            return true;
        }

        public static List<double> newtoneMethod(
            List<FunctionType> listOfFunctions,
            List<FunctionType> listOfDerivatives,
            List<double> innitApprx,
            double eps
        )
        {
            List<double> innitApprxPrev = new List<double>(innitApprx);
            innitApprxPrev[0] += 100; //чтобы не выходил из цикла
            int iterationsCount = 0;

            while (
                !lessThanEpsilon(ref innitApprx, ref innitApprxPrev, eps)
                && iterationsCount++ < MAX_ITERATIONS
            )
            {
                Matrix<double> system = new Matrix<double>(
                    listOfFunctions.Count,
                    listOfFunctions.Count
                );
                Matrix<double> b = new Matrix<double>(listOfFunctions.Count, 1);
                for (int i = 0; i < system.Rows; i++)
                {
                    for (int j = 0; j < system.Cols; j++)
                    {
                        system[i][j] = listOfDerivatives[i * system.Cols + j](innitApprx);
                    }
                }
                for (int i = 0; i < system.Rows; i++)
                {
                    b[i][0] = -listOfFunctions[i](innitApprx);
                }

                double det = system[0][0] * system[1][1] - system[1][0] * system[0][1];
                if (det == 0)
                {
                    return new List<double>(0);
                }
                innitApprxPrev = innitApprx.ToList<double>();

                double detX = b[0][0] * system[1][1] - b[1][0] * system[0][1]; //с помощью формулы
                double detY = system[0][0] * b[1][0] - system[1][0] * b[0][0];
                innitApprx[0] = innitApprx[0] + (detX / det);
                innitApprx[1] = innitApprx[1] + (detY / det);

                /* var res = Matrix<double>.LUSolutionMethod(system, b); //решение системы другим методом

                innitApprxPrev = innitApprx.ToList<double>();

                for (int i = 0; i < innitApprx.Count; i++)
                {
                    innitApprx[i] = innitApprx[i] + res[i][0];
                } */
                //Console.WriteLine($"Iteration {iterationsCount}");
            }
            Console.WriteLine($"{iterationsCount} итераций");
            return innitApprx;
        }

        public static List<double> iterationalMethod(
            List<FunctionType> listOfPhi,
            List<double> innitApprx,
            double eps
        )
        {
            List<double> innitApprxPrev = innitApprx.ToList<double>();
            innitApprxPrev[0] += 100;
            int iterationsCount = 0;
            while (
                !lessThanEpsilon(ref innitApprx, ref innitApprxPrev, eps)
                && iterationsCount++ < MAX_ITERATIONS
            )
            {
                innitApprxPrev = innitApprx.ToList<double>();

                for (int i = 0; i < innitApprx.Count; i++)
                {
                    innitApprx[i] = listOfPhi[i](innitApprxPrev);
                }
            }
            Console.WriteLine($"{iterationsCount} итераций");
            return innitApprx;
        }

        public static void Main(String[] args)
        {
            List<FunctionType> functions = new List<FunctionType> { f1, f2 };
            List<FunctionType> functionXTypes = new List<FunctionType>
            {
                df1dx1,
                df1dx2,
                df2dx1,
                df2dx2,
            };

            var plot = drawPlot(f1X1, f2X2);
            String path = "Plot.png";
            plot.SavePng(path, 800, 600);

            Process.Start("xdg-open", path);

            String? input = null;

            Console.WriteLine("Введите начальное приближение x и y...");
            input = Console.ReadLine();
            List<double> approx = new double[2].ToList<double>();
            if (input == null)
            {
                Console.WriteLine("Начальное приближение не дано.");
                return;
            }
            else
            {
                string[] parts = input.Split(' ');
                if (parts.Length < 2)
                {
                    Console.WriteLine("Начальное приближение не дано.");
                    return;
                }
                else
                {
                    approx[0] = double.Parse(parts[0]); //тут почему-то outOfrange exception92
                    approx[1] = double.Parse(parts[1]);
                }
            }
            Console.WriteLine("Введите эпсилон...");
            input = Console.ReadLine();
            double eps;
            if (input == null)
            {
                Console.WriteLine("Эпсилон не дан.");
                return;
            }
            else
            {
                eps = double.Parse(input);
            }

            Console.WriteLine("Метод ньютона:");
            var res = newtoneMethod(functions, functionXTypes, approx, eps);

            for (int i = 0; i < res.Count; i++)
            {
                Console.WriteLine($"x{i + 1} = {res[i]}");
            }

            Console.WriteLine("Метод итераций:");
            res = iterationalMethod(new List<FunctionType> { phi1, phi2 }, approx, eps);
            for (int i = 0; i < res.Count; i++)
            {
                Console.WriteLine($"x{i + 1} = {res[i]}");
            }
            return;
        }
    }
}
