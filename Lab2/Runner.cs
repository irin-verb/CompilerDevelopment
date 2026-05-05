using Lab2.Tokens;
using Lab3;
using Lab3.Analyzer;
using System.Runtime.Serialization.Formatters.Binary;


namespace Lab2
{
    internal static class Runner
    {
        public enum Command 
        { 
            LEX, 
            SYN,
            SEM,
            GEN1,
            GEN1_OPT,
            GEN2,
            GEN2_OPT,
            GEN3
        }

        private const string SRC = "files/input.txt";

        private const string TKN = "files/tokens.txt";
        private const string SMBL = "files/symbols.txt";

        private const string TREE = "files/tree.txt";
        private const string TREE_SEM = "files/tree_sem.txt";

        private const string INTRMD_CODE = "files/code_intermediate.txt";

        private const string PTFX_CODE = "files/code_postfix.txt";

        private const string SMBL_INTERMEDIATE = "files/symbols_intermediate.txt";
        private const string SMBL_POSTFIX = "files/symbols_postfix.txt";

        private const string BINARY = "files/bin/post_code.bin";


        private const string SEPARATOR = "\n--------------------------------------------------------------\n";

        public static void Main(string[] args)
        {
            Console.WriteLine(SEPARATOR);
            try
            {
                Command command = DefineCommand(args);
                string inputExpression = FileLogic.ReadFromFile(SRC);
                Console.WriteLine("Исходное выражение из файла " + SRC + ":\n" + LexicalAnalyzer.PrepareExpression(inputExpression));
                Console.WriteLine();

                switch (command)
                {
                    case Command.LEX:
                        {
                            var result = Compiler.DoLex(inputExpression);
                            string tokens = string.Join("\n", result.tokens);
                            string symbols = result.symbolTable.ToString();

                            FileLogic.WriteInFile(TKN, tokens);
                            Console.WriteLine("Последовательность токенов записана в файл " + TKN + ":");
                            Console.WriteLine(tokens);

                            Console.WriteLine();

                            FileLogic.WriteInFile(SMBL, symbols);
                            Console.WriteLine("Таблица символов записана в файл " + SMBL + ":");
                            Console.WriteLine(symbols);
                            break;
                        }
                    case Command.SYN:
                        {
                            var result = Compiler.DoSyn(inputExpression);
                            string syntaxTree = result.ToString();

                            FileLogic.WriteInFile(TREE, syntaxTree);
                            Console.WriteLine("Синтаксическое дерево записано в файл " + TREE + ":");
                            Console.WriteLine(syntaxTree);
                            break;
                        }
                    case Command.SEM:
                        {
                            var result = Compiler.DoSem(inputExpression);
                            string syntaxTree = result.ToString();

                            FileLogic.WriteInFile(TREE_SEM, syntaxTree);
                            Console.WriteLine("Модифицированное синтаксическое дерево записано в файл " + TREE_SEM + ":");
                            Console.WriteLine(syntaxTree);
                            break;
                        }
                    case Command.GEN1:
                    case Command.GEN1_OPT:
                        {
                            var result = command == Command.GEN1 ? 
                                Compiler.DoGen1(inputExpression) :
                                Compiler.DoGen1Optimized(inputExpression);

                            string code = result.code.ToString();
                            string table = result.symbolTable.ToString();

                            FileLogic.WriteInFile(INTRMD_CODE, code);
                            Console.WriteLine("Трехадресный код записан в файл " + INTRMD_CODE + ":");
                            Console.WriteLine(code);

                            Console.WriteLine();

                            FileLogic.WriteInFile(SMBL_INTERMEDIATE, table);
                            Console.WriteLine("Таблица символов записана в файл " + SMBL_INTERMEDIATE + ":");
                            Console.WriteLine(table);
                            break;
                        }
                    case Command.GEN2:
                    case Command.GEN2_OPT:
                        {
                            var result = command == Command.GEN2 ? 
                                Compiler.DoGen2(inputExpression) : 
                                Compiler.DoGen2Optimized(inputExpression);

                            string tokens = string.Join(" ", result.tokens);
                            string symbols = result.symbolTable.ToString();

                            FileLogic.WriteInFile(PTFX_CODE, tokens);
                            Console.WriteLine("Постфиксная запись записана в файл " + PTFX_CODE + ":");
                            Console.WriteLine(tokens);

                            Console.WriteLine();

                            FileLogic.WriteInFile(SMBL_POSTFIX, symbols);
                            Console.WriteLine("Таблица символов записана в файл " + SMBL_POSTFIX + ":");
                            Console.WriteLine(symbols);
                            break;
                        }
                    case Command.GEN3:
                        {
                            var result = Compiler.DoGen1Optimized(inputExpression);
                            var code = result.code;
                            var table = result.symbolTable;

                            FileLogic.WriteInFile(BINARY, code, table);
                            Console.WriteLine("Трехадресный код записан в файл " + BINARY + ":");
                            Console.WriteLine(code.ToString());
                            Console.WriteLine();
                            Console.WriteLine("Таблица символов записана в файл " + BINARY + ":");
                            Console.WriteLine(table.ToString());
                            break;
                        }
                }
                Console.WriteLine(SEPARATOR);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message + ex.StackTrace);
            }
        }

        private static Command DefineCommand(string[] args)
        {
            if (args.Length != 2 && args.LongLength != 1)
                throw new Exception("Ошибка! Укажите один аргумент - режим работы утилиты [второй аргумент опционально]");

            Command command;
            string commandName = args[0].ToUpper();

            if ("LEX".Equals(commandName))
                command = Command.LEX;
            else if ("SYN".Equals(commandName))
                command = Command.SYN;
            else if ("SEM".Equals(commandName))
                command = Command.SEM;
            else if ("GEN3".Equals(commandName))
                command = Command.GEN3;

            else if ("GEN1".Equals(commandName))
            {
                if (args.LongLength == 1)
                    command = Command.GEN1;
                else
                {
                    if ("OPT".Equals(args[1].ToUpper()))
                        command = Command.GEN1_OPT;
                    else throw new Exception("После команды gen1 допустим только параметр opt!");
                }
            }
            else if ("GEN2".Equals(commandName))
            {
                if (args.LongLength == 1)
                    command = Command.GEN2;
                else
                {
                    if ("OPT".Equals(args[1].ToUpper()))
                        command = Command.GEN2_OPT;
                    else throw new Exception("После команды gen1 допустим только параметр opt!");
                }
            }
            else throw new Exception("Неизвестное имя команды! Введите одну из команд: lex, syn, sem, gen1, gen2");

            return command;            
        }


    }
}
