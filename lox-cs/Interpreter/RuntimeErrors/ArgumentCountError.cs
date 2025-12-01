using Lox.Scanner;

namespace Lox.Interpreter.RuntimeErrors
{
    public class ArgumentCountError(Token token, int actualCount, int expectedCount)
        : RuntimeError(token, $"Wrong amount of arguments passed to callable on line {token.Line}, {actualCount} passed and exactly {expectedCount} expected.")
    {
    }
}
