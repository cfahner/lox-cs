namespace Lox.Interpreter.Operations
{
    internal static class Conversions
    {
        public static bool ToTruthy(object? value) => value switch
        {
            null => false,
            bool boolValue => boolValue,
            _ => true
        };

        public static string Stringify(object? value)
        {
            return value switch
            {
                null => "nil",
                double doubleValue => $"{doubleValue}",
                bool boolValue => boolValue ? "true" : "false",
                _ => value.ToString()
            } ?? string.Empty;
        }
    }
}
