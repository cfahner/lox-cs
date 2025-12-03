using Lox.Parser;

namespace Lox.Interpreter
{
    public class LoxFunction : ILoxCallable
    {
        public int Arity => _declaration.Parameters.Length;

        private readonly Stmt.Function _declaration;

        public LoxFunction(Stmt.Function declaration)
        {
            _declaration = declaration;
        }

        public object? Call(Interpreter interpreter, object?[] arguments)
        {
            var environment = new Environment(interpreter.Globals);
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
