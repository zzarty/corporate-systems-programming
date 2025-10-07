using System;
using System.Threading;

namespace MatrixCalculator
{
    class Program
    {
        static void Main()
        {
            // Оформление консоли
            Console.Title = "Калькулятор матриц";
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // Обозначение главных переменных
            bool exit = false;
            double[,] matrix1 = null;
            double[,] matrix2 = null;

            // Главное меню
            while (!exit)
            {
                Console.Clear();
                DisplayHeader();
                DisplayMenu();

                string choice = Console.ReadLine()?.Trim();

                switch (choice)
                {
                    case "1":
                        matrix1 = CreateMatrix("первую");
                        break;
                    case "2":
                        matrix2 = CreateMatrix("вторую");
                        break;
                    case "3":
                        DisplayBothMatrices(matrix1, matrix2);
                        break;
                    case "4":
                        CalculateDeterminant(matrix1, "первой");
                        break;
                    case "5":
                        CalculateDeterminant(matrix2, "второй");
                        break;
                    case "6":
                        CalculateInverse(matrix1, "первой");
                        break;
                    case "7":
                        CalculateInverse(matrix2, "второй");
                        break;
                    case "red":
                        MatrixRainEffect();
                        break;
                    case "0":
                        exit = true;
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("\nСпасибо за использование программы!");
                        Console.ResetColor();
                        break;
                    default:
                        ShowError("Неверный выбор! Попробуйте снова.");
                        Console.ReadKey();
                        break;
                }

                if (!exit && choice != "0" && choice != "red")
                {
                    Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                    Console.ReadKey();
                }
            }
        }

        static void MatrixRainEffect()
        {
            Console.CursorVisible = false;

            int width = Console.WindowWidth;
            int height = Console.WindowHeight - 1;

            // Массив для хранения позиций "капель"
            int[] drops = new int[width];
            Random random = new Random();

            for (int i = 0; i < width; i++)
            {
                drops[i] = random.Next(-height, 0);
            }

            string chars = "01アイウエオカキクケコサシスセソタチツテトナニヌネノハヒフヘホマミムメモヤユヨラリルレロワヲン";

            // Цикл анимации
            while (true)
            {
                if (Console.KeyAvailable)
                {
                    var key = Console.ReadKey(true);
                    if (key.Key == ConsoleKey.Enter)
                    {
                        break;
                    }
                }

                for (int i = 0; i < width; i++)
                {
                    if (drops[i] >= 0 && drops[i] < height)
                    {
                        try
                        {
                            // Яркий белый символ (голова капли)
                            if (drops[i] < height)
                            {
                                Console.SetCursorPosition(i, drops[i]);
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.Write(chars[random.Next(chars.Length)]);
                            }

                            if (drops[i] - 1 >= 0 && drops[i] - 1 < height)
                            {
                                Console.SetCursorPosition(i, drops[i] - 1);
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write(chars[random.Next(chars.Length)]);
                            }

                            if (drops[i] - 3 >= 0 && drops[i] - 3 < height)
                            {
                                Console.SetCursorPosition(i, drops[i] - 3);
                                Console.ForegroundColor = ConsoleColor.DarkGreen;
                                Console.Write(chars[random.Next(chars.Length)]);
                            }

                            if (drops[i] - 6 >= 0 && drops[i] - 6 < height)
                            {
                                Console.SetCursorPosition(i, drops[i] - 6);
                                Console.ForegroundColor = ConsoleColor.DarkGreen;
                                Console.Write(chars[random.Next(chars.Length)]);
                            }

                            if (drops[i] - 10 >= 0 && drops[i] - 10 < height)
                            {
                                Console.SetCursorPosition(i, drops[i] - 10);
                                Console.ForegroundColor = ConsoleColor.Black;
                                Console.BackgroundColor = ConsoleColor.Black;
                                Console.Write(chars[random.Next(chars.Length)]);
                                Console.ResetColor();
                            }

                            if (drops[i] - 15 >= 0 && drops[i] - 15 < height)
                            {
                                Console.SetCursorPosition(i, drops[i] - 15);
                                Console.Write(" ");
                            }
                        }
                        catch (ArgumentOutOfRangeException)
                        {
                            // Игнорируем ошибки выхода за границы консоли
                        }
                    }

                    // Двигаем каплю вниз
                    drops[i]++;

                    if (drops[i] - 15 > height)
                    {
                        drops[i] = random.Next(-height / 2, -5);
                    }

                    if (random.Next(1000) < 5)
                    {
                        drops[i] = 0;
                    }
                }

                Thread.Sleep(80);
            }

            Console.ResetColor();
            Console.CursorVisible = true;
            Console.Clear();
        }

        // Шапка приложения
        static void DisplayHeader()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("===========================================");
            Console.WriteLine("          КАЛЬКУЛЯТОР МАТРИЦ");
            Console.WriteLine("===========================================\n");
            Console.ResetColor();
        }

        // Главное меню
        static void DisplayMenu()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("ГЛАВНОЕ МЕНЮ:");
            Console.ResetColor();
            Console.WriteLine();
            Console.WriteLine("1. Создать первую матрицу");
            Console.WriteLine("2. Создать вторую матрицу");
            Console.WriteLine("3. Показать обе матрицы");
            Console.WriteLine("4. Детерминант первой матрицы");
            Console.WriteLine("5. Детерминант второй матрицы");
            Console.WriteLine("6. Обратная первая матрица");
            Console.WriteLine("7. Обратная вторая матрица");
            Console.WriteLine("0. Выход");
            Console.WriteLine();
            Console.Write("Ваш выбор: ");
        }

        // Функция создания матрицы
        static double[,] CreateMatrix(string matrixName)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"\n=== СОЗДАНИЕ {matrixName.ToUpper()} МАТРИЦЫ ===\n");
            Console.ResetColor();

            int rows = ReadPositiveInt("Введите количество строк (n): ");
            int cols = ReadPositiveInt("Введите количество столбцов (m): ");

            double[,] matrix = new double[rows, cols];

            Console.WriteLine("\nСпособ заполнения:");
            Console.WriteLine("1 - Ввод с клавиатуры");
            Console.WriteLine("2 - Случайные числа");
            Console.Write("Ваш выбор: ");

            string choice = Console.ReadLine()?.Trim();

            switch (choice)
            {
                case "1":
                    FillMatrixManually(matrix);
                    break;
                case "2":
                    FillMatrixRandom(matrix);
                    break;
                default:
                    ShowWarning("Неверный выбор. Используется случайное заполнение.");
                    FillMatrixRandom(matrix);
                    break;
            }

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n=== МАТРИЦА СОЗДАНА ===\n");
            Console.ResetColor();
            PrintMatrix(matrix);

            return matrix;
        }

        // Заполнение матрицы самостоятельно
        static void FillMatrixManually(double[,] matrix)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);
            bool[,] filled = new bool[rows, cols];

            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("=== ЗАПОЛНЕНИЕ МАТРИЦЫ ===\n");
            Console.ResetColor();

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    bool validInput = false;
                    while (!validInput)
                    {
                        // Позиция курсора для матрицы
                        int matrixStartLine = 3;

                        // Очищаем область матрицы и ввода
                        Console.SetCursorPosition(0, matrixStartLine);

                        // Рисуем матрицу с подсветкой текущей ячейки
                        DrawMatrixWithHighlight(matrix, filled, i, j);

                        // Позиция для ввода (под матрицей)
                        int inputLine = matrixStartLine + rows + 2;
                        Console.SetCursorPosition(0, inputLine);
                        Console.Write(new string(' ', Console.WindowWidth - 1)); // Очистка строки
                        Console.SetCursorPosition(0, inputLine);

                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.Write($"Введите элемент [{i + 1},{j + 1}]: ");
                        Console.ResetColor();

                        string input = Console.ReadLine();

                        if (double.TryParse(input, out double value))
                        {
                            matrix[i, j] = value;
                            filled[i, j] = true;
                            validInput = true;
                        }
                        else
                        {
                            int errorLine = inputLine + 1;
                            Console.SetCursorPosition(0, errorLine);
                            ShowError("Ошибка! Введите числовое значение.");
                            System.Threading.Thread.Sleep(1000);
                            Console.SetCursorPosition(0, errorLine);
                            Console.Write(new string(' ', Console.WindowWidth - 1));
                        }
                    }
                }
            }
        }

        // Отображение заполнения матрицы
        static void DrawMatrixWithHighlight(double[,] matrix, bool[,] filled, int currentRow, int currentCol)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            Console.WriteLine("Матрица:");
            Console.WriteLine();

            for (int i = 0; i < rows; i++)
            {
                Console.Write("  ");
                for (int j = 0; j < cols; j++)
                {
                    bool isCurrent = (i == currentRow && j == currentCol);
                    bool isFilled = filled[i, j];

                    if (isCurrent)
                    {
                        // Текущая ячейка - яркое выделение (8 символов)
                        Console.BackgroundColor = ConsoleColor.DarkGreen;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(" [____] ");
                        Console.ResetColor();
                    }
                    else if (isFilled)
                    {
                        // Заполненная ячейка (8 символов с выравниванием)
                        Console.ForegroundColor = ConsoleColor.Green;
                        string formattedValue = FormatCellValue(matrix[i, j], 6);
                        Console.Write($" {formattedValue} ");
                        Console.ResetColor();
                    }
                    else
                    {
                        // Незаполненная ячейка (8 символов)
                        Console.ForegroundColor = ConsoleColor.DarkGray;
                        Console.Write("  [--]  ");
                        Console.ResetColor();
                    }
                }
                // Очистка остатка строки
                Console.Write(new string(' ', Math.Max(0, Console.WindowWidth - Console.CursorLeft - 1)));
                Console.WriteLine();
            }
        }

        // Форматирование ячейки
        static string FormatCellValue(double value, int width)
        {
            // Пробуем форматировать с 2 знаками после запятой
            string formatted = value.ToString("F2");

            // Если число слишком длинное, используем научную нотацию
            if (formatted.Length > width)
            {
                formatted = value.ToString("E1");
            }

            // Если всё ещё длинное, обрезаем
            if (formatted.Length > width)
            {
                formatted = formatted.Substring(0, width);
            }

            // Выравниваем по правому краю
            return formatted.PadLeft(width);
        }

        // Случайное заполнение матрицы
        static void FillMatrixRandom(double[,] matrix)
        {
            Console.WriteLine();
            int a = ReadInt("Нижняя граница диапазона (a): ");
            int b = ReadInt("Верхняя граница диапазона (b): ");

            if (a > b)
            {
                int temp = a;
                a = b;
                b = temp;
                ShowWarning("Границы переставлены местами.");
            }

            Random random = new Random();

            Console.WriteLine("\nЗаполнение матрицы...\n");

            for (int i = 0; i < matrix.GetLength(0); i++)
            {
                for (int j = 0; j < matrix.GetLength(1); j++)
                {
                    matrix[i, j] = random.Next(a, b + 1);
                }
            }

            ShowSuccess("Матрица заполнена случайными числами.");
            System.Threading.Thread.Sleep(1000);
        }

        // Вывод матрицы
        static void PrintMatrix(double[,] matrix)
        {
            if (matrix == null)
            {
                ShowError("Матрица не создана!");
                return;
            }

            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            Console.WriteLine($"Размерность: {rows}×{cols}\n");

            for (int i = 0; i < rows; i++)
            {
                Console.Write("  ");
                for (int j = 0; j < cols; j++)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    string formattedValue = FormatCellValue(matrix[i, j], 8);
                    Console.Write($"{formattedValue} ");
                    Console.ResetColor();
                }
                Console.WriteLine();
            }
        }

        // отображение обеих матриц
        static void DisplayBothMatrices(double[,] matrix1, double[,] matrix2)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\n=== СОЗДАННЫЕ МАТРИЦЫ ===\n");
            Console.ResetColor();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("ПЕРВАЯ МАТРИЦА:");
            Console.ResetColor();
            if (matrix1 != null)
                PrintMatrix(matrix1);
            else
                ShowWarning("Первая матрица не создана!");

            Console.WriteLine();

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("ВТОРАЯ МАТРИЦА:");
            Console.ResetColor();
            if (matrix2 != null)
                PrintMatrix(matrix2);
            else
                ShowWarning("Вторая матрица не создана!");
        }

        static void CalculateDeterminant(double[,] matrix, string matrixName)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"\n=== ДЕТЕРМИНАНТ {matrixName.ToUpper()} МАТРИЦЫ ===\n");
            Console.ResetColor();

            if (matrix == null)
            {
                ShowError($"Матрица не создана!");
                return;
            }

            if (!IsSquareMatrix(matrix))
            {
                ShowError("Детерминант можно вычислить только для квадратных матриц!");
                Console.WriteLine($"Текущая размерность: {matrix.GetLength(0)}×{matrix.GetLength(1)}");
                return;
            }

            Console.WriteLine("Матрица:");
            PrintMatrix(matrix);

            try
            {
                double determinant = GetDeterminant(matrix);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\nДетерминант = {determinant:F4}");
                Console.ResetColor();
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка: {ex.Message}");
            }
        }

        static void CalculateInverse(double[,] matrix, string matrixName)
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"\n=== ОБРАТНАЯ {matrixName.ToUpper()} МАТРИЦА ===\n");
            Console.ResetColor();

            if (matrix == null)
            {
                ShowError("Матрица не создана!");
                return;
            }

            if (!IsSquareMatrix(matrix))
            {
                ShowError("Обратная матрица существует только для квадратных матриц!");
                Console.WriteLine($"Текущая размерность: {matrix.GetLength(0)}×{matrix.GetLength(1)}");
                return;
            }

            Console.WriteLine("Исходная матрица:");
            PrintMatrix(matrix);

            try
            {
                double determinant = GetDeterminant(matrix);
                Console.WriteLine($"\nДетерминант = {determinant:F4}");

                if (Math.Abs(determinant) < 1e-10)
                {
                    ShowError("Обратная матрица не существует (детерминант = 0)!");
                    return;
                }

                double[,] inverse = GetInverseMatrix(matrix);
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\nОбратная матрица:");
                Console.ResetColor();
                PrintMatrix(inverse);
            }
            catch (Exception ex)
            {
                ShowError($"Ошибка: {ex.Message}");
            }
        }

        static bool IsSquareMatrix(double[,] matrix)
        {
            return matrix.GetLength(0) == matrix.GetLength(1);
        }

        static double GetDeterminant(double[,] matrix)
        {
            int n = matrix.GetLength(0);

            if (n == 1)
                return matrix[0, 0];

            if (n == 2)
                return matrix[0, 0] * matrix[1, 1] - matrix[0, 1] * matrix[1, 0];

            double determinant = 0;

            for (int j = 0; j < n; j++)
            {
                determinant += (j % 2 == 0 ? 1 : -1) * matrix[0, j] * GetDeterminant(GetMinor(matrix, 0, j));
            }

            return determinant;
        }

        static double[,] GetMinor(double[,] matrix, int row, int col)
        {
            int n = matrix.GetLength(0);
            double[,] minor = new double[n - 1, n - 1];

            int minorRow = 0;
            for (int i = 0; i < n; i++)
            {
                if (i == row) continue;

                int minorCol = 0;
                for (int j = 0; j < n; j++)
                {
                    if (j == col) continue;

                    minor[minorRow, minorCol] = matrix[i, j];
                    minorCol++;
                }
                minorRow++;
            }

            return minor;
        }

        static double[,] GetInverseMatrix(double[,] matrix)
        {
            int n = matrix.GetLength(0);
            double determinant = GetDeterminant(matrix);

            if (Math.Abs(determinant) < 1e-10)
                throw new InvalidOperationException("Матрица вырождена");

            double[,] cofactorMatrix = new double[n, n];

            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    double minorDet = GetDeterminant(GetMinor(matrix, i, j));
                    cofactorMatrix[j, i] = ((i + j) % 2 == 0 ? 1 : -1) * minorDet / determinant;
                }
            }

            return cofactorMatrix;
        }

        static int ReadPositiveInt(string prompt)
        {
            int result;
            Console.Write(prompt);
            while (!int.TryParse(Console.ReadLine(), out result) || result <= 0)
            {
                ShowError("Введите положительное целое число!");
                Console.Write(prompt);
            }
            return result;
        }

        static int ReadInt(string prompt)
        {
            int result;
            Console.Write(prompt);
            while (!int.TryParse(Console.ReadLine(), out result))
            {
                ShowError("Введите целое число!");
                Console.Write(prompt);
            }
            return result;
        }

        static void ShowError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"✗ {message}\n");
            Console.ResetColor();
        }

        static void ShowWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"⚠ {message}\n");
            Console.ResetColor();
        }

        static void ShowSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"✓ {message}\n");
            Console.ResetColor();
        }
    }
}