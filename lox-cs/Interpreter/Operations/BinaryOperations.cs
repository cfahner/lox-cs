using Lox.Interpreter.RuntimeErrors;
using Lox.Scanner;

namespace Lox.Interpreter.Operations
{
    internal static class BinaryOperations
    {
        public static bool IsEqual(object? left, object? right)
        {
            return left is double lDouble && right is double rDouble && Math.Abs(lDouble - rDouble) < lDouble * .01
                || left == right;
        }

        public static bool IsGreater(Token token, object? left, object? right)
        {
            var (leftDouble, rightDouble) = TypeError.ThrowIfNotNumbers(token, left, right);
            return leftDouble > rightDouble;
        }

        public static bool IsGreaterEqual(Token token, object? left, object? right)
        {
            var (leftDouble, rightDouble) = TypeError.ThrowIfNotNumbers(token, left, right);
            return leftDouble >= rightDouble;
        }

        public static bool IsLess(Token token, object? left, object? right)
        {
            var (leftDouble, rightDouble) = TypeError.ThrowIfNotNumbers(token, left, right);
            return leftDouble < rightDouble;
        }

        public static bool IsLessEqual(Token token, object? left, object? right)
        {
            var (leftDouble, rightDouble) = TypeError.ThrowIfNotNumbers(token, left, right);
            return leftDouble <= rightDouble;
        }

        public static double Subtract(Token token, object? left, object? right)
        {
            var (leftDouble, rightDouble) = TypeError.ThrowIfNotNumbers(token, left, right);
            return leftDouble - rightDouble;
        }

        public static object Add(Token token, object? left, object? right)
        {
            return left is double lDouble && right is double rDouble
                ? lDouble + rDouble
                : left is string || right is string
                ? $"{left}{right}"
                : throw new TypeError(token, "Expected '+' operands to be both numbers or one to be a string.");
        }

        public static double Multiply(Token token, object? left, object? right)
        {
            var (leftDouble, rightDouble) = TypeError.ThrowIfNotNumbers(token, left, right);
            return leftDouble * rightDouble;
        }

        public static double Divide(Token token, object? left, object? right)
        {
            var (leftDouble, rightDouble) = TypeError.ThrowIfNotNumbers(token, left, right);
            return rightDouble == 0.0
                ? throw new DivisionByZeroError(token)
                : leftDouble / rightDouble;
        }
    }
}
