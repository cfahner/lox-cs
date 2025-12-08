namespace Lox.Interpreter
{
    public class LoxClass(string name, IDictionary<string, LoxFunction> methods) : ILoxCallable
    {
        public string Name { get; private init; } = name;

        public int Arity => TryGetMethod("init", out var initializer)
            ? initializer.Arity
            : 0;

        private readonly IDictionary<string, LoxFunction> _methods = methods;

        public override string ToString() => Name;

        public object? Call(Interpreter interpreter, object?[] arguments)
        {
            var instance = new LoxInstance(this);

            if (TryGetMethod("init", out var initializer))
            {
                _ = initializer.Bind(instance).Call(interpreter, arguments);
            }

            return instance;
        }

        public bool TryGetMethod(string name, out LoxFunction method)
        {
            return _methods.TryGetValue(name, out method!);
        }
    }
}
