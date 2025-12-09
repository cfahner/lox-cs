using Lox.Scanner;

namespace Lox.Interpreter.RuntimeErrors
{
    public class UnextendableVariableError(Token token)
        : RuntimeError(token, $"Cannot extend variable '{token.Lexeme}'.")
    {
    }
}
