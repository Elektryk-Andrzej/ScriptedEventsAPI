using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.MethodSystem.BaseMethods;

namespace ScriptedEventsAPI.MethodSystem;

public static class MethodIndex
{
    public static readonly Dictionary<string, Type> NameToMethodIndex = new();

    public static void Initalize()
    {
        NameToMethodIndex.Clear();

        var allApiMethods = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(BaseMethod).IsAssignableFrom(t))
            .Select(t => Activator.CreateInstance(t) as BaseMethod);

        foreach (var method in allApiMethods)
        {
            if (method is null) continue;
            AddMethod(method);
        }
    }

    public static void AddMethod(BaseMethod method)
    {
        if (NameToMethodIndex.ContainsKey(method.Name))
        {
            Logger.Warn($"method {method.Name} is already registered!");
            return;
        }

        NameToMethodIndex.Add(method.Name, method.GetType());
    }

    public static void Clear()
    {
        NameToMethodIndex.Clear();
    }
}