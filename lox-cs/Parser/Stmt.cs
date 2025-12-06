using Lox.Scanner;

namespace Lox.Parser
{
    public abstract record Stmt
    {
        public record Block(IEnumerable<Stmt> Statements) : Stmt
        {
            public override R Accept<R>(IStmtVisitor<R> visitor) => visitor.VisitBlockStmt(this);
        }

        public record Class(Token Name, IEnumerable<Stmt.Function> Methods) : Stmt
        {
            public override R Accept<R>(IStmtVisitor<R> visitor) => visitor.VisitClassStmt(this);
        }

        public record Expression(Expr Expr) : Stmt
        {
            public override R Accept<R>(IStmtVisitor<R> visitor) => visitor.VisitExpressionStmt(this);
        }

        public record Function(Token Name, Token[] Parameters, IEnumerable<Stmt> Body) : Stmt
        {
            public override R Accept<R>(IStmtVisitor<R> visitor) => visitor.VisitFunctionStmt(this);
        }

        public record If(Expr Condition, Stmt Then, Stmt? Else) : Stmt
        {
            public override R Accept<R>(IStmtVisitor<R> visitor) => visitor.VisitIfStmt(this);
        }

        public record Print(Expr Expr) : Stmt
        {
            public override R Accept<R>(IStmtVisitor<R> visitor) => visitor.VisitPrintStmt(this);
        }

        public record Return(Token Keyword, Expr? Value) : Stmt
        {
            public override R Accept<R>(IStmtVisitor<R> visitor) => visitor.VisitReturnStmt(this);
        }

        public record While(Expr Condition, Stmt Body) : Stmt
        {
            public override R Accept<R>(IStmtVisitor<R> visitor) => visitor.VisitWhileStmt(this);
        }

        public record Var(Token Name, Expr? Initializer) : Stmt
        {
            public override R Accept<R>(IStmtVisitor<R> visitor) => visitor.VisitVarStmt(this);
        }

        public abstract R Accept<R>(IStmtVisitor<R> visitor);
    }
}
