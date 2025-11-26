using Lox.Scanner;

namespace Lox.Interpreter
{
    public class RuntimeError(Token token, string message) : Exception(message)
    {
        public Token Token { get; private init; } = token;
    }
}
