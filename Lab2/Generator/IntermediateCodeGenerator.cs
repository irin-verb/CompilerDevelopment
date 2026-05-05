using Lab2.Tokens;
using Lab3.DataType;
using Lab3.SymbolTables;
using Lab3.SyntaxTrees;
using Lab3.ThreeAddressCodes;
using Lab3.Tokens.Children;
using System.Collections.Generic;


namespace Lab3.Generator
{
    internal static class IntermediateCodeGenerator
    {
        private const string ERROR_TEXT = "Ошибка генерации промежуточного кода! ";

        private static int TempCounter;
        private static ThreeAddressCode Code = new ThreeAddressCode();
        private static SymbolTable SymbolTable = new SymbolTable();
        private static SyntaxTree SyntaxTree = new SyntaxTree();

        private static Dictionary<int, VariableToken> BusyVars = new Dictionary<int, VariableToken>();
        private static Dictionary<int, VariableToken> FreeVars = new Dictionary<int, VariableToken>();

        public static (SymbolTable table, ThreeAddressCode code) Generate(SymbolTable table, SyntaxTree tree)
        {
            try
            {
                Code = new ThreeAddressCode();
                SymbolTable = table.Clone();
                SyntaxTree = tree.Clone();
                TempCounter = 0;

                if (tree.RootNode.LeftNode == null && tree.RootNode.RightNode == null)
                    throw new Exception("Нельзя сгенерировать код к дереву с одним узлом");

                GenerateIntermediateCodeForNode(SyntaxTree.RootNode);
                return (SymbolTable, Code);
            }
            catch (Exception ex)
            {
                throw new Exception(ERROR_TEXT + ex.Message);
            }
        }

        public static (SymbolTable table, ThreeAddressCode code) GenerateOptimized(SymbolTable table, SyntaxTree tree)
        {
            try
            {
                BusyVars.Clear();
                FreeVars.Clear();

                Code = new ThreeAddressCode();
                SymbolTable = table.Clone();
                SyntaxTree = tree.Clone();
                TempCounter = 0;

                if (tree.RootNode.LeftNode == null && tree.RootNode.RightNode == null)
                    throw new Exception("Нельзя сгенерировать код к дереву с одним узлом");

                GenerateIntermediateOptimizedCodeForNode(SyntaxTree.RootNode);
                return (SymbolTable, Code);
            }
            catch (Exception ex)
            {
                throw new Exception(ERROR_TEXT + ex.Message);
            }
        }

        public static SyntaxTreeNode GenerateIntermediateOptimizedCodeForNode(SyntaxTreeNode root)
        {
            if (root.Token.GetTokenType() != Tokens.TokenTypeEnum.TokenType.OPERATION)
                throw new Exception("Непредвиденная ошибка! Попытка построить трехадресную команду для узла, не являющегося операцией: " + root.Token.ToString());

            Token? par1 = ProcessChildNodeOptimized(root.LeftNode);
            Token? par2 = ProcessChildNodeOptimized(root.RightNode);

            VariableToken tempVar = AddTempOptimizedVarToken(root, par1, par2);
            AddCommand((OperationToken)root.Token, tempVar, par1, par2);
            root = new SyntaxTreeNode(tempVar);
            return root;
        }

        private static Token? ProcessChildNodeOptimized(SyntaxTreeNode? child)
        {
            if (child == null) return null;
            if (child.Token.GetTokenType() == Tokens.TokenTypeEnum.TokenType.OPERATION)
                child = GenerateIntermediateOptimizedCodeForNode(child);
            return child.Token;
        }

        private static VariableToken AddTempOptimizedVarToken(SyntaxTreeNode node, Token? par1, Token? par2)
        {
            if (node.DataType == null) throw new Exception("Синтаксическое дерево было построено неправильно! Не удается определить тип узла с токеном " + node.Token.ToString());

            Action<Token?> processVariable = param =>
            {
                if (param != null && param.GetTokenType() == Tokens.TokenTypeEnum.TokenType.VARIABLE)
                {
                    var id = ((VariableToken)param).VariableID;
                    if (BusyVars.ContainsKey(id))
                    {
                        var var = Remove(id, BusyVars);
                        Add(var, FreeVars);
                    }
                }
            };
            processVariable(par1);
            processVariable(par2);

            VariableToken? tempToken = Find((DataTypeEnum.DataType)node.DataType, FreeVars);
            if (tempToken == null)
            {
                int id = SymbolTable.Add("#T" + TempCounter.ToString(), (DataTypeEnum.DataType)node.DataType);
                TempCounter++;
                tempToken = new VariableToken(id, (DataTypeEnum.DataType)node.DataType, -1);
            }
            Add(tempToken, BusyVars);
            return tempToken;
        }

        public static void Add(VariableToken token, Dictionary<int, VariableToken> dict)
        {
            dict.Add(token.VariableID, token);
        }

        public static VariableToken? Remove(int id, Dictionary<int, VariableToken> dict)
        {
            if (dict.TryGetValue(id, out var value))
            {
                dict.Remove(id);
                return value;
            }
            return null;
        }

        public static VariableToken? Find(DataTypeEnum.DataType dataType, Dictionary<int, VariableToken> dict)
        {
            foreach (var kvp in dict)
            {
                if (kvp.Value.DataType == dataType)
                {
                    dict.Remove(kvp.Key);
                    return kvp.Value;
                }
            }
            return null;
        }

        public static SyntaxTreeNode GenerateIntermediateCodeForNode(SyntaxTreeNode root)
        {
            if (root.Token.GetTokenType() != Tokens.TokenTypeEnum.TokenType.OPERATION) throw new Exception("Непредвиденная ошибка! Попытка построить трехадресную команду для узла, не являющегося операцией: " + root.Token.ToString());

            Token? par1 = ProcessChildNode(root.LeftNode);
            Token? par2 = ProcessChildNode(root.RightNode);

            var tempVar = AddTempVarToken(root);
            AddCommand((OperationToken)root.Token, tempVar, par1, par2);
            root = new SyntaxTreeNode(tempVar);
            return root;
        }

        private static Token? ProcessChildNode(SyntaxTreeNode? child)
        {
            if (child == null) return null;
            if (child.Token.GetTokenType() == Tokens.TokenTypeEnum.TokenType.OPERATION)
                child = GenerateIntermediateCodeForNode(child);
            return child.Token;
        }

        private static void AddCommand(OperationToken token, VariableToken variable, Token? par1, Token? par2)
        {
            ThreeAddressCommand codeLine;
            if (par1 == null && par2 == null) throw new Exception("Непредвиденная ошибка! Попытка построить трехадресную команду для узла, у которого нет детей: " + token.ToString());

            if (par1 == null || par2 == null)
                codeLine = new ThreeAddressCommand(token, variable, par2 ?? par1);
            else
                codeLine = new ThreeAddressCommand(token, variable, par1, par2);
            Code.Code.Add(codeLine);
        }

        private static VariableToken AddTempVarToken(SyntaxTreeNode node)
        {
            if (node.DataType == null) throw new Exception("Синтаксическое дерево было построено неправильно! Не удается определить тип узла с токеном " + node.Token.ToString());

            int id = SymbolTable.Add("#T" + TempCounter.ToString(), (DataTypeEnum.DataType)node.DataType);
            TempCounter++;

            VariableToken variableToken = new VariableToken(id, (DataTypeEnum.DataType)node.DataType, -1);
            return variableToken;
        }

    }
}
