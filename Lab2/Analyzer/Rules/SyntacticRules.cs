using Lab3.DataType;
using Lab3.Tokens;
using Lab3.Tokens.Children;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Lab3.Tokens.Children.OperationToken;

namespace Lab3.Analyzer.Rules
{
    internal static class SyntacticRules
    {
        private const string OPERATION_ERR = "отсутствует операция";
        private const string OPERAND_ERR = "отсутствует операнд";

        public static readonly string?[,] PROHIBITIONS = new string?[5, 5]
        {
                     //    NUMBER,        VARIABLE,      OPERATION,   LEFT_BKT,      RIGHT_BKT
        /*    NUMBER */  { OPERATION_ERR, OPERATION_ERR, null,        OPERATION_ERR, null           },
        /*  VARIABLE */  { OPERATION_ERR, OPERATION_ERR, null,        OPERATION_ERR, null           },
        /* OPERATION */  { null,          null,          OPERAND_ERR, null,          OPERAND_ERR    },
        /*  LEFT_BKT */  { null,          null,          OPERAND_ERR, null,          OPERAND_ERR    },
        /* RIGHT_BKT */  { OPERATION_ERR, OPERATION_ERR, null,        OPERATION_ERR, null           }
        };

        public static readonly Dictionary<OperationType, int> OPERATIONS_PRIORITY = new Dictionary<OperationType, int>()
        {
            { OperationType.MULTIPLY, 2 },
            { OperationType.DIVIDE, 2 },
            { OperationType.MINUS, 1 },
            { OperationType.PLUS, 1 }
        };

        public static readonly List<TokenTypeEnum.TokenType> SYNTAX_TREE_PERMITTED_TOKEN_TYPES = new List<TokenTypeEnum.TokenType>()
        {
            TokenTypeEnum.TokenType.NUMBER,
            TokenTypeEnum.TokenType.VARIABLE,
            TokenTypeEnum.TokenType.OPERATION
        };

    }
}
