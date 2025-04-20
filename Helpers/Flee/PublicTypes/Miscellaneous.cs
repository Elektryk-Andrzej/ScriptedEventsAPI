using System;
using System.Collections.Generic;

namespace ScriptedEventsAPI.Helpers.Flee.PublicTypes;

public interface IExpression
{
    string Text { get; }
    ExpressionInfo Info { get; }
    ExpressionContext Context { get; }
    object Owner { get; set; }
    IExpression Clone();
}

public interface IDynamicExpression : IExpression
{
    object Evaluate();
}

public interface IGenericExpression<T> : IExpression
{
    T Evaluate();
}

public sealed class ExpressionInfo
{
    private readonly IDictionary<string, object> _myData;

    internal ExpressionInfo()
    {
        _myData = new Dictionary<string, object>
        {
            { "ReferencedVariables", new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase) }
        };
    }

    internal void AddReferencedVariable(string name)
    {
        IDictionary<string, string> dict = (IDictionary<string, string>)_myData["ReferencedVariables"];
        dict[name] = name;
    }

    public string[] GetReferencedVariables()
    {
        IDictionary<string, string> dict = (IDictionary<string, string>)_myData["ReferencedVariables"];
        string[] arr = new string[dict.Count];
        dict.Keys.CopyTo(arr, 0);
        return arr;
    }
}

[AttributeUsage(AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property, Inherited = false)]
public sealed class ExpressionOwnerMemberAccessAttribute : Attribute
{
    public ExpressionOwnerMemberAccessAttribute(bool allowAccess)
    {
        AllowAccess = allowAccess;
    }

    internal bool AllowAccess { get; }
}

public class ResolveVariableTypeEventArgs : EventArgs
{
    internal ResolveVariableTypeEventArgs(string name)
    {
        VariableName = name;
    }

    public string VariableName { get; }

    public Type VariableType { get; set; }
}

public class ResolveVariableValueEventArgs : EventArgs
{
    internal ResolveVariableValueEventArgs(string name, Type t)
    {
        VariableName = name;
        VariableType = t;
    }

    public string VariableName { get; }

    public Type VariableType { get; }

    public object VariableValue { get; set; }
}

public class ResolveFunctionEventArgs : EventArgs
{
    internal ResolveFunctionEventArgs(string name, Type[] argumentTypes)
    {
        FunctionName = name;
        ArgumentTypes = argumentTypes;
    }

    public string FunctionName { get; }

    public Type[] ArgumentTypes { get; }

    public Type ReturnType { get; set; }
}

public class InvokeFunctionEventArgs : EventArgs
{
    internal InvokeFunctionEventArgs(string name, object[] arguments)
    {
        FunctionName = name;
        Arguments = arguments;
    }

    public string FunctionName { get; }

    public object[] Arguments { get; }

    public object Result { get; set; }
}

public enum RealLiteralDataType
{
    Single,
    Double,
    Decimal
}