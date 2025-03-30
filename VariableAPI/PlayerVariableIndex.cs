using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ScriptedEventsAPI.VariableAPI.Structures;

namespace ScriptedEventsAPI.VariableAPI;

public static class PlayerVariableIndex
{
    public static readonly HashSet<PlayerVariable> GlobalVariables = [];

    public static void Initalize()
    {
        GlobalVariables.Clear();

        var allApiVariables = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsClass && t != typeof(PlayerVariable) && typeof(PlayerVariable).IsAssignableFrom(t))
            .Select(t => Activator.CreateInstance(t) as PlayerVariable);

        foreach (var variable in allApiVariables)
        {
            if (variable is null) continue;
            AddVariable(variable);
        }
    }

    public static void AddVariable(PlayerVariable variable)
    {
        GlobalVariables.Add(variable);
    }

    public static void Clear()
    {
        GlobalVariables.Clear();
    }
}