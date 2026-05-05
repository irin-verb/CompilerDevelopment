using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab3.DataType;

namespace Lab3.SymbolTables
{    
    public class CompositeSymbol
    {
        public string Name { get; set; }
        public DataTypeEnum.DataType DataType { get; set; }
        public double Value { get; set; }

        public CompositeSymbol(string name, DataTypeEnum.DataType dataType)
        {
            Name = name;
            DataType = dataType;
            Value = 0;
        }

        public CompositeSymbol(CompositeSymbol compositeSymbol)
        {
            this.Name = compositeSymbol.Name;
            this.DataType = compositeSymbol.DataType;
            this.Value = compositeSymbol.Value;
        }

        public override string ToString() => Name + ", " + DataType.ToStr();

        public void WriteBinary(BinaryWriter writer)
        {
            writer.Write(Name);
            writer.Write((int)DataType);
            writer.Write(Value);
        }

        public CompositeSymbol(BinaryReader reader)
        {
            Name = reader.ReadString();
            DataType = (DataTypeEnum.DataType)reader.ReadInt32();
            Value = reader.ReadDouble();
        }


    }
}
