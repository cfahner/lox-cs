using Lox.Interpreter.RuntimeErrors;
using Lox.Parser;
using Lox.Scanner;

namespace Lox.Interpreter
{
    public class Interpreter : IExprVisitor<object?>, IStmtVisitor<object?>
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

        public void Interpret(IEnumerable<Stmt> stmts)
        {
            foreach (var stmt in stmts)
            {
                _ = Execute(stmt);
            }
        }

        public object? VisitBinary(Expr.Binary expr)
        {
            var left = Evaluate(expr.Left);
            var right = Evaluate(expr.Right);

            return expr.Operator.Type switch
            {
                TokenType.Greater => IsGreater(expr.Operator, left, right),
                TokenType.GreaterEqual => IsGreaterEqual(expr.Operator, left, right),
                TokenType.Less => IsLess(expr.Operator, left, right),
                TokenType.LessEqual => IsLessEqual(expr.Operator, left, right),
                TokenType.BangEqual => !IsEqual(left, right),
                TokenType.EqualEqual => IsEqual(left, right),
                TokenType.Minus => Subtract(expr.Operator, left, right),
                TokenType.Plus => Add(expr.Operator, left, right),
                TokenType.Slash => Divide(expr.Operator, left, right),
                TokenType.Asterisk => Multiply(expr.Operator, left, right),
                _ => null
            };
        }

        public object? VisitGrouping(Expr.Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        public object? VisitLiteral(Expr.Literal expr)
        {
            return expr.Value;
        }

        public object? VisitVariable(Expr.Variable expr)
        {
            throw new NotImplementedException();
        }

        public object? VisitUnary(Expr.Unary expr)
        {
            var right = Evaluate(expr.Right);

            return expr.Operator.Type switch
            {
                TokenType.Minus => right is null ? 0.0 : -(double)right,
                TokenType.Bang => !IsTruthy(right),
                _ => null
            };
        }

        public object? VisitExpression(Stmt.Expression stmt)
        {
            _ = Evaluate(stmt.Expr);
            return null;
        }

        public object? VisitPrint(Stmt.Print stmt)
        {
            var value = Evaluate(stmt.Expr);
            Console.WriteLine(Stringify(value));
            return null;
        }

        public object? VisitVar(Stmt.Var stmt)
        {
            throw new NotImplementedException();
        }

        private object? Evaluate(Expr expr) => expr.Accept(this);

        private object? Execute(Stmt stmt) => stmt.Accept(this);
    }
}
