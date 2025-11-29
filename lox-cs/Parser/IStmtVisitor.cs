namespace Lox.Parser
{
    public interface IStmtVisitor<R>
    {
        R VisitBlockStmt(Stmt.Block stmt);

        R VisitExpressionStmt(Stmt.Expression stmt);

        R VisitIfStmt(Stmt.If stmt);

        R VisitPrintStmt(Stmt.Print stmt);

        R VisitWhileStmt(Stmt.While stmt);

        R VisitVarStmt(Stmt.Var stmt);
    }
}
