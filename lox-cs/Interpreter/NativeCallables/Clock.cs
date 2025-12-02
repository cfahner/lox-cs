namespace Lox.Interpreter.NativeCallables
{
    internal class Clock : NativeLoxCallable
    {
        public override int Arity => 0;

        public override object? Call(Interpreter interpreter, object?[] arguments)
        {
            return (double)(TimeProvider.System.GetLocalNow().Ticks / TimeSpan.TicksPerMillisecond);
        }
    }
}
