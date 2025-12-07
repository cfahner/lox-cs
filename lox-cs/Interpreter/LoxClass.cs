namespace Lox.Interpreter
{
    public class LoxClass(string name, IDictionary<string, LoxFunction> methods) : ILoxCallable
    {
        public string Name { get; private init; } = name;

        public int Arity => 0;

        private readonly IDictionary<string, LoxFunction> _methods = methods;

        public override string ToString() => Name;

        public object? Call(Interpreter interpreter, object?[] arguments) => new LoxInstance(this);

        public bool TryGetMethod(string name, out LoxFunction method)
        {
            return _methods.TryGetValue(name, out method!);
        }
    }
}
