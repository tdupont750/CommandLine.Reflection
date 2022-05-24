namespace System.CommandLine.Reflection;

public static class ArgumentArityExtensions
{
    public static ArgumentArity ToArgumentArity(this ArityValue value) =>
        value switch
        {
            ArityValue.Zero => ArgumentArity.Zero,
            ArityValue.ZeroOrOne => ArgumentArity.ZeroOrOne,
            ArityValue.ExactlyOne => ArgumentArity.ExactlyOne,
            ArityValue.ZeroOrMore => ArgumentArity.ZeroOrMore,
            ArityValue.OneOrMore => ArgumentArity.OneOrMore,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
        };
}