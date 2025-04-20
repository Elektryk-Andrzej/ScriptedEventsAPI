using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.MethodAPI.Arguments.Structures;

namespace ScriptedEventsAPI.MethodAPI.Arguments.Args;

public abstract class BaseMethodArgument(string name)
{
    public string Name { get; } = name;
    public RequiredArgumentInfo RequiredInfo { get; init; } = RequiredArgumentInfo.Required();
    public string? AdditionalDescription { get; init; } = null;
    
    protected ResultStacker Rs => new($"Converting argument {Name} ({GetType().Name}) failed.");
}