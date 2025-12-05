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

        public Environment Ancestor(int distance)
        {
            var environment = this;
            for (var i = 0; i < distance; i += 1)
            {
                environment = environment?._enclosing;
            }
            return environment is null
                ? throw new Exception($"No environment found at distance {distance}.")
                : environment;
        }

        public object? GetAt(int distance, string name)
        {
            return Ancestor(distance)._values.TryGetValue(name, out var value)
                ? value
                : null;
        }

        public void AssignAt(int distance, Token name, object? value)
        {
            Ancestor(distance)._values.Add(name.Lexeme, value);
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
