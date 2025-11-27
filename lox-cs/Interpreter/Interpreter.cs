using Lox.Interpreter.Operations;
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
                TokenType.Greater => BinaryOperations.IsGreater(expr.Operator, left, right),
                TokenType.GreaterEqual => BinaryOperations.IsGreaterEqual(expr.Operator, left, right),
                TokenType.Less => BinaryOperations.IsLess(expr.Operator, left, right),
                TokenType.LessEqual => BinaryOperations.IsLessEqual(expr.Operator, left, right),
                TokenType.BangEqual => !BinaryOperations.IsEqual(left, right),
                TokenType.EqualEqual => BinaryOperations.IsEqual(left, right),
                TokenType.Minus => BinaryOperations.Subtract(expr.Operator, left, right),
                TokenType.Plus => BinaryOperations.Add(expr.Operator, left, right),
                TokenType.Slash => BinaryOperations.Divide(expr.Operator, left, right),
                TokenType.Asterisk => BinaryOperations.Multiply(expr.Operator, left, right),
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
