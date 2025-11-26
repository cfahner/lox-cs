namespace Lox.Parser
{
    public abstract record Stmt
    {
        public record Expression(Expr Expr) : Stmt
        {
            public override void Accept(IStmtVisitor visitor) => visitor.VisitExpression(this);
        }

        public record Print(Expr Expr) : Stmt
        {
            public override void Accept(IStmtVisitor visitor) => visitor.VisitPrint(this);
        }

        public abstract void Accept(IStmtVisitor visitor);
    }
}
