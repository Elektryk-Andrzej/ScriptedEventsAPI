using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.MethodAPI.BaseMethods;

namespace ScriptedEventsAPI.MethodAPI;

public static class MethodIndex
{
    public static readonly Dictionary<string, Type> NameToActionIndex = new();

    public static void Initalize()
    {
        NameToActionIndex.Clear();

        var allApiActions = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(BaseMethod).IsAssignableFrom(t))
            .Select(t => Activator.CreateInstance(t) as BaseMethod);

        foreach (var action in allApiActions)
        {
            if (action is null) continue;
            AddAction(action);
        }
    }

    public static void AddAction(BaseMethod method)
    {
        if (NameToActionIndex.ContainsKey(method.Name))
        {
            Logger.Debug("method is already registered");
            return;
        }
        
        NameToActionIndex.Add(method.Name, method.GetType());
    }

    public static void Clear()
    {
        NameToActionIndex.Clear();
    }
}