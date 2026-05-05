using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Analyzer
{
    internal static class LexicalRules
    {
        public const string TOKENIZETION_PATTERN = @"(?<=[+\-*/()])|(?=[+\-*/()])";
        public const string GENERAL_TOKEN_PATTERN = @"^([a-zA-Z_]\w*)(\[[a-zA-Z]*\])?$|^(\d+(\.\d+)?)$|^[+*/()-]$";

        public const string VARIABLE_TOKEN_PATTERN = @"^([a-zA-Z_]\w*)(\[[a-zA-Z]*\])?$";
        public const string VARIABLE_WITH_BRACKETS_PATTERN = @"^([a-zA-Z_]\w*\[[a-zA-Z]*\])$";
        public const string VARIABLE_WITH_SOMETHING_BETWEEN_BRACKETS_PATTERN = @"^([a-zA-Z_]\w*\[[a-zA-Z]+\])$";

        public const string OPERATION_TOKEN_PATTERN = @"^[+\-*/]$";
        public const string LEFT_BKT_TOKEN_PATTERN = "(";
        public const string RIGHT_BKT_TOKEN_PATTERN = ")";

        public const string NUMBER_TOKEN_PATTERN = @"^\d+(\.\d+)?$";
        public const string NUMBER_FLOAT_TOKEN_PATTERN = @"^\d+\.\d+$";

        public const string VARIABLE_LIKELY_PATTERN = @"[a-zA-Z_]";
        public const string DIGIT_LIKELY_PATTERN = @"^[0-9.]+$";
    }
}
