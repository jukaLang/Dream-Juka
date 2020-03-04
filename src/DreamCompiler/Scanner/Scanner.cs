﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Diagnostics;
using System.Dynamic;
using Antlr4.Runtime;
using DreamCompiler.Lexer;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using DreamCompiler.Tokens;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static System.Char;

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

                this.fileData = new byte[(int)fileLength];
                fileStream.Read(this.fileData, 0, (int)fileLength);
            }
        }

        public Scanner(MemoryStream memoryStream)
        {
            int memoryStreamLength = (int)memoryStream.Length;
            
            fileData = new byte[memoryStreamLength];
            
            int dataRead = memoryStream.Read(fileData, 0, memoryStreamLength);

            if (dataRead != memoryStreamLength)
            {
                throw new Exception("bad memory read");
            }
        }


        internal IToken ReadToken()
        {
            TokenType tokenType = TokenType.NotValid;

            if (IsEOF())
            {
                tokenType = TokenType.Eof;
                return new Token(tokenType);
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
                    //NumberDigit numberDigit = null;
                    //int value;
                    //if (int.TryParse(t.ToString(), out value))
                    //{
                    //    numberDigit = new NumberDigit() { tokenIntValue = value };
                    //}

                    //return numberDigit;
                }
                else if (IsPunctuation(t) || IsSymbol(t))
                {
                    tokenType = TokenType.Symbol;
                }
                else if (Char.IsWhiteSpace(t))
                {
                    tokenType = TokenType.WhiteSpace;
                }

                return new Token(tokenType, t);
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
            if (Char.IsWhiteSpace((char)fileData[position]))
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

    internal enum LexemeType
    {
        Identifier,
        Number,
        WhiteSpace,
    }

    public class Lexeme : IDisposable
    {
        private List<IToken> tokenList = new List<IToken>();
        private LexemeType _lexemeType;
        private bool isKeyWord;

        internal Lexeme(LexemeType ltype)
        {
            this._lexemeType = ltype;
        }

        internal void AddToken(IToken token)
        {
            tokenList.Add(token);
        }

        public bool IsKeyWord()
        {
            return isKeyWord;
        }

        public override string ToString()
        {
            var s = new StringBuilder();
            foreach (var t in tokenList)
            {
                s.Append(t.GetTokenData());
            }

            return s.ToString();
        }

        void IDisposable.Dispose()
        {
            if (_lexemeType == LexemeType.Identifier)
            {
                if (KeyWords.keyValuePairs.ContainsKey(ToString()))
                {
                    isKeyWord = true;
                }
            }
        }

#if DEBUG
        internal void PrintLexeme(string lexemeType, Action action = null)
        {
            Trace.Write($"Lexeme - Type:{lexemeType} {{ '");

            if (action == null)
            {
                foreach (var token in tokenList)
                {
                    if (token is Token t)
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


    public interface IToken
    {
        TokenType TokenType();

        char GetTokenData();
    }

    internal class Token : IToken
    {
        internal TokenType tokenType;
        internal char data;

        internal Token(TokenType t)
        {
            this.tokenType = t;
        }

        internal Token(TokenType t, char tokenData) : this(t)
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
