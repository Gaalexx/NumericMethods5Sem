#include <iostream>
#include <cmath>
#include <vector>
#include <complex>
#include <limits>
#include "MyDataStructures.h"

using namespace MyDataStructures;

template <typename T>
int sign(T val)
{
    return (T(0) < val) - (val < T(0));
}



std::pair<Matrix<long double>, Matrix<long double>> QRMatrix(const Matrix<long double> &matrix)
{
    auto A0 = matrix;
    Matrix<long double> E = Matrix<long double>::getIdentityMatrix(matrix.getRows(), matrix.getCols()), H(matrix.getRows(), matrix.getCols(), 0);

    Matrix<long double> v(matrix.getRows(), 1);

    std::vector<Matrix<long double>> Hn(matrix.getRows() - 1);

    for (size_t i = 0; i < matrix.getCols() - 1; i++)
    {
        for (size_t j = 0; j < i; j++)
        {
            v[j][0] = 0;
        }
        v[i][0] = A0[i][i];

        long double sum = 0;
        for (size_t j = i; j < matrix.getRows(); j++)
        {
            sum += A0[j][i] * A0[j][i];
        }

        v[i][0] += sign(A0[i][i]) * sqrt(sum);

        for (size_t j = i + 1; j < matrix.getCols(); j++)
        {
            v[j][0] = A0[j][i];
        }

        auto v_trans = Matrix<long double>::transposition(v);

        H = E - (v * v_trans) * ((long double)2 / (v_trans * v)[0][0]);

        A0 = H * A0;

        Hn[i] = H;
    }

    auto Q = Hn[0];
    for (size_t i = 1; i < Q.getRows() - 1; i++)
    {
        Q = Q * Hn[i];
    }

    return std::pair<Matrix<long double>, Matrix<long double>>(Q, A0);
}

std::vector<std::complex<long double>> getEigenvalues(const Matrix<long double> &matrix, long double eps)
{
    if (matrix.getRows() != matrix.getCols())
    {
        throw std::invalid_argument("Matrix must be square");
    }

    int n = matrix.getRows();
    Matrix<long double> A = matrix;
    std::vector<std::complex<long double>> eigenvalues(n);

    int max_iterations = 1000;
    int iteration = 0;

    while (iteration < max_iterations)
    {
        auto [Q, R] = QRMatrix(A);

        A = R * Q;

        bool converged = true;
        for (int i = 1; i < n; i++)
        {
            for (int j = 0; j < i; j++)
            {
                if (std::abs(A[i][j]) > eps)
                {
                    converged = false;
                    break;
                }
            }
            if (!converged)
                break;
        }

        if (converged)
        {
            break;
        }

        iteration++;
    }

    for (int i = 0; i < n; i++)
    {
        if (i < n - 1 && std::abs(A[i + 1][i]) > eps)
        {
            long double a = A[i][i];
            long double b = A[i][i + 1];
            long double c = A[i + 1][i];
            long double d = A[i + 1][i + 1];

            long double trace = a + d;
            long double determinant = a * d - b * c;
            long double discriminant = trace * trace - 4 * determinant;

            if (discriminant < 0)
            {
                long double real_part = trace / 2;
                long double imag_part = std::sqrt(-discriminant) / 2;
                eigenvalues[i] = {real_part, imag_part};
                eigenvalues[i + 1] = {real_part, -imag_part};
                i++;
            }
            else
            {
                eigenvalues[i] = A[i][i];
            }
        }
        else
        {
            eigenvalues[i] = A[i][i];
        }
    }

    return eigenvalues;
}

int main()
{

    std::cout << "Input the number of rows (or cols)...\n";

    int n;

    long double eps;

    std::cin >> n;

    std::cout << "Input eps...\n";

    std::cin >> eps;

    std::cout << "Input " << n * n << " elements of the matrix\n";

    Matrix<long double> A(n, n);

    for (int i = 0; i < n; i++)
    {
        for (int j = 0; j < n; j++)
        {
            std::cin >> A[i][j];
        }
    }

    auto [Q, R] = QRMatrix(A);

    std::cout << "Q matrix:\n"
              << Q << "\n_________\n"
              << "R matrix:\n"
              << R << "\n_________\n";
    std::cout << "A = Q * R\n"
              << Q * R << "\n";

    auto res = getEigenvalues(A, eps);

    std::cout << "Here are eigenvalues of this matrix:\n";
    for (size_t i = 0; i < res.size(); i++)
    {
        std::cout << "lambda" << i + 1 << " " << res[i].real() << " + " << res[i].imag() << "i\n";
    }

    return 0;
}