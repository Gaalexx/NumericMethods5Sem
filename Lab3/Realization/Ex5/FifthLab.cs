using System;

namespace Program
{
    public delegate double funcToCount(double x);

    class FifthLab
    {
        public static double RectangleIntegralMethod(
            double step,
            funcToCount function,
            double left,
            double right
        )
        {
            double answer = 0;
            int n = (int)((right - left) / step);

            for (int j = 0; j < n; j++)
            {
                double xMid = left + step * j + step / 2;
                answer += function(xMid);
            }
            return answer * step;
        }

        public static double TrapezoidIntegralMethod(
            double step,
            funcToCount function,
            double left,
            double right
        )
        {
            double answer = (function(left) + function(right)) / 2;
            int n = (int)((right - left) / step);

            for (int j = 1; j < n; j++)
            {
                double x = left + step * j;
                answer += function(x);
            }
            return answer * step;
        }

        public static double SimpsonIntegralMethod(
            double step,
            funcToCount function,
            double left,
            double right
        )
        {
            int n = (int)((right - left) / step);
            if (n % 2 != 0)
            {
                n--;
            }

            double answer = function(left) + function(right);

            for (int j = 1; j < n; j++)
            {
                double x = left + j * step;
                if (j % 2 == 0)
                {
                    answer += 2 * function(x);
                }
                else
                {
                    answer += 4 * function(x);
                }
            }
            return answer * step / 3;
        }

        public static double RungeRombergIntegralMethod(
            double step1,
            double step2,
            double res1,
            double res2,
            double p
        )
        {
            return res1 + (res1 - res2) / (Math.Pow(step1 / step2, p) - 1);
        }
    }
}
