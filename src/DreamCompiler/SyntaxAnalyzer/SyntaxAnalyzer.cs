using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DreamCompiler.Constants;
using DreamCompiler.Expressions;
using DreamCompiler.Scanner;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace DreamCompiler.SyntaxAnalyzer
{
    public class SyntaxAnalyzer
    {
        private LexemeListManager list;
        private int currentLocation = 0;
        private LexemeEnumerator lexemeEnumerator;

        public void Analyze(LexemeListManager listManager)
        {
            list = listManager;

            lexemeEnumerator = listManager.GetEnumerator();

            while (lexemeEnumerator.MoveNext())
            {
                if (lexemeEnumerator.Current.LexemeType == LexemeType.Identifier)
                {
                    Lexeme next = null;
                    lexemeEnumerator.PeekNext(out next);

                    if (next.IsKeyWord())
                    {
                        KeyWords(lexemeEnumerator.Current);
                    }

                    if (next.LexemeType == LexemeType.Number)
                    {
                        ParseBinaryExpression();
                    }
                }
            }
        }

        private void ParseBinaryExpression()
        {
            
            if (lexemeEnumerator.MoveNext() && lexemeEnumerator.Current.LexemeType == LexemeType.Number)
            {
                NumberExpression leftNumber = new NumberExpression() {number = lexemeEnumerator.Current};

                Lexeme next = null;
                lexemeEnumerator.PeekNext(out next);

              //  if (next != null && next.LexemeType == LexemeType.WhiteSpace)
            }
        }

        private void KeyWords(Lexeme lex)
        {
            Trace.WriteLine(lex.ToString());
            switch (lex.KeyWordType)
            {
                case Lexer.KeyWords.KeyWordsEnum.Function:
                {
                    ParseFunction();
                    break;
                }


                case Lexer.KeyWords.KeyWordsEnum.If:
                {
                    ParseIfStatement();
                    break;
                }
            }
        }


        private void ParseFunction()
        {
            while (lexemeEnumerator.MoveNext())
            {
                var current = lexemeEnumerator.Current;
                if (current.IsKeyWord() && current.KeyWordType == Lexer.KeyWords.KeyWordsEnum.If)
                {
                    lexemeEnumerator.MoveNext();
                    List<Lexeme> ifExpression = new List<Lexeme>();

                    if (lexemeEnumerator.Current.ToString().Equals("("))
                    {
                        lexemeEnumerator.MoveNext();
                        while (!lexemeEnumerator.Current.ToString().Equals(")"))
                        {
                            ifExpression.Add(lexemeEnumerator.Current);
                            lexemeEnumerator.MoveNext();
                        }
                    }
                }
            }
        }

        private void ParseIfStatement()
        {
            /*
            if (lexemeEnumerator.MoveNextSkipWhite())
            {
                var token = lexemeEnumerator.Current;
                if (!token.IsKeyWord() && token.ToString().Equals(SymbolConstants.LeftParen))
                {
                    ParseExpression();
                }
            }
            */
        }


        private void ParseExpression()
        {
            var token = lexemeEnumerator.MoveNext();

        }
    }
}
