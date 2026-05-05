using Lab3.SymbolTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.ThreeAddressCodes
{
    public class ThreeAddressCode
    {

        public List<ThreeAddressCommand> Code;

        public ThreeAddressCode(List<ThreeAddressCommand> code) { Code = code; }
        public ThreeAddressCode() { Code = new List<ThreeAddressCommand>(); }

        public void WriteBinary(BinaryWriter writer)
        {
            writer.Write(Code.Count);
            foreach (var command in Code)
            {     
                command.WriteBinary(writer);
            }
                
        }

        public ThreeAddressCode(BinaryReader reader)
        {
            Code = new List<ThreeAddressCommand>();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                var command = new ThreeAddressCommand(reader);
                Code.Add(command);
            }
        }

        public override string ToString() => string.Join("\n", Code);

    }
}
