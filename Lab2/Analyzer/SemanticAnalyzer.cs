using Lab2.Tokens;
using Lab3.DataType;
using Lab3.SyntaxTrees;
using Lab3.Tokens.Children;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Analyzer
{
    internal static class SemanticAnalyzer
    {
        private const string ERROR_TEXT = "Семантическая ошибка! ";
        private const double EPS = 0e0001;

        private static SyntaxTree SyntaxTree = new SyntaxTree();

        public static SyntaxTree Analyze(SyntaxTree tree)
        {
            try
            {
                SyntaxTree = tree.Clone();

                CheckDivisionByZero(SyntaxTree.RootNode);
                FormUpdatedSyntaxTree(SyntaxTree.RootNode);
           
                return SyntaxTree;
            }
            catch (Exception ex)
            {
                throw new Exception(ERROR_TEXT + ex.Message);
            }
        }

        private static DataTypeEnum.DataType FormUpdatedSyntaxTree(SyntaxTreeNode root) 
        {
            DataTypeEnum.DataType? leftType = root.LeftNode != null ? FormUpdatedSyntaxTree(root.LeftNode) : null;
            DataTypeEnum.DataType? rightType = root.RightNode != null ? FormUpdatedSyntaxTree(root.RightNode) : null;
            var rootType = DataTypeEnum.DefineDominantType(new List<DataTypeEnum.DataType?>() { root.DataType, leftType, rightType });
            if (rootType == null)
                throw new Exception("Ошибка приведения типов! Невозможно определить тип токена " + root.Token.ToString() + " на позиции " + root.Token.Position + " в дереве, так как у него отсутствуют дочерние узлы");
            if (root.DataType == null)
                root.DataType = rootType;
            if (leftType != null && rootType != leftType)
                SyntaxTree.InsertNode(root, new OperationToken(OperationToken.OperationType.INT2FLOAT, -1), SyntaxTreeNode.NodeSide.LEFT);
            if (rightType != null && rootType != rightType)
                SyntaxTree.InsertNode(root, new OperationToken(OperationToken.OperationType.INT2FLOAT, -1), SyntaxTreeNode.NodeSide.RIGHT);
            return (DataTypeEnum.DataType)root.DataType;
        }

        private static void CheckDivisionByZero(SyntaxTreeNode? root)
        {
            if (root != null 
                && root.Token.GetTokenType() == Tokens.TokenTypeEnum.TokenType.OPERATION
                && root.RightNode != null
                && root.RightNode.Token.GetTokenType() == Tokens.TokenTypeEnum.TokenType.NUMBER)
            {
                var op = ((OperationToken)root.Token).OprtnType;
                var val = ((NumberToken)root.RightNode.Token).NumValue;
                if (val <= EPS && op == OperationToken.OperationType.DIVIDE)   
                        throw new Exception("Деление на ноль: токен " + root.RightNode.Token.ToString() + " на позиции " + root.RightNode.Token.Position);
                CheckDivisionByZero(root.LeftNode);
                CheckDivisionByZero(root.RightNode);
            }
        }
    }
}
