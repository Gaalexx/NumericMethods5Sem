#include <iostream>
#include <vector>
#include <cmath>
#include "MyDataStructures.h"

using namespace MyDataStructures;

unsigned long long iterations_count = 0;

template <class Type>
Matrix<Type> runThroughMethod(const Matrix<Type> &matrix)
{
    std::vector<Type> Pi(matrix.getRows()), Qi(matrix.getRows());

    Pi[0] = -matrix[0][1] / matrix[0][0], Qi[0] = matrix[0][matrix.getCols() - 1] / matrix[0][0];

    size_t c = 2, b = 1, a = 0, d = matrix.getRows();

    for (size_t i = 1; i < matrix.getRows(); i++)
    {
        Pi[i] = -matrix[i][c] / (matrix[i][b] + matrix[i][a] * Pi[i - 1]);
        Qi[i] = (matrix[i][d] - matrix[i][a] * Qi[i - 1]) / (matrix[i][b] + matrix[i][a] * Pi[i - 1]);
        ++c, ++b, ++a;
        iterations_count++;
    }
    Matrix<Type> res(matrix.getRows(), 1);
    --c;
    --b;
    --a;
    res[matrix.getRows() - 1][0] = Qi[matrix.getRows() - 1];

    for (int i = matrix.getRows() - 2; i >= 0; i--)
    {
        res[i][0] = Pi[i] * res[i + 1][0] + Qi[i];
        --c;
        --b;
        --a;
        iterations_count++;
    }
    return res;
}

int main()
{
    /* Matrix<double> a(5, 6);
    a = {-14, -6, 0, 0, 0, -78, -9, 15, -1, 0,0,-91, 0, 1, -11, 1, 0, -38, 0, 0, -7, 12, 3, 77, 0,0,0,6,-7,91};
    auto res = runThroughMethod(a);
    std::cout << res << std::endl;
    return 0; */

    int n;
    std::cout << "Welcome to Ax = b solution program!\nInput amount of unknown...\n";
    std::cin >> n;
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

    auto res = runThroughMethod(Ax);

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

    std::cout << "It took " << iterations_count << " iterations to solve the system of equations." << std::endl;
    return 0;
}