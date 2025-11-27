using Lox.Interpreter.RuntimeErrors;
using Lox.Scanner;

namespace Lox.Interpreter
{
    internal class Environment()
    {
        private readonly Dictionary<string, object?> _values = [];

        public void Define(string name, object? value)
        {
            _values[name] = value;
        }

        public object? Get(Token name)
        {
            return _values.TryGetValue(name.Lexeme, out var value)
                ? value
                : throw new UndefinedVariableError(name);
        }
    }
}
