namespace Lox.Interpreter
{
    public class LoxClass(string name)
    {
        public string Name { get; private init; } = name;

        public override string ToString() => Name;
    }
}
