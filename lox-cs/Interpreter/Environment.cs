using Lox.Interpreter.RuntimeErrors;
using Lox.Scanner;

namespace Lox.Interpreter
{
    public class Environment
    {
        private readonly Dictionary<string, object?> _values = [];

        private readonly Environment? _enclosing;

        public Environment()
        {
            _enclosing = null;
        }

        public Environment(Environment enclosing)
        {
            _enclosing = enclosing;
        }

        public void Define(string name, object? value)
        {
            _values[name] = value;
        }

        public object? Get(Token name)
        {
            return _values.TryGetValue(name.Lexeme, out var value)
                ? value
                : _enclosing is not null
                ? _enclosing.Get(name)
                : throw new UndefinedVariableError(name);
        }

        public object? Assign(Token name, object? value)
        {
            if (_values.ContainsKey(name.Lexeme))
            {
                _values[name.Lexeme] = value;
                return value;
            }
            return _enclosing is not null
                ? _enclosing.Assign(name, value)
                : throw new UndefinedVariableError(name);
        }
    }
}
