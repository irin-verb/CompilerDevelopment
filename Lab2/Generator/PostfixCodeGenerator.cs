using Lab2.Tokens;
using Lab3.SymbolTables;
using Lab3.SyntaxTrees;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3.Generator
{
    internal static class PostfixCodeGenerator
    {

        private static List<Token> Tokens = new List<Token>();


        public static List<Token> Generate(SyntaxTree tree)
        {
            Tokens.Clear();
            Tokens = FormPostfixTokenList(tree.RootNode);
            return Tokens;
        }

        private static List<Token> FormPostfixTokenList(SyntaxTreeNode? root)
        {
            List<Token> tokens = new List<Token>();
            if (root != null)
            {
                List<Token> leftPart = FormPostfixTokenList(root.LeftNode);
                foreach (var tkn in leftPart)
                    tokens.Add(tkn);

                List<Token> rightPart = FormPostfixTokenList(root.RightNode);
                foreach (var tkn in rightPart)
                    tokens.Add(tkn);

                tokens.Add(root.Token);
                return tokens;
            }
            else
            {
                return new List<Token>();
            }
        }

    }
}
