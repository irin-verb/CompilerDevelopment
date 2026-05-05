using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Lab2.Tokens;
using Lab3.DataType;
using static Lab3.Tokens.TokenTypeEnum;

namespace Lab3.Tokens.Children
{
    public class OperationToken : Token
    {
        public enum OperationType
        {
            PLUS = 0,
            MINUS = 1,
            MULTIPLY = 2,
            DIVIDE = 3,
            INT2FLOAT = 4
        }

        public OperationType OprtnType { get; private set; }

        public OperationToken(OperationToken token) : base(token)
        {
            OprtnType = token.OprtnType;
        }

        public OperationToken(OperationType operationType, int position) : base(DataTypeEnum.DataType.UNDEFINED, position)
        {
            OprtnType = operationType;
            if (OprtnType == OperationType.INT2FLOAT)
                DataType = DataTypeEnum.DataType.FLOAT;
        }

        public override TokenType GetTokenType() => TokenType.OPERATION;

        public override string ToString() => $"<{OperationTypeToString(OprtnType)}>";

        public override Token Clone() => new OperationToken(this);

        public string ToCommandString()
        {
            Dictionary<OperationType, string> operationToCommandStringMap = new Dictionary<OperationType, string>()
            {
                { OperationType.DIVIDE, "div" },
                { OperationType.MINUS, "sub"},
                { OperationType.PLUS, "add"},
                { OperationType.MULTIPLY, "mul"},
                { OperationType.INT2FLOAT, "i2f" }
            };
            return operationToCommandStringMap[OprtnType];
        }

        public static OperationType DefineOperationType(string token)
        {
            Dictionary<string, OperationType> stringToOperationMap = new Dictionary<string, OperationType>()
            {
                { "/", OperationType.DIVIDE },
                { "-", OperationType.MINUS},
                { "+", OperationType.PLUS},
                { "*", OperationType.MULTIPLY}
            };
            return stringToOperationMap[token];
        }

        private static string OperationTypeToString(OperationType token)
        {
            Dictionary<OperationType, string> operationToStringMap = new Dictionary<OperationType, string>()
            {
                { OperationType.DIVIDE, "/" },
                { OperationType.MINUS, "-"},
                { OperationType.PLUS, "+"},
                { OperationType.MULTIPLY, "*"},
                { OperationType.INT2FLOAT, "i2f" }
            };
            return operationToStringMap[token];
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != typeof(OperationToken)) return false;
            var tkn = (OperationToken)obj;
            return base.Equals((Token)obj) && OprtnType == tkn.OprtnType;
        }

        public override bool Similar(object? obj)
        {
            if (obj == null || obj.GetType() != typeof(OperationToken)) return false;
            var tkn = (OperationToken)obj;
            return base.Similar((Token)obj) && OprtnType == tkn.OprtnType;
        }

        public override void WriteBinary(BinaryWriter writer)
        {
            base.WriteBinary(writer);
            writer.Write((int)OprtnType);
        }

        public OperationToken(BinaryReader reader) : base(reader) 
        {
            OprtnType = (OperationType)reader.ReadInt32();
        }

    }
}
