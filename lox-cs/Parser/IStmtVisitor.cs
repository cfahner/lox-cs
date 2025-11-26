namespace Lox.Parser
{
    public interface IStmtVisitor
    {
        void VisitExpression(Stmt.Expression expression);

        void VisitPrint(Stmt.Print print);
    }
}
