namespace Lox.Interpreter.NativeCallables
{
    internal abstract class NativeLoxCallable : ILoxCallable
    {
        public abstract int Arity { get; }

        public abstract object? Call(Interpreter interpreter, object?[] arguments);

        public sealed override string ToString() => "<native fn>";
    }
}
