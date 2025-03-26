using System;
using System.Collections.Generic;
using System.Linq;
using ScriptedEventsAPI.ActionAPI.ActionArguments.Arguments;

namespace ScriptedEventsAPI.ActionAPI.ActionArguments.Structures;

public class ProvidedArguments
{
    private List<BaseActionArgument> Arguments { get; set; } = [];

    public TArgument Get<TArgument>(string name) 
        where TArgument : BaseActionArgument
    {
        return Arguments.FirstOrDefault(a => a.Name == name) as TArgument 
               ?? throw new ArgumentOutOfRangeException(nameof(name), 
                   $"There is no assigned argument to this action with a name of '{name}'");
    }
    
    public TArgument? GetOptional<TArgument>(string name) 
        where TArgument : BaseActionArgument
    {
        return Arguments.SingleOrDefault(a => a.Name == name) as TArgument;
    }

    public void Add(BaseActionArgument argument)
    {
        Arguments.Add(argument);
    }

    public int Count => Arguments.Count;
}