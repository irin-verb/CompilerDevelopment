using Lab2.Tokens;
using Lab3.DataType;
using Lab3.SymbolTables;
using Lab3.SyntaxTrees;
using Lab3.Tokens;
using Lab3.Tokens.Children;
using System.ComponentModel.Design;
using System.Linq.Expressions;
using System.Net.WebSockets;
using System.Security.Authentication;
using static Lab3.SyntaxTrees.SyntaxTreeNode;

namespace Lab3.Generator
{
    internal static class SyntaxTreeOptimizer
    {
        private static SyntaxTree SyntaxTree;
        private static SymbolTable SymbolTable;
        private static SymbolTable SymbolTableOptimized;

        private const string ERROR_TEXT = "Ошибка оптимизации! ";

        public static (SyntaxTree tree, SymbolTable table) Optimize(SyntaxTree syntaxTree, SymbolTable symbolTable)
        {
            try
            {
                SyntaxTree = syntaxTree.Clone();
                SymbolTable = symbolTable.Clone();
                SymbolTableOptimized = new SymbolTable();

                SyntaxTree = new SyntaxTree(Optimize(SyntaxTree.RootNode));
                RefreshSymbolTable(SyntaxTree.RootNode);

                return (SyntaxTree, SymbolTableOptimized);
            }
            catch (Exception ex)
            {
                throw new Exception(ERROR_TEXT + ex.Message);
            }  
        }

        private static void RefreshSymbolTable(SyntaxTreeNode root)
        {
            if (root.LeftNode != null) RefreshSymbolTable(root.LeftNode);
            if (root.RightNode != null) RefreshSymbolTable(root.RightNode);

            if (root.Token.GetTokenType() == TokenTypeEnum.TokenType.VARIABLE)
            {
                var tkn = (VariableToken)root.Token;
                var name = SymbolTable.Get(tkn.VariableID).Name;
                int id = SymbolTableOptimized.Add(name, tkn.DataType);
                ((VariableToken)(root.Token)).VariableID = id;
            }
        }

        private static SyntaxTreeNode Optimize(SyntaxTreeNode root)
        {
            if (root.Token.GetTokenType() != TokenTypeEnum.TokenType.OPERATION)
                throw new Exception("Непредвиденный вид дерева!");
            SyntaxTreeNode? leftSide = OptimizeChild(root.LeftNode);
            SyntaxTreeNode? rightSide = OptimizeChild(root.RightNode);

            if (leftSide != null) root.LeftNode = leftSide;
            if (rightSide != null) root.RightNode = rightSide;

            OptimizingAction action = DefineOptimizingAction(leftSide, root, rightSide);
            switch (action)
            {
                case OptimizingAction.NONE:
                    {
                        break;
                    }
                case OptimizingAction.NULL:
                    {
                        root = new SyntaxTreeNode(new NumberToken(0, (DataTypeEnum.DataType)root.DataType, -1));
                        break;
                    }
                case OptimizingAction.ONE:
                    {
                        root = new SyntaxTreeNode(new NumberToken(1, (DataTypeEnum.DataType)root.DataType, -1));
                        break;
                    }
                case OptimizingAction.NULL_DIVISION:
                    {
                        throw new Exception("Обнаружено деление на ноль на позиции " + root.Token.Position);
                    }
                case OptimizingAction.UNNECESSARY_LEFT:
                    {
                        root = ReplaceByNode(root, rightSide);
                        break;
                    }
                case OptimizingAction.UNNECESSARY_RIGHT:
                    {
                        root = ReplaceByNode(root, leftSide);
                        break;
                    }
                case OptimizingAction.CALCULATABLE:
                    {
                        var res = Calculate((NumberToken)leftSide.Token, ((OperationToken)(root.Token)).OprtnType, (NumberToken)rightSide.Token);
                        root = new SyntaxTreeNode(new NumberToken(res, (DataTypeEnum.DataType)root.DataType, -1));
                        break;
                    }
                case OptimizingAction.INT_2_FLOAT:
                    {
                        var child = (NumberToken)(leftSide == null ? rightSide.Token : leftSide.Token);
                        root = ReplaceByNode(root, new SyntaxTreeNode(new NumberToken(child.NumValue, DataTypeEnum.DataType.FLOAT, child.Position)));
                        break;
                    } 
                default: break;
            }
            return root;
        }

        private static SyntaxTreeNode ReplaceByNode(SyntaxTreeNode parent, SyntaxTreeNode child)
        {
            SyntaxTreeNode? grandParent = parent.ParentNode;
            if (grandParent == null)
                return child;
            NodeSide? side = parent.Side;
            if (side == null)
                throw new Exception("Ошибка: Указанная сторона родительского узла не определена.");
   
            if (side == NodeSide.LEFT)
                grandParent.LeftNode = child;
            else
                grandParent.RightNode = child;

            child.ParentNode = grandParent;
            return child;

        }

        private static double Calculate(NumberToken left, OperationToken.OperationType op, NumberToken right)
        {
            var var1 = left.NumValue;
            var var2 = right.NumValue;
            switch (op)
            {
                case OperationToken.OperationType.PLUS:
                    return var1 + var2;
                case OperationToken.OperationType.MINUS:
                    return var1 - var2;
                case OperationToken.OperationType.DIVIDE:
                    return var1 / var2;
                case OperationToken.OperationType.MULTIPLY:
                    return var1 * var2;
                case OperationToken.OperationType.INT2FLOAT:
                    throw new Exception("Ошибка при определении типа оптимизации");
            }
            throw new Exception("Ошибка при вычислении");
        }

        private static SyntaxTreeNode? OptimizeChild(SyntaxTreeNode? child)
        {
            return child == null ? null :
            child.Token.GetTokenType() == TokenTypeEnum.TokenType.OPERATION ? 
            Optimize(child) : child;
        }

        private enum OptimizingAction
        {
            NONE,
            ONE,                // x / x
            NULL,               //  0 * x   x * 0   0 / x   x - x 
            NULL_DIVISION,      //  x / 0
            UNNECESSARY_LEFT,   //  1 * x   0 + x
            UNNECESSARY_RIGHT,  //  x / 1   x * 1   x + 0   x - 0
            CALCULATABLE,       //  digit ? digit
            INT_2_FLOAT         //  i2f digit
        }

        private static OptimizingAction DefineOptimizingAction(SyntaxTreeNode? left, SyntaxTreeNode oper, SyntaxTreeNode? right) 
        {
            if (left == null && right == null) return OptimizingAction.NONE;

            OptimizingType optType = DefineOptimizingType(left == null ? null : left.Token, 
                                                            oper.Token, 
                                                            right == null ? null : right.Token);
            
            if ((left == null || right == null) && optType != OptimizingType.I2F) throw new Exception("Ошибка при определении типа оптимизации");

            OperationToken.OperationType op = ((OperationToken)(oper.Token)).OprtnType;
            switch (optType)
            {
                case OptimizingType.I2F:
                    {
                        var child = left == null ? right : left;
                        if (child.Token.GetTokenType() == TokenTypeEnum.TokenType.NUMBER)
                            return OptimizingAction.INT_2_FLOAT;
                        break;
                    }
                    
                case OptimizingType.BOTH_DIGIT:
                    if (op == OperationToken.OperationType.DIVIDE
                        && ((NumberToken)right.Token).NumValue == 0)
                        return OptimizingAction.NULL_DIVISION;
                    return OptimizingAction.CALCULATABLE;

                case OptimizingType.DIGIT_VAR:
                    {
                        double val = ((NumberToken)left.Token).NumValue;
       
                        if ((val == 1 && op == OperationToken.OperationType.MULTIPLY)
                            || (val == 0 && op == OperationToken.OperationType.PLUS))
                            return OptimizingAction.UNNECESSARY_LEFT;

                        if (val == 0
                            && (op == OperationToken.OperationType.DIVIDE || op == OperationToken.OperationType.MULTIPLY))
                            return OptimizingAction.NULL;
                            
                        break;
                    }
                case OptimizingType.VAR_DIGIT:
                    {
                        double val = ((NumberToken)right.Token).NumValue;

                        if (val == 0 && op == OperationToken.OperationType.DIVIDE)
                            return OptimizingAction.NULL_DIVISION;

                        if (val == 0 && op == OperationToken.OperationType.MULTIPLY)
                            return OptimizingAction.NULL;

                        if ((val == 1
                            && (op == OperationToken.OperationType.DIVIDE || op == OperationToken.OperationType.MULTIPLY))
                            || ((val == 0)
                            && (op == OperationToken.OperationType.PLUS || op == OperationToken.OperationType.MINUS)))
                            return OptimizingAction.UNNECESSARY_RIGHT;

                        break;
                    }
                case OptimizingType.BOTH_VAR:
                    {
                        var equals = SyntaxTreeNode.Equals(left, right);
                        if (equals && op == OperationToken.OperationType.MINUS) 
                            return OptimizingAction.NULL;
                        if (equals && op == OperationToken.OperationType.DIVIDE)
                            return OptimizingAction.ONE;
                        break;
                    }
            }
            return OptimizingAction.NONE;
        }

        private enum OptimizingType
        {
            BOTH_DIGIT,
            DIGIT_VAR,
            VAR_DIGIT,
            BOTH_VAR,
            I2F
        }

        private static OptimizingType DefineOptimizingType(Token? left, Token op, Token? right)
        {
            if (left == null || right == null) {
                if (((OperationToken)op).OprtnType == OperationToken.OperationType.INT2FLOAT)
                    return OptimizingType.I2F;
                else throw new Exception("Непредвиденный вид дерева!");
            }
            TokenTypeEnum.TokenType leftType = left.GetTokenType();
            TokenTypeEnum.TokenType rightType = right.GetTokenType();
            TokenTypeEnum.TokenType number = TokenTypeEnum.TokenType.NUMBER;
            return
                leftType == number && rightType == number ? OptimizingType.BOTH_DIGIT :
                leftType == number ? OptimizingType.DIGIT_VAR :
                rightType == number ? OptimizingType.VAR_DIGIT :
                OptimizingType.BOTH_VAR;
        }

      
    }
}
