namespace Lox.Interpreter
{
    internal class Return(object? value) : Exception
    {
        public object? Value { get; private set; } = value;
    }
}
