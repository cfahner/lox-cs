namespace Lox.Scanner
{
    public readonly record struct Token(TokenType Type, string Lexeme, object? Literal, int Line)
    {
        public override string ToString() => $"{Type} {Lexeme} {Literal}";
    }
}
