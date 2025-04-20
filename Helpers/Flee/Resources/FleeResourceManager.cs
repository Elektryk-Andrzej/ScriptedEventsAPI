using System;
using System.Collections.Generic;
using System.Resources;

namespace ScriptedEventsAPI.Helpers.Flee.Resources;

internal class FleeResourceManager
{
    private readonly Dictionary<string, ResourceManager> MyResourceManagers;

    private FleeResourceManager()
    {
        MyResourceManagers = new Dictionary<string, ResourceManager>(StringComparer.OrdinalIgnoreCase);
    }

    public static FleeResourceManager Instance { get; } = new();

    private ResourceManager GetResourceManager(string resourceFile)
    {
        lock (this)
        {
            ResourceManager rm = null;
            if (MyResourceManagers.TryGetValue(resourceFile, out rm) == false)
            {
                var t = typeof(FleeResourceManager);
                rm = new ResourceManager(string.Format("{0}.{1}", t.Namespace, resourceFile), t.Assembly);
                MyResourceManagers.Add(resourceFile, rm);
            }

            return rm;
        }
    }

    private string GetResourceString(string resourceFile, string key)
    {
        var rm = GetResourceManager(resourceFile);
        return rm.GetString(key);
    }

    public string GetCompileErrorString(string key)
    {
        return GetResourceString("CompileErrors", key);
    }

    public string GetElementNameString(string key)
    {
        return GetResourceString("ElementNames", key);
    }

    public string GetGeneralErrorString(string key)
    {
        return GetResourceString("GeneralErrors", key);
    }
}