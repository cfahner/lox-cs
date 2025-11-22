namespace Lox.Scanner
{
    public class Scanner(string source)
    {
        private static readonly Dictionary<string, TokenType> _keywords = new()
        {
            { "and", TokenType.And },
            { "class", TokenType.Class },
            { "else", TokenType.Else },
            { "false", TokenType.False },
            { "for", TokenType.For },
            { "fun", TokenType.Fun },
            { "if", TokenType.If },
            { "nil", TokenType.Nil },
            { "or", TokenType.Or },
            { "print", TokenType.Print },
            { "return", TokenType.Return },
            { "super", TokenType.Super },
            { "this", TokenType.This },
            { "true", TokenType.True },
            { "var", TokenType.Var },
            { "while", TokenType.While },
        };

        private bool IsAtEnd => _current >= source.Length;

        private int _start = 0;
        private int _current = 0;
        private int _line = 1;

        public IEnumerable<Token> Scan()
        {
            while (!IsAtEnd)
            {
                yield return ScanToken();
            }
            yield return new Token(TokenType.Eof, "", null, _line);
        }

        private Token ScanToken()
        {
            _start = _current;
            var @char = Consume();
            switch (@char)
            {
                case '(': return CreateToken(TokenType.LeftParenthesis);
                case ')': return CreateToken(TokenType.RightParenthesis);
                case '{': return CreateToken(TokenType.LeftBrace);
                case '}': return CreateToken(TokenType.RightBrace);
                case ',': return CreateToken(TokenType.Comma);
                case '.': return CreateToken(TokenType.Dot);
                case '-': return CreateToken(TokenType.Minus);
                case '+': return CreateToken(TokenType.Plus);
                case ';': return CreateToken(TokenType.Semicolon);
                case '*': return CreateToken(TokenType.Asterisk);
                case '!': return CreateToken(Match('=') ? TokenType.BangEqual : TokenType.Bang);
                case '=': return CreateToken(Match('=') ? TokenType.EqualEqual : TokenType.Equal);
                case '<': return CreateToken(Match('=') ? TokenType.LessEqual : TokenType.Less);
                case '>': return CreateToken(Match('=') ? TokenType.GreaterEqual : TokenType.Greater);
                case '/':
                    if (Match('/'))
                    {
                        AdvanceWhile(c => c != '\n');
                        return CreateToken(TokenType.Comment);
                    }
                    return CreateToken(TokenType.Slash);
                case ' ':
                case '\t':
                case '\r':
                    AdvanceWhile(c => c is ' ' or '\t' or '\r');
                    return CreateToken(TokenType.Whitespace);
                case '\n':
                    _line += 1;
                    return CreateToken(TokenType.Whitespace);
                case '"': return ScanStringToken();
                default:
                    if (char.IsDigit(@char))
                    {
                        return ScanNumberToken();
                    }
                    else if (char.IsAsciiLetter(@char) || @char == '_')
                    {
                        return ScanIdentifierToken();
                    }
                    throw new ScannerException(_line, "Unexpected character.");
            }
        }

        private Token ScanIdentifierToken()
        {
            AdvanceWhile(c => char.IsAsciiLetterOrDigit(c) || c == '_');
            return _keywords.TryGetValue(source[_start.._current], out var tokenType)
                ? CreateToken(tokenType)
                : CreateToken(TokenType.Identifier);
        }

        private Token ScanNumberToken()
        {
            AdvanceWhile(char.IsDigit);
            // check PeekNext() to disallow numbers ending in a period
            if (Peek() == '.' && char.IsDigit(PeekNext()))
            {
                Advance(); // consume the period
                AdvanceWhile(char.IsDigit);
            }
            return CreateToken(TokenType.Number, double.Parse(source[_start.._current]));
        }

        private Token ScanStringToken()
        {
            AdvanceWhile(c => c != '"');
            if (IsAtEnd)
            {
                throw new ScannerException(_line, "Unterminated string.");
            }
            Advance(); // closing DQUOTE
            return CreateToken(TokenType.String, source.Substring(_start + 1, _current - _start - 2));
        }

        private Token CreateToken(TokenType type, object? literal = null)
        {
            return new Token(type, source[_start.._current], literal, _line);
        }

        private bool Match(char compare)
        {
            if (IsAtEnd)
            {
                return false;
            }
            if (source[_current] != compare)
            {
                return false;
            }
            _current += 1;
            return true;
        }

        private char Consume()
        {
            return source[_current++];
        }

        private void AdvanceWhile(Predicate<char> predicate)
        {
            var @char = Peek();
            while (predicate(@char) && !IsAtEnd)
            {
                if (@char == '\n')
            {
                    _line += 1;
                }
                Advance();
                @char = Peek();
            }
        }

        private void Advance()
        {
            _current += 1;
        }

        private char Peek()
        {
            return IsAtEnd
                ? '\0'
                : source[_current];
        }

        private char PeekNext()
        {
            return _current + 1 >= source.Length
                ? '\0'
                : source[_current + 1];
        }
    }
}
