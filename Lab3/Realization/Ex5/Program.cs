using System;

namespace Program
{
    class Program
    {
        public static double f(double x)
        {
            return (3 * x + 4) / (2 * x + 7);
        }

        public static void Main(String[] args)
        {
            Console.WriteLine(FifthLab.RectangleIntegralMethod(0.0000001, f, -2, 2));
            Console.WriteLine(FifthLab.TrapezoidIntegralMethod(0.0000001, f, -2, 2));
            Console.WriteLine(FifthLab.SimpsonIntegralMethod(0.0000001, f, -2, 2));
            Console.WriteLine("____________________________________________");
            Console.WriteLine(FifthLab.RectangleIntegralMethod(0.5, f, -2, 2));
            Console.WriteLine(FifthLab.TrapezoidIntegralMethod(0.5, f, -2, 2));
            Console.WriteLine(FifthLab.SimpsonIntegralMethod(0.5, f, -2, 2));
        }
    }
}
