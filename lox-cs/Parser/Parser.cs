using Lox.Scanner;

namespace Lox.Parser
{
    public class Parser(Token[] tokens)
    {
        private readonly List<ParseError> _accumulatedErrors = [];
        public IEnumerable<ParseError> AccumulatedErrors => _accumulatedErrors;

        private bool IsAtEnd => Peek().Type == TokenType.Eof;

        private int _current = 0;

        public IEnumerable<Stmt> Parse()
        {
            while (!IsAtEnd)
            {
                yield return Declaration();
            }
        }

        private Stmt Declaration()
        {
            try
            {
                return Match(TokenType.Var)
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
            return Match(TokenType.Print)
                ? PrintStatement()
                : ExpressionStatement();
        }

        private Stmt.Print PrintStatement()
        {
            var value = Expression();
            _ = Consume(TokenType.Semicolon, "Expected ';' after print value.");
            return new Stmt.Print(value);
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

        private Stmt.Expression ExpressionStatement()
        {
            var expr = Expression();
            _ = Consume(TokenType.Semicolon, "Expecting ';' after expression.");
            return new Stmt.Expression(expr);
        }

        private Expr Expression()
        {
            return Equality();
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
            return Match(TokenType.Bang, TokenType.Minus) ? new Expr.Unary(Previous(), Unary()) : Primary();
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
            DiscardNonCode();
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
