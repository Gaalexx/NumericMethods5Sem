using System;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;
using ScottPlot;

class Program
{
    public delegate double FunctionToSolve(double arg);
    public const int MAX_ITERATIONS = 1000000000;

    public static double derivative(double arg)
    {
        return 3 * arg * arg + 2 * arg - 1;
    }

    public static double function(double arg)
    {
        return arg * arg * arg + arg * arg - arg - 0.5;
    }
    public static double phi(double arg)
    {
        return Math.Sqrt((arg + 0.5) / (arg + 1));
    }

    public static double xFunc(double arg)
    {
        return arg;
    }

    public static double iterationalMethod(double startEq, double epsilon)
    {
        int iterationsCount = 0;
        double oldRes = startEq;
        double curRes = 0;

        while (iterationsCount++ < MAX_ITERATIONS)
        {
            curRes = phi(oldRes);

            if (Math.Abs(curRes - oldRes) < epsilon && Math.Abs(function(curRes)) < epsilon)
            {
                Console.WriteLine($"Метод сошелся за {iterationsCount} итераций");
                return curRes;
            }

            oldRes = curRes;
        }

        throw new InvalidOperationException($"Метод не сошелся за {MAX_ITERATIONS} итераций");
    }

    public static double newtoneMethod(double startEq, double epsilon)
    {
        int iterationsCount = 0;
        double oldRes = startEq,
            curRes = 0;
        while (iterationsCount++ < MAX_ITERATIONS)
        {
            curRes = oldRes - function(oldRes) / derivative(oldRes);

            if (Math.Abs(curRes - oldRes) < epsilon)
            {
                Console.WriteLine($"Метод сошелся за {iterationsCount} итераций");
                return curRes;
            }
        }
        throw new InvalidOperationException($"Метод не сошелся за {MAX_ITERATIONS} итераций");
    }

    public static Plot drawPlot(List<FunctionToSolve> ftS, String name = "")
    {
        if (ftS.Count <= 0)
        {
            throw new InvalidOperationException("В списке нет функций на построение графика.");
        }

        Plot plt = new();
        if (name != "")
        {
            plt.Title(name);
        }
        plt.XLabel("X");
        plt.YLabel("Y");

        plt.Add.HorizontalLine(0, color: ScottPlot.Color.FromHex("#000000"), width: 2);

        for (int i = 0; i < ftS.Count; i++)
        {
            double[] x = new double[75];
            double[] y = new double[75];

            for (int j = 0; j < x.Length; j++)
            {
                x[j] = j * 0.05 - 2;
                y[j] = ftS[i](x[j]);
            }

            plt.Add.Scatter(x, y);
        }

        return plt;
    }

    static void Main()
    {
        var plotFunc = drawPlot(new List<FunctionToSolve> { function }, "function");
        var phiXFunc = drawPlot(new List<FunctionToSolve> { phi, xFunc }, "Phi and X");
        var deriFunc = drawPlot(new List<FunctionToSolve> { derivative }, "derivative");

        String plotFuncImage = $"plotFunc.png";
        String phiXFuncImage = $"phiXFunc.png";
        String deriFuncImage = $"deriFunc.png";

        plotFunc.SavePng(plotFuncImage, 800, 600);
        phiXFunc.SavePng(phiXFuncImage, 800, 600);
        deriFunc.SavePng(deriFuncImage, 800, 600);

        Console.WriteLine("График сохранен как 'my_plot.png'");

        Process.Start("xdg-open", plotFuncImage);
        Process.Start("xdg-open", phiXFuncImage);
        Process.Start("xdg-open", deriFuncImage);

        Console.WriteLine("Введите начальное приближение...");
        var input = Console.ReadLine();
        double beginEq = Convert.ToDouble(input);

        Console.WriteLine("Введите точность вычислений...");
        input = Console.ReadLine();
        double epsilon = Convert.ToDouble(input);

        var res = iterationalMethod(beginEq, epsilon);
        Console.WriteLine($"Итерационным алгоритмом: {res}");
        Console.WriteLine($"Equation({res}) = {function(res)}");

        res = iterationalMethod(beginEq, epsilon);
        Console.WriteLine($"Методом Ньютона: {res}");
        Console.WriteLine($"Equation({res}) = {function(res)}");
        return;
    }
}
