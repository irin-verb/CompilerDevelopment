using Lab3.SymbolTables;
using Lab3.ThreeAddressCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Lab3
{
    public class FileLogic
    {

        private const string CODE_MARKER = "CODE";
        private const string TABLE_MARKER = "TABLE";

        public static string ReadFromFile(string fileName)
        {
            if (!File.Exists(fileName))
                throw new FileNotFoundException("Ошибка! Файл для чтения " + fileName + " не найден");
            string text = File.ReadAllText(fileName);
            return text;
        }

        public static (ThreeAddressCode code, SymbolTable table) ReadCodeAndTable(string fileName)
        {
            ThreeAddressCode code;
            SymbolTable table;
            try
            {
                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                using (BinaryReader reader = new BinaryReader(fs)) 
                {
                    var label = reader.ReadString();
                    if (!CODE_MARKER.Equals(label)) 
                        throw new Exception();

                    code = new ThreeAddressCode(reader);

                    label = reader.ReadString();
                    if (!TABLE_MARKER.Equals(label)) 
                        throw new Exception();

                    table = new SymbolTable(reader);
                }
                    
            }
            catch (FileNotFoundException)
            {
                throw new FileNotFoundException("Ошибка! Файл для чтения " + fileName + " не найден");
            }
            catch (Exception)
            {
                throw new Exception("Ошибка! Неверный формат входного файла!");
            }
            return (code, table);
        }

        public static void WriteInFile(string fileName, ThreeAddressCode code, SymbolTable table)
        {
            using (FileStream fs = new FileStream(fileName, FileMode.Create))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                writer.Write(CODE_MARKER);
                code.WriteBinary(writer);
                writer.Write(TABLE_MARKER);
                table.WriteBinary(writer);
            }          
        }

        public static void WriteInFile(string fileName, string content)
        {
            File.WriteAllText(fileName, content);
        }
    }
}
