using System;

namespace Program
{
    class Program
    {
        public static double f(double x)
        {
            return (3 * x + 4) / (2 * x + 7);
        }

        /* public static double fTest(double x)
        {
            return x / ((3 * x + 4) * (3 * x + 4));
        } */

        public static void Main(String[] args)
        {
            double h1 = 1.0,
                h2 = 0.5;


            double rect1 = FifthLab.RectangleIntegralMethod(h1, f, -2, 2);
            double rect2 = FifthLab.RectangleIntegralMethod(h2, f, -2, 2);
            double rectRR = FifthLab.RungeRombergIntegralMethod(h1, h2, rect1, rect2, 2);
            double rectError = Math.Abs(rectRR - rect2);

            double trap1 = FifthLab.TrapezoidIntegralMethod(h1, f, -2, 2);
            double trap2 = FifthLab.TrapezoidIntegralMethod(h2, f, -2, 2);
            double trapRR = FifthLab.RungeRombergIntegralMethod(h1, h2, trap1, trap2, 2);
            double trapError = Math.Abs(trapRR - trap2);

            double simp1 = FifthLab.SimpsonIntegralMethod(h1, f, -2, 2);
            double simp2 = FifthLab.SimpsonIntegralMethod(h2, f, -2, 2);
            double simpRR = FifthLab.RungeRombergIntegralMethod(h1, h2, simp1, simp2, 4);
            double simpError = Math.Abs(simpRR - simp2);

            Console.WriteLine("Метод прямоугольников:");
            Console.WriteLine($"Результат: {rectRR:F6}");
            Console.WriteLine($"Погрешность: {rectError}\n");

            Console.WriteLine("Метод трапеций:");
            Console.WriteLine($"Результат: {trapRR:F6}");
            Console.WriteLine($"Погрешность: {trapError}\n");

            Console.WriteLine("Метод Симпсона:");
            Console.WriteLine($"Результат: {simpRR:F6}");
            Console.WriteLine($"Погрешность: {simpError}");
        }
    }
}
