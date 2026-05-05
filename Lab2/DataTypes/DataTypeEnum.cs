using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.DataType
{
    public static class DataTypeEnum
    {
        public enum DataType { INT = 0, FLOAT = 1, UNDEFINED = 3 }

        public static DataType? DefineDominantType(List<DataType?> dataTypes)
            => dataTypes.Contains(DataType.FLOAT) ?
            DataType.FLOAT :
            dataTypes.Contains(DataType.INT) ?
            DataType.INT :
            null;

        public static string ToStr(this DataType dataType)
        {
            Dictionary<DataType, string> dataTypeToStringMap = new Dictionary<DataType, string>()
            {
                { DataType.INT, "integer" },
                { DataType.FLOAT, "float"},
                { DataType.UNDEFINED, "undefined" }
            };
            return dataTypeToStringMap[dataType];
        }
    }

}
