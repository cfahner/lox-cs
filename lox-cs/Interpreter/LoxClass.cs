namespace Lox.Interpreter
{
    public class LoxClass(string name) : ILoxCallable
    {
        public string Name { get; private init; } = name;

        public int Arity => 0;

        public override string ToString() => Name;

        public object? Call(Interpreter interpreter, object?[] arguments) => new LoxInstance(this);
    }
}
