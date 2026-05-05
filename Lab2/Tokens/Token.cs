using Lab3.DataType;
using Lab3.SymbolTables;
using Lab3.SyntaxTrees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static Lab3.Tokens.TokenTypeEnum;

namespace Lab2.Tokens
{
    public abstract class Token
    {
        public int Position {  get; protected set; }
        public DataTypeEnum.DataType DataType { get; protected set; }

        public Token(Token token) 
        {
            Position = token.Position;
            DataType = token.DataType;
        }

        public Token(DataTypeEnum.DataType dataType, int position)
        {
            DataType = dataType;
            Position = position; 
        }

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != typeof(Token)) return false;
            var tkn = (Token)obj;
            return Position == tkn.Position && DataType == tkn.DataType;
        } 

        public virtual bool Similar(object? obj)
        {
            if (obj == null) return false;
            var tkn = (Token)obj;
            return DataType == tkn.DataType;
        }

        public virtual void WriteBinary(BinaryWriter writer)
        {
            writer.Write((int)GetTokenType());
            writer.Write(Position);
            writer.Write((int)DataType);
        }

        public Token(BinaryReader reader)
        {
            Position = reader.ReadInt32();
            DataType = (DataTypeEnum.DataType)reader.ReadInt32();
        }

        public abstract TokenType GetTokenType();

        public abstract Token Clone();

    }
}
