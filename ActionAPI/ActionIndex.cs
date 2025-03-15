using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ScriptedEventsAPI.ActionAPI.Actions;
using ScriptedEventsAPI.ActionAPI.Actions.BaseActions;
using ScriptedEventsAPI.Other;

namespace ScriptedEventsAPI.ActionAPI;

// todo: this has to be enabled on plugin start
public static class ActionIndex
{
    public static readonly Dictionary<string, Type> NameToActionIndex = new();

    public static void Initalize()
    {
        NameToActionIndex.Clear();

        var allApiActions = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(BaseAction).IsAssignableFrom(t))
            .Select(t => Activator.CreateInstance(t) as BaseAction);

        foreach (var action in allApiActions)
        {
            if (action is null) continue;
            AddAction(action);
        }
    }

    public static void AddAction(BaseAction action)
    {
        if (NameToActionIndex.ContainsKey(action.Name))
        {
            Log.Debug("action is already registered");
            return;
        }
        
        NameToActionIndex.Add(action.Name, action.GetType());
    }

    public static void Clear()
    {
        NameToActionIndex.Clear();
    }
}