using Lab2.Tokens;
using Lab3.Analyzer.Rules;
using Lab3.DataType;
using Lab3.Tokens;
using Lab3.Tokens.Children;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Lab3.SyntaxTrees
{
    internal class SyntaxTreeNode
    {
        public enum NodeSide  { LEFT, RIGHT }
        public Token Token { get; set; }
        public DataTypeEnum.DataType? DataType { get; set; }
        public SyntaxTreeNode? ParentNode { get; set; }
        public SyntaxTreeNode? LeftNode { get; set; }
        public SyntaxTreeNode? RightNode { get; set; }
        public NodeSide? Side { get; set; }

        public SyntaxTreeNode(Token token) 
        {
            if (!SyntacticRules.SYNTAX_TREE_PERMITTED_TOKEN_TYPES.Contains(token.GetTokenType()))
                throw new Exception("Ошибка создания узла синтаксического дерева с запрещенным типом " + token.GetTokenType().ToStr());
            Token = token;
            DataType = DefineDataType(token);
            ParentNode = null;
            LeftNode = null;
            RightNode = null;
            Side = null;
        }

        public SyntaxTreeNode(SyntaxTreeNode node)
        {
            Token = node.Token.Clone();
            DataType = node.DataType;
            ParentNode = node.ParentNode;
            Side = node.Side;
            if (node.LeftNode != null)
                LeftNode = new SyntaxTreeNode(node.LeftNode);
            if (node.RightNode != null)
                RightNode = new SyntaxTreeNode(node.RightNode);
        }

        public override string? ToString() => Token.ToString();
    
        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != typeof(SyntaxTreeNode)) return false;

            var node = (SyntaxTreeNode)obj;
            if (!Token.Equals(node.Token)) return false;

            if (DataType != null)
            {
                if (node.DataType == null || DataType != node.DataType)
                    return false;
            }         
            else if (node.DataType != null)
                return false;

            if (Side != null)
            {
                if (node.Side == null || Side != node.Side) return false;
            }
            else if (node.Side != null) 
                return false;

            if (LeftNode != null)
            {
                if (node.LeftNode == null || !LeftNode.Equals(node.LeftNode))
                    return false;
            }
            else if (node.LeftNode != null)
                return false;

            if (RightNode != null)
            {
                if (node.RightNode == null || !RightNode.Equals(node.RightNode))
                    return false;
            }  
            else if (node.RightNode != null)
                return false;
   
            return true;
        }

        private DataTypeEnum.DataType? DefineDataType(Token token)
        {
            switch (token.GetTokenType())
            {
                case TokenTypeEnum.TokenType.NUMBER:
                case TokenTypeEnum.TokenType.VARIABLE:
                    return token.DataType;
                case TokenTypeEnum.TokenType.OPERATION:
                    {
                        var type = ((OperationToken)token).OprtnType;
                        if (type == OperationToken.OperationType.INT2FLOAT
                            || type == OperationToken.OperationType.DIVIDE)
                            return DataTypeEnum.DataType.FLOAT;
                        return null;
                    }
                default:
                    return null;
            }
        }

        public static bool Equals(SyntaxTreeNode node1, SyntaxTreeNode node2)
        {
            if (!node1.Token.Similar(node2.Token)) return false;
            if (node1.DataType != node2.DataType) return false;

            bool directComparison = EqualsNodes(node1.LeftNode, node2.LeftNode) && EqualsNodes(node1.RightNode, node2.RightNode);
            bool swappedComparison = EqualsNodes(node1.LeftNode, node2.RightNode) && EqualsNodes(node1.RightNode, node2.LeftNode);
            return directComparison || swappedComparison;
        }

        private static bool EqualsNodes(SyntaxTreeNode? node1, SyntaxTreeNode? node2)
        {
            if (node1 == null && node2 == null) return true;
            if (node1 == null || node2 == null) return false;
            return Equals(node1, node2);
        }


    }
}
