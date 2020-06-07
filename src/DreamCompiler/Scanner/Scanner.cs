using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using DreamCompiler.Lexer;
using static System.Char;
using System.Runtime.Remoting.Messaging;

namespace DreamCompiler.Scanner
{
    public class Scanner
    {
        private int position;
        private byte[] fileData;

        public Scanner(string path)
        {
            using (FileStream fileStream = File.Open(path, FileMode.Open))
            {
                double fileLength = fileStream.Length;

                this.fileData = new byte[(int) fileLength];
                fileStream.Read(this.fileData, 0, (int) fileLength);
            }
        }

        public Scanner(MemoryStream memoryStream)
        {
            int memoryStreamLength = (int) memoryStream.Length;

            fileData = new byte[memoryStreamLength];

            int dataRead = memoryStream.Read(fileData, 0, memoryStreamLength);

            if (dataRead != memoryStreamLength)
            {
                throw new Exception("bad memory read");
            }
        }


        internal ILexeme ReadToken()
        {
            TokenType tokenType = TokenType.NotValid;

            if (IsEOF())
            {
                tokenType = TokenType.Eof;
                return new Lexeme(tokenType);
            }
            else
            {
                char t = (char) fileData[position++];

                if (IsLetter(t))
                {
                    tokenType = TokenType.Character;
                }
                else if (IsDigit(t) || IsNumber(t))
                {
                    tokenType = TokenType.NumberDigit;
                }
                else if (IsPunctuation(t) || IsSymbol(t))
                {
                    tokenType = TokenType.Symbol;
                }
                else if (Char.IsWhiteSpace(t))
                {
                    tokenType = TokenType.WhiteSpace;
                }

                return new Lexeme(tokenType, t);
            }
        }

        internal void PutTokenBack()
        {
            this.position--;
        }

        private bool IsEOF()
        {
            if (position >= fileData.Length)
            {
                return true;
            }

            return false;
        }

        internal bool IsWhiteSpace()
        {
            if (Char.IsWhiteSpace((char) fileData[position]))
            {
                return true;
            }

            return false;
        }

    }


    public enum TokenType
    {
        NotValid,
        Character,
        NumberDigit,
        WhiteSpace,
        Eof,
        Symbol, 
    }

    public enum LexemeType
    {
        Identifier,
        Number,
        WhiteSpace,
        Operator,
        Eof,
    }

    public class Token : IDisposable
    {
        private List<ILexeme> tokenList = new List<ILexeme>();
        private LexemeType lexemeType;
        private bool isKeyWord;
        private string tokenAsString = String.Empty;
        // ReSharper disable once InconsistentNaming
        private KeyWords.KeyWordsEnum keyWordType;

        internal Token(LexemeType ltype)
        {
            this.lexemeType = ltype;
        }

        internal void AddLexeme(ILexeme lexeme)
        {
            tokenList.Add(lexeme);
        }

        public bool IsKeyWord()
        {
            return isKeyWord;
        }

        public override string ToString()
        {
            if (string.IsNullOrEmpty(tokenAsString))
            {
                var s = new StringBuilder();
                foreach (var t in tokenList)
                {
                    s.Append(t.GetTokenData());
                }

                return s.ToString();
            }

            return tokenAsString;
        }

        public LexemeType LexemeType => lexemeType;

        internal KeyWords.KeyWordsEnum KeyWordType => keyWordType;

        void IDisposable.Dispose()
        {
            this.tokenAsString = ToString();

            if (lexemeType == LexemeType.Identifier)
            {
                if (KeyWords.keyValuePairs.TryGetValue(this.tokenAsString, out KeyWords.KeyWordsEnum keyWordValue))
                {
                    isKeyWord = true;
                    this.keyWordType = keyWordValue;
                }
            }
        }

#if DEBUG
        internal void PrintLexeme(string lexemeType, Action action = null)
        {
            Trace.Write($"Token - Type:{lexemeType} {{ '");

            if (action == null)
            {
                foreach (var token in tokenList)
                {
                    if (token is Lexeme t)
                    {
                        Trace.Write(t.data);
                    }
                }
            }
            else
            {
                action();
            }

            Trace.Write("' }");

            Trace.WriteLine(string.Empty);

        }
#endif
    }


    public class TokenListManager : IEnumerable
    {
        private List<Token> lexemList;

        public TokenListManager(List<Token> list)
        {
            this.lexemList = list;
        }

        /*
        public Token NextNotWhiteSpace(out Token lexeme)
        {
            while (lexemList[currentLexem].LexemeType == LexemeType.WhiteSpace)
            {
                currentLexem++;
            }

            lexeme = lexemList[currentLexem];
            return lexeme;
        }
        */

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator) GetEnumerator();
        }

        public TokenEnumerator GetEnumerator()
        {
            return new TokenEnumerator(this.lexemList);
        }

    }


    public class TokenEnumerator : IEnumerator
    {
        private List<Token> lexemeList;
        private int position = -1;

        public TokenEnumerator(List<Token> list)
        {
            lexemeList = list;
        }

        public bool MoveNext()
        {
            position++;
            if (position < lexemeList.Count)
            {
                return true;
            }

            return false;
        }

        /*
        public bool MoveNextEx()
        {
            position++;
            if (lexemeList[position].LexemeType == LexemeType.WhiteSpace)
            {
                return MoveNextEx();
            }

            if (position < lexemeList.Count)
            {
                return true;
            }

            return false;
        }
        */

        public bool PeekNext(out Token token)
        {
            token = null;

            if (position + 1 < lexemeList.Count)
            {
                token = lexemeList[position];
                return true;
            }

            return false;
        }
        /*
        public bool MoveBackEx()
        {
            position--;
            if (lexemeList[position].LexemeType == LexemeType.WhiteSpace)
            {
                return MoveNextEx();
            }

            if (position < 0)
            {
                return false;
            }

            if (position >= 0)
            {
                return true;
            }

            throw new InvalidOperationException();
        }
        */

        public void Reset()
        {
            position = -1;
        }

        object IEnumerator.Current => Current;

        public Token Current
        {
            get
            {
                try
                {
                    return lexemeList[position];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }

    public interface ILexeme
    {
        TokenType TokenType();

        char GetTokenData();
    }

    internal class Lexeme : ILexeme
    {
        internal TokenType tokenType;
        internal char data;

        internal Lexeme(TokenType t)
        {
            this.tokenType = t;
        }

        internal Lexeme(TokenType t, char tokenData) : this(t)
        {
            this.data = tokenData;
        }

        public char GetTokenData()
        {
            return this.data;
        }

        public TokenType TokenType()
        {
            return this.tokenType;
        }
    }

}
