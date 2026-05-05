using Lab3;
using Lab3.DataType;
using Lab3.SymbolTables;
using Lab3.ThreeAddressCodes;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace postCodeInterp
{
    internal static class Runner
    {
        private const string SEPARATOR = "\n--------------------------------------------------------------\n";
        private const string FILE_PATH = "C:/__study__/4_class/РазработкаКомпиляторов/Lab7/Lab2/bin/Debug/net8.0/files/bin/";

        public static void Main(string[] args)
        {
            Console.WriteLine(SEPARATOR);
            try
            {
                CheckArgs(args);
                string inputFile = args[0];

                var res = FileLogic.ReadCodeAndTable(FILE_PATH + inputFile);
                ThreeAddressCode code = res.code;
                SymbolTable table = res.table;
                
                Console.WriteLine("Исходный файл: " + inputFile + "\n");
                Console.WriteLine("Трехадресный код:");
                Console.WriteLine(code.ToString() + "\n");
                Console.WriteLine("Таблица символов:");
                Console.WriteLine(table.ToString() + "\n");

                var varValues = GetDictionary(table);
                var result = Interpretator.Calculate(code, table, varValues);

                Console.WriteLine();
                Console.WriteLine("Результат вычислений:");
                Console.WriteLine(result);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
            }
            Console.WriteLine(SEPARATOR);
        }

        private static Dictionary<int, double> GetDictionary(SymbolTable table)
        {
            Dictionary<int, double> variableValues = new Dictionary<int, double>();

            foreach (var kvp in table.Table)
            {
                if (!kvp.Value.Name.StartsWith("#"))
                {
                    while (true)
                    {
                        Console.Write($"{kvp.Value.DataType.ToStr()} {kvp.Value.Name} = ");
                        string? input = Console.ReadLine();
                        bool isInt = kvp.Value.DataType == DataTypeEnum.DataType.INT;
                        try
                        {
                            double value = isInt ? int.Parse(input) : double.Parse(input);
                            variableValues.Add(kvp.Key, value);
                            break;
                        } 
                        catch (Exception ex)
                        {
                            Console.WriteLine(isInt ? "Введите целое число!" : "Введите вещественное число!");
                            continue;
                        }
                    }
                }
            }
            return variableValues;
        }

        private static void CheckArgs(string[] args)
        {
            if (args.Length != 1)
                throw new Exception("Ошибка! Укажите один аргумент - название бинарного файла");
            if (!args[0].EndsWith(".bin"))
                throw new Exception("Ошибка! Формат файла должен быть бинарным!");
        }


    }
}
