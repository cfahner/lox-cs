
namespace Lox.Interpreter.Natives
{
    internal class Clock : NativeLoxCallable
    {
        public override int Arity => 0;

        public override object? Call(Interpreter interpreter, IEnumerable<object?> arguments)
        {
            return (double)(TimeProvider.System.GetLocalNow().Ticks / TimeSpan.TicksPerMillisecond);
        }
    }
}
