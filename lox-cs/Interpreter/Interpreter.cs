using Lox.Interpreter.RuntimeErrors;
using Lox.Parser;
using Lox.Scanner;

namespace Lox.Interpreter
{
    public class Interpreter : IExprVisitor<object?>
    {
        private static bool IsTruthy(object? value) => value switch
        {
            null => false,
            bool boolValue => boolValue,
            _ => true
        };

        private static bool IsEqual(object? left, object? right)
        {
            return left is double lDouble && right is double rDouble && Math.Abs(lDouble - rDouble) < lDouble * .01
                || left == right;
        }

        private static bool IsGreater(Token token, object? left, object? right)
        {
            var (leftDouble, rightDouble) = TypeError.ThrowIfNotNumbers(token, left, right);
            return leftDouble > rightDouble;
        }

        private static bool IsGreaterEqual(Token token, object? left, object? right)
        {
            var (leftDouble, rightDouble) = TypeError.ThrowIfNotNumbers(token, left, right);
            return leftDouble >= rightDouble;
        }

        private static bool IsLess(Token token, object? left, object? right)
        {
            var (leftDouble, rightDouble) = TypeError.ThrowIfNotNumbers(token, left, right);
            return leftDouble < rightDouble;
        }

        private static bool IsLessEqual(Token token, object? left, object? right)
        {
            var (leftDouble, rightDouble) = TypeError.ThrowIfNotNumbers(token, left, right);
            return leftDouble <= rightDouble;
        }

        private static double Subtract(Token token, object? left, object? right)
        {
            var (leftDouble, rightDouble) = TypeError.ThrowIfNotNumbers(token, left, right);
            return leftDouble - rightDouble;
        }

        private static object Add(Token token, object? left, object? right)
        {
            return left is double lDouble && right is double rDouble
                ? lDouble + rDouble
                : left is string || right is string
                ? $"{left}{right}"
                : throw new TypeError(token, "Expected '+' operands to be both numbers or one to be a string.");
        }

        private static double Multiply(Token token, object? left, object? right)
        {
            var (leftDouble, rightDouble) = TypeError.ThrowIfNotNumbers(token, left, right);
            return leftDouble * rightDouble;
        }

        private static double Divide(Token token, object? left, object? right)
        {
            var (leftDouble, rightDouble) = TypeError.ThrowIfNotNumbers(token, left, right);
            return rightDouble == 0.0
                ? throw new DivisionByZeroError(token)
                : leftDouble / rightDouble;
        }

        private static string Stringify(object? value)
        {
            return value switch
            {
                null => "nil",
                double doubleValue => $"{doubleValue}",
                _ => value.ToString()
            } ?? string.Empty;
        }

        public void Interpret(Expr expr)
        {
            var value = Evaluate(expr);
            Console.WriteLine(Stringify(value));
        }

        public object? VisitBinary(Expr.Binary binary)
        {
            var left = Evaluate(binary.Left);
            var right = Evaluate(binary.Right);

            return binary.Operator.Type switch
            {
                TokenType.Greater => IsGreater(binary.Operator, left, right),
                TokenType.GreaterEqual => IsGreaterEqual(binary.Operator, left, right),
                TokenType.Less => IsLess(binary.Operator, left, right),
                TokenType.LessEqual => IsLessEqual(binary.Operator, left, right),
                TokenType.BangEqual => !IsEqual(left, right),
                TokenType.EqualEqual => IsEqual(left, right),
                TokenType.Minus => Subtract(binary.Operator, left, right),
                TokenType.Plus => Add(binary.Operator, left, right),
                TokenType.Slash => Divide(binary.Operator, left, right),
                TokenType.Asterisk => Multiply(binary.Operator, left, right),
                _ => null
            };
        }

        public object? VisitGrouping(Expr.Grouping grouping)
        {
            return Evaluate(grouping.Expression);
        }

        public object? VisitLiteral(Expr.Literal literal)
        {
            return literal.Value;
        }

        public object? VisitUnary(Expr.Unary unary)
        {
            var right = Evaluate(unary.Right);

            return unary.Operator.Type switch
            {
                TokenType.Minus => right is null ? 0.0 : -(double)right,
                TokenType.Bang => !IsTruthy(right),
                _ => null
            };
        }

        private object? Evaluate(Expr expr) => expr.Accept(this);
    }
}
