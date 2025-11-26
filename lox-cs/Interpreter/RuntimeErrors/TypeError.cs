using Lox.Scanner;

namespace Lox.Interpreter.RuntimeErrors
{
    public class TypeError(Token token, string message) : RuntimeError(token, message)
    {
        public static (double, double) ThrowIfNotNumbers(Token token, object? left, object? right)
        {
            if (left is double leftDouble && right is double rightDouble)
            {
                return (leftDouble, rightDouble);
            }
            if (left is not double)
            {
                throw new TypeError(token, "Left operand is not a number.");
            }
            if (right is not double)
            {
                throw new TypeError(token, "Right operand is not a number.");
            }
            throw new TypeError(token, "Neither left nor right operand is a number.");
        }
    }
}
