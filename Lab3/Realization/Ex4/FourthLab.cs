using System;

namespace Program
{
    class FourthProgram
    {
        /* public static List<Tuple<double, double>> derivateLeftRightSided(
            in List<Tuple<double, double>> functionResults
        )
        {
            List<Tuple<double, double>> result =
            [
                new Tuple<double, double>(
                    functionResults[0].Item1,
                    (functionResults[1].Item2 - functionResults[0].Item2)
                        / (functionResults[1].Item1 - functionResults[0].Item1)
                ),
            ];
            for (int i = 1; i < functionResults.Count - 1; i++)
            {
                double add =
                    (
                        (functionResults[i + 1].Item2 - functionResults[i].Item2)
                        / (functionResults[i + 1].Item1 - functionResults[i].Item1)
                    )
                    - (
                        (functionResults[i].Item2 - functionResults[i - 1].Item2)
                        / (functionResults[i].Item1 - functionResults[i - 1].Item1)
                    );
                result.Add(new Tuple<double, double>(functionResults[i].Item1, add));
            }
            result.Add(
                new Tuple<double, double>(
                    functionResults[functionResults.Count - 1].Item1,
                    (
                        functionResults[functionResults.Count - 1].Item2
                        - functionResults[functionResults.Count - 2].Item2
                    )
                        / (
                            functionResults[functionResults.Count - 1].Item1
                            - functionResults[functionResults.Count - 2].Item1
                        )
                )
            );
            return result;
        } */

        /* public static List<Tuple<double, double>> secondDerivateLeftRightSided(
            in List<Tuple<double, double>> functionResults
        )
        {
            List<Tuple<double, double>> result =
            [
                new Tuple<double, double>(
                    functionResults[0].Item1,
                    2
                        * (functionResults[1].Item2 - functionResults[0].Item2)
                        / (functionResults[1].Item1 - functionResults[0].Item1)
                ),
            ];
            for (int i = 1; i < functionResults.Count - 1; i++)
            {
                double add =
                    2
                        * (
                            (functionResults[i + 1].Item2 - functionResults[i].Item2)
                            / (functionResults[i + 1].Item1 - functionResults[i].Item1)
                        )
                    - (
                        (functionResults[i].Item2 - functionResults[i - 1].Item2)
                        / (functionResults[i].Item1 - functionResults[i - 1].Item1)
                    );
                result.Add(new Tuple<double, double>(functionResults[i].Item1, add));
            }
            result.Add(
                new Tuple<double, double>(
                    functionResults[functionResults.Count - 1].Item1,
                    2
                        * (
                            functionResults[functionResults.Count - 1].Item2
                            - functionResults[functionResults.Count - 2].Item2
                        )
                        / (
                            functionResults[functionResults.Count - 1].Item1
                            - functionResults[functionResults.Count - 2].Item1
                        )
                )
            );
            return result;
        } */

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
            return 2 * FirstLab._devDif(in functionResults, indexes.Item1, indexes.Item1 + 2);
        }
    }
}
