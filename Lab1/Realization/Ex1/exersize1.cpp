#include <iostream>
#include <cmath>
#include "MyDataStructures.h"

using namespace MyDataStructures;

unsigned long long iterations_count = 0;

Matrix<long double> LUSolutionMethod(Matrix<long double> Ax, Matrix<long double> b);

template <class Type>
std::pair<Matrix<Type>, Matrix<Type>> getLUMatrix(const Matrix<Type> &a)
{
    if (a.getRows() != a.getCols())
    {
        throw std::runtime_error("Matrix must be square for LU decomposition");
    }

    size_t N = a.getRows();
    Matrix<Type> L(N, N);
    Matrix<Type> U(N, N);

    for (size_t i = 0; i < N; i++)
    {
        L[i][i] = 1;
    }

    for (size_t i = 0; i < N; i++)
    {
        for (size_t j = 0; j < N; j++)
        {
            if (i <= j)
            {
                U[i][j] = a[i][j];
                for (size_t k = 0; k < i; k++)
                {
                    U[i][j] = U[i][j] - L[i][k] * U[k][j];
                    ++iterations_count;
                }
            }
            else
            {
                L[i][j] = a[i][j];
                for (size_t k = 0; k < j; k++)
                {
                    L[i][j] = L[i][j] - L[i][k] * U[k][j];
                    ++iterations_count;
                }
                L[i][j] = L[i][j] / U[j][j];
            }
        }
    }

    return std::make_pair(L, U);
}

template <class Type>
Type LU_determinant(const std::pair<Matrix<Type>, Matrix<Type>> &pr)
{
    Type res = 1;
    for (size_t i = 0; i < pr.second.getRows(); i++)
    {
        res *= pr.second[i][i];
        ++iterations_count;
    }
    return res;
}

template <class Type>
Matrix<Type> LURevMatrix(const Matrix<Type> &matrix)
{
    std::vector<Matrix<Type>> res_n(matrix.getCols());
    Matrix<Type> vecI(matrix.getRows(), 1, 0);
    Matrix<Type> rev_matrix(matrix.getRows(), matrix.getCols());
    for (size_t i = 0; i < matrix.getCols(); i++)
    {
        if (i != 0)
        {
            vecI[i - 1][0] = 0;
        }
        vecI[i][0] = 1;
        res_n[i] = LUSolutionMethod(matrix, vecI);
        ++iterations_count;
    }

    for (size_t i = 0; i < matrix.getRows(); i++)
    {
        for (size_t j = 0; j < matrix.getCols(); j++)
        {
            rev_matrix[j][i] = res_n[i][j][0];
        }
    }

    return rev_matrix;
}

Matrix<long double> upGaussMethod(Matrix<long double> system) // это метод Гаусса вверх
{
    if (system.getCols() != system.getRows() + 1)
    {
        throw std::runtime_error("System matrix must have N rows and N+1 columns");
    }

    size_t N = system.getRows();
    Matrix<long double> res(N, 1);

    for (int i = N - 1; i >= 0; i--)
    {
        system[i] = system[i] / system[i][i];
        system[i][i] = 1.0;
        long double x = system[i][N];
        for (int j = i - 1; j >= 0; j--)
        {
            system[j][N] -= (system[j][i] * x);
            system[j][i] = 0;
            ++iterations_count;
        }
    }

    for (size_t i = 0; i < N; i++)
    {
        res[i][0] = system[i][N];
    }

    return res;
}

Matrix<long double> downTriangMatrixSolution(Matrix<long double> system) // решение для нижнедиаг матрицы
{
    if (system.getCols() != system.getRows() + 1)
    {
        throw std::runtime_error("System matrix must have N rows and N+1 columns");
    }

    size_t N = system.getRows();
    Matrix<long double> res(N, 1);

    for (size_t i = 0; i < N; i++)
    {
        res[i][0] = system[i][N];
        for (size_t j = 0; j < i; j++)
        {
            res[i][0] = res[i][0] - system[i][j] * res[j][0];
            ++iterations_count;
        }
    }
    return res;
}

Matrix<long double> LUSolutionMethod(Matrix<long double> Ax, Matrix<long double> b)
{
    if (Ax.getRows() != Ax.getCols())
    {
        throw std::runtime_error("Coefficient matrix must be square");
    }
    if (Ax.getRows() != b.getRows() || b.getCols() != 1)
    {
        throw std::runtime_error("Vector b must have same number of rows as A and 1 column");
    }

    size_t N = Ax.getRows();
    auto LUPair = getLUMatrix(Ax);

    Matrix<long double> L_extended(N, N + 1);
    for (size_t i = 0; i < N; i++)
    {
        for (size_t j = 0; j < N; j++)
        {
            L_extended[i][j] = LUPair.first[i][j];
        }
        L_extended[i][N] = b[i][0];
    }

    auto y = downTriangMatrixSolution(L_extended);

    Matrix<long double> U_extended(N, N + 1);
    for (size_t i = 0; i < N; i++)
    {
        for (size_t j = 0; j < N; j++)
        {
            U_extended[i][j] = LUPair.second[i][j];
        }
        U_extended[i][N] = y[i][0];
    }

    auto x = upGaussMethod(U_extended);
    return x;
}

int main()
{
    int n;
    std::cout << "Welcome to Ax = b solution program!\nInput amount of unknown...\n";
    std::cin >> n;
    if (n < 1)
    {
        std::cout << "The amount of unknown must not be less than 1!\n";
        return -1;
    }
    Matrix<long double> Ax(n, n);
    Matrix<long double> b(n, 1);
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
        std::cin >> b[i][0];
    }

    auto lu = getLUMatrix(Ax);
    auto det = LU_determinant(lu);

    if (fabsf64x(det) <= 0.00000000000000001)
    {
        std::cout << "The determinant must not be equal to zero!";
        return -1;
    }
    else
    {
        std::cout << "_____________________\nThe determinant of this matrix is: " << det << std::endl;
        std::cout << "It took " << iterations_count << " iterations to count a determinant.\n";
        iterations_count = 0;
        auto Ax_rev = LURevMatrix(Ax);

        std::cout << "_____________________\nThe reverse matrix for A is:\n"
                  << Ax_rev << std::endl;
        std::cout << "_____________________\nProduct between A and A_rev:\n"
                  << Ax * Ax_rev << std::endl;
        std::cout << "It took " << iterations_count << " iterations to get a reverse matrix.\n";
        iterations_count = 0;
        auto res = LUSolutionMethod(Ax, b);

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
            std::cout << res_sum << " = " << b[i][0] << "\n";
        }
    }

    std::cout << "It took " << iterations_count << " iterations to solve the system of equations." << std::endl;
    return 0;
}
