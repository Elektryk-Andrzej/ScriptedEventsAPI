namespace SER.MethodSystem.ArgumentSystem.Structures;

public record Option(string Value, string Description = "")
{
    public static implicit operator Option(string value)
    {
        return new Option(value);
    }
}