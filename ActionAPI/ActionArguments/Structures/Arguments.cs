using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ScriptedEventsAPI.ActionAPI.ActionArguments.Structures;

public class Arguments : IList<BaseActionArgument>
{
    public List<BaseActionArgument> ArgumentsProvided { get; set; } = [];

    public T Get<T>(string name) where T : BaseActionArgument
    {
        return ArgumentsProvided.Single(a => a.Name == name) as T 
               ?? throw new Exception();
    }
    
    public bool TryGet<T>(string name, out T arg) where T : BaseActionArgument
    {
        var fetched = ArgumentsProvided.SingleOrDefault(a => a.Name == name) as T;
        arg = fetched!;
        return fetched != null;
    }
    
    public IEnumerator<BaseActionArgument> GetEnumerator()
    {
        return ArgumentsProvided.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Add(BaseActionArgument item)
    {
        ArgumentsProvided.Add(item);
    }

    public void Clear()
    {
        ArgumentsProvided.Clear();
    }

    public bool Contains(BaseActionArgument item)
    {
        return ArgumentsProvided.Contains(item);
    }

    public void CopyTo(BaseActionArgument[] array, int arrayIndex)
    {
        ArgumentsProvided.CopyTo(array, arrayIndex);
    }

    public bool Remove(BaseActionArgument item)
    {
        return ArgumentsProvided.Remove(item);
    }

    public int Count => ArgumentsProvided.Count;
    public bool IsReadOnly => false;
    public int IndexOf(BaseActionArgument item)
    {
        return ArgumentsProvided.IndexOf(item);
    }

    public void Insert(int index, BaseActionArgument item)
    {
        ArgumentsProvided.Insert(index, item);
    }

    public void RemoveAt(int index)
    {
        ArgumentsProvided.RemoveAt(index);
    }

    public BaseActionArgument this[int index]
    {
        get => ArgumentsProvided[index];
        set => ArgumentsProvided[index] = value;
    }
}