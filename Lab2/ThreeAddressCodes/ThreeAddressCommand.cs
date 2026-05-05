using Lab2.Tokens;
using Lab3.DataType;
using Lab3.SymbolTables;
using Lab3.Tokens.Children;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using static Lab3.Tokens.Children.OperationToken;

namespace Lab3.ThreeAddressCodes
{
    public class ThreeAddressCommand
    {
        public OperationToken Command { get; set; }
        public VariableToken Dest { get; set; }
        public Token Par1 { get; set; }
        public Token? Par2 { get; set; }

        public ThreeAddressCommand(OperationToken command, VariableToken dest, Token par1, Token? par2)
        {
            Command = command;
            Dest = dest;
            Par1 = par1;
            Par2 = par2;
        }

        public ThreeAddressCommand(OperationToken command, VariableToken dest, Token par) : this(command, dest, par, null) { }

        public override string ToString() => Command.ToCommandString() + " " +
            Dest.ToString() + " " + Par1.ToString() + (Par2 == null ? "" : (" " + Par2.ToString()));

        public void WriteBinary(BinaryWriter writer)
        {
            Command.WriteBinary(writer);
            Dest.WriteBinary(writer);
            Par1.WriteBinary(writer);
            writer.Write(Par2 != null);
            if (Par2 != null)
                Par2.WriteBinary(writer);    
        }

        public ThreeAddressCommand(BinaryReader reader)
        {

            Command = (OperationToken)TokenFactory.ReadBinary(reader);
            Dest = (VariableToken)TokenFactory.ReadBinary(reader);
            Par1 = TokenFactory.ReadBinary(reader);
            bool isPar2 = reader.ReadBoolean();
            Par2 = isPar2 ? TokenFactory.ReadBinary(reader) : null;
        }
    }
}
