using ScriptedEventsAPI.Helpers.ResultStructure;

namespace ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;

public abstract class BaseMethodArgument(string name)
{
    public string Name { get; } = name;
    public object? DefaultValue { get; init; } = null;
    public string? AdditionalDescription { get; init; } = null;
    
    protected ResultStacker Rs => new($"Converting argument {Name} ({GetType().Name}) failed.");
}