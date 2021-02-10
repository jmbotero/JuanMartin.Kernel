namespace JuanMartin.Kernel.Formatters
{
    public interface IReader
    {
        string ToString();

        ValueHolder Value { get; }
    }
}
