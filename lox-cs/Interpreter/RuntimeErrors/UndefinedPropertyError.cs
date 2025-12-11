using Lox.Scanner;

namespace Lox.Interpreter.RuntimeErrors
{
    public class UndefinedPropertyError(Token property)
        : RuntimeError(property, $"Undefined property '{property.Lexeme}'.")
    {
    }
}
