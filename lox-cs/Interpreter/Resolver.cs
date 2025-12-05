using Lox.Parser;
using Lox.Scanner;

namespace Lox.Interpreter
{
    internal class Resolver(Interpreter interpreter) : IExprVisitor<object?>, IStmtVisitor<object?>
    {
        private enum FunctionType
        {
            None, Function
        }

        private readonly Interpreter _interpreter = interpreter;

        private readonly Stack<Dictionary<string, bool>> _scopes = [];

        private FunctionType _currentFunctionType = FunctionType.None;

        public void Resolve(IEnumerable<Stmt> stmts)
        {
            foreach (var stmt in stmts)
            {
                Resolve(stmt);
            }
        }

        public void Resolve(Stmt stmt) => stmt.Accept(this);

        public void Resolve(Expr expr) => expr.Accept(this);

        public object? VisitBlockStmt(Stmt.Block stmt)
        {
            BeginScope();
            Resolve(stmt.Statements);
            EndScope();
            return null;
        }

        public object? VisitExpressionStmt(Stmt.Expression stmt)
        {
            Resolve(stmt.Expr);
            return null;
        }

        public object? VisitFunctionStmt(Stmt.Function stmt)
        {
            Declare(stmt.Name);
            Define(stmt.Name);
            ResolveFunction(stmt, FunctionType.Function);
            return null;
        }

        public object? VisitIfStmt(Stmt.If stmt)
        {
            Resolve(stmt.Condition);
            Resolve(stmt.Then);
            if (stmt.Else is not null)
            {
                Resolve(stmt.Else);
            }
            return null;
        }

        public object? VisitPrintStmt(Stmt.Print stmt)
        {
            Resolve(stmt.Expr);
            return null;
        }

        public object? VisitReturnStmt(Stmt.Return stmt)
        {
            if (_currentFunctionType == FunctionType.None)
            {
                throw new ResolutionError(stmt.Keyword, "Attempt to return from top-level code.");
            }

            if (stmt.Value is not null)
            {
                Resolve(stmt.Value);
            }
            return null;
        }

        public object? VisitVarStmt(Stmt.Var stmt)
        {
            Declare(stmt.Name);
            if (stmt.Initializer is not null)
            {
                Resolve(stmt.Initializer);
            }
            Define(stmt.Name);
            return null;
        }

        public object? VisitWhileStmt(Stmt.While stmt)
        {
            Resolve(stmt.Condition);
            Resolve(stmt.Body);
            return null;
        }

        public object? VisitVariable(Expr.Variable expr)
        {
            if (_scopes.TryPeek(out var scope)
                && scope.TryGetValue(expr.Name.Lexeme, out var defined)
                && !defined)
            {
                throw new ResolutionError(expr.Name, $"Can't read variable '{expr.Name.Lexeme}' in its own initializer.");
            }

            ResolveLocal(expr, expr.Name);
            return null;
        }

        public object? VisitAssign(Expr.Assign expr)
        {
            Resolve(expr.Value);
            ResolveLocal(expr, expr.Name);
            return null;
        }

        public object? VisitBinary(Expr.Binary expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);
            return null;
        }

        public object? VisitCall(Expr.Call expr)
        {
            Resolve(expr.Callee);
            foreach (var arg in expr.Arguments)
            {
                Resolve(arg);
            }
            return null;
        }

        public object? VisitGrouping(Expr.Grouping expr)
        {
            Resolve(expr.Expression);
            return null;
        }

        public object? VisitLiteral(Expr.Literal expr)
        {
            return null;
        }

        public object? VisitLogical(Expr.Logical expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);
            return null;
        }

        public object? VisitUnary(Expr.Unary expr)
        {
            Resolve(expr.Right);
            return null;
        }

        private void BeginScope() => _scopes.Push([]);

        private void EndScope() => _scopes.Pop();

        private void Declare(Token name)
        {
            if (_scopes.TryPeek(out var scope))
            {
                if (scope.ContainsKey(name.Lexeme))
                {
                    throw new ResolutionError(name, $"A variable with name '{name.Lexeme}' already exists in this scope.");
                }
                scope[name.Lexeme] = false;
            }
        }

        private void Define(Token name)
        {
            if (_scopes.TryPeek(out var scope))
            {
                scope[name.Lexeme] = true;
            }
        }

        private void ResolveFunction(Stmt.Function function, FunctionType functionType)
        {
            var enclosingFunctionType = _currentFunctionType;
            _currentFunctionType = functionType;

            BeginScope();
            foreach (var param in function.Parameters)
            {
                Declare(param);
                Define(param);
            }
            Resolve(function.Body);
            EndScope();

            _currentFunctionType = enclosingFunctionType;
        }

        private void ResolveLocal(Expr expr, Token name)
        {
            for (var i = _scopes.Count - 1; i >= 0; i -= 1)
            {
                if (_scopes.ElementAt(i).ContainsKey(name.Lexeme))
                {
                    _interpreter.Resolve(expr, _scopes.Count - 1 - i);
                }
            }
        }
    }
}
