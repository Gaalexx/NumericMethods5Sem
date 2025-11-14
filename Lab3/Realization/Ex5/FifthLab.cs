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
            for (double i = left; i < right; i += step)
            {
                answer += function(i) * step;
            }
            return answer;
        }

        public static double TrapezoidIntegralMethod(
            double step,
            funcToCount function,
            double left,
            double right
        )
        {
            double answer = 0;
            for (double i = left; i < right; i += step)
            {
                answer += (function(i) + function(i + step)) / 2 * step;
            }
            return answer;
        }


        public static double SimpsonIntegralMethod(
            double step,
            funcToCount function,
            double left,
            double right
        )
        {
            double answer = 0;
            double h = step / 2;
            for (double i = left; i < right; i += step)
            {
                answer += (function(i) + 4 * function(i - step / 2) + function(i + step)) * h;
            }
            return answer / 3;
        }

        public static double RungeRombergIntegralMethod(
            double step,
            funcToCount function,
            double left,
            double right
        )
        {
            return 0;
        }
    }
}
