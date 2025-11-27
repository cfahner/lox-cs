using Lox.Interpreter.Operations;
using Lox.Parser;
using Lox.Scanner;

namespace Lox.Interpreter
{
    public class Interpreter : IExprVisitor<object?>, IStmtVisitor<object?>
    {
        private readonly Environment _environment = new();

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
            return _environment.Get(expr.Name);
        }

        public object? VisitUnary(Expr.Unary expr)
        {
            var right = Evaluate(expr.Right);

            return expr.Operator.Type switch
            {
                TokenType.Minus => right is null ? 0.0 : -(double)right,
                TokenType.Bang => !Conversions.ToTruthy(right),
                _ => null
            };
        }

        public object? VisitExpressionStmt(Stmt.Expression stmt)
        {
            _ = Evaluate(stmt.Expr);
            return null;
        }

        public object? VisitPrintStmt(Stmt.Print stmt)
        {
            var value = Evaluate(stmt.Expr);
            Console.WriteLine(Conversions.Stringify(value));
            return null;
        }

        public object? VisitVarStmt(Stmt.Var stmt)
        {
            object? value = null;
            if (stmt.Initializer is not null)
            {
                value = Evaluate(stmt.Initializer);
            }

            _environment.Define(stmt.Name.Lexeme, value);
            return null;
        }

        private object? Evaluate(Expr expr) => expr.Accept(this);

        private object? Execute(Stmt stmt) => stmt.Accept(this);
    }
}
