using System;
using ScriptedEventsAPI.Helpers.Flee.PublicTypes;

namespace ScriptedEventsAPI.Helpers.Flee.InternalTypes;

internal interface IVariable
{
    Type VariableType { get; }
    object ValueAsObject { get; set; }
    IVariable Clone();
}

internal interface IGenericVariable<T>
{
    object GetValue();
}

internal class DynamicExpressionVariable<T> : IVariable, IGenericVariable<T>
{
    private IDynamicExpression _myExpression;

    public object GetValue()
    {
        return (T)_myExpression.Evaluate();
    }

    public IVariable Clone()
    {
        DynamicExpressionVariable<T> copy = new DynamicExpressionVariable<T>();
        copy._myExpression = _myExpression;
        return copy;
    }

    public object ValueAsObject
    {
        get => _myExpression;
        set => _myExpression = value as IDynamicExpression;
    }

    public Type VariableType => _myExpression.Context.Options.ResultType;
}

internal class GenericExpressionVariable<T> : IVariable, IGenericVariable<T>
{
    private IGenericExpression<T> _myExpression;

    public object GetValue()
    {
        return _myExpression.Evaluate();
    }

    public IVariable Clone()
    {
        GenericExpressionVariable<T> copy = new GenericExpressionVariable<T>();
        copy._myExpression = _myExpression;
        return copy;
    }

    public object ValueAsObject
    {
        get => _myExpression;
        set => _myExpression = (IGenericExpression<T>)value;
    }

    public Type VariableType => _myExpression.Context.Options.ResultType;
}

internal class GenericVariable<T> : IVariable, IGenericVariable<T>
{
    public object MyValue;

    public object GetValue()
    {
        return MyValue;
    }

    public IVariable Clone()
    {
        GenericVariable<T> copy = new GenericVariable<T> { MyValue = MyValue };
        return copy;
    }

    public Type VariableType => typeof(T);

    public object ValueAsObject
    {
        get => MyValue;
        set
        {
            if (value == null)
                MyValue = default(T);
            else
                MyValue = value;
        }
    }
}