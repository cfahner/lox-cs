namespace Lox.Parser
{
    public interface IExprVisitor<R>
    {
        R VisitAssign(Expr.Assign expr);

        R VisitBinary(Expr.Binary expr);

        R VisitGrouping(Expr.Grouping expr);

        R VisitLiteral(Expr.Literal expr);

        R VisitVariable(Expr.Variable expr);

        R VisitUnary(Expr.Unary expr);
    }
}
