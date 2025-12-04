using Lox.Parser;

namespace Lox.Interpreter
{
    public class LoxFunction : ILoxCallable
    {
        public int Arity => _declaration.Parameters.Length;

        private readonly Stmt.Function _declaration;

        private readonly Environment _closure;

        public LoxFunction(Stmt.Function declaration, Environment closure)
        {
            _declaration = declaration;
            _closure = closure;
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
                return returnValue.Value;
            }
            return null;
        }

        public override string ToString() => $"<fn {_declaration.Name.Lexeme}>";
    }
}
