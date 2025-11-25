using Lox.Scanner;

namespace Lox.Parser
{
    public class ParseError(Token token, string message) : Exception(message)
    {
        public Token Token { get; } = token;
    }
}
