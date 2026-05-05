using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab2.Tokens;
using Lab3.DataType;
using Lab3.SymbolTables;
using static Lab3.Tokens.TokenTypeEnum;

namespace Lab3.Tokens.Children
{
    public class VariableToken : Token
    {
        public int VariableID { get; set; }

        public VariableToken(VariableToken token) : base(token)
        {
            VariableID = token.VariableID;
        }

        public VariableToken(int variableID, DataTypeEnum.DataType dataType, int position) 
            : base(dataType, position)
        {
            VariableID = variableID;
        }

        public override TokenType GetTokenType() => TokenType.VARIABLE;

        public override string ToString() => $"<id,{VariableID}>";

        public override Token Clone() => new VariableToken(this);

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != typeof(VariableToken)) return false;
            var tkn = (VariableToken)obj;
            return base.Equals((Token)obj) && VariableID == tkn.VariableID;
        }

        public override void WriteBinary(BinaryWriter writer)
        {
            base.WriteBinary(writer);
            writer.Write(VariableID);
        }

        public VariableToken(BinaryReader reader) : base(reader)
        {
            VariableID = reader.ReadInt32();
        }

    }
}
