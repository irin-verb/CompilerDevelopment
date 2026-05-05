using Lab2.Tokens;
using Lab3.DataType;
using Lab3.SyntaxTrees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Lab2.Tokens.Token;
using static Lab3.Tokens.TokenTypeEnum;

namespace Lab3.Tokens.Children
{
    internal class BracketToken : Token
    {
        public enum BracketType { LEFT, RIGHT }

        public BracketType BktType { get; private set; }

        public BracketToken(BracketToken token) : base(token)
        {
            BktType = token.BktType;
        }

        public BracketToken(BracketType bracketType, int position) : base(DataTypeEnum.DataType.UNDEFINED, position)
        {
            BktType = bracketType;
        }

        public override TokenType GetTokenType() => BktType == BracketType.LEFT ? TokenType.LEFT_BKT : TokenType.RIGHT_BKT;

        public override string ToString() => GetTokenType() == TokenType.LEFT_BKT ? "<(>" : "<)>";

        public override Token Clone() => new BracketToken(this);

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != typeof(BracketToken)) return false;
            var tkn = (BracketToken)obj;
            return base.Equals((Token)obj) && BktType == tkn.BktType;
        }

        public override void WriteBinary(BinaryWriter writer)
        {
            base.WriteBinary(writer);
            writer.Write((int)BktType);
        }

        public BracketToken(BinaryReader reader) : base(reader) 
        {
            BktType = (BracketType)reader.ReadInt32();        
        }

    }
}
