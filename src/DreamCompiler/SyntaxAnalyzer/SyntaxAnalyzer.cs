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
        private TokenListManager list;
        private int currentLocation = 0;
        private TokenEnumerator _tokenEnumerator;

        public void Analyze(TokenListManager listManager)
        {
            list = listManager;

            _tokenEnumerator = listManager.GetEnumerator();

            while (_tokenEnumerator.MoveNext())
            {
                if (_tokenEnumerator.Current.LexemeType == LexemeType.Identifier)
                {
                    Token next = null;
                    _tokenEnumerator.PeekNext(out next);

                    if (next.IsKeyWord())
                    {
                        KeyWords(_tokenEnumerator.Current);
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
            
            if (_tokenEnumerator.MoveNext() && _tokenEnumerator.Current.LexemeType == LexemeType.Number)
            {
                NumberExpression leftNumber = new NumberExpression() {number = _tokenEnumerator.Current};

                Token next = null;
                _tokenEnumerator.PeekNext(out next);

              //  if (next != null && next.LexemeType == LexemeType.WhiteSpace)
            }
        }

        private void KeyWords(Token lex)
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
            while (_tokenEnumerator.MoveNext())
            {
                var current = _tokenEnumerator.Current;
                if (current.IsKeyWord() && current.KeyWordType == Lexer.KeyWords.KeyWordsEnum.If)
                {
                    _tokenEnumerator.MoveNext();
                    List<Token> ifExpression = new List<Token>();

                    if (_tokenEnumerator.Current.ToString().Equals("("))
                    {
                        _tokenEnumerator.MoveNext();
                        while (!_tokenEnumerator.Current.ToString().Equals(")"))
                        {
                            ifExpression.Add(_tokenEnumerator.Current);
                            _tokenEnumerator.MoveNext();
                        }
                    }
                }
            }
        }

        private void ParseIfStatement()
        {
            /*
            if (_tokenEnumerator.MoveNextSkipWhite())
            {
                var token = _tokenEnumerator.Current;
                if (!token.IsKeyWord() && token.ToString().Equals(SymbolConstants.LeftParen))
                {
                    ParseExpression();
                }
            }
            */
        }


        private void ParseExpression()
        {
            var token = _tokenEnumerator.MoveNext();

        }
    }
}
