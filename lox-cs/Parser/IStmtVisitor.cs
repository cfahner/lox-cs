namespace Lox.Parser
{
    public interface IStmtVisitor<R>
    {
        R VisitBlockStmt(Stmt.Block stmt);

        R VisitExpressionStmt(Stmt.Expression stmt);

        R VisitFunctionStmt(Stmt.Function stmt);

        R VisitIfStmt(Stmt.If stmt);

        R VisitPrintStmt(Stmt.Print stmt);

        R VisitReturnStmt(Stmt.Return stmt);

        R VisitWhileStmt(Stmt.While stmt);

        R VisitVarStmt(Stmt.Var stmt);
    }
}
