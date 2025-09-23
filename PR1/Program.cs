using System;

namespace calculator
{
    internal class Program
    {
        static void Main(string[] args)
        {
            double memory = 0;
            string choice;
            do
            {
                Console.WriteLine("Добро пожаловать в калькулятор. Вам необходимо ввести первое число, затем знак действия (+, -, *, /, %, sqr, sqrt, inv, m+, m-, mr), и для бинарных операций - второе число.");
                Console.Write("Введите первое число: ");
                double num1;
                while (!double.TryParse(Console.ReadLine(), out num1))
                {
                    Console.WriteLine("Ошибка. Введите нормальное число.");
                    Console.Write("Введите первое число: ");
                }

                Console.Write("Введите знак действия: ");
                string op = Console.ReadLine().Trim().ToLower();

                double num2 = 0;
                bool isBinary = op == "+" || op == "-" || op == "*" || op == "/" || op == "%";
                if (isBinary)
                {
                    Console.Write("Введите второе число: ");
                    while (!double.TryParse(Console.ReadLine(), out num2))
                    {
                        Console.WriteLine("Ошибка. Введите нормальное число.");
                        Console.Write("Введите второе число: ");
                    }
                }

                double result = 0;
                bool valid = true;

                switch (op)
                {
                    case "+":
                        result = num1 + num2;
                        Console.WriteLine("Сумма ваших чисел равна " + result);
                        break;
                    case "-":
                        result = num1 - num2;
                        Console.WriteLine("Разность ваших чисел равна " + result);
                        break;
                    case "*":
                        result = num1 * num2;
                        Console.WriteLine("Произведение ваших чисел равно " + result);
                        break;
                    case "/":
                        if (num2 == 0)
                        {
                            Console.WriteLine("Ошибка. Делитель не может быть равным нулю.");
                            valid = false;
                        }
                        else
                        {
                            result = num1 / num2;
                            Console.WriteLine("Частное ваших чисел равно " + result);
                        }
                        break;
                    case "%":
                        result = num1 % num2;
                        Console.WriteLine("Остаток от деления равен " + result);
                        break;
                    case "sqr":
                        result = num1 * num1;
                        Console.WriteLine("Квадрат числа равен " + result);
                        break;
                    case "sqrt":
                        if (num1 < 0)
                        {
                            Console.WriteLine("Ошибка. Квадратный корень из отрицательного числа не возможен.");
                            valid = false;
                        }
                        else
                        {
                            result = Math.Sqrt(num1);
                            Console.WriteLine("Квадратный корень равен " + result);
                        }
                        break;
                    case "inv":
                        if (num1 == 0)
                        {
                            Console.WriteLine("Ошибка. Обратное значение для нуля не возможно.");
                            valid = false;
                        }
                        else
                        {
                            result = 1 / num1;
                            Console.WriteLine("Обратное значение равно " + result);
                        }
                        break;
                    case "m+":
                        memory += num1;
                        result = memory;
                        Console.WriteLine("Значение в памяти: " + result);
                        break;
                    case "m-":
                        memory -= num1;
                        result = memory;
                        Console.WriteLine("Значение в памяти: " + result);
                        break;
                    case "mr":
                        result = memory;
                        Console.WriteLine("Значение из памяти: " + result);
                        break;
                    default:
                        Console.WriteLine("Ошибка. Вы ввели неверный знак.");
                        valid = false;
                        break;
                }

                if (!valid)
                {
                    Console.WriteLine("Операция не выполнена из-за ошибки.");
                }

                Console.WriteLine("Для продолжения нажмите y, для выхода n...");
                choice = Console.ReadLine().Trim().ToLower();
                Console.WriteLine();
            } while (choice == "y");
        }
    }
}