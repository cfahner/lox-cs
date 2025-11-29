using Lox.Scanner;

namespace Lox.Parser
{
    public abstract record Expr
    {
        public record Assign(Token Name, Expr Value) : Expr
        {
            public override R Accept<R>(IExprVisitor<R> visitor) => visitor.VisitAssign(this);
        }

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

        public record Logical(Expr Left, Token Operator, Expr Right) : Expr
        {
            public override R Accept<R>(IExprVisitor<R> visitor) => visitor.VisitLogical(this);
        }

        public record Unary(Token Operator, Expr Right) : Expr
        {
            public override R Accept<R>(IExprVisitor<R> visitor) => visitor.VisitUnary(this);
        }

        public record Variable(Token Name) : Expr
        {
            public override R Accept<R>(IExprVisitor<R> visitor) => visitor.VisitVariable(this);
        }

        public abstract R Accept<R>(IExprVisitor<R> visitor);
    }
}
