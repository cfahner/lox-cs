namespace Lox.Parser
{
    public interface IStmtVisitor<R>
    {
        R VisitBlockStmt(Stmt.Block stmt);

        R VisitExpressionStmt(Stmt.Expression stmt);

        R VisitPrintStmt(Stmt.Print stmt);

        R VisitVarStmt(Stmt.Var stmt);
    }
}
