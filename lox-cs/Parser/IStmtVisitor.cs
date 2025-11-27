namespace Lox.Parser
{
    public interface IStmtVisitor<R>
    {
        R VisitExpression(Stmt.Expression stmt);

        R VisitPrint(Stmt.Print stmt);
    }
}
