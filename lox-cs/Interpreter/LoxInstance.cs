namespace Lox.Interpreter
{
    internal class LoxInstance(LoxClass @class)
    {
        private readonly LoxClass _class = @class;

        public override string ToString() => $"<instance of {_class.Name}>";
    }
}
