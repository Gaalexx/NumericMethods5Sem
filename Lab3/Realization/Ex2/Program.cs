using System;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;

namespace Program
{
    class Program
    {
        public static void Main(String[] args)
        {
            List<Tuple<double, double>> functionResults = new List<Tuple<double, double>>()
            {
                new Tuple<double, double>(0, 0),
                new Tuple<double, double>(0.9, 0.36892),
                new Tuple<double, double>(1.8, 0.85408),
                new Tuple<double, double>(2.7, 1.7856),
                new Tuple<double, double>(3.6, 6.3138),
            };
            double x = 2.66666667;

            Console.WriteLine(
                $"Значение в точке {x} = {SecondLab.SplinePolynomial(x, in functionResults)}"
            );
        }
    }
}
