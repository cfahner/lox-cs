using Lox.Scanner;

namespace Lox.Parser
{
    public class Parser(Token[] tokens)
    {
        public record Result(IEnumerable<Stmt> Statements, IEnumerable<ParseError> Errors)
        {
        }

        public const int ParameterLimit = 255;

        private readonly List<ParseError> _accumulatedErrors = [];

        private bool IsAtEnd => Peek().Type == TokenType.Eof;

        private int _current = 0;

        public Result Parse()
        {
            List<Stmt> stmts = [];
            try
            {
                while (!IsAtEnd)
                {
                    stmts.Add(Declaration());
                }
            }
            catch (ParseError)
            {
            }
            return new Result(stmts, _accumulatedErrors);
        }

        private Stmt Declaration()
        {
            try
            {
                return Match(TokenType.Fun)
                    ? Function("function")
                    : Match(TokenType.Var)
                    ? VarDeclaration()
                    : Statement();
            }
            catch (ParseError)
            {
                Synchronize();
                return new Stmt.Expression(new Expr.Literal(null));
            }
        }

        private Stmt Statement()
        {
            return Match(TokenType.For)
                ? ForStatement()
                : Match(TokenType.If)
                ? IfStatement()
                : Match(TokenType.Print)
                ? PrintStatement()
                : Match(TokenType.Return)
                ? ReturnStatement()
                : Match(TokenType.While)
                ? WhileStatement()
                : Match(TokenType.LeftBrace)
                ? new Stmt.Block(Block())
                : ExpressionStatement();
        }

        private Stmt ForStatement()
        {
            _ = Consume(TokenType.LeftParenthesis, "Expected '(' after 'for'.");

            Stmt? initializer = Match(TokenType.Semicolon)
                ? null
                : Match(TokenType.Var)
                ? VarDeclaration()
                : ExpressionStatement();

            var condition = !Check(TokenType.Semicolon)
                ? Expression()
                : null;
            _ = Consume(TokenType.Semicolon, "Expected ';' after for-condition.");

            var increment = !Check(TokenType.RightParenthesis)
                ? Expression()
                : null;
            _ = Consume(TokenType.RightParenthesis, "Expected ')' after for-increment.");

            var body = Statement();

            if (increment is not null)
            {
                body = new Stmt.Block([body, new Stmt.Expression(increment)]);
            }
            body = new Stmt.While(condition ?? new Expr.Literal(true), body);
            if (initializer is not null)
            {
                body = new Stmt.Block([initializer, body]);
            }
            return body;
        }

        private Stmt.If IfStatement()
        {
            _ = Consume(TokenType.LeftParenthesis, "Expected '(' after 'if'.");
            var condition = Expression();
            _ = Consume(TokenType.RightParenthesis, "Expected ')' after if-condition.");

            var thenBranch = Statement();
            var elseBranch = Match(TokenType.Else)
                ? Statement()
                : null;

            return new Stmt.If(condition, thenBranch, elseBranch);
        }

        private Stmt.Print PrintStatement()
        {
            var value = Expression();
            _ = Consume(TokenType.Semicolon, "Expected ';' after print value.");
            return new Stmt.Print(value);
        }

        private Stmt.Return ReturnStatement()
        {
            var keyword = Previous();
            var value = !Check(TokenType.Semicolon)
                ? Expression()
                : null;

            _ = Consume(TokenType.Semicolon, "Expected ';' after return value.");
            return new Stmt.Return(keyword, value);
        }

        private Stmt.Var VarDeclaration()
        {
            var name = Consume(TokenType.Identifier, "Expected variable name after 'var'.");

            var initializer = Match(TokenType.Equal)
                ? Expression()
                : null;

            _ = Consume(TokenType.Semicolon, "Expected ';' after variable declaration.");
            return new Stmt.Var(name, initializer);
        }

        private Stmt.While WhileStatement()
        {
            _ = Consume(TokenType.LeftParenthesis, "Expected '(' after 'while'.");
            var condition = Expression();
            _ = Consume(TokenType.RightParenthesis, "Expected ')' after while-condition.");
            var body = Statement();

            return new Stmt.While(condition, body);
        }

        private Stmt.Expression ExpressionStatement()
        {
            var expr = Expression();
            _ = Consume(TokenType.Semicolon, "Expecting ';' after expression.");
            return new Stmt.Expression(expr);
        }

        private Stmt.Function Function(string kind)
        {
            var name = Consume(TokenType.Identifier, $"Expected {kind} name after 'fun'.");
            _ = Consume(TokenType.LeftParenthesis, $"Expected '(' after {kind} name.");
            var parameters = new List<Token>();
            if (!Check(TokenType.RightParenthesis))
            {
                do
                {
                    if (parameters.Count >= ParameterLimit)
                    {
                        _ = Error(Peek(), $"Can't have more than {ParameterLimit} parameters.");
                    }
                    parameters.Add(Consume(TokenType.Identifier, "Expected a parameter name."));
                }
                while (Match(TokenType.Comma));
            }
            _ = Consume(TokenType.RightParenthesis, "Expected ')' after parameter list.");
            _ = Consume(TokenType.LeftBrace, "Expected '{' before " + kind + " body.");
            var body = Block();
            return new Stmt.Function(name, [.. parameters], body);
        }

        private List<Stmt> Block()
        {
            List<Stmt> stmts = [];
            while (!Check(TokenType.RightBrace) && !IsAtEnd)
            {
                stmts.Add(Declaration());
            }
            _ = Consume(TokenType.RightBrace, "Expecting '}' after block.");
            return stmts;
        }

        private Expr Expression()
        {
            return Assignment();
        }

        private Expr Assignment()
        {
            var expr = Or();

            if (Match(TokenType.Equal))
            {
                var equals = Previous();
                var value = Assignment();

                if (expr is Expr.Variable variableExpr)
                {
                    return new Expr.Assign(variableExpr.Name, value);
                }

                _ = Error(equals, "Invalid assignment target.");
            }

            return expr;
        }

        private Expr Or()
        {
            var expr = And();

            while (Match(TokenType.Or))
            {
                var op = Previous();
                var right = And();
                expr = new Expr.Logical(expr, op, right);
            }

            return expr;
        }

        private Expr And()
        {
            var expr = Equality();

            while (Match(TokenType.And))
            {
                var op = Previous();
                var right = Equality();
                expr = new Expr.Logical(expr, op, right);
            }

            return expr;
        }

        private Expr Equality()
        {
            var expr = Comparison();
            while (Match(TokenType.BangEqual, TokenType.EqualEqual))
            {
                expr = new Expr.Binary(expr, Previous(), Comparison());
            }
            return expr;
        }

        private Expr Comparison()
        {
            var expr = Term();
            while (Match(TokenType.Greater, TokenType.GreaterEqual, TokenType.Less, TokenType.LessEqual))
            {
                expr = new Expr.Binary(expr, Previous(), Term());
            }
            return expr;
        }

        private Expr Term()
        {
            var expr = Factor();
            while (Match(TokenType.Minus, TokenType.Plus))
            {
                expr = new Expr.Binary(expr, Previous(), Factor());
            }
            return expr;
        }

        private Expr Factor()
        {
            var expr = Unary();
            while (Match(TokenType.Slash, TokenType.Asterisk))
            {
                expr = new Expr.Binary(expr, Previous(), Unary());
            }
            return expr;
        }

        private Expr Unary()
        {
            return Match(TokenType.Bang, TokenType.Minus)
                ? new Expr.Unary(Previous(), Unary())
                : Call();
        }

        private Expr.Call FinishCall(Expr callee)
        {
            List<Expr> arguments = [];
            if (!Check(TokenType.RightParenthesis))
            {
                do
                {
                    if (arguments.Count >= ParameterLimit)
                    {
                        _ = Error(Peek(), $"Can't have more than {ParameterLimit} arguments.");
                    }
                    arguments.Add(Expression());
                }
                while (Match(TokenType.Comma));
            }

            var parenthesis = Consume(TokenType.RightParenthesis, "Expected ')' after argument list.");

            return new Expr.Call(callee, parenthesis, arguments);
        }

        private Expr Call()
        {
            var expr = Primary();

            while (true)
            {
                if (Match(TokenType.LeftParenthesis))
                {
                    expr = FinishCall(expr);
                }
                else
                {
                    break;
                }
            }

            return expr;
        }

        private Expr Primary()
        {
            if (Match(TokenType.False))
            {
                return new Expr.Literal(false);
            }
            if (Match(TokenType.True))
            {
                return new Expr.Literal(true);
            }
            if (Match(TokenType.Nil))
            {
                return new Expr.Literal(null);
            }
            if (Match(TokenType.Number, TokenType.String))
            {
                return new Expr.Literal(Previous().Literal);
            }
            if (Match(TokenType.Identifier))
            {
                return new Expr.Variable(Previous());
            }
            if (Match(TokenType.LeftParenthesis))
            {
                var expr = Expression();
                _ = Consume(TokenType.RightParenthesis, "Expected ')' after expression.");
                return new Expr.Grouping(expr);
            }
            throw Error(Peek(), "Expected an expression.");
        }

        private bool Match(params TokenType[] tokenTypes)
        {
            DiscardNonCode();
            foreach (var tokenType in tokenTypes)
            {
                if (Check(tokenType))
                {
                    _ = Advance();
                    return true;
                }
            }
            return false;
        }

        private Token Consume(TokenType type, string message)
        {
            return Check(type) ? Advance() : throw Error(Peek(), message);
        }

        private Token Advance()
        {
            if (!IsAtEnd)
            {
                _current += 1;
            }
            return Previous();
        }

        private bool Check(TokenType tokenType)
        {
            DiscardNonCode();
            return !IsAtEnd && Peek().Type == tokenType;
        }

        private void DiscardNonCode()
        {
            while (!IsAtEnd && Peek().Type is TokenType.Whitespace or TokenType.Comment)
            {
                _current += 1;
            }
        }

        private Token Peek()
        {
            return tokens[_current];
        }

        private Token Previous()
        {
            return tokens[_current - 1];
        }

        private ParseError Error(Token token, string message)
        {
            var error = new ParseError(token, message);
            _accumulatedErrors.Add(error);
            return error;
        }

        private void Synchronize()
        {
            _ = Advance();
            while (!IsAtEnd)
            {
                if (Previous().Type == TokenType.Semicolon)
                {
                    return;
                }
                switch (Peek().Type)
                {
                    case TokenType.Class:
                    case TokenType.For:
                    case TokenType.Fun:
                    case TokenType.If:
                    case TokenType.Print:
                    case TokenType.Return:
                    case TokenType.Var:
                    case TokenType.While:
                        return;
                }
                _ = Advance();
            }
        }
    }
}
