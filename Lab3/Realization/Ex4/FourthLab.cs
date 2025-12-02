using System;

namespace Program
{
    class FourthProgram
    {

        private static Tuple<int, int>? getBorders(
            double value,
            in List<Tuple<double, double>> functionResults
        )
        {
            for (int i = 0; i < functionResults.Count - 1; i++)
            {
                if (functionResults[i].Item1 <= value && value <= functionResults[i + 1].Item1)
                {
                    return new Tuple<int, int>(i, i + 1);
                }
            }
            return null;
        }

        public static double getFuncValue(double x, in List<Tuple<double, double>> functionResults)
        {
            var indexes = getBorders(x, in functionResults);
            if (indexes == null)
            {
                throw new Exception("X не внутри границ функции");
            }

            return functionResults[indexes.Item1].Item2
                + FirstLab._devDif(in functionResults, indexes.Item1, indexes.Item2)
                    * (x - functionResults[indexes.Item1].Item1)
                + FirstLab._devDif(in functionResults, indexes.Item1, indexes.Item1 + 2)
                    * (x - functionResults[indexes.Item1].Item1)
                    * (x - functionResults[indexes.Item2].Item1);
        }

        public static double getFirstDerivate(
            double x,
            in List<Tuple<double, double>> functionResults
        )
        {
            var indexes = getBorders(x, in functionResults);
            if (indexes == null)
            {
                throw new Exception("X не внутри границ функции");
            }
            return FirstLab._devDif(in functionResults, indexes.Item1, indexes.Item2)
                + FirstLab._devDif(in functionResults, indexes.Item1, indexes.Item1 + 2)
                    * (
                        2 * x
                        - functionResults[indexes.Item1].Item1
                        - functionResults[indexes.Item2].Item1
                    );
        }

        public static double getSecondDerivate(
            double x,
            in List<Tuple<double, double>> functionResults
        )
        {
            var indexes = getBorders(x, in functionResults);
            if (indexes == null)
            {
                throw new Exception("X не внутри границ функции");
            }
            return 2.0f * FirstLab._devDif(in functionResults, indexes.Item1, indexes.Item2 + 1);
        }
    }
}
