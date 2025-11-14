using System;

public class FirstLab
{
    public static double LagranzhInterpolationPolynomial(
        double x,
        in List<Tuple<double, double>> functionResults
    )
    {
        double answer = 0;
        for (int i = 0; i < functionResults.Count; i++)
        {
            double li = 1;
            for (int j = 0; j < functionResults.Count; j++)
            {
                if (i == j)
                {
                    continue;
                }
                li *=
                    (x - functionResults[j].Item1)
                    / (functionResults[i].Item1 - functionResults[j].Item1);
            }
            answer += functionResults[i].Item2 * li;
        }
        return answer;
    }

    public delegate double Polynomial(double x, in List<Tuple<double, double>> coefficients);
    public delegate double MyFunction(double x);

    private double Omega(double x, in List<Tuple<double, double>> functionResults, int n)
    {
        double omega = x - functionResults[0].Item1;
        for (int i = 1; i < n; i++)
        {
            omega *= x - functionResults[i].Item1;
        }
        return omega;
    }

    private double OmegaDerivative(double x, in List<Tuple<double, double>> functionResults)
    {
        double answer = 0;
        for (int i = 0; i < functionResults.Count; i++)
        {
            double add = 1;
            for (int j = 0; j < functionResults.Count; j++)
            {
                if (i == j)
                {
                    continue;
                }
                add *= x - functionResults[j].Item1;
            }
            answer += add;
        }
        return answer;
    }

    public static double NewtonInterpolationPolynomial(
        double x,
        in List<Tuple<double, double>> functionResults
    )
    {
        double answer = functionResults[0].Item2;
        for (int i = 1; i < functionResults.Count; i++)
        {
            double add = 1;
            for (int j = 0; j < i; j++)
            {
                add *= x - functionResults[j].Item1;
            }
            add *= _devDif(in functionResults, 0, i);
            answer += add;
        }
        return answer;
    }

    public static double ErrorState(
        double x,
        Polynomial func,
        in List<Tuple<double, double>> functionResults,
        MyFunction func1
    )
    {
        return Math.Abs(func(x, in functionResults) - func1(x));
    }

    public static double _devDif(in List<Tuple<double, double>> functionResults, int l, int r)
    {
        if (l == r)
        {
            return functionResults[l].Item2;
        }
        else
        {
            return (_devDif(in functionResults, l, r - 1) - _devDif(in functionResults, l + 1, r))
                / (functionResults[l].Item1 - functionResults[r].Item1);
        }
    }

    public static double DevidedDifference(in List<Tuple<double, double>> functionResults)
    {
        if (functionResults.Count == 2)
        {
            return (functionResults[0].Item2 - functionResults[1].Item2)
                / (functionResults[0].Item1 - functionResults[1].Item1);
        }
        else
        {
            return (
                    _devDif(in functionResults, 0, functionResults.Count - 2)
                    - _devDif(in functionResults, 1, functionResults.Count - 1)
                ) / (functionResults[0].Item1 - functionResults[functionResults.Count - 1].Item1);
        }
    }
}
