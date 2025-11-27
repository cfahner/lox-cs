namespace Lox.Parser
{
    public interface IStmtVisitor<R>
    {
        R VisitExpression(Stmt.Expression expression);

        R VisitPrint(Stmt.Print print);
    }
}
