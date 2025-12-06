using Lox.Interpreter.NativeCallables;
using Lox.Interpreter.Operations;
using Lox.Interpreter.RuntimeErrors;
using Lox.Parser;
using Lox.Scanner;

namespace Lox.Interpreter
{
    public class Interpreter : IExprVisitor<object?>, IStmtVisitor<object?>
    {
        private readonly Environment _globals = new();

        private readonly Dictionary<Expr, int> _locals = [];

        private Environment _environment;

        public Interpreter()
        {
            _environment = _globals;
            _globals.Define("clock", new Clock());
            _globals.Define("currentDirectory", new CurrentDirectory());
        }

        public void Interpret(IEnumerable<Stmt> stmts)
        {
            foreach (var stmt in stmts)
            {
                _ = Execute(stmt);
            }
        }

        public void Resolve(Expr expr, int depth)
        {
            _locals.Add(expr, depth);
        }

        public object? VisitAssignExpr(Expr.Assign expr)
        {
            var value = Evaluate(expr.Value);

            if (_locals.TryGetValue(expr, out var distance))
            {
                _environment.AssignAt(distance, expr.Name, value);
            }
            else
            {
                _ = _globals.Assign(expr.Name, value);
            }

            return value;
        }

        public object? VisitBinaryExpr(Expr.Binary expr)
        {
            var left = Evaluate(expr.Left);
            var right = Evaluate(expr.Right);

            return expr.Operator.Type switch
            {
                TokenType.Greater => BinaryOperations.IsGreater(expr.Operator, left, right),
                TokenType.GreaterEqual => BinaryOperations.IsGreaterEqual(expr.Operator, left, right),
                TokenType.Less => BinaryOperations.IsLess(expr.Operator, left, right),
                TokenType.LessEqual => BinaryOperations.IsLessEqual(expr.Operator, left, right),
                TokenType.BangEqual => !BinaryOperations.IsEqual(left, right),
                TokenType.EqualEqual => BinaryOperations.IsEqual(left, right),
                TokenType.Minus => BinaryOperations.Subtract(expr.Operator, left, right),
                TokenType.Plus => BinaryOperations.Add(expr.Operator, left, right),
                TokenType.Slash => BinaryOperations.Divide(expr.Operator, left, right),
                TokenType.Asterisk => BinaryOperations.Multiply(expr.Operator, left, right),
                _ => null
            };
        }

        public object? VisitCallExpr(Expr.Call expr)
        {
            var callee = Evaluate(expr.Callee);

            var arguments = expr.Arguments.Select(Evaluate).ToArray();

            return callee is ILoxCallable callable
                ? arguments.Length != callable.Arity
                    ? throw new ArgumentCountError(expr.Parenthesis, arguments.Length, callable.Arity)
                    : callable.Call(this, arguments)
                : throw new TypeError(expr.Parenthesis, "Callee is not callable.");
        }

        public object? VisitGroupingExpr(Expr.Grouping expr)
        {
            return Evaluate(expr.Expression);
        }

        public object? VisitLiteralExpr(Expr.Literal expr)
        {
            return expr.Value;
        }

        public object? VisitLogicalExpr(Expr.Logical expr)
        {
            var left = Evaluate(expr.Left);

            if (expr.Operator.Type is TokenType.Or)
            {
                if (Conversions.ToTruthy(left))
                {
                    return left;
                }
            }
            else
            {
                if (!Conversions.ToTruthy(left))
                {
                    return left;
                }
            }

            return Evaluate(expr.Right);
        }

        public object? VisitVariableExpr(Expr.Variable expr)
        {
            return LookupVariable(expr.Name, expr);
        }

        public object? VisitUnaryExpr(Expr.Unary expr)
        {
            var right = Evaluate(expr.Right);

            return expr.Operator.Type switch
            {
                TokenType.Minus => right is null ? 0.0 : -(double)right,
                TokenType.Bang => !Conversions.ToTruthy(right),
                _ => null
            };
        }

        public object? VisitBlockStmt(Stmt.Block stmt)
        {
            ExecuteBlock(stmt.Statements, new Environment(_environment));
            return null;
        }

        public object? VisitExpressionStmt(Stmt.Expression stmt)
        {
            _ = Evaluate(stmt.Expr);
            return null;
        }

        public object? VisitFunctionStmt(Stmt.Function stmt)
        {
            var function = new LoxFunction(stmt, _environment);
            _environment.Define(stmt.Name.Lexeme, function);
            return null;
        }

        public object? VisitIfStmt(Stmt.If stmt)
        {
            if (Conversions.ToTruthy(Evaluate(stmt.Condition)))
            {
                _ = Execute(stmt.Then);
            }
            else if (stmt.Else is not null)
            {
                _ = Execute(stmt.Else);
            }
            return null;
        }

        public object? VisitPrintStmt(Stmt.Print stmt)
        {
            var value = Evaluate(stmt.Expr);
            Console.WriteLine(Conversions.Stringify(value));
            return null;
        }

        public object? VisitReturnStmt(Stmt.Return stmt)
        {
            throw new Return(stmt.Value is not null ? Evaluate(stmt.Value) : null);
        }

        public object? VisitVarStmt(Stmt.Var stmt)
        {
            object? value = null;
            if (stmt.Initializer is not null)
            {
                value = Evaluate(stmt.Initializer);
            }

            _environment.Define(stmt.Name.Lexeme, value);
            return null;
        }

        public object? VisitWhileStmt(Stmt.While stmt)
        {
            while (Conversions.ToTruthy(Evaluate(stmt.Condition)))
            {
                _ = Execute(stmt.Body);
            }
            return null;
        }

        public void ExecuteBlock(IEnumerable<Stmt> statements, Environment environment)
        {
            var previousEnv = _environment;
            try
            {
                _environment = environment;
                foreach (var stmt in statements)
                {
                    _ = Execute(stmt);
                }
            }
            finally
            {
                _environment = previousEnv;
            }
        }

        private object? Evaluate(Expr expr)
        {
            return expr.Accept(this);
        }

        private object? Execute(Stmt stmt)
        {
            return stmt.Accept(this);
        }

        private object? LookupVariable(Token name, Expr expr)
        {
            return _locals.TryGetValue(expr, out var distance)
                ? _environment.GetAt(distance, name.Lexeme)
                : _globals.Get(name);
        }
    }
}
