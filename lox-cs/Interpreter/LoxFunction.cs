using Lox.Parser;

namespace Lox.Interpreter
{
    public class LoxFunction : ILoxCallable
    {
        public int Arity => _declaration.Parameters.Length;

        private readonly Stmt.Function _declaration;

        private readonly Environment _closure;

        private readonly bool _isInitializer;

        public LoxFunction(Stmt.Function declaration, Environment closure, bool isInitializer)
        {
            _declaration = declaration;
            _closure = closure;
            _isInitializer = isInitializer;
        }

        public LoxFunction Bind(LoxInstance instance)
        {
            var environment = new Environment(_closure);
            environment.Define("this", instance);
            return new LoxFunction(_declaration, environment, _isInitializer);
        }

        public object? Call(Interpreter interpreter, object?[] arguments)
        {
            var environment = new Environment(_closure);
            for (var i = 0; i < _declaration.Parameters.Length; i += 1)
            {
                environment.Define(_declaration.Parameters[i].Lexeme, arguments[i]);
            }

            try
            {
                interpreter.ExecuteBlock(_declaration.Body, environment);
            }
            catch (Return returnValue)
            {
                return _isInitializer
                    ? _closure.GetAt(0, "this")
                    : returnValue.Value;
            }

            return _isInitializer
                ? _closure.GetAt(0, "this")
                : null;
        }

        public override string ToString() => $"<fn {_declaration.Name.Lexeme}>";
    }
}
