namespace Lox.Interpreter.Natives
{
    internal abstract class NativeLoxCallable : ILoxCallable
    {
        public abstract int Arity { get; }

        public abstract object? Call(Interpreter interpreter, IEnumerable<object?> arguments);

        public sealed override string ToString() => "<native fn>";
    }
}
