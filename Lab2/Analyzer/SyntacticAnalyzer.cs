using Lab2.Tokens;
using Lab3.Analyzer.Rules;
using Lab3.SyntaxTrees;
using Lab3.Tokens;
using Lab3.Tokens.Children;
using static Lab3.SyntaxTrees.SyntaxTreeNode;
using static Lab3.Tokens.TokenTypeEnum;

namespace Lab3.Analyzer
{
    internal static class SyntacticAnalyzer
    {
        private const string ERROR_TEXT = "Синтаксическая ошибка! ";

        private static SyntaxTree SyntaxTree = new SyntaxTree();
        private static int[] NestingLevels = Array.Empty<int>();
        private static List<Token> Tokens = new List<Token>(); 

        public static SyntaxTree Analize(List<Token> tokens)
        {
            try
            {
                Tokens = tokens.Select(token => token.Clone()).ToList();
                CheckBracketBalance();
                CheckTokenCombinations();

                DefineNestingLevels();
                RemoveBrackets();
                DefineAllNestingLevels();

                SyntaxTree = FormSyntaxTree(Tokens, NestingLevels);
                return SyntaxTree;
            }
            catch (Exception ex) 
            { 
                throw new Exception(ERROR_TEXT + ex.Message); 
            }         
        }

        private static SyntaxTree FormSyntaxTree(List<Token> tokens, int[] levels)
        {
            int lvl = levels.Min();
            int ind = FindLowestPriorityOperation(tokens, levels, lvl);
            int leftInd = ind;
            int rightInd = ind + 1;

            SyntaxTreeNode root = new SyntaxTreeNode(tokens[ind].Clone());
            SyntaxTreeNode left, right;
            SyntaxTree tree = new SyntaxTree(root);

            if (leftInd != 1)
            {
                List<Token> subTokens = tokens.GetRange(0, leftInd);
                int[] subLevels = new int[subTokens.Count];
                Array.Copy(levels, 0, subLevels, 0, subLevels.Length);
                left = FormSyntaxTree(subTokens, subLevels).RootNode;
            }
            else
            {
                left = new SyntaxTreeNode(tokens[0].Clone());
            }
            if (rightInd != levels.Length - 1)
            {
                List<Token> subTokens = tokens.GetRange(rightInd, levels.Length - rightInd);
                int[] subLevels = new int[subTokens.Count];
                Array.Copy(levels, rightInd, subLevels, 0, subLevels.Length);
                right = FormSyntaxTree(subTokens, subLevels).RootNode;
            }
            else
            {
                right = new SyntaxTreeNode(tokens[levels.Length - 1].Clone());
            }
            tree.AddNode(root, right, NodeSide.RIGHT);
            tree.AddNode(root, left, NodeSide.LEFT);
            return tree;
        }

        private static void DefineAllNestingLevels()
        {
            NestingLevels = DefineAllNestingLevels(Tokens, NestingLevels);
        }

        private static int[] DefineAllNestingLevels(List<Token> tokens, int[] nestingLevels)     
        {
            // метод расстановки уровней вложенности / ВСЕХ скобок выражения / приоритетов операций
            int level = nestingLevels.Min();
            // если где-то еще не расставлены скобки 
            if (!AllBracketsAreDone(nestingLevels))
            {
                // наименее приоритетная операция
                int lowestPriorityIndex = FindLowestPriorityOperation(tokens, nestingLevels, level);
                int leftBorder = lowestPriorityIndex;
                int rightBorder = lowestPriorityIndex + 1;
                // слева от операции
                if (leftBorder != 1)
                {
                    // расставляем скобки (уровни вложенности), если необходимо на этом уровне
                    if (!AllBracketsAreDone(nestingLevels, level)) 
                        for (int i = 0; i < leftBorder; ++i)
                            nestingLevels[i] += 1;
                    // спускаемся в след уровень вложенности слева
                    // выделям токены и сооотв им уровни вложенности
                    List<Token> subTokens = tokens.GetRange(0, leftBorder);
                    int[] subLevels = new int[subTokens.Count];
                    Array.Copy(nestingLevels, 0, subLevels, 0, subLevels.Length);
                    // рекурсивно вызываем для уровней метод расстановки уровней вложенности
                    subLevels = DefineAllNestingLevels(subTokens, subLevels);
                    Array.Copy(subLevels, nestingLevels, subLevels.Length);
                }
                // справа от операции
                if (rightBorder != nestingLevels.Length - 1)
                {
                    // расставляем скобки (уровни вложенности), если необходимо на этом уровне
                    if (!AllBracketsAreDone(nestingLevels, level))
                        for (int i = rightBorder; i < nestingLevels.Length; ++i)
                            nestingLevels[i] += 1;
                    // спускаемся в след уровень вложенности справа
                    // выделям токены и сооотв им уровни вложенности
                    List<Token> subTokens = tokens.GetRange(rightBorder, nestingLevels.Length - rightBorder);
                    int[] subLevels = new int[subTokens.Count];
                    Array.Copy(nestingLevels, rightBorder, subLevels, 0, subLevels.Length);
                    // рекурсивно вызываем для уровней метод расстановки уровней вложенности
                    subLevels = DefineAllNestingLevels(subTokens, subLevels);
                    Array.Copy(subLevels, 0, nestingLevels, rightBorder, subLevels.Length);
                }
            }
            return nestingLevels;
        }

        private static int FindLowestPriorityOperation(List<Token> tokens, int[] nestingLevels, int level)
        {
            int lowestPriority = int.MaxValue;
            int lowestPriorityIndex = -1;
            for (int i = 0; i < tokens.Count; i++)
            {
                if (tokens[i].GetTokenType() == TokenType.OPERATION && nestingLevels[i] == level)
                {
                    int priority = GetPriority(tokens[i]);
                    if (priority <= lowestPriority)
                    {
                        lowestPriority = priority;
                        lowestPriorityIndex = i;
                    }
                }
            }
            return lowestPriorityIndex;
        }

        private static bool AllBracketsAreDone(int[] nestingLevels)
        {
            int repeatCount = 1;
            int prevLvl = nestingLevels[0];
            for (int i = 1; i < nestingLevels.Length; ++i)
            {
                int currLvl = nestingLevels[i];
                if (currLvl == prevLvl) repeatCount++;
                if (currLvl != prevLvl) repeatCount = 1;
                prevLvl = currLvl;
                if (repeatCount > 3) return false;
            }
            return true;
        }

        private static bool AllBracketsAreDone(int[] nestingLevels, int level)
        {
            int repeatCount = 0;
            foreach (int lvl in nestingLevels)
            {
                if (lvl == level) repeatCount++;
            }
            return repeatCount <= 3;
        }

        private static int GetPriority(Token token) => SyntacticRules.OPERATIONS_PRIORITY[((OperationToken)token).OprtnType];

        private static void RemoveBrackets()
        {
            List<int> bracketIndices = Tokens
                .Select((token, index) => new { token, index })
                .Where(item => item.token.GetTokenType() == TokenType.LEFT_BKT || item.token.GetTokenType() == TokenType.RIGHT_BKT)
                .Select(item => item.index)
                .ToList();
            Tokens = Tokens
                .Where((token, index) => !bracketIndices.Contains(index))
                .ToList();
            NestingLevels = NestingLevels
                .Where((_, index) => !bracketIndices.Contains(index))
                .ToArray();
        }

        private static void DefineNestingLevels()
        {
            int nestingLevel = 0;
            NestingLevels = new int[Tokens.Count];
            for (int i = 0; i < Tokens.Count; ++i)
            {
                if (Tokens[i].GetTokenType() == TokenType.LEFT_BKT)
                    nestingLevel++;
                NestingLevels[i] = nestingLevel;
                if (Tokens[i].GetTokenType() == TokenType.RIGHT_BKT)
                    nestingLevel --;
            }
        }

        private static void CheckTokenCombinations()
        {
            Token prevToken = Tokens[0];
            for (int i = 1; i < Tokens.Count; i++)
            {
                Token currToken = Tokens[i];
                string? notValidСombination = SyntacticRules.PROHIBITIONS[(int)prevToken.GetTokenType(), (int)currToken.GetTokenType()];
                if (notValidСombination != null)
                    throw new Exception("После " + prevToken.GetTokenType().ToStr() + " "
                        + prevToken.ToString() + " на позиции " + prevToken.Position + " " + notValidСombination);
                prevToken = currToken;
            }
        }

        private static void CheckBracketBalance()
        {        
            int balance = 0;
            int lastOpenedPosition = 0;
            foreach (Token token in Tokens)
            {
                if (token.GetTokenType() == TokenType.LEFT_BKT)
                {
                    balance++;
                    lastOpenedPosition = token.Position;
                }

                if (token.GetTokenType() == TokenType.RIGHT_BKT)
                {
                    balance--;
                    if (balance < 0) 
                        throw new Exception("Лишняя закрывающая скобочка на позиции " + token.Position);
                }  
            }
            if (balance > 0)
                throw new Exception("Незакрытая открывающая скобочка на позиции " + lastOpenedPosition);
        }

    }
}
