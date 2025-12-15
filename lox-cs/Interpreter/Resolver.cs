using Lox.Parser;
using Lox.Scanner;

namespace Lox.Interpreter
{
    internal class Resolver(Interpreter interpreter) : IExprVisitor<object?>, IStmtVisitor<object?>
    {
        private enum FunctionType
        {
            None, Function, Initializer, Method
        }

        private enum ClassType
        {
            None, Class, Subclass
        }

        private readonly Interpreter _interpreter = interpreter;

        private readonly Stack<Dictionary<string, bool>> _scopes = [];

        private FunctionType _currentFunctionType = FunctionType.None;

        private ClassType _currentClassType = ClassType.None;

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

        public object? VisitClassStmt(Stmt.Class stmt)
        {
            var enclosingClassType = _currentClassType;
            _currentClassType = ClassType.Class;

            Declare(stmt.Name);
            Define(stmt.Name);

            if (stmt.Superclass is not null)
            {
                _currentClassType = ClassType.Subclass;
                if (stmt.Superclass.Name.Lexeme == stmt.Name.Lexeme)
                {
                    throw new ResolverError(stmt.Name, $"Class '{stmt.Name}' tries to extend itself.");
                }
                Resolve(stmt.Superclass);
                BeginScope();
                _scopes.Peek()["super"] = true;
            }

            BeginScope();
            _scopes.Peek()["this"] = true;

            foreach (var method in stmt.Methods)
            {
                ResolveFunction(method, method.Name.Lexeme == "init" ? FunctionType.Initializer : FunctionType.Method);
            }

            EndScope();

            if (stmt.Superclass is not null)
            {
                EndScope();
            }

            _currentClassType = enclosingClassType;
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
                throw new ResolverError(stmt.Keyword, "Attempt to return from top-level code.");
            }

            if (stmt.Value is not null)
            {
                if (_currentFunctionType == FunctionType.Initializer)
                {
                    throw new ResolverError(stmt.Keyword, "Attempt to return a value from 'init'.");
                }
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

        public object? VisitVariableExpr(Expr.Variable expr)
        {
            if (_scopes.TryPeek(out var scope)
                && scope.TryGetValue(expr.Name.Lexeme, out var defined)
                && !defined)
            {
                throw new ResolverError(expr.Name, $"Can't read variable '{expr.Name.Lexeme}' in its own initializer.");
            }

            ResolveLocal(expr, expr.Name);
            return null;
        }

        public object? VisitAssignExpr(Expr.Assign expr)
        {
            Resolve(expr.Value);
            ResolveLocal(expr, expr.Name);
            return null;
        }

        public object? VisitBinaryExpr(Expr.Binary expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);
            return null;
        }

        public object? VisitCallExpr(Expr.Call expr)
        {
            Resolve(expr.Callee);
            foreach (var arg in expr.Arguments)
            {
                Resolve(arg);
            }
            return null;
        }

        public object? VisitGetExpr(Expr.Get expr)
        {
            Resolve(expr.Object);
            return null;
        }

        public object? VisitGroupingExpr(Expr.Grouping expr)
        {
            Resolve(expr.Expression);
            return null;
        }

        public object? VisitLiteralExpr(Expr.Literal expr)
        {
            return null;
        }

        public object? VisitLogicalExpr(Expr.Logical expr)
        {
            Resolve(expr.Left);
            Resolve(expr.Right);
            return null;
        }

        public object? VisitSetExpr(Expr.Set expr)
        {
            Resolve(expr.Value);
            Resolve(expr.Object);
            return null;
        }

        public object? VisitSuperExpr(Expr.Super expr)
        {
            if (_currentClassType == ClassType.None)
            {
                throw new ResolverError(expr.Keyword, "Can't use 'super' outside of a class.");
            }
            else if (_currentClassType != ClassType.Subclass)
            {
                throw new ResolverError(expr.Keyword, "Can't use 'super' in a class without a superclass.");
            }
            ResolveLocal(expr, expr.Keyword);
            return null;
        }

        public object? VisitThisExpr(Expr.This expr)
        {
            if (_currentClassType == ClassType.None)
            {
                throw new ResolverError(expr.Keyword, "Can't use 'this' outside class declaration.");
            }

            ResolveLocal(expr, expr.Keyword);
            return null;
        }

        public object? VisitUnaryExpr(Expr.Unary expr)
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
                    throw new ResolverError(name, $"A variable with name '{name.Lexeme}' already exists in this scope.");
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
            // Apparently in C#, Stack.ElementAt() counts from the top of the stack
            // But in Java, Stack.get() counts from the bottom of the stack
            // So we have to count up in the C# implementation where the Java implementation counts down
            for (var i = 0; i < _scopes.Count; i += 1)
            {
                if (_scopes.ElementAt(i).ContainsKey(name.Lexeme))
                {
                    _interpreter.Resolve(expr, i);
                }
            }
        }
    }
}
