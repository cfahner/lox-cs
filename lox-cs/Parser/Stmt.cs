using Lox.Scanner;

namespace Lox.Parser
{
    public abstract record Stmt
    {
        public record Expression(Expr Expr) : Stmt
        {
            public override R Accept<R>(IStmtVisitor<R> visitor) => visitor.VisitExpression(this);
        }

        public record Print(Expr Expr) : Stmt
        {
            public override R Accept<R>(IStmtVisitor<R> visitor) => visitor.VisitPrint(this);
        }

        public record Var(Token Name, Expr? Initializer) : Stmt
        {
            public override R Accept<R>(IStmtVisitor<R> visitor) => visitor.VisitVar(this);
        }

        public abstract R Accept<R>(IStmtVisitor<R> visitor);
    }
}
