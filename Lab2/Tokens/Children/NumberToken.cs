using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Lab2.Tokens;
using Lab3.DataType;
using static Lab3.Tokens.TokenTypeEnum;

namespace Lab3.Tokens.Children
{
    public class NumberToken : Token
    {
        public double NumValue { get; private set; }

        public NumberToken(NumberToken token) : base(token)
        {
            NumValue = token.NumValue;
        }

        public NumberToken(double numberValue, DataTypeEnum.DataType dataType, int position) 
            : base(dataType, position)
        {
            NumValue = numberValue;
        }

        public override TokenType GetTokenType() => TokenType.NUMBER;

        public override string ToString() => DataType == DataTypeEnum.DataType.INT ? $"<{NumValue:F0}>" : $"<{NumValue:F2}>";

        public override Token Clone() => new NumberToken(this);

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != typeof(NumberToken)) return false;
            var tkn = (NumberToken)obj;
            return base.Equals((Token)obj) && NumValue == tkn.NumValue;
        }

        public override void WriteBinary(BinaryWriter writer)
        {
            base.WriteBinary(writer);
            writer.Write(NumValue);
        }

        public NumberToken(BinaryReader reader) : base(reader) 
        {
            NumValue = reader.ReadDouble();
        }

    }
}
