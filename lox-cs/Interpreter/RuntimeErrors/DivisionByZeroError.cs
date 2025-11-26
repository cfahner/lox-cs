using Lox.Scanner;

namespace Lox.Interpreter.RuntimeErrors
{
    public class DivisionByZeroError(Token token) : RuntimeError(token, ErrorMessage)
    {
        private const string ErrorMessage = "Division by zero.";
    }
}
