using System;
using System.Collections;
using System.Collections.Generic;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;

namespace ScriptedEventsAPI.Helpers.Flee.ExpressionElements.MemberElements;

[Obsolete("Encapsulates an argument list")]
internal class ArgumentList
{
    private readonly IList<ExpressionElement> _myElements;

    public ArgumentList(ICollection elements)
    {
        ExpressionElement[] arr = new ExpressionElement[elements.Count];
        elements.CopyTo(arr, 0);
        _myElements = arr;
    }

    public ExpressionElement this[int index] => _myElements[index];

    public int Count => _myElements.Count;

    private string[] GetArgumentTypeNames()
    {
        List<string> l = new List<string>();

        foreach (var e in _myElements) l.Add(e.ResultType.Name);

        return l.ToArray();
    }

    public Type[] GetArgumentTypes()
    {
        List<Type> l = new List<Type>();

        foreach (var e in _myElements) l.Add(e.ResultType);

        return l.ToArray();
    }

    public override string ToString()
    {
        string[] typeNames = GetArgumentTypeNames();
        return Utility.FormatList(typeNames);
    }

    public ExpressionElement[] ToArray()
    {
        ExpressionElement[] arr = new ExpressionElement[_myElements.Count];
        _myElements.CopyTo(arr, 0);
        return arr;
    }
}