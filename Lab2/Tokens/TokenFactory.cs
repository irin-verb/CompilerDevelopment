using Lab3.Analyzer;
using Lab3.DataType;
using Lab3.SymbolTables;
using Lab3.Tokens.Children;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Lab2.Runner;
using static Lab2.Tokens.Token;
using static Lab3.Tokens.Children.BracketToken;
using static Lab3.Tokens.Children.OperationToken;
using static Lab3.Tokens.TokenTypeEnum;

namespace Lab2.Tokens
{
    internal static class TokenFactory
    {
        public static Token DefineToken(string token, int position, SymbolTable table)
        {
            if (!Regex.IsMatch(token, LexicalRules.GENERAL_TOKEN_PATTERN))
                CheckWrongToken(token, position);
            TokenType type = DefineTokenType(token, position);
            Token resultToken = GetToken(type, token, position, table);
            return resultToken;
        }

        private static (DataTypeEnum.DataType dataType, string varName) DefineVariableDataTypeAndName(string token, int position)
        {
            DataTypeEnum.DataType resultType = DataTypeEnum.DataType.INT;
            string resultName = token;

            TokenType tokenType = DefineTokenType(token, position);
            if (tokenType != TokenType.VARIABLE)
                throw new Exception("Ошибка определения типа переменной! Попытка определить тип данных у " + tokenType.ToStr());
            
            if (Regex.IsMatch(token, LexicalRules.VARIABLE_WITH_BRACKETS_PATTERN))
            {
                if (Regex.IsMatch(token, LexicalRules.VARIABLE_WITH_SOMETHING_BETWEEN_BRACKETS_PATTERN))
                {
                    int startIndex = token.IndexOf('[');
                    int endIndex = token.IndexOf(']', startIndex);
                    resultType = DefineDataType(token, token.Substring(startIndex + 1, endIndex - startIndex - 1), position);
                    resultName = token.Substring(0, resultName.Length - startIndex - 2);
                }              
                else
                    resultName = token.Substring(0, resultName.Length - 2);
            }
            return (resultType, resultName);
        }

        private static DataTypeEnum.DataType DefineNumberDataType(string token, int position)
        {
            TokenType tokenType = DefineTokenType(token, position);
            if (tokenType != TokenType.NUMBER)
                throw new Exception("Ошибка определения типа константы! Попытка определить тип данных у " + tokenType.ToStr());
            return
                Regex.IsMatch(token, LexicalRules.NUMBER_FLOAT_TOKEN_PATTERN) ?
                DataTypeEnum.DataType.FLOAT :
                DataTypeEnum.DataType.INT;
        }

        private static Token GetToken(TokenType type, string token, int position, SymbolTable table) 
        {
            switch (type)
            {
                case TokenType.VARIABLE:
                    {
                        var tokenTypeAndName = DefineVariableDataTypeAndName(token, position);
                        int id = table.Add(tokenTypeAndName.varName, tokenTypeAndName.dataType);                        
                        return new VariableToken(id, tokenTypeAndName.dataType, position);
                    }
                case TokenType.NUMBER:
                    {
                        var tokenType = DefineNumberDataType(token, position);
                        return new NumberToken(double.Parse(token, CultureInfo.InvariantCulture), tokenType, position);
                    }
                case TokenType.OPERATION:
                    {
                        OperationType operationType = OperationToken.DefineOperationType(token);
                        return new OperationToken(operationType, position);
                    }
                case TokenType.LEFT_BKT:
                    {
                        return new BracketToken(BracketType.LEFT, position);
                    }
                case TokenType.RIGHT_BKT:
                    {
                        return new BracketToken(BracketType.RIGHT, position);
                    }
                default: throw new Exception("Ошибка создания токена! В фабричном методе нет реализации данного типа токена!");
            }
        }

        private static TokenType DefineTokenType(string token, int position)
        {
            if (Regex.IsMatch(token, LexicalRules.VARIABLE_TOKEN_PATTERN))
                return TokenType.VARIABLE;
            if (Regex.IsMatch(token, LexicalRules.OPERATION_TOKEN_PATTERN))
                return TokenType.OPERATION;
            if (Regex.IsMatch(token, LexicalRules.NUMBER_TOKEN_PATTERN))
                return TokenType.NUMBER;
            if (LexicalRules.LEFT_BKT_TOKEN_PATTERN.Equals(token))
                return TokenType.LEFT_BKT;
            if (LexicalRules.RIGHT_BKT_TOKEN_PATTERN.Equals(token))
                return TokenType.RIGHT_BKT;
            throw new Exception("Ошибка создания токена! В фабричном методе нет реализации данного типа токена!");
        }

        private static DataTypeEnum.DataType DefineDataType(string token, string type, int position)
        {
            if (type == null) return DataTypeEnum.DataType.INT;
            if ("I".Equals(type.ToUpper())) return DataTypeEnum.DataType.INT;
            if ("F".Equals(type.ToUpper())) return DataTypeEnum.DataType.FLOAT;
            throw new Exception("Неизвестный тип " + type + " переменной " + token + " на позиции " + position);
        }

        private static void CheckWrongToken(string token, int position)
        {
            if (Regex.IsMatch(token, LexicalRules.VARIABLE_LIKELY_PATTERN))
            {
                if (token.IndexOfAny(new char[] { '[', ']' }) == -1)
                    throw new Exception("Имя переменной " + token + " на позиции " + position + " может содержать только буквы, цифры, квадратные скобки, символ нижн.подч. и не начинаться на цифры или скобки");
                if (token.IndexOf("[") == -1)
                    throw new Exception("В имени переменной " + token + " на позиции " + position + " не хватает открывающей квадратной скобки");
                if (token.IndexOf("]") == -1)
                    throw new Exception("В имени переменной " + token + " на позиции " + position + " не хватает закрывающей квадратной скобки");
                if (token.IndexOf("[") == 0)
                    throw new Exception("Имя переменной " + token + " на позиции " + position + " не должно начинаться на квадратную скобку");
                if (token.LastIndexOf("]") != token.Length - 1)
                    throw new Exception("Имя переменной " + token + " на позиции " + position + " должно заканчиваться закрывающей скобкой");
                if (token.Count(c => c == '[') > 1 || token.Count(c => c == ']') > 1)
                    throw new Exception("Имя переменной " + token + " на позиции " + position + " должно либо не содержать квадратных скобок, либо содержать по одной закрывающей и открывающей");
                throw new Exception("Недопустимый символ в квадратных скобках в переменной " + token + " на позиции " + position);
            }    
            if (Regex.IsMatch(token, LexicalRules.DIGIT_LIKELY_PATTERN))
                throw new Exception("Неправильно задана константа " + token + " на позиции " + position);
            throw new Exception("Недопустимый символ " + token + " на позиции " + position);
        }

        public static Token ReadBinary(BinaryReader reader)
        {
            var type = (TokenType)reader.ReadInt32();
            switch (type)
            {
                case TokenType.LEFT_BKT:
                case TokenType.RIGHT_BKT:
                    return new BracketToken(reader);
                case TokenType.NUMBER:
                    return new NumberToken(reader);
                case TokenType.OPERATION:
                    return new OperationToken(reader);
                case TokenType.VARIABLE:
                    return new VariableToken(reader);
                default:
                    throw new Exception("Ошибка создания токена! В фабричном методе нет реализации данного типа токена!");
            }
        }
  
    }
}
