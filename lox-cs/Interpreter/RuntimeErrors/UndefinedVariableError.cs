using Lox.Scanner;

namespace Lox.Interpreter.RuntimeErrors
{
    public class UndefinedVariableError(Token token)
        : RuntimeError(token, $"Undefined variable '{token.Lexeme}'.")
    {
    }
}
