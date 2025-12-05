using System.Text;

namespace Lox.Parser.ExprVisitors
{
    public sealed class AstPrinter : IExprVisitor<string>
    {
        public string Print(Expr expr) => expr.Accept(this);

        public string VisitAssignExpr(Expr.Assign expr)
            => Parenthesize($"{expr.Name.Lexeme}=", expr.Value);

        public string VisitBinaryExpr(Expr.Binary binary)
            => Parenthesize(binary.Operator.Lexeme, binary.Left, binary.Right);

        public string VisitCallExpr(Expr.Call expr)
            => Parenthesize(expr.Parenthesis.Lexeme, [.. expr.Arguments]);

        public string VisitGroupingExpr(Expr.Grouping grouping)
            => Parenthesize("group", grouping.Expression);

        public string VisitLiteralExpr(Expr.Literal literal)
            => literal.Value?.ToString() ?? "nil";

        public string VisitLogicalExpr(Expr.Logical expr)
            => Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);

        public string VisitVariableExpr(Expr.Variable expr)
            => $"(var '{expr.Name}')";

        public string VisitUnaryExpr(Expr.Unary unary)
            => Parenthesize(unary.Operator.Lexeme, unary.Right);

        private string Parenthesize(string name, params Expr[] exprs)
        {
            return new StringBuilder($"({name}")
                .Append(string.Concat(exprs.Select(expr => $" {expr.Accept(this)}")))
                .Append(')')
                .ToString();
        }
    }
}
