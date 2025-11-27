namespace Lox.Parser
{
    public interface IStmtVisitor<R>
    {
        R VisitExpressionStmt(Stmt.Expression stmt);

        R VisitPrintStmt(Stmt.Print stmt);

        R VisitVarStmt(Stmt.Var stmt);
    }
}
