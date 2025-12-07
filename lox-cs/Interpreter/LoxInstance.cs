using Lox.Scanner;

namespace Lox.Interpreter
{
    internal class LoxInstance(LoxClass @class)
    {
        public LoxClass Class { get; private init; } = @class;

        private readonly Dictionary<string, object?> _fields = [];

        public override string ToString() => $"<instance of {Class.Name}>";

        public object? Get(Token name)
        {
            return _fields.TryGetValue(name.Lexeme, out var fieldValue)
                ? fieldValue
                : Class.TryGetMethod(name.Lexeme, out var method)
                ? method
                : throw new RuntimeError(name, $"Undefined property '{name.Lexeme}'.");
        }

        public void Set(Token name, object? value)
        {
            _fields[name.Lexeme] = value;
        }
    }
}
