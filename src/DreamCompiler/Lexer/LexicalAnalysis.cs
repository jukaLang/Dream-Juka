using DreamCompiler.Scanner;
using DreamCompiler.Tokens;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;
using Token = DreamCompiler.Scanner.Token;
// ReSharper disable All


namespace DreamCompiler.Lexer
{
    public class LexicalAnalysis
    {
        private Scanner.Scanner scanner;
        public Dictionary<KeyWords.KeyWordsEnum, Action<ILexeme>> keywordActions = new Dictionary<KeyWords.KeyWordsEnum, Action<ILexeme>>();
        
        
        public LexicalAnalysis(Scanner.Scanner scanner)
        {
            this.scanner = scanner;

            keywordActions = new Dictionary<KeyWords.KeyWordsEnum, Action<ILexeme>>()
            {
                {KeyWords.KeyWordsEnum.Main, MainAction},
                {KeyWords.KeyWordsEnum.Function, MainAction },
            };
        }

        public TokenListManager Analyze()
        {
            List<Token> lexemeList = new List<Token>();
            while (true)
            {
                ILexeme lexeme = scanner.ReadToken();
                if (lexeme.TokenType() == TokenType.Eof)
                {
                    break;
                }

                switch (lexeme.TokenType())
                {
                    case TokenType.Character:
                    {
                        lexemeList.Add(GetIdentifier(lexeme));
                        break;
                    }

                    case TokenType.NumberDigit:
                    {
                        lexemeList.Add(GetNumber(lexeme));
                        break;
                    }

                    case TokenType.Symbol:
                    {
                        lexemeList.Add(GetSymbol(lexeme));
                        break;
                    }

                    case TokenType.WhiteSpace:
                    {
                        lexemeList.Add(GetWhiteSpace(lexeme));
                        break;
                    }
                }
            }

            return new TokenListManager(lexemeList);
        }

        //public void 


        private Token GetIdentifier(ILexeme lexeme)
        {
            if (lexeme.TokenType() == TokenType.WhiteSpace)
            {
                while (lexeme.TokenType() == TokenType.WhiteSpace)
                {
                    lexeme = scanner.ReadToken();
                }
            }

            using (Token identifier = new Token(LexemeType.Identifier))
            {
                identifier.AddLexeme(lexeme);

                while (true)
                {
                    var next = this.scanner.ReadToken();
                    if (next.TokenType() == TokenType.Symbol || next.TokenType() == TokenType.WhiteSpace)
                    {
                        this.scanner.PutTokenBack();
                        break;
                    }

                    identifier.AddLexeme(next);
                }

                identifier.PrintLexeme("Identifier");

                return identifier;
            }

            throw new Exception();
        }


        private Token GetNumber(ILexeme lexeme)
        {
            Token number = new Token(LexemeType.Number);
            number.AddLexeme(lexeme);

            while (true)
            {
                var next = scanner.ReadToken();

                if (next.TokenType() == TokenType.Eof) 
                {
                    break;
                }

                if (next.TokenType() == TokenType.NumberDigit)
                {
                    number.AddLexeme(next);
                }
                else
                {
                    this.scanner.PutTokenBack();
                    break;
                }
            }

            number.PrintLexeme("Number");
            return number;
        }

        private ILexeme GetPunctuation(ILexeme lexeme)
        {
            while (lexeme.TokenType() == TokenType.WhiteSpace)
            {
                lexeme = scanner.ReadToken();
            }

            if (lexeme.TokenType() == TokenType.Symbol)
            {
                return lexeme;
            }

            throw new Exception("No Punctuation found;");
        }

        private Token GetSymbol(ILexeme lexeme)
        {
            Token symbol = new Token(LexemeType.Operator);

            var currentSymbol = lexeme.GetTokenData();
            if (currentSymbol == '(' ||
                currentSymbol == ')' ||
                currentSymbol == '"' ||
                currentSymbol == '{' ||
                currentSymbol == '}' ||
                currentSymbol == ';'
                )
            {
                symbol.AddLexeme(lexeme);
                symbol.PrintLexeme("Symbol");
                return symbol;
            }

            if (currentSymbol == '+' || currentSymbol == '/' || currentSymbol == '-' || currentSymbol == '*')
            {
                symbol.AddLexeme(lexeme);
                symbol.PrintLexeme("Symbol");
                return symbol;
            }

            if (currentSymbol == '=')
            {
                symbol.AddLexeme(lexeme);
                while (true)
                {
                    var next = this.scanner.ReadToken();
                    if (next.GetTokenData() == '=')
                    {
                        symbol.AddLexeme(next);
                    }
                    else
                    {
                        this.scanner.PutTokenBack();
                        break;
                    }
                }
            }
            else if (currentSymbol == '<')
            {
                symbol.AddLexeme(lexeme);
                while (true)
                {
                    var next = this.scanner.ReadToken();
                    if (next.GetTokenData() == '=')
                    {
                        symbol.AddLexeme(next);
                    }
                    else
                    {
                        this.scanner.PutTokenBack();
                        break;
                    }
                }
            }

            symbol.PrintLexeme("Symbol");
            return symbol;
        }

        private Token GetWhiteSpace(ILexeme lexeme)
        {
            Token whiteSpace = new Token(LexemeType.WhiteSpace);

            if (lexeme.GetTokenData().Equals('\n') ||
                lexeme.GetTokenData().Equals('\r') ||
                lexeme.GetTokenData().Equals('\t') ||
                lexeme.GetTokenData().Equals(' '))
            {
                whiteSpace.AddLexeme(lexeme);
                whiteSpace.PrintLexeme("WhiteSpace", ()=>
                {
                    if (lexeme.GetTokenData().Equals('\n'))
                    {
                        Trace.Write("\\n");
                    }
                    else if (lexeme.GetTokenData().Equals('\r'))
                    {
                        Trace.Write("\\r");
                    }
                    else if (lexeme.GetTokenData().Equals('\t'))
                    {
                        Trace.Write("\\t");
                    }
                    else if (lexeme.GetTokenData().Equals(' '))
                    {
                        Trace.Write("_");
                    }
                });
            }

            return whiteSpace;
        }

        private void VisitIdentifier(ILexeme lexeme)
        {
            Token tokenIdentifier = GetIdentifier(lexeme);

            /*
            if (KeyWords.keyValuePairs.TryGetValue(tokenIdentifier.ToString(), out KeyWords.KeyWordType keyWordsEnum))
            {
                if (keywordActions.ContainsKey(keyWordsEnum))
                {
                    keywordActions[keyWordsEnum](tokenIdentifier);
                }
            }
            */
        }

        private void MainAction(ILexeme lexeme)
        {
            if (lexeme.TokenType() != TokenType.Character)
            {
                throw new Exception("no function name");
            }

            if (KeyWords.keyWordNames.Contains(lexeme.ToString()))
            {
                if (lexeme.ToString().Equals(KeyWords.FUNCTION))
                {

                    Token functionName = GetIdentifier(scanner.ReadToken());
                    ILexeme leftParen = GetPunctuation(scanner.ReadToken());

                    if (!leftParen.ToString().Equals(KeyWords.LPAREN))
                    {
                        throw new Exception("no left paren");
                    }

                    // TODO handle function parameters;

                    ILexeme rightParen = GetPunctuation(scanner.ReadToken());

                    if (!rightParen.ToString().Equals(KeyWords.RPAREN))
                    {
                        throw new Exception();
                    }

                    Token isEqualSign = GetSymbol(scanner.ReadToken());
                    ILexeme isLeftBracket = GetPunctuation(scanner.ReadToken());
                }
            }
        }
    };
}


