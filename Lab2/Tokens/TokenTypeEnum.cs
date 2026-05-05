using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lab3.Tokens
{
    public static class TokenTypeEnum
    {
        public enum TokenType
        {
            NUMBER = 0,
            VARIABLE = 1,
            OPERATION = 2,
            LEFT_BKT = 3,
            RIGHT_BKT = 4
        }
        public static string ToStr(this TokenType token)
        {
            Dictionary<TokenType, string> tokenTypeToStringMap = new Dictionary<TokenType, string>()
            {
                { TokenType.NUMBER, "числа" },
                { TokenType.VARIABLE, "переменной"},
                { TokenType.OPERATION, "операции"},
                { TokenType.LEFT_BKT, "открывающей скобки"},
                { TokenType.RIGHT_BKT, "закрывающей скобки"}
            };
            return tokenTypeToStringMap[token];
        }
    }
}
