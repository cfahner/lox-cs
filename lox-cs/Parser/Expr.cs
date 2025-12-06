using Lox.Scanner;

namespace Lox.Parser
{
    public abstract record Expr
    {
        public record Assign(Token Name, Expr Value) : Expr
        {
            public override R Accept<R>(IExprVisitor<R> visitor) => visitor.VisitAssignExpr(this);
        }

        public record Binary(Expr Left, Token Operator, Expr Right) : Expr
        {
            public override R Accept<R>(IExprVisitor<R> visitor) => visitor.VisitBinaryExpr(this);
        }

        public record Call(Expr Callee, Token Parenthesis, IEnumerable<Expr> Arguments) : Expr
        {
            public override R Accept<R>(IExprVisitor<R> visitor) => visitor.VisitCallExpr(this);
        }

        public record Get(Expr Object, Token Name) : Expr
        {
            public override R Accept<R>(IExprVisitor<R> visitor) => visitor.VisitGetExpr(this);
        }

        public record Grouping(Expr Expression) : Expr
        {
            public override R Accept<R>(IExprVisitor<R> visitor) => visitor.VisitGroupingExpr(this);
        }

        public record Literal(object? Value) : Expr
        {
            public override R Accept<R>(IExprVisitor<R> visitor) => visitor.VisitLiteralExpr(this);
        }

        public record Logical(Expr Left, Token Operator, Expr Right) : Expr
        {
            public override R Accept<R>(IExprVisitor<R> visitor) => visitor.VisitLogicalExpr(this);
        }

        public record Set(Expr Object, Token Name, Expr Value) : Expr
        {
            public override R Accept<R>(IExprVisitor<R> visitor) => visitor.VisitSetExpr(this);
        }

        public record Unary(Token Operator, Expr Right) : Expr
        {
            public override R Accept<R>(IExprVisitor<R> visitor) => visitor.VisitUnaryExpr(this);
        }

        public record Variable(Token Name) : Expr
        {
            public override R Accept<R>(IExprVisitor<R> visitor) => visitor.VisitVariableExpr(this);
        }

        public abstract R Accept<R>(IExprVisitor<R> visitor);
    }
}
