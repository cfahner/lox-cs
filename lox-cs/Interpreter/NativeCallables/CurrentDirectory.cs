namespace Lox.Interpreter.NativeCallables
{
    internal class CurrentDirectory : NativeLoxCallable
    {
        public override int Arity => 0;

        public override object? Call(Interpreter interpreter, object?[] arguments)
        {
            return Directory.GetCurrentDirectory();
        }
    }
}
