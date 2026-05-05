using Lab2.Tokens;
using Lab3.Analyzer;
using Lab3.Generator;
using Lab3.SymbolTables;
using Lab3.SyntaxTrees;
using Lab3.ThreeAddressCodes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lab3
{
    internal static class Compiler
    {
        private static SymbolTable Table { get; set; }
        private static List<Token> Tokens { get; set; }
        private static SyntaxTree Tree { get; set; }
        private static List<Token> PostfixTokens { get; set; }
        private static ThreeAddressCode IntermediateCode { get; set; }


        public static (SymbolTable symbolTable, List<Token> tokens) DoLex(string expression)
        {
            var result = LexicalAnalyzer.Analyze(expression);
            Table = result.symbolTable;
            Tokens = result.tokens;
            return (Table, Tokens);
        }

        public static SyntaxTree DoSyn(string expression)
        {
            DoLex(expression);
            Tree = SyntacticAnalyzer.Analize(Tokens);
            return Tree;
        }

        public static SyntaxTree DoSem(string expression)
        {
            DoSyn(expression);
            Tree = SemanticAnalyzer.Analyze(Tree);
            return Tree;
        }

        public static (SymbolTable symbolTable, ThreeAddressCode code)  DoGen1(string expression)
        {
            DoSem(expression);
            var result = IntermediateCodeGenerator.Generate(Table, Tree);
            IntermediateCode = result.code;
            Table = result.table;
            return (Table, IntermediateCode);
        }

        public static (SymbolTable symbolTable, ThreeAddressCode code) DoGen1Optimized(string expression)
        {
            DoSem(expression);
            DoOpt();

            var result = IntermediateCodeGenerator.GenerateOptimized(Table, Tree);
            IntermediateCode = result.code;
            Table = result.table;
            return (Table, IntermediateCode);
        }

        public static (SymbolTable symbolTable, List<Token> tokens) DoGen2(string expression)
        {
            DoSem(expression);
            PostfixTokens = PostfixCodeGenerator.Generate(Tree);
            return (Table, PostfixTokens);
        }

        public static (SymbolTable symbolTable, List<Token> tokens) DoGen2Optimized(string expression)
        {
            DoSem(expression);
            DoOpt();

            PostfixTokens = PostfixCodeGenerator.Generate(Tree);
            return (Table, PostfixTokens);
        }

        private static void DoOpt()
        {
            var res = SyntaxTreeOptimizer.Optimize(Tree, Table);
            Tree = res.tree;
            Table = res.table;
        }

    }
}
