using Hygge.Entry;
using Hygge.Tokens;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Hygge.Scanners
{
    public class Scanner
    {
        readonly string _source;
        readonly List<Token> _tokens = new ();

        private int _start = 0;
        private int _current = 0;
        private int _line = 1;

        private static readonly Dictionary<string, TokenType> _keyWords = new()
        {
                { "og", TokenType.OG },
                { "klasse", TokenType.KLASSE },
                { "ellers", TokenType.ELLERS },
                { "falsk", TokenType.FALSK },
                { "for", TokenType.FOR },
                { "funktion", TokenType.FUNKTION },
                { "hvis", TokenType.HVIS },
                { "ingenting", TokenType.INGENTING },
                { "eller", TokenType.ELLER },
                { "skriv", TokenType.SKRIV },
                { "returner", TokenType.RETURNER },
                { "superduper", TokenType.SUPERDUPER },
                { "dette", TokenType.DETTE },
                { "sandt", TokenType.SANDT },
                { "sæt", TokenType.SÆT },
                { "imens", TokenType.IMENS }
        };

        public Scanner(string source)
        {
            _source = source;
        }

        public List<Token> ScanTokens()
        {
            
            while (!IsAtEnd())
            {
                _start = _current;
                ScanToken();
            }

            _tokens.Add(new Token(TokenType.EOF, "", null, _line));
            return _tokens;
        }

        private void ScanToken()
        {
            char c = Advance();

            switch (c)
            {
                // Single character
                case '(': AddToken(TokenType.LEFT_PAREN); break;
                case ')': AddToken(TokenType.RIGHT_PAREN); break;
                case '{': AddToken(TokenType.LEFT_BRACE); break;
                case '}': AddToken(TokenType.RIGHT_BRACE); break;
                case ',': AddToken(TokenType.COMMA); break;
                case '.': AddToken(TokenType.DOT); break;
                case '-': AddToken(TokenType.MINUS); break;
                case '+': AddToken(TokenType.PLUS); break;
                case ';': AddToken(TokenType.SEMICOLON); break;
                case '*': AddToken(TokenType.STAR); break;
                // Two character
                case '!':
                    AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG);
                    break;
                case '=':
                    AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL);
                    break;
                case '<':
                    AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS);
                    break;
                case '>':
                    AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER);
                    break;
                case '/':
                    if (Match('/'))
                    {
                        // A comment goes until the end of the line
                        while (Peek() != '\n' && !IsAtEnd()) Advance();
                    }
                    else
                    {
                        AddToken(TokenType.SLASH);
                    }
                    break;
                case ' ':
                case '\r':
                case '\t':
                    // Ignore whitespace
                    break;
                case '\n':
                    _line++;
                    break;
                    // string literals
                case '"': String(); break;
                default:
                    if (IsDigit(c))
                    {
                        Number();
                    }
                    else if (IsAlpha(c))
                    {
                        Identifier();
                    }
                    else
                        Start.Error(_line, "Uvevtet tegn");
                    break;
            }
        }

        private void Identifier()
        {
            while (IsAlphaNumeric(Peek()))
                Advance();

            string text = _source.Substring(_start, _current - _start);

            // Try to get the type from the keywords dictionary
            if (!_keyWords.TryGetValue(text, out TokenType type))
                // If the keyword doesn't exist in the dictionary, it's an identifier
                type = TokenType.IDENTIFIER;

            AddToken(type);
        }

        private bool IsDigit(char c)
        {
            return c >= '0' && c <= '9';
        }

        private void Number()
        {
            while (IsDigit(Peek())) Advance();

            // Look for a fractional part.
            if (Peek() == '.' && IsDigit(PeekNext()))
            {
                // Consume the "."
                Advance();

                while (IsDigit(Peek())) Advance();
            }

            AddToken(TokenType.NUMBER,Double.Parse(_source.Substring(_start, _current)));
        }

        private void String()
        {
            while(Peek() != '"' && !IsAtEnd())
            {
                if (Peek() == '\n') _line++;
                Advance();
            }

            if (IsAtEnd())
            {
                Start.Error(_line, "Hov! Den sætning blev ikke afsluttet. Mangler et \"");
            }

            // The closing "
            Advance();

            // Trim the surrounding quotes.
            string value = _source.Substring(_start + 1, _current - 2);
           AddToken(TokenType.STRING, value);
        }

        private char Peek()
        {
            if (IsAtEnd()) return '\0';
            return _source[_current];
        }

        private char PeekNext()
        {
            if (_current + 1 >= _source.Length) return '\0';
            return _source[_current + 1];
        }

        private bool IsAlpha(char c)
        {
            return (c >= 'a' && c <= 'z') ||
                   (c >= 'A' && c <= 'Z') ||
                    c == '_' ||
                    // Danish letters
                    c == 'æ' || c == 'Æ' ||
                    c == 'ø' || c == 'Ø' ||
                    c == 'å' || c == 'Å';
        }

        private bool IsAlphaNumeric(char c)
        {
            return IsAlpha(c) || IsDigit(c);
        }

        private bool Match(char expected)
        {
            if (IsAtEnd()) return false;
            if (_source[_current] != expected) return false;

            _current++;
            return true;
        }

        private bool IsAtEnd()
        {
            return _current >= _source.Length;
        }

        private char Advance()
        {
            return _source[_current++];
        }

        private void AddToken(TokenType type)
        {
            AddToken(type, null);
        }

        private void AddToken(TokenType type, object? literal)
        {
            string text = _source.Substring(_start, _current - _start);
            _tokens.Add(new Token(type, text, literal, _line));
        }
    }
}
