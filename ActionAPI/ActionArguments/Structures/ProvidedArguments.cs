using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ScriptedEventsAPI.ActionAPI.ActionArguments.Structures;

public class ProvidedArguments
{
    private List<BaseActionArgument> Arguments { get; set; } = [];

    public T Get<T>(string name) 
        where T : BaseActionArgument
    {
        return Arguments.FirstOrDefault(a => a.Name == name) as T 
               ?? throw new ArgumentOutOfRangeException(nameof(name), 
                   $"There is no assigned argument to this action with a name of '{name}'");
    }
    
    public T? GetOptional<T>(string name) 
        where T : BaseActionArgument
    {
        return Arguments.SingleOrDefault(a => a.Name == name) as T;
    }

    public void Add(BaseActionArgument argument)
    {
        Arguments.Add(argument);
    }

    public int Count => Arguments.Count;
}