using Lab2.Tokens;
using Lab3.SymbolTables;
using Lab3.ThreeAddressCodes;
using Lab3.Tokens;
using Lab3.Tokens.Children;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace postCodeInterp
{
    internal static class Interpretator
    {
        private static ThreeAddressCode Code;
        private static SymbolTable Table;

        private const string ERROR_TEXT = "Ошибка интерпретации! ";

        public static double Calculate(ThreeAddressCode code, SymbolTable table, Dictionary<int, double> variableValues)
        {
            Code = code;
            Table = table;
            try
            {
                FillTableValues(variableValues);
                ExecuteCode();
                var id = Code.Code[Code.Code.Count - 1].Dest.VariableID;
                var res = Table.Get(id);
                if (res == null) throw new Exception("");
                return res.Value;
            }
            catch (Exception ex)
            {
                throw new Exception(ERROR_TEXT + ex.Message);
            }
        }

        private static void FillTableValues(Dictionary<int, double> variableValues)
        {
            foreach (var kvp in variableValues)
                Table.Table[kvp.Key].Value = kvp.Value;
        }

        private static void ExecuteCode()
        {
            foreach (var command in Code.Code)
            {
                var cmnd = command.Command.OprtnType;
                if (cmnd != OperationToken.OperationType.INT2FLOAT && command.Par2 == null ||
                    cmnd == OperationToken.OperationType.INT2FLOAT && command.Par2 != null) 
                    throw new Exception("Непредвиденный формат трехадресного кода!");

                var dest = Table.Get(command.Dest.VariableID);
                if (dest == null) throw new Exception("Непредвиденный формат трехадресного кода!");

                ProcessCommand(command); 
            }
        }

        private static void ProcessCommand(ThreeAddressCommand cmnd)
        {
            var oprtnType = cmnd.Command.OprtnType;
            var dest = Table.Get(cmnd.Dest.VariableID);

            double par1 = GetTokenValue(cmnd.Par1);
            double? par2 = cmnd.Par2 == null ? null : GetTokenValue(cmnd.Par2);

            switch (oprtnType)
            {
                case OperationToken.OperationType.PLUS:
                    dest.Value = par1 + (double)par2;
                    break;
                case OperationToken.OperationType.MINUS:
                    dest.Value = par1 - (double)par2;
                    break;
                case OperationToken.OperationType.MULTIPLY:
                    dest.Value = par1 * (double)par2;       
                    break;
                case OperationToken.OperationType.DIVIDE:
                    if ((double)par2 == 0) throw new Exception("Обнаружено деление на ноль!");
                    dest.Value = par1 / (double)par2;
                    break;
                case OperationToken.OperationType.INT2FLOAT:
                    dest.Value = par1;
                    break;
            }
        }

        private static double GetTokenValue(Token token)
        {
            var tknType = token.GetTokenType();
            if (tknType == TokenTypeEnum.TokenType.NUMBER)
                return ((NumberToken)token).NumValue;
            else if (tknType == TokenTypeEnum.TokenType.VARIABLE)
            {
                var id = ((VariableToken)token).VariableID;
                if (Table.Get(id) == null) 
                    throw new Exception("Непредвиденный формат трехадресного кода!");
                return Table.Get(id).Value;
            } 
            throw new Exception("Непредвиденный формат трехадресного кода!");
        }


    }
}
