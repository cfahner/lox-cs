namespace Lox.Scanner
{
    public class ScannerException(int line, string message) : Exception(message)
    {
        public int Line { get; private init; } = line;
    }
}
