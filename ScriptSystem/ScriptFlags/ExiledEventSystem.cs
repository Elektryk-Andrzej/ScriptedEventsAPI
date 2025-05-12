using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using Exiled.API.Features;
using Exiled.Events.EventArgs.Interfaces;
using Exiled.Events.Features;
using Exiled.Loader;
using SER.MethodSystem.Exceptions;
using SER.VariableSystem;

namespace SER.ScriptSystem.ScriptFlags;

public static class ExiledEventSystem
{
    private static List<List<PropertyInfo>> _events = [];
    private static readonly Dictionary<string, List<string>> EventNameToScriptNameDict = [];

    public static void Initialize()
    {
        try
        {
            _events = Loader.Plugins.First(plug => plug.Name == "Exiled.Events").Assembly.GetTypes()
                .Where(t => t.FullName?.Equals($"Exiled.Events.Handlers.{t.Name}") is true)
                .Select(t => t.GetProperties(BindingFlags.Public | BindingFlags.Static).ToList())
                .ToList();
        }
        catch
        {
            Log.Error($"Fetching HandlerTypes failed! Exiled.Events does not exist in loaded plugins:\n{string.Join(", ", Loader.Plugins.Select(x => x.Name))}");
        }
    }

    private static void OnArgumentedExiledEvent<T>(T ev) where T : IExiledEvent
    {
        var eventName =  new StackFrame(3).GetMethod().Name.Substring(2);
        Log.Info($"Event {eventName} (argumented) triggered. Executing scripts for event.");
        
        if (!EventNameToScriptNameDict.TryGetValue(eventName, out var scriptNames))
        {
            return;
        }
        
        List<Script> scripts = scriptNames
            .Select(Script.CreateByScriptName)
            .Where(res => res.WasSuccess)
            .Select(res => res.Value)
            .ToList()!;
        
        var properties = (
            from prop in ev.GetType().GetProperties() 
            let value = prop.GetValue(ev) 
            where value is not null 
            select new Tuple<object, string>(value, prop.Name)).ToList();
        
        foreach (var (value, nameUppercase) in properties)
        {
            if (nameUppercase is "IsAllowed")
            {
                continue;
            }
            
            var name = nameUppercase.First().ToString().ToLower() + nameUppercase.Substring(1);
            
            switch (value)
            {
                case string:
                case int:
                case float:
                case bool:
                    scripts.ForEach(scr => scr.AddLocalLiteralVariable(new()
                    {
                        Name = name,
                        Value = () => value.ToString()
                    }));
                    continue;
                
                case Player plr:
                    scripts.ForEach(scr => scr.AddLocalPlayerVariable(new()
                    {
                        Name = name,
                        Players = () => [plr]
                    }));
                    continue;
                
                case IEnumerable<Player> plrs:
                    scripts.ForEach(scr => scr.AddLocalPlayerVariable(new()
                    {
                        Name = name,
                        Players = () => plrs.ToList()
                    }));
                    continue;
                
                default:
                    scripts.ForEach(scr => scr.AddLocalLiteralVariable(new()
                    {
                        Name = name,
                        Value = () => $"*{ObjectReferenceSystem.RegisterObject(value)}"
                    }));
                    continue;
            }
        }
        
        scripts.ForEach(scr => scr.Execute());
    }
    
    private static void OnNonArgumentedExiledEvent()
    {
        var eventName = new StackFrame(3).GetMethod().Name.Substring(2);
        Log.Info($"Event {eventName} (non-argumented) triggered. Executing scripts for event.");
        
        if (!EventNameToScriptNameDict.TryGetValue(eventName, out var scriptNames))
        {
            return;
        }

        foreach (var scriptName in scriptNames)
        {
            if (Script.CreateByScriptName(scriptName).HasErrored(out var err, out var script))
            {
                Log.Error($"Script '{scriptName}' failed to execute on event '{eventName}'. {err}");
                continue;
            }
            
            script.Execute();
        }
    }

    public static bool TryBindScriptToExiledEvent(string eventName, string scriptName)
    {
        if (TryBindEvent(eventName) == false)
        {
            return false;
        }

        if (EventNameToScriptNameDict.TryGetValue(eventName, out var list))
        {
            list.Add(scriptName);
        }
        else
        {
            EventNameToScriptNameDict.Add(eventName, [scriptName]);
        }
        
        return true;
    }
    
    private static bool TryBindEvent(string eventName)
    {
        if (EventNameToScriptNameDict.ContainsKey(eventName))
        {
            return true;
        }
        
        // Get the static event field of the correct type
        PropertyInfo? matchingEventPropertyInfo = (
            from lst in _events 
            from prop in lst
            where prop.Name == eventName 
            select prop).FirstOrDefault();
            
        if (matchingEventPropertyInfo == null)
        {
            return false;
        }

        var eventClass = matchingEventPropertyInfo.GetValue(null);
        var eventArgs = eventClass.GetType().GetGenericArguments().FirstOrDefault();
        
        Log.Info($"{eventName} has attribute {eventArgs?.Name ?? "none"}");
        if (eventArgs is null)
        {
            BindNonArgumented(eventClass);
            return true;
        }

        BindArgumented(eventClass, eventArgs);
        return true;
    }

    private static void BindArgumented(object eventClass, Type eventArgs)
    {
        var onArgumentedExiledEventMethodInfo = typeof(ExiledEventSystem).GetMethod(
            nameof(OnArgumentedExiledEvent),
            BindingFlags.Static | BindingFlags.NonPublic);

        if (onArgumentedExiledEventMethodInfo == null)
        {
            throw new DeveloperFuckupException();      
        }
        
        // Create the delegate for the method
        var argumentedEventHandlerDelegate = Delegate.CreateDelegate(
            typeof(CustomEventHandler<>).MakeGenericType(eventArgs),
            null,  // OnArgumentedExiledEvent is static, no instance needed
            onArgumentedExiledEventMethodInfo.MakeGenericMethod(eventArgs));  // Use the method info here

        // Find the Subscribe method on the event type
        var subscribeMethodInfo = eventClass.GetType().GetMethod(
            "Subscribe", 
            [argumentedEventHandlerDelegate.GetType()]);

        if (subscribeMethodInfo == null)
        {
            throw new DeveloperFuckupException();
        }

        // Invoke the Subscribe method to subscribe to the event
        subscribeMethodInfo.Invoke(
            eventClass,
            [argumentedEventHandlerDelegate]);

        Log.Info("Successfully subscribed!");
            
        // Find the Subscribe method on the event type
        var unsubscribeMethodInfo = eventClass.GetType().GetMethod(
            "Unsubscribe", 
            [argumentedEventHandlerDelegate.GetType()]);

        if (unsubscribeMethodInfo == null)
        {
            throw new DeveloperFuckupException();
        }
    }

    private static void BindNonArgumented(object eventClass)
    {
        var onNonArgumentedExiledEventMethodInfo = typeof(ExiledEventSystem).GetMethod(
            nameof(OnNonArgumentedExiledEvent),
            BindingFlags.Static | BindingFlags.NonPublic);

        if (onNonArgumentedExiledEventMethodInfo == null)
        {
            throw new DeveloperFuckupException();
        }
        
        var nonArgumentedEventHandlerDelegate = Delegate.CreateDelegate(
            typeof(CustomEventHandler),
            null,
            onNonArgumentedExiledEventMethodInfo);
        
        var subscribeMethodInfo = eventClass.GetType().GetMethod(
            "Subscribe", 
            [nonArgumentedEventHandlerDelegate.GetType()]);
        
        if (subscribeMethodInfo == null)
        {
            throw new DeveloperFuckupException();
        }

        subscribeMethodInfo.Invoke(
            eventClass,
            [nonArgumentedEventHandlerDelegate]);
    }
}