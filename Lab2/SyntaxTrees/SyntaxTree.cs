using Lab2.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using static Lab3.SyntaxTrees.SyntaxTreeNode;

namespace Lab3.SyntaxTrees
{
    internal class SyntaxTree
    {
        public SyntaxTreeNode RootNode { get; set; }

        public SyntaxTree() { }

        public SyntaxTree(SyntaxTreeNode rootNode)
        {
            RootNode = rootNode;
        }

        public SyntaxTree(SyntaxTree tree)
        {
            RootNode = new SyntaxTreeNode(tree.RootNode);
        }

        public SyntaxTree Clone() => new SyntaxTree(this);

        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != typeof(SyntaxTree)) return false;
            return RootNode == ((SyntaxTree)obj).RootNode;
        }

        public SyntaxTreeNode AddNode(SyntaxTreeNode targetNode, Token token, NodeSide side)
        {
            SyntaxTreeNode? parentNode = FindNode(RootNode, targetNode);
            if (parentNode == null) throw new Exception("Ошибка создания синтаксического дерева! Указанного узла нет в дереве");

            SyntaxTreeNode childNode = new SyntaxTreeNode(token);
            if (side == NodeSide.LEFT)
                parentNode.LeftNode = childNode;
            else
                parentNode.RightNode = childNode;    
            childNode.Side = side;
            childNode.ParentNode = parentNode;
            return childNode;
        }

        public SyntaxTreeNode AddNode(SyntaxTreeNode targetNode, SyntaxTreeNode addNode, NodeSide side)
        {
            SyntaxTreeNode? parentNode = FindNode(RootNode, targetNode);
            if (parentNode == null) throw new Exception("Ошибка создания синтаксического дерева! Указанного узла нет в дереве");

            SyntaxTreeNode childNode = addNode;
            if (side == NodeSide.LEFT)
                parentNode.LeftNode = childNode;
            else
                parentNode.RightNode = childNode;
            childNode.Side = side;
            childNode.ParentNode = parentNode;
            return childNode;
        }

        public void InsertNode(SyntaxTreeNode targetNode, Token token, NodeSide side)
        {
            SyntaxTreeNode? parent = FindNode(RootNode, targetNode);
            if (parent == null) throw new Exception("Ошибка создания синтаксического дерева! Указанного узла нет в дереве");

            SyntaxTreeNode? child = side == NodeSide.LEFT ? FindNode(RootNode, parent.LeftNode) : FindNode(RootNode, parent.RightNode);
            if (child == null) throw new Exception("Ошибка создания синтаксического дерева! Попытка добавить токен к родителю, у которого нет детей");

            SyntaxTreeNode inserted = new SyntaxTreeNode(token);

            inserted.Side = side;
            inserted.ParentNode = parent;
            if (side == NodeSide.LEFT) inserted.LeftNode = child; else inserted.RightNode = child;

            child.ParentNode = inserted;
            if (side == NodeSide.LEFT) parent.LeftNode = inserted; else parent.RightNode = inserted;
        }

        public SyntaxTreeNode? FindNode(SyntaxTreeNode? currentNode, SyntaxTreeNode targetNode)
        {
            if (currentNode == null) return null;
            if (currentNode == targetNode) return currentNode;

            SyntaxTreeNode foundNode = FindNode(currentNode.LeftNode, targetNode);
            if (foundNode != null) return foundNode;
            return FindNode(currentNode.RightNode, targetNode);
        }

        public int CountInternalNodes()
        {
            return CountInternalNodes(RootNode);
        }

        private int CountInternalNodes(SyntaxTreeNode? node)
        {
            if (node == null) return 0;
            bool isInternal = node.LeftNode != null || node.RightNode != null;
            int countLeft = CountInternalNodes(node.LeftNode);
            int countRight = CountInternalNodes(node.RightNode);
            return (isInternal ? 1 : 0) + countLeft + countRight;
        }

        public override string ToString()
        {
            StringBuilder result = new StringBuilder();
            PrintNode(RootNode, result, "", true);
            return result.ToString();
        }

        private void PrintNode(SyntaxTreeNode node, StringBuilder result, string indent, bool isLast)
        {
            if (node == null) return;
            result.AppendLine(indent + (isLast ? "|---" : "|---") + node.ToString());
            indent += isLast ? "    " : "|   ";
            if (node.LeftNode != null)
                PrintNode(node.LeftNode, result, indent, node.RightNode == null); ;
            if (node.RightNode != null)
                PrintNode(node.RightNode, result, indent, true);
        }

    }
}
