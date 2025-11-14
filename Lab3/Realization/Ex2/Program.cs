using System;
using System.Diagnostics;
using System.Reflection.Metadata.Ecma335;


namespace Program
{
    class Program
    {
        

        public static void Main(String[] args)
        {
            /* List<Tuple<double, double>> functionResults = new List<Tuple<double, double>>()
            {
                new Tuple<double, double>(
                    Math.PI / 8,
                    Math.Cos(Math.PI / 8) / Math.Sin(Math.PI / 8)
                ),
                new Tuple<double, double>(
                    Math.PI / 4,
                    Math.Cos(Math.PI / 4) / Math.Sin(Math.PI / 4)
                ),
                new Tuple<double, double>(
                    3 * Math.PI / 8,
                    Math.Cos(3 * Math.PI / 8) / Math.Sin(3 * Math.PI / 8)
                ),
                new Tuple<double, double>(
                    Math.PI / 2,
                    Math.Cos(Math.PI / 2) / Math.Sin(Math.PI / 2)
                ),
            }; */

            //Console.WriteLine(LagranzhInterpolationPolynomial(Math.PI / 3, in functionResults));
            //Console.WriteLine(NewtonInterpolationPolynomial(Math.PI / 3, in functionResults));

            /* List<Tuple<double, double>> functionResults2 = new List<Tuple<double, double>>()
            {
                new Tuple<double, double>(
                    Math.PI / 8,
                    Math.Cos(Math.PI / 8) / Math.Sin(Math.PI / 8)
                ),
                new Tuple<double, double>(
                    5 * Math.PI / 16,
                    Math.Cos(5 * Math.PI / 16) / Math.Sin(5 * Math.PI / 16)
                ),
                new Tuple<double, double>(
                    3 * Math.PI / 8,
                    Math.Cos(3 * Math.PI / 8) / Math.Sin(3 * Math.PI / 8)
                ),
                new Tuple<double, double>(
                    Math.PI / 2,
                    Math.Cos(Math.PI / 2) / Math.Sin(Math.PI / 2)
                ),
            }; */
            /* Console.WriteLine(LagranzhInterpolationPolynomial(Math.PI / 3, in functionResults2));
            Console.WriteLine(1 / Math.Tan(Math.PI / 3)); */

            /* List<Tuple<double, double>> functionResults3 = new List<Tuple<double, double>>()
            {
                new Tuple<double, double>(1, 2.4142),
                new Tuple<double, double>(1.9, 1.0818),
                new Tuple<double, double>(2.8, 0.50953),
                new Tuple<double, double>(3.7, 0.11836),
                new Tuple<double, double>(4.6, -0.24008),
            };

            List<Tuple<double, double>> testLab2 = new List<Tuple<double, double>>()
            {
                new Tuple<double, double>(0, 0),
                new Tuple<double, double>(1, 1.8415),
                new Tuple<double, double>(2, 2.9093),
                new Tuple<double, double>(3, 3.1411),
                new Tuple<double, double>(4, 3.2432),
            }; */

            //Console.WriteLine(SecondLab.SplinePolynomial(1.5, in test));

            

            /* for (int i = 0; i < res1.Count; i++)
            {
                Console.WriteLine($"x = {res1[i].Item1}         F{i} = {res1[i].Item2}");
            } */

            /* var test4 = new List<Tuple<double, double>>()
            {
                new Tuple<double, double>(0, 0.4713),
                new Tuple<double, double>(1.7, 1.0114),
                new Tuple<double, double>(3.4, 1.5515),
                new Tuple<double, double>(5.1, 2.0916),
                new Tuple<double, double>(6.8, 2.6317),
                new Tuple<double, double>(8.5, 3.1718),
            };
            Console.WriteLine("_________________________________________________");

            res2 = ThirdLab.MinimalSqaresMethod(2, in testLab3);
            for (int i = 0; i < res2.Count; i++)
            {
                Console.WriteLine($"x = {res2[i].Item1}         F{i} = {res2[i].Item2}");
            } */

        }
    }
}
