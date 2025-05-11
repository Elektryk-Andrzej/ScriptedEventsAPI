using System;
using System.Collections.Generic;

namespace ScriptedEventsAPI.VariableSystem;

/// <summary>
///     Used when a value cannot be expressed with text, like a list, struct etc.
///     This doesn't include players, as there are player variables.
/// </summary>
public static class ObjectReferenceSystem
{
    private static readonly Dictionary<string, (object value, Type type)> StoredObjects = [];

    public static string RegisterObject(object obj)
    {
        var type = obj.GetType();
        var key = $"[{type.Name} reference | {obj.GetHashCode()}]";
        StoredObjects.Add(key, (obj, type));
        return key;
    }

    public static bool TryRetreiveObject(string key, out object obj)
    {
        obj = null!;

        if (!StoredObjects.TryGetValue(key, out var storedObject)) 
            return false;

        obj = storedObject.value;
        return true;
    }
}