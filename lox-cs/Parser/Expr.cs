using Lox.Scanner;

namespace Lox.Parser
{
    public abstract record Expr
    {
        public record Binary(Expr Left, Token Operator, Expr Right) : Expr
        {
            public override R Accept<R>(IExprVisitor<R> visitor) => visitor.VisitBinary(this);
        }

        public record Grouping(Expr Expression) : Expr
        {
            public override R Accept<R>(IExprVisitor<R> visitor) => visitor.VisitGrouping(this);
        }

        public record Literal(object? Value) : Expr
        {
            public override R Accept<R>(IExprVisitor<R> visitor) => visitor.VisitLiteral(this);
        }

        public record Unary(Token Operator, Expr Right) : Expr
        {
            public override R Accept<R>(IExprVisitor<R> visitor) => visitor.VisitUnary(this);
        }

        public abstract R Accept<R>(IExprVisitor<R> visitor);
    }
}
