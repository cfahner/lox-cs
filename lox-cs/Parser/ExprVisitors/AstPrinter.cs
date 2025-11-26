using System.Text;

namespace Lox.Parser.ExprVisitors
{
    public sealed class AstPrinter : IExprVisitor<string>
    {
        public string Print(Expr expr) => expr.Accept(this);

        public string VisitBinary(Expr.Binary binary)
            => Parenthesize(binary.Operator.Lexeme, binary.Left, binary.Right);

        public string VisitGrouping(Expr.Grouping grouping)
            => Parenthesize("group", grouping.Expression);

        public string VisitLiteral(Expr.Literal literal)
            => literal.Value?.ToString() ?? "nil";

        public string VisitUnary(Expr.Unary unary)
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
