using System;
using System.Collections.Generic;
using ScriptedEventsAPI.Helpers.Flee.PublicTypes;

namespace ScriptedEventsAPI.Helpers.Flee.CalcEngine.InternalTypes;

internal class PairEqualityComparer : EqualityComparer<ExpressionResultPair>
{
    public override bool Equals(ExpressionResultPair x, ExpressionResultPair y)
    {
        return string.Equals(x.Name, y.Name, StringComparison.OrdinalIgnoreCase);
    }

    public override int GetHashCode(ExpressionResultPair obj)
    {
        return StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Name);
    }
}

internal abstract class ExpressionResultPair
{
    protected IDynamicExpression MyExpression;

    public string Name { get; private set; }

    public abstract Type ResultType { get; }
    public abstract object ResultAsObject { get; set; }

    public IDynamicExpression Expression => MyExpression;

    public abstract void Recalculate();

    public void SetExpression(IDynamicExpression e)
    {
        MyExpression = e;
    }

    public void SetName(string name)
    {
        Name = name;
    }

    public override string ToString()
    {
        return Name;
    }
}

internal class GenericExpressionResultPair<T> : ExpressionResultPair
{
    public T MyResult;

    public T Result => MyResult;

    public override Type ResultType => typeof(T);

    public override object ResultAsObject
    {
        get => MyResult;
        set => MyResult = (T)value;
    }

    public override void Recalculate()
    {
        MyResult = (T)MyExpression.Evaluate();
    }
}

internal class BatchLoadInfo
{
    public ExpressionContext Context;
    public string ExpressionText;
    public string Name;

    public BatchLoadInfo(string name, string text, ExpressionContext context)
    {
        Name = name;
        ExpressionText = text;
        Context = context;
    }
}

public sealed class NodeEventArgs : EventArgs
{
    internal NodeEventArgs()
    {
    }

    public string Name { get; private set; }

    public object Result { get; private set; }

    internal void SetData(string name, object result)
    {
        Name = name;
        Result = result;
    }
}