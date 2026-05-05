using Lab2.Tokens;
using Lab3.SymbolTables;
using Lab3.Tokens.Children;
using System.Text.RegularExpressions;
using static Lab3.Tokens.TokenTypeEnum;

namespace Lab3.Analyzer
{
    internal static class LexicalAnalyzer
    {
        private const string ERROR_TEXT = "Лексическая ошибка! ";

        private static SymbolTable symbolTable = new SymbolTable();
        private static List<Token> tokens = new List<Token>();

        public static (SymbolTable symbolTable, List<Token> tokens) Analyze(string expression)
        {
            tokens.Clear();
            symbolTable = new SymbolTable();

            int index = 0;
            string[] WhiteSpaceSymbols = PrepareExpression(expression).Split(" ");

            for (int i = 0; i < WhiteSpaceSymbols.Length; ++i)
            {
                string symbols = WhiteSpaceSymbols[i];
                string[] parts = Regex.Split(symbols, LexicalRules.TOKENIZETION_PATTERN).Where(p => !string.IsNullOrWhiteSpace(p)).ToArray();

                for (int j = 0; j < parts.Length; ++j)
                {
                    string part = parts[j];
                    try
                    {
                        Token token = TokenFactory.DefineToken(part, index, symbolTable);
                        tokens.Add(token);
                    }
                    catch (Exception ex)
                    {
                        throw new Exception(ERROR_TEXT + ex.Message);
                    }
                    index += part.Length;
                }
                index++;
            }
            return (symbolTable, tokens);
        }

        public static string PrepareExpression(string expression)
        {
            string processedExpression = Regex.Replace(expression, @"[\t\r\n]+", " ");
            processedExpression = Regex.Replace(processedExpression, @"\s{2,}", " ");
            processedExpression = processedExpression.Trim();
            return processedExpression;
        }

    }
}
