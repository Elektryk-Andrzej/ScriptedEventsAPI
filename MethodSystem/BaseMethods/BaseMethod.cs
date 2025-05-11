using System;
using System.Linq;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.ScriptSystem;

namespace ScriptedEventsAPI.MethodSystem.BaseMethods;

/// <summary>
///     Represents a base method.
/// </summary>
/// <remarks>
///     Do NOT use this to define a SER method, as it has no Execute() method.
///     Use <see cref="Method" /> or <see cref="YieldingMethod" />.
/// </remarks>
public abstract class BaseMethod
{
    protected BaseMethod()
    {
        var type = GetType();
        Subgroup = type.Namespace?.Split('.').LastOrDefault()?.Replace("Methods", "") ?? "Unknown";
        
        var name = type.Name;
        if (!name.EndsWith("Method"))
        {
            throw new ArgumentException($"Method class name '{name}' must end with 'Method'.");
        }
        
        Name = name.Substring(0, name.Length - "Method".Length);
        Args = new(this);
    }

    public readonly string Name;
    
    public abstract string Description { get; }
    
    public abstract BaseMethodArgument[] ExpectedArguments { get; }
    
    public ProvidedArguments Args { get; }
    
    public Script Script { get; set; } = null!;

    public readonly string Subgroup;
}