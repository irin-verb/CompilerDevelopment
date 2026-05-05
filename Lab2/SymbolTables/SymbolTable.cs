using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Lab2.Tokens;
using Lab3.DataType;

namespace Lab3.SymbolTables
{
    public class SymbolTable
    {
        public Dictionary<int, CompositeSymbol> Table;

        public SymbolTable() { Table = new Dictionary<int, CompositeSymbol>(); }

        public SymbolTable(SymbolTable table)
        {
            Table = new Dictionary<int, CompositeSymbol>();
            foreach (var kvp in table.Table)
                Table.Add(kvp.Key, new CompositeSymbol(kvp.Value));
        }

        public SymbolTable(BinaryReader reader)
        {
            Table = new Dictionary<int, CompositeSymbol>();
            int count = reader.ReadInt32();
            for (int i = 0; i < count; i++)
            {
                int key = reader.ReadInt32();
                var symbol = new CompositeSymbol(reader);
                Table.Add(key, symbol);
            }
        }

        public SymbolTable Clone() => new SymbolTable(this);

        public int Add(string varName, DataTypeEnum.DataType varType)
        {
            int id = DefineID(varName);
            if (!Table.Values.Any(value => value.Name == varName))
            {
                Table.Add(id, new CompositeSymbol(varName, varType));
            } 
            else
            {
                var currType = Table[id].DataType;
                if (currType != varType)
                    throw new Exception("Ошибка добавления переменной " + varName + 
                        " в синтаксическое дерево! Эта переменная уже записана с типом " + currType.ToStr() +
                        ", нельзя записать эту же переменную с типом " + varType.ToStr());
            }
            return id;
        }

        public void Add(int id, string varName, DataTypeEnum.DataType varType)
        {
            Table.Add(id, new CompositeSymbol(varName, varType));
        }

        public CompositeSymbol? Get(int id) => Table.ContainsKey(id) ? Table[id] : null;

        private int DefineID(string varName) => Table.Values.Any(value => value.Name == varName) ? Table.FirstOrDefault(kv => kv.Value.Name == varName).Key : Table.Count;

        public override string ToString() => string.Join("\n", Table.Select(kvp => $"<id,{kvp.Key}> - " + kvp.Value.ToString()).ToList());

        public void WriteBinary(BinaryWriter writer)
        {
            writer.Write(Table.Count);
            foreach (var kvp in Table)
            {
                writer.Write(kvp.Key);
                kvp.Value.WriteBinary(writer);
            }
        }

    }
}
