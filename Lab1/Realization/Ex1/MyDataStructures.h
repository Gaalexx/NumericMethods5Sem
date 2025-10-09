#include <iostream>
#include <vector>
#include <initializer_list>
#include <exception>
#include <ostream>
#include <stdexcept>
#include <algorithm>

#define p 2


namespace MyDataStructures
{
    #define epsilon 0.0000000000001

    template <class Ttype>
    class Line
    {
        template <class TtypeOs>
        friend std::ostream &operator<<(std::ostream &os, const Line<TtypeOs> &line);

    public:
        std::vector<Ttype> data;

        Line() = default;

        Line(size_t size) : data(size) {}

        Line(const Line &other) : data(other.data) {}

        Line(std::initializer_list<Ttype> init_list) : data(init_list) {}

        friend void swap(Line& first, Line& second) noexcept
        {
            using std::swap;
            swap(first.data, second.data);
        }

        Line &operator=(const Line &other)
        {
            if (this != &other)
            {
                data = other.data;
            }
            return *this;
        }

        Line &operator=(Line &&other) noexcept
        {
            if (this != &other)
            {
                data = std::move(other.data);
            }
            return *this;
        }

        Line operator+(const Line<Ttype> &other) const
        {
            if (data.size() != other.data.size())
            {
                throw std::runtime_error("Line sizes must match for addition");
            }

            Line<Ttype> res(data.size());
            for (size_t i = 0; i < data.size(); i++)
            {
                res.data[i] = data[i] + other.data[i];
            }
            return res;
        }

        Line operator-(const Line<Ttype> &other) const
        {
            if (data.size() != other.data.size())
            {
                throw std::runtime_error("Line sizes must match for subtraction");
            }

            Line<Ttype> res(data.size());
            for (size_t i = 0; i < data.size(); i++)
            {
                res.data[i] = data[i] - other.data[i];
            }
            return res;
        }

        Line operator+(const Ttype &element) const
        {
            Line<Ttype> res(data.size());
            for (size_t i = 0; i < data.size(); i++)
            {
                res.data[i] = data[i] + element;
            }
            return res;
        }

        Line operator-(const Ttype &element) const
        {
            Line<Ttype> res(data.size());
            for (size_t i = 0; i < data.size(); i++)
            {
                res.data[i] = data[i] - element;
            }
            return res;
        }

        Ttype &operator[](size_t index)
        {
            if (index >= data.size())
            {
                throw std::out_of_range("Line index out of range");
            }
            return data[index];
        }

        const Ttype &operator[](size_t index) const
        {
            if (index >= data.size())
            {
                throw std::out_of_range("Line index out of range");
            }
            return data[index];
        }

        Line operator*(const Ttype &element) const
        {
            Line<Ttype> res(data.size());
            for (size_t i = 0; i < data.size(); i++)
            {
                res.data[i] = data[i] * element;
            }
            return res;
        }

        Line operator/(const Ttype &element) const
        {
            Line<Ttype> res(data.size());
            if (element == 0)
            {
                throw std::runtime_error("Division by zero error");
            }
            for (size_t i = 0; i < data.size(); i++)
            {
                res.data[i] = data[i] / element;
            }
            return res;
        }

        size_t size() const
        {
            return data.size();
        }

        void resize(size_t new_size)
        {
            data.resize(new_size);
        }

        void resize(size_t new_size, const Ttype &value)
        {
            data.resize(new_size, value);
        }

        auto begin() { return data.begin(); }
        auto end() { return data.end(); }
        auto begin() const { return data.begin(); }
        auto end() const { return data.end(); }
        auto cbegin() const { return data.cbegin(); }
        auto cend() const { return data.cend(); }

        ~Line() = default;
    };

    template <class Type>
    class Matrix
    {
        template <class T>
        friend std::ostream &operator<<(std::ostream &os, const Matrix<T> &mx);

    public:
        std::vector<Line<Type>> elements;

        Matrix() = default;

        Matrix(size_t rows, size_t cols) : rows(rows), cols(cols)
        {
            elements.resize(rows);
            for (auto &row : elements)
            {
                row.resize(cols);
            }
        }

        Matrix(size_t rows, size_t cols, const Type &value) : rows(rows), cols(cols)
        {
            elements.resize(rows);
            for (auto &row : elements)
            {
                row.resize(cols, value);
            }
        }

        Matrix(const Matrix &other)
            : elements(other.elements), rows(other.rows), cols(other.cols) {}

        Matrix(Matrix &&other) noexcept
            : elements(std::move(other.elements)),
              rows(other.rows),
              cols(other.cols)
        {
            other.rows = 0;
            other.cols = 0;
        }

        Matrix &operator=(const Matrix &other)
        {
            if (this != &other)
            {
                elements = other.elements;
                rows = other.rows;
                cols = other.cols;
            }
            return *this;
        }

        Matrix &operator=(Matrix &&other) noexcept
        {
            if (this != &other)
            {
                elements = std::move(other.elements);
                rows = other.rows;
                cols = other.cols;
                other.rows = 0;
                other.cols = 0;
            }
            return *this;
        }

        Matrix operator+(const Matrix &other) const
        {
            if (rows != other.rows || cols != other.cols)
            {
                throw std::runtime_error("Matrix dimensions must match for addition");
            }

            Matrix<Type> res(rows, cols);
            for (size_t i = 0; i < rows; i++)
            {
                for (size_t j = 0; j < cols; j++)
                {
                    res.elements[i][j] = elements[i][j] + other.elements[i][j];
                }
            }
            return res;
        }

        Matrix operator-(const Matrix &other) const
        {
            if (rows != other.rows || cols != other.cols)
            {
                throw std::runtime_error("Matrix dimensions must match for subtraction");
            }

            Matrix<Type> res(rows, cols);
            for (size_t i = 0; i < rows; i++)
            {
                for (size_t j = 0; j < cols; j++)
                {
                    res.elements[i][j] = elements[i][j] - other.elements[i][j];
                }
            }
            return res;
        }

        Matrix &operator=(std::initializer_list<Type> init_list)
        {
            if (init_list.size() != cols * rows)
            {
                throw std::runtime_error("Amount of elements must be equal!");
            }
            Matrix<Type> copy = *this;
            auto iter = init_list.begin();
            for (size_t i = 0; i < rows; i++)
            {
                for (size_t j = 0; j < cols; j++)
                {
                    elements[i][j] = *(iter++);
                }
            }
            return *this;
        }

        Line<Type> &operator[](size_t index)
        {
            if (index >= rows)
            {
                throw std::out_of_range("Matrix row index out of range");
            }
            return elements[index];
        }

        const Line<Type> &operator[](size_t index) const
        {
            if (index >= rows)
            {
                throw std::out_of_range("Matrix row index out of range");
            }
            return elements[index];
        }

        Matrix operator*(const Type &num) const
        {
            Matrix<Type> res(rows, cols);
            for (size_t i = 0; i < rows; i++)
            {
                for (size_t j = 0; j < cols; j++)
                {
                    res[i][j] = elements[i][j] * num;
                }
            }
            return res;
        }

        static Matrix transposition(const Matrix &matrix)
        {
            Matrix<Type> res(matrix.cols, matrix.rows);
            for (size_t i = 0; i < matrix.rows; i++)
            {
                for (size_t j = 0; j < matrix.cols; j++)
                {
                    res[j][i] = matrix[i][j];
                }
            }
            return res;
        }

        static Matrix getAdjMatrix(const Matrix &matrix)
        {
            if (matrix.rows != matrix.cols)
            {
                throw std::runtime_error("Matrix must be square for adjugate calculation");
            }

            size_t n = matrix.rows;
            Matrix<Type> res(n, n);

            for (size_t i = 0; i < n; i++)
            {
                for (size_t j = 0; j < n; j++)
                {
                    Matrix<Type> A(n - 1, n - 1);
                    for (size_t k = 0, k1 = 0; k < n; k++)
                    {
                        if (k == i)
                            continue;
                        for (size_t l = 0, l1 = 0; l < n; l++)
                        {
                            if (l == j)
                                continue;
                            A[k1][l1++] = matrix[k][l];
                        }
                        k1++;
                    }
                    res[i][j] = ((i + j) % 2 ? -1 : 1) * determinant(A);
                }
            }
            return res;
        }

        static Matrix<Type> getIdentityMatrix(size_t rows, size_t cols)
        {
            Matrix<Type> I(rows, cols, {});
            for (size_t i = 0; i < I.getCols(); i++)
            {
                I[i][i] = 1;
            }
            return I;
        }

        static Type determinant(const Matrix &matrix)
        {
            return diagDetRealization(matrix);
        }

        Matrix getReverseMatrix() const
        {
            if (rows != cols)
            {
                throw std::runtime_error("Matrix must be square for inversion");
            }

            Type det = determinant(*this);
            if (det == Type{})
            {
                throw std::runtime_error("Matrix is singular (determinant is zero)");
            }

            Type revDet = Type{1} / det;
            Matrix res = transposition(getAdjMatrix(*this));
            return res * revDet;
        }

        static Matrix uniteMatrix(const Matrix &matrix1, const Matrix &matrix2)
        {
            if (matrix1.rows != matrix2.rows)
            {
                throw std::runtime_error("Matrices must have the same number of rows for unification");
            }

            Matrix<Type> res(matrix1.rows, matrix1.cols + matrix2.cols);
            for (size_t i = 0; i < matrix1.rows; i++)
            {
                for (size_t j = 0; j < matrix1.cols + matrix2.cols; j++)
                {
                    if (j < matrix1.cols)
                    {
                        res[i][j] = matrix1[i][j];
                    }
                    else
                    {
                        res[i][j] = matrix2[i][j - matrix1.cols];
                    }
                }
            }
            return res;
        }

        Matrix operator*(const Matrix &other) const
        {
            if (cols != other.rows)
            {
                throw std::runtime_error("Matrix dimensions must be compatible for multiplication");
            }

            Matrix<Type> res(rows, other.cols);
            for (size_t i = 0; i < rows; i++)
            {
                for (size_t j = 0; j < other.cols; j++)
                {
                    for (size_t k = 0; k < cols; k++)
                    {
                        res.elements[i][j] += elements[i][k] * other.elements[k][j];
                    }
                }
            }
            return res;
        }

        void resize(size_t new_rows, size_t new_cols)
        {
            elements.resize(new_rows);
            for (auto &row : elements)
            {
                row.resize(new_cols);
            }
            rows = new_rows;
            cols = new_cols;
        }

        void resize(size_t new_rows, size_t new_cols, const Type &value)
        {
            elements.resize(new_rows);
            for (auto &row : elements)
            {
                row.resize(new_cols, value);
            }
            rows = new_rows;
            cols = new_cols;
        }

        size_t getRows() const { return rows; }
        size_t getCols() const { return cols; }

        ~Matrix() = default;

    private:
        size_t rows = 0;
        size_t cols = 0;

        static Type _makeZeroesUnderDiag(Matrix<Type> &system)
        {
            if (system.getCols() != system.getRows())
            {
                throw std::runtime_error("System matrix must have N rows and N+1 columns");
            }

            size_t N = system.getRows();
            Type k = 1;
            bool hasNill = false;
            for (int i = 0; i < N; i++)
            {
                if (!hasNill)
                {
                    k *= system[i][i];
                }
                if (system[i][i] == 0)
                {
                    hasNill = true;
                    continue;
                }
                system[i] = system[i] / system[i][i];
                system[i][i] = 1.0;
                // long double x = system[i][N];
                for (int j = i + 1; j < N; j++)
                {
                    system[j] = system[j] - (system[i] * system[j][i]);
                    system[j][i] = 0;
                }
            }

            // std::cout << system << std::endl;

            return k;
        }

        static Type recursiveDetRealization(const Matrix &matrix)
        {
            if (matrix.rows != matrix.cols)
            {
                throw std::runtime_error("Matrix must be square for determinant calculation");
            }

            size_t n = matrix.rows;

            if (n == 0)
            {
                return Type{};
            }
            else if (n == 1)
            {
                return matrix[0][0];
            }
            else if (n == 2)
            {
                return matrix[0][0] * matrix[1][1] - matrix[0][1] * matrix[1][0];
            }
            else
            {
                Type res = {};
                for (size_t i = 0; i < n; i++)
                {
                    Matrix<Type> minor(n - 1, n - 1);
                    for (size_t j = 1, j1 = 0; j < n; j++, j1++)
                    {
                        for (size_t k = 0, k1 = 0; k < n; k++)
                        {
                            if (k == i)
                                continue;
                            minor[j1][k1++] = matrix[j][k];
                        }
                    }
                    res = ((i % 2) ? -1 : 1) * matrix[0][i] * recursiveDetRealization(minor) + res;
                }
                return res;
            }
        }

        static Type diagDetRealization(const Matrix &matrix)
        {
            auto copy = matrix;
            auto res = _makeZeroesUnderDiag(copy);
            return res;
        }
    };

    template <class Type>
    std::ostream &operator<<(std::ostream &os, const Matrix<Type> &mx)
    {
        for (size_t i = 0; i < mx.rows; i++)
        {
            for (size_t j = 0; j < mx.cols; j++)
            {
                if (fabsf64x((long double)mx.elements[i][j]) < epsilon)
                {
                    os << 0 << " ";
                }
                else
                {
                    os << mx.elements[i][j] << " ";
                }
            }
            os << std::endl;
        }
        return os;
    }

    template <class Ttype>
    std::ostream &operator<<(std::ostream &os, const Line<Ttype> &line)
    {
        for (size_t i = 0; i < line.data.size(); i++)
        {
            os << line[i] << " ";
        }
        return os;
    }

};