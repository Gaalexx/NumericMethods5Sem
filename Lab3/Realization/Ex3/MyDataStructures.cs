using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MyDataStructures
{
    public class Line<Ttype> : IEnumerable<Ttype>
    {
        public List<Ttype> Data { get; private set; }
        private const double epsilon = 0.0000000000001;

        public Line()
        {
            Data = new List<Ttype>();
        }

        public Line(int size)
        {
            Data = new List<Ttype>(new Ttype[size]);
        }

        public Line(Line<Ttype> other)
        {
            Data = new List<Ttype>(other.Data);
        }

        public Line(IEnumerable<Ttype> initCollection)
        {
            Data = new List<Ttype>(initCollection);
        }

        public static void Swap(Line<Ttype> first, Line<Ttype> second)
        {
            (first.Data, second.Data) = (second.Data, first.Data);
        }

        public Line<Ttype> this[Range range]
        {
            get
            {
                var (start, length) = range.GetOffsetAndLength(Data.Count);
                return new Line<Ttype>(Data.GetRange(start, length));
            }
        }

        public Ttype this[int index]
        {
            get
            {
                if (index < 0 || index >= Data.Count)
                    throw new IndexOutOfRangeException("Line index out of range");
                return Data[index];
            }
            set
            {
                if (index < 0 || index >= Data.Count)
                    throw new IndexOutOfRangeException("Line index out of range");
                Data[index] = value;
            }
        }

        public static Line<Ttype> operator +(Line<Ttype> left, Line<Ttype> right)
        {
            if (left.Data.Count != right.Data.Count)
                throw new ArgumentException("Line sizes must match for addition");

            var res = new Line<Ttype>(left.Data.Count);
            for (int i = 0; i < left.Data.Count; i++)
            {
                res.Data[i] = (dynamic)left.Data[i] + (dynamic)right.Data[i];
            }
            return res;
        }

        public static Line<Ttype> operator -(Line<Ttype> left, Line<Ttype> right)
        {
            if (left.Data.Count != right.Data.Count)
                throw new ArgumentException("Line sizes must match for subtraction");

            var res = new Line<Ttype>(left.Data.Count);
            for (int i = 0; i < left.Data.Count; i++)
            {
                res.Data[i] = (dynamic)left.Data[i] - (dynamic)right.Data[i];
            }
            return res;
        }

        public static Line<Ttype> operator +(Line<Ttype> line, Ttype element)
        {
            var res = new Line<Ttype>(line.Data.Count);
            for (int i = 0; i < line.Data.Count; i++)
            {
                res.Data[i] = (dynamic)line.Data[i] + (dynamic)element;
            }
            return res;
        }

        public static Line<Ttype> operator -(Line<Ttype> line, Ttype element)
        {
            var res = new Line<Ttype>(line.Data.Count);
            for (int i = 0; i < line.Data.Count; i++)
            {
                res.Data[i] = (dynamic)line.Data[i] - (dynamic)element;
            }
            return res;
        }

        public static Line<Ttype> operator *(Line<Ttype> line, Ttype element)
        {
            var res = new Line<Ttype>(line.Data.Count);
            for (int i = 0; i < line.Data.Count; i++)
            {
                res.Data[i] = (dynamic)line.Data[i] * (dynamic)element;
            }
            return res;
        }

        public static Line<Ttype> operator /(Line<Ttype> line, Ttype element)
        {
            if (EqualityComparer<Ttype>.Default.Equals(element, default(Ttype)))
                throw new DivideByZeroException("Division by zero error");

            var res = new Line<Ttype>(line.Data.Count);
            for (int i = 0; i < line.Data.Count; i++)
            {
                res.Data[i] = (dynamic)line.Data[i] / (dynamic)element;
            }
            return res;
        }

        public int Size => Data.Count;

        public void Resize(int newSize)
        {
            if (newSize < Data.Count)
                Data.RemoveRange(newSize, Data.Count - newSize);
            else if (newSize > Data.Count)
                Data.AddRange(Enumerable.Repeat(default(Ttype), newSize - Data.Count));
        }

        public void Resize(int newSize, Ttype value)
        {
            if (newSize < Data.Count)
                Data.RemoveRange(newSize, Data.Count - newSize);
            else if (newSize > Data.Count)
                Data.AddRange(Enumerable.Repeat(value, newSize - Data.Count));
        }

        public IEnumerator<Ttype> GetEnumerator() => Data.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Data.GetEnumerator();

        public override string ToString()
        {
            return string.Join(" ", Data);
        }
    }

    public class Matrix<Ttype> : IEnumerable<Line<Ttype>>
    {
        public List<Line<Ttype>> Elements { get; private set; }
        public int Rows { get; private set; }
        public int Cols { get; private set; }
        private const double epsilon = 0.0000000000001;

        public Matrix()
        {
            Elements = new List<Line<Ttype>>();
            Rows = 0;
            Cols = 0;
        }

        public Matrix(int rows, int cols)
        {
            Rows = rows;
            Cols = cols;
            Elements = new List<Line<Ttype>>(rows);
            for (int i = 0; i < rows; i++)
            {
                Elements.Add(new Line<Ttype>(cols));
            }
        }

        public Matrix(int rows, int cols, Ttype value)
        {
            Rows = rows;
            Cols = cols;
            Elements = new List<Line<Ttype>>(rows);
            for (int i = 0; i < rows; i++)
            {
                var line = new Line<Ttype>(cols);
                for (int j = 0; j < cols; j++)
                {
                    line[j] = value;
                }
                Elements.Add(line);
            }
        }

        public Matrix(Matrix<Ttype> other)
        {
            Rows = other.Rows;
            Cols = other.Cols;
            Elements = new List<Line<Ttype>>(other.Elements.Select(line => new Line<Ttype>(line)));
        }

        public Line<Ttype> this[int index]
        {
            get
            {
                if (index < 0 || index >= Rows)
                    throw new IndexOutOfRangeException("Matrix row index out of range");
                return Elements[index];
            }
            set
            {
                if (index < 0 || index >= Rows)
                    throw new IndexOutOfRangeException("Matrix row index out of range");
                Elements[index] = value;
            }
        }

        public Ttype this[int row, int col]
        {
            get
            {
                if (row < 0 || row >= Rows || col < 0 || col >= Cols)
                    throw new IndexOutOfRangeException("Matrix index out of range");
                return Elements[row][col];
            }
            set
            {
                if (row < 0 || row >= Rows || col < 0 || col >= Cols)
                    throw new IndexOutOfRangeException("Matrix index out of range");
                Elements[row][col] = value;
            }
        }

        public static Matrix<Ttype> operator +(Matrix<Ttype> left, Matrix<Ttype> right)
        {
            if (left.Rows != right.Rows || left.Cols != right.Cols)
                throw new ArgumentException("Matrix dimensions must match for addition");

            var res = new Matrix<Ttype>(left.Rows, left.Cols);
            for (int i = 0; i < left.Rows; i++)
            {
                for (int j = 0; j < left.Cols; j++)
                {
                    res[i, j] = (dynamic)left[i, j] + (dynamic)right[i, j];
                }
            }
            return res;
        }

        public static Matrix<Ttype> operator -(Matrix<Ttype> left, Matrix<Ttype> right)
        {
            if (left.Rows != right.Rows || left.Cols != right.Cols)
                throw new ArgumentException("Matrix dimensions must match for subtraction");

            var res = new Matrix<Ttype>(left.Rows, left.Cols);
            for (int i = 0; i < left.Rows; i++)
            {
                for (int j = 0; j < left.Cols; j++)
                {
                    res[i, j] = (dynamic)left[i, j] - (dynamic)right[i, j];
                }
            }
            return res;
        }

        public static Matrix<Ttype> operator *(Matrix<Ttype> matrix, Ttype num)
        {
            var res = new Matrix<Ttype>(matrix.Rows, matrix.Cols);
            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Cols; j++)
                {
                    res[i, j] = (dynamic)matrix[i, j] * (dynamic)num;
                }
            }
            return res;
        }

        public static Matrix<Ttype> operator *(Matrix<Ttype> left, Matrix<Ttype> right)
        {
            if (left.Cols != right.Rows)
                throw new ArgumentException(
                    "Matrix dimensions must be compatible for multiplication"
                );

            var res = new Matrix<Ttype>(left.Rows, right.Cols);
            for (int i = 0; i < left.Rows; i++)
            {
                for (int j = 0; j < right.Cols; j++)
                {
                    dynamic sum = default(Ttype);
                    for (int k = 0; k < left.Cols; k++)
                    {
                        sum += (dynamic)left[i, k] * (dynamic)right[k, j];
                    }
                    res[i, j] = sum;
                }
            }
            return res;
        }

        public static Matrix<Ttype> Transposition(Matrix<Ttype> matrix)
        {
            var res = new Matrix<Ttype>(matrix.Cols, matrix.Rows);
            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Cols; j++)
                {
                    res[j, i] = matrix[i, j];
                }
            }
            return res;
        }

        public static Matrix<Ttype> GetAdjMatrix(Matrix<Ttype> matrix)
        {
            if (matrix.Rows != matrix.Cols)
                throw new ArgumentException("Matrix must be square for adjugate calculation");

            int n = matrix.Rows;
            var res = new Matrix<Ttype>(n, n);

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    var A = new Matrix<Ttype>(n - 1, n - 1);
                    for (int k = 0, k1 = 0; k < n; k++)
                    {
                        if (k == i)
                            continue;
                        for (int l = 0, l1 = 0; l < n; l++)
                        {
                            if (l == j)
                                continue;
                            A[k1, l1++] = matrix[k, l];
                        }
                        k1++;
                    }
                    res[i, j] = ((i + j) % 2 != 0 ? -1 : 1) * (dynamic)Determinant(A);
                }
            }
            return res;
        }

        public static Matrix<Ttype> GetIdentityMatrix(int rows, int cols)
        {
            var I = new Matrix<Ttype>(rows, cols);
            for (int i = 0; i < Math.Min(rows, cols); i++)
            {
                I[i, i] = (dynamic)1;
            }
            return I;
        }

        public static Ttype Determinant(Matrix<Ttype> matrix)
        {
            return DiagDetRealization(matrix);
        }

        public Matrix<Ttype> GetReverseMatrix()
        {
            if (Rows != Cols)
                throw new InvalidOperationException("Matrix must be square for inversion");

            dynamic det = Determinant(this);
            if (det == (dynamic)default(Ttype))
                throw new InvalidOperationException("Matrix is singular (determinant is zero)");

            dynamic revDet = (dynamic)1 / det;
            var res = Transposition(GetAdjMatrix(this));
            return res * revDet;
        }

        public static Matrix<Ttype> UniteMatrix(Matrix<Ttype> matrix1, Matrix<Ttype> matrix2)
        {
            if (matrix1.Rows != matrix2.Rows)
                throw new ArgumentException(
                    "Matrices must have the same number of rows for unification"
                );

            var res = new Matrix<Ttype>(matrix1.Rows, matrix1.Cols + matrix2.Cols);
            for (int i = 0; i < matrix1.Rows; i++)
            {
                for (int j = 0; j < matrix1.Cols + matrix2.Cols; j++)
                {
                    if (j < matrix1.Cols)
                    {
                        res[i, j] = matrix1[i, j];
                    }
                    else
                    {
                        res[i, j] = matrix2[i, j - matrix1.Cols];
                    }
                }
            }
            return res;
        }

        public void Resize(int newRows, int newCols)
        {
            if (newRows < Rows)
                Elements.RemoveRange(newRows, Rows - newRows);
            else if (newRows > Rows)
                Elements.AddRange(
                    Enumerable.Range(0, newRows - Rows).Select(_ => new Line<Ttype>(newCols))
                );

            foreach (var row in Elements)
            {
                row.Resize(newCols);
            }

            Rows = newRows;
            Cols = newCols;
        }

        public void Resize(int newRows, int newCols, Ttype value)
        {
            if (newRows < Rows)
                Elements.RemoveRange(newRows, Rows - newRows);
            else if (newRows > Rows)
                Elements.AddRange(
                    Enumerable
                        .Range(0, newRows - Rows)
                        .Select(_ => new Line<Ttype>(Enumerable.Repeat(value, newCols)))
                );

            foreach (var row in Elements)
            {
                row.Resize(newCols, value);
            }

            Rows = newRows;
            Cols = newCols;
        }

        public IEnumerator<Line<Ttype>> GetEnumerator() => Elements.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => Elements.GetEnumerator();

        public override string ToString()
        {
            return string.Join(Environment.NewLine, Elements.Select(line => line.ToString()));
        }

        private static Ttype _makeZeroesUnderDiag(Matrix<Ttype> system)
        {
            if (system.Cols != system.Rows)
                throw new ArgumentException("System matrix must have N rows and N+1 columns");

            int N = system.Rows;
            dynamic k = 1;
            bool hasNill = false;

            for (int i = 0; i < N; i++)
            {
                if (!hasNill)
                {
                    k *= system[i, i];
                }
                if (system[i, i] == (dynamic)default(Ttype))
                {
                    hasNill = true;
                    continue;
                }

                system.Elements[i] = system.Elements[i] / system[i, i];
                system[i, i] = (dynamic)1;

                for (int j = i + 1; j < N; j++)
                {
                    system.Elements[j] = system.Elements[j] - (system.Elements[i] * system[j, i]);
                    system[j, i] = default(Ttype);
                }
            }

            return k;
        }

        private static Ttype RecursiveDetRealization(Matrix<Ttype> matrix)
        {
            if (matrix.Rows != matrix.Cols)
                throw new ArgumentException("Matrix must be square for determinant calculation");

            int n = matrix.Rows;

            if (n == 0)
                return default(Ttype);
            else if (n == 1)
                return matrix[0, 0];
            else if (n == 2)
                return (dynamic)matrix[0, 0] * matrix[1, 1] - (dynamic)matrix[0, 1] * matrix[1, 0];
            else
            {
                dynamic res = default(Ttype);
                for (int i = 0; i < n; i++)
                {
                    var minor = new Matrix<Ttype>(n - 1, n - 1);
                    for (int j = 1, j1 = 0; j < n; j++, j1++)
                    {
                        for (int k = 0, k1 = 0; k < n; k++)
                        {
                            if (k == i)
                                continue;
                            minor[j1, k1++] = matrix[j, k];
                        }
                    }
                    res =
                        ((i % 2 != 0) ? -1 : 1)
                            * (dynamic)matrix[0, i]
                            * RecursiveDetRealization(minor)
                        + res;
                }
                return res;
            }
        }

        private static Ttype DiagDetRealization(Matrix<Ttype> matrix)
        {
            var copy = new Matrix<Ttype>(matrix);
            var res = _makeZeroesUnderDiag(copy);
            return res;
        }

        public static (Matrix<double> L, Matrix<double> U) GetLUMatrix(Matrix<double> a)
        {
            if (a.Rows != a.Cols)
            {
                throw new ArgumentException("Matrix must be square for LU decomposition");
            }

            int N = a.Rows;
            Matrix<double> L = new Matrix<double>(N, N);
            Matrix<double> U = new Matrix<double>(N, N);

            for (int i = 0; i < N; i++)
            {
                L[i, i] = 1.0;
            }

            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    if (i <= j)
                    {
                        U[i, j] = a[i, j];
                        for (int k = 0; k < i; k++)
                        {
                            U[i, j] = U[i, j] - L[i, k] * U[k, j];
                        }
                    }
                    else
                    {
                        L[i, j] = a[i, j];
                        for (int k = 0; k < j; k++)
                        {
                            L[i, j] = L[i, j] - L[i, k] * U[k, j];
                        }
                        L[i, j] = L[i, j] / U[j, j];
                    }
                }
            }

            return (L, U);
        }

        public static double LUDeterminant((Matrix<double> L, Matrix<double> U) pr)
        {
            double res = 1.0;
            for (int i = 0; i < pr.U.Rows; i++)
            {
                res *= pr.U[i, i];
            }
            return res;
        }

        public static Matrix<double> LUReverseMatrix(Matrix<double> matrix)
        {
            Matrix<double>[] resN = new Matrix<double>[matrix.Cols];
            Matrix<double> vecI = new Matrix<double>(matrix.Rows, 1, 0.0);
            Matrix<double> revMatrix = new Matrix<double>(matrix.Rows, matrix.Cols);

            for (int i = 0; i < matrix.Cols; i++)
            {
                if (i != 0)
                {
                    vecI[i - 1, 0] = 0.0;
                }
                vecI[i, 0] = 1.0;
                resN[i] = LUSolutionMethod(matrix, vecI);
            }

            for (int i = 0; i < matrix.Rows; i++)
            {
                for (int j = 0; j < matrix.Cols; j++)
                {
                    revMatrix[j, i] = resN[i][j, 0];
                }
            }

            return revMatrix;
        }

        public static Matrix<double> UpGaussMethod(Matrix<double> system)
        {
            if (system.Cols != system.Rows + 1)
            {
                throw new ArgumentException("System matrix must have N rows and N+1 columns");
            }

            int N = system.Rows;
            Matrix<double> res = new Matrix<double>(N, 1);

            for (int i = N - 1; i >= 0; i--)
            {
                system.Elements[i] = system.Elements[i] / system[i, i];
                system[i, i] = 1.0;
                double x = system[i, N];

                for (int j = i - 1; j >= 0; j--)
                {
                    system[j, N] = system[j, N] - (system[j, i] * x);
                    system[j, i] = 0.0;
                }
            }

            for (int i = 0; i < N; i++)
            {
                res[i, 0] = system[i, N];
            }

            return res;
        }

        public static Matrix<double> DownTriangMatrixSolution(Matrix<double> system)
        {
            if (system.Cols != system.Rows + 1)
            {
                throw new ArgumentException("System matrix must have N rows and N+1 columns");
            }

            int N = system.Rows;
            Matrix<double> res = new Matrix<double>(N, 1);

            for (int i = 0; i < N; i++)
            {
                res[i, 0] = system[i, N];
                for (int j = 0; j < i; j++)
                {
                    res[i, 0] = res[i, 0] - system[i, j] * res[j, 0];
                }
            }
            return res;
        }

        public static Matrix<double> LUSolutionMethod(Matrix<double> Ax, Matrix<double> b)
        {
            if (Ax.Rows != Ax.Cols)
            {
                throw new ArgumentException("Coefficient matrix must be square");
            }
            if (Ax.Rows != b.Rows || b.Cols != 1)
            {
                throw new ArgumentException(
                    "Vector b must have same number of rows as A and 1 column"
                );
            }

            int N = Ax.Rows;
            var LUPair = GetLUMatrix(Ax);

            Matrix<double> L_extended = new Matrix<double>(N, N + 1);
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    L_extended[i, j] = LUPair.L[i, j];
                }
                L_extended[i, N] = b[i, 0];
            }

            var y = DownTriangMatrixSolution(L_extended);

            Matrix<double> U_extended = new Matrix<double>(N, N + 1);
            for (int i = 0; i < N; i++)
            {
                for (int j = 0; j < N; j++)
                {
                    U_extended[i, j] = LUPair.U[i, j];
                }
                U_extended[i, N] = y[i, 0];
            }

            var x = UpGaussMethod(U_extended);
            return x;
        }
    }
}
