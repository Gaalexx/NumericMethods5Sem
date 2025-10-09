#include <iostream>
#include <thread>
#include <cmath>
#include "MyDataStructures.h"

using namespace MyDataStructures;

unsigned long long global_iterations_count = 0;

long double norm(const Matrix<long double> &matrix)
{
    long double maximum = std::numeric_limits<long double>::lowest();
    for (int i = 0; i < matrix.getRows(); i++)
    {
        long double sum = 0;
        for (int j = 0; j < matrix.getRows(); j++)
        {
            sum += matrix[i][j];
        }
        maximum = std::max(maximum, sum);
    }
    return maximum;
}

long double norm_v(const Matrix<long double> &matrix)
{
    long double maximum = std::numeric_limits<long double>::lowest();
    for (int i = 0; i < matrix.getCols(); i++)
    {
        std::cout << i << std::endl;
        maximum = std::max(maximum, matrix[i][0]);
    }
    return maximum;
}

template <class Type>
Matrix<Type> iterationalMethodWithNorm(const Matrix<Type> &system, const long double &eps)
{
    size_t n = system.getRows(), m = system.getCols();
    Matrix<Type> beta(n, 1), alpha(n, n);
    Matrix<Type> res(n, 1);
    for (int i = 0; i < n; i++)
    {
        beta[i][0] = system[i][m - 1] / system[i][i];
    }

    for (int i = 0; i < n; i++)
    {
        for (int j = 0; j < n; j++)
        {
            if (i != j)
            {
                alpha[i][j] = -system[i][j] / system[i][i];
            }
            else
            {
                alpha[i][j] = 0;
            }
        }
    }

    int iterationsCount = 0;
    long double epsilonOfIIteration = 0;
    res = beta;
    auto norm_alpha = norm(system);
    while (iterationsCount++ < 10000000)
    {
        ++global_iterations_count;
        bool flag = true;
        auto pr = alpha * res;

        Matrix<long double> prev(n, 1);
        Matrix<long double> razn(n, 1);

        for (int i = 0; i < n; i++)
        {
            // auto prev = res[i][0];
            prev[i][0] = res[i][0];
            res[i][0] = beta[i][0] + pr[i][0];
            /* if (fabsf64x(res[i][0] - prev) > eps)
            {
                flag = false;
            } */
        }
        /* if (flag)
        {
            break;
        } */
        for (int i = 0; i < n; i++)
        {
            razn[i][0] = res[i][0] - prev[i][0];
        }
        auto norma_vec = norm_v(razn);
        if((norm_alpha / (1 - norm_alpha)) * norma_vec < eps){
            return res;
        }
    }
    return res;
}

template <class Type>
Matrix<Type> iterationalMethod(const Matrix<Type> &system, const long double &eps)
{
    size_t n = system.getRows(), m = system.getCols();
    Matrix<Type> beta(n, 1), alpha(n, n);
    Matrix<Type> res(n, 1);
    for (int i = 0; i < n; i++)
    {
        beta[i][0] = system[i][m - 1] / system[i][i];
    }

    for (int i = 0; i < n; i++)
    {
        for (int j = 0; j < n; j++)
        {
            if (i != j)
            {
                alpha[i][j] = -system[i][j] / system[i][i];
            }
            else
            {
                alpha[i][j] = 0;
            }
        }
    }

    int iterationsCount = 0;
    long double epsilonOfIIteration = 0;
    res = beta;
    while (iterationsCount++ < 10000000)
    {
        ++global_iterations_count;
        bool flag = true;
        auto pr = alpha * res;
        for (int i = 0; i < n; i++)
        {
            auto prev = res[i][0];
            res[i][0] = beta[i][0] + pr[i][0];
            if (fabsf64x(res[i][0] - prev) > eps)
            {
                flag = false;
            }
        }
        if (flag)
        {
            break;
        }
    }
    return res;
}

template <class Type>
Matrix<Type> iterationalZeidelMethod(const Matrix<Type> &system, const long double &eps)
{
    size_t n = system.getRows(), m = system.getCols();
    Matrix<Type> beta(n, 1), alpha(n, n);
    Matrix<Type> res(n, 1);
    for (int i = 0; i < n; i++)
    {
        beta[i][0] = system[i][m - 1] / system[i][i];
    }

    for (int i = 0; i < n; i++)
    {
        for (int j = 0; j < n; j++)
        {
            if (i != j)
            {
                alpha[i][j] = -system[i][j] / system[i][i];
            }
            else
            {
                alpha[i][j] = 0;
            }
        }
    }

    int iterationsCount = 0;
    long double epsilonOfIIteration = 0;
    res = beta;
    while (iterationsCount++ < 10000000)
    {
        auto old_res = res;
        bool flag = true;

        for (int i = 0; i < res.getRows(); i++)
        {
            res[i][0] = beta[i][0];
            for (int j = 0; j < i; j++)
            {
                res[i][0] += res[j][0] * alpha[i][j];
            }
            for (int j = i + 1; j < res.getRows(); j++)
            {
                res[i][0] += old_res[j][0] * alpha[i][j];
            }
        }
        ++global_iterations_count;
        for (int i = 0; i < n; i++)
        {
            if (fabsf64x(res[i][0] - old_res[i][0]) > eps)
            {
                flag = false;
            }
        }
        if (flag)
        {
            break;
        }
    }
    return res;
}

int main()
{

    int n;
    long double eps;
    std::cout << "Welcome to Ax = b solution program!\nInput amount of unknown...\n";
    std::cin >> n;
    std::cout << "Input epsilon...\n";
    std::cin >> eps;
    if (n < 1)
    {
        std::cout << "The amount of unknown must not be less than 1!\n";
        return -1;
    }
    Matrix<long double> Ax(n, n + 1);
    std::cout << "Input A matrix...\n";

    for (size_t i = 0; i < n; i++)
    {
        for (size_t j = 0; j < n; j++)
        {
            std::cin >> Ax[i][j];
        }
    }

    std::cout << "Input b vector...\n";
    for (int i = 0; i < n; i++)
    {
        std::cin >> Ax[i][n];
    }

    auto res = iterationalMethod(Ax, eps);
    std::cout << "It took " << global_iterations_count << " iterations for iterational algorithm\n";

    global_iterations_count = 0;

    for (int i = 0; i < res.getRows(); i++)
    {
        std::cout << "x" << i + 1 << " = " << res[i][0] << std::endl;
    }

    std::cout << "Checking if the answer is right:\n";
    for (size_t i = 0; i < n; i++)
    {
        long double res_sum = 0;
        for (size_t j = 0; j < n; j++)
        {
            res_sum += res[j][0] * Ax[i][j];
        }
        std::cout << res_sum << " = " << Ax[i][n] << "\n";
    }

    res = iterationalZeidelMethod(Ax, eps);
    std::cout << "It took " << global_iterations_count << " iterations for iterational algorithm\n";

    for (int i = 0; i < res.getRows(); i++)
    {
        std::cout << "x" << i + 1 << " = " << res[i][0] << std::endl;
    }

    std::cout << "Checking if the answer is right:\n";
    for (size_t i = 0; i < n; i++)
    {
        long double res_sum = 0;
        for (size_t j = 0; j < n; j++)
        {
            res_sum += res[j][0] * Ax[i][j];
        }
        std::cout << res_sum << " = " << Ax[i][n] << "\n";
    }

    return 0;
}