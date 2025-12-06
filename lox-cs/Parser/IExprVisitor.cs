namespace Lox.Parser
{
    public interface IExprVisitor<R>
    {
        R VisitAssignExpr(Expr.Assign expr);

        R VisitBinaryExpr(Expr.Binary expr);

        R VisitCallExpr(Expr.Call expr);

        R VisitGetExpr(Expr.Get expr);

        R VisitGroupingExpr(Expr.Grouping expr);

        R VisitLiteralExpr(Expr.Literal expr);

        R VisitLogicalExpr(Expr.Logical expr);

        R VisitUnaryExpr(Expr.Unary expr);

        R VisitVariableExpr(Expr.Variable expr);
    }
}
