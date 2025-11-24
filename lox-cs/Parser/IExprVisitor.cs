namespace Lox.Parser
{
    public interface IExprVisitor<R>
    {
        R VisitBinary(Expr.Binary binary);

        R VisitGrouping(Expr.Grouping grouping);

        R VisitLiteral(Expr.Literal literal);

        R VisitUnary(Expr.Unary unary);
    }
}
