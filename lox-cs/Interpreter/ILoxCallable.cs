namespace Lox.Interpreter
{
    public interface ILoxCallable
    {
        int Arity { get; }

        object? Call(Interpreter interpreter, object?[] arguments);
    }
}
