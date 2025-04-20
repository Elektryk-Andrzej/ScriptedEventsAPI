using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;
using ScriptedEventsAPI.Helpers.Flee.Resources;

namespace ScriptedEventsAPI.Helpers.Flee.PublicTypes;

/// <summary>
/// </summary>
public sealed class VariableCollection : IDictionary<string, object>
{
    private readonly ExpressionContext _myContext;
    private IDictionary<string, IVariable> _myVariables;

    internal VariableCollection(ExpressionContext context)
    {
        _myContext = context;
        CreateDictionary();
        HookOptions();
    }

    public event EventHandler<ResolveVariableTypeEventArgs> ResolveVariableType;

    public event EventHandler<ResolveVariableValueEventArgs> ResolveVariableValue;

    public event EventHandler<ResolveFunctionEventArgs> ResolveFunction;

    public event EventHandler<InvokeFunctionEventArgs> InvokeFunction;

    #region "Methods - Non Public"

    private void HookOptions()
    {
        _myContext.Options.CaseSensitiveChanged += OnOptionsCaseSensitiveChanged;
    }

    private void CreateDictionary()
    {
        _myVariables = new Dictionary<string, IVariable>(_myContext.Options.StringComparer);
    }

    private void OnOptionsCaseSensitiveChanged(object sender, EventArgs e)
    {
        CreateDictionary();
    }

    internal void Copy(VariableCollection dest)
    {
        dest.CreateDictionary();
        dest.HookOptions();

        foreach (KeyValuePair<string, IVariable> pair in _myVariables)
        {
            var copyVariable = pair.Value.Clone();
            dest._myVariables.Add(pair.Key, copyVariable);
        }
    }

    internal void DefineVariableInternal(string name, Type variableType, object variableValue)
    {
        Utility.AssertNotNull(variableType, "variableType");

        if (_myVariables.ContainsKey(name))
        {
            var msg = Utility.GetGeneralErrorMessage(GeneralErrorResourceKeys.VariableWithNameAlreadyDefined, name);
            throw new ArgumentException(msg);
        }

        var v = CreateVariable(variableType, variableValue);
        _myVariables.Add(name, v);
    }

    internal Type GetVariableTypeInternal(string name)
    {
        IVariable value = null;
        var success = _myVariables.TryGetValue(name, out value);

        if (success) return value.VariableType;

        var args = new ResolveVariableTypeEventArgs(name);
        ResolveVariableType?.Invoke(this, args);

        return args.VariableType;
    }

    private IVariable GetVariable(string name, bool throwOnNotFound)
    {
        IVariable value = null;
        var success = _myVariables.TryGetValue(name, out value);

        if ((success == false) & throwOnNotFound)
        {
            var msg = Utility.GetGeneralErrorMessage(GeneralErrorResourceKeys.UndefinedVariable, name);
            throw new ArgumentException(msg);
        }

        return value;
    }

    private IVariable CreateVariable(Type variableValueType, object variableValue)
    {
        var variableType = default(Type);

        // Is the variable value an expression?
        var expression = variableValue as IExpression;
        ExpressionOptions options = null;

        if (expression != null)
        {
            options = expression.Context.Options;
            // Get its result type
            variableValueType = options.ResultType;

            // Create a variable that wraps the expression

            if (options.IsGeneric == false)
                variableType = typeof(DynamicExpressionVariable<>);
            else
                variableType = typeof(GenericExpressionVariable<>);
        }
        else
        {
            // Create a variable for a regular value
            _myContext.AssertTypeIsAccessible(variableValueType);
            variableType = typeof(GenericVariable<>);
        }

        // Create the generic variable instance
        variableType = variableType.MakeGenericType(variableValueType);
        var v = (IVariable)Activator.CreateInstance(variableType);

        return v;
    }

    internal Type ResolveOnDemandFunction(string name, Type[] argumentTypes)
    {
        var args = new ResolveFunctionEventArgs(name, argumentTypes);
        ResolveFunction?.Invoke(this, args);
        return args.ReturnType;
    }

    private static T ReturnGenericValue<T>(object value)
    {
        if (value == null) return default;

        return (T)value;
    }

    private static void ValidateSetValueType(Type requiredType, object value)
    {
        if (value == null)
            // Can always assign null value
            return;

        var valueType = value.GetType();

        if (requiredType.IsAssignableFrom(valueType) == false)
        {
            var msg = Utility.GetGeneralErrorMessage(GeneralErrorResourceKeys.VariableValueNotAssignableToType,
                valueType.Name, requiredType.Name);
            throw new ArgumentException(msg);
        }
    }

    internal static MethodInfo GetVariableLoadMethod(Type variableType)
    {
        var mi = typeof(VariableCollection).GetMethod("GetVariableValueInternal",
            BindingFlags.Public | BindingFlags.Instance);
        mi = mi.MakeGenericMethod(variableType);
        return mi;
    }

    internal static MethodInfo GetFunctionInvokeMethod(Type returnType)
    {
        var mi = typeof(VariableCollection).GetMethod("GetFunctionResultInternal",
            BindingFlags.Public | BindingFlags.Instance);
        mi = mi.MakeGenericMethod(returnType);
        return mi;
    }

    internal static MethodInfo GetVirtualPropertyLoadMethod(Type returnType)
    {
        var mi = typeof(VariableCollection).GetMethod("GetVirtualPropertyValueInternal",
            BindingFlags.Public | BindingFlags.Instance);
        mi = mi.MakeGenericMethod(returnType);
        return mi;
    }

    private Dictionary<string, object> GetNameValueDictionary()
    {
        Dictionary<string, object> dict = new Dictionary<string, object>();

        foreach (KeyValuePair<string, IVariable> pair in _myVariables) dict.Add(pair.Key, pair.Value.ValueAsObject);

        return dict;
    }

    #endregion "Methods - Non Public"

    #region "Methods - Public"

    public Type GetVariableType(string name)
    {
        var v = GetVariable(name, true);
        return v.VariableType;
    }

    public void DefineVariable(string name, Type variableType)
    {
        DefineVariableInternal(name, variableType, null);
    }

    public T GetVariableValueInternal<T>(string name)
    {
        if (_myVariables.TryGetValue(name, out var variable))
            if (variable is IGenericVariable<T> generic)
                return (T)generic.GetValue();

        GenericVariable<T> result = new GenericVariable<T>();
        var vTemp = new GenericVariable<T>();
        var args = new ResolveVariableValueEventArgs(name, typeof(T));
        ResolveVariableValue?.Invoke(this, args);

        ValidateSetValueType(typeof(T), args.VariableValue);
        vTemp.ValueAsObject = args.VariableValue;
        result = vTemp;
        return (T)result.GetValue();
    }

    public T GetVirtualPropertyValueInternal<T>(string name, object component)
    {
        var coll = TypeDescriptor.GetProperties(component);
        var pd = coll.Find(name, true);

        var value = pd.GetValue(component);
        ValidateSetValueType(typeof(T), value);
        return ReturnGenericValue<T>(value);
    }

    public T GetFunctionResultInternal<T>(string name, object[] arguments)
    {
        var args = new InvokeFunctionEventArgs(name, arguments);
        if (InvokeFunction != null) InvokeFunction(this, args);

        var result = args.Result;
        ValidateSetValueType(typeof(T), result);

        return ReturnGenericValue<T>(result);
    }

    #endregion "Methods - Public"

    #region "IDictionary Implementation"

    private void Add1(KeyValuePair<string, object> item)
    {
        Add(item.Key, item.Value);
    }

    void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
    {
        Add1(item);
    }

    public void Clear()
    {
        _myVariables.Clear();
    }

    private bool Contains1(KeyValuePair<string, object> item)
    {
        return ContainsKey(item.Key);
    }

    bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
    {
        return Contains1(item);
    }

    private void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
    {
        Dictionary<string, object> dict = GetNameValueDictionary();
        ICollection<KeyValuePair<string, object>> coll = dict;
        coll.CopyTo(array, arrayIndex);
    }

    private bool Remove1(KeyValuePair<string, object> item)
    {
        return Remove(item.Key);
    }

    bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
    {
        return Remove1(item);
    }

    public void Add(string name, object value)
    {
        Utility.AssertNotNull(value, "value");
        DefineVariableInternal(name, value.GetType(), value);
        this[name] = value;
    }

    public bool ContainsKey(string name)
    {
        return _myVariables.ContainsKey(name);
    }

    public bool Remove(string name)
    {
        return _myVariables.Remove(name);
    }

    public bool TryGetValue(string key, out object value)
    {
        var v = GetVariable(key, false);
        value = v?.ValueAsObject;
        return v != null;
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        Dictionary<string, object> dict = GetNameValueDictionary();
        return dict.GetEnumerator();
    }

    private IEnumerator GetEnumerator1()
    {
        return GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator1();
    }

    public int Count => _myVariables.Count;

    public bool IsReadOnly => false;

    public object this[string name]
    {
        get
        {
            var v = GetVariable(name, true);
            return v.ValueAsObject;
        }
        set
        {
            IVariable v = null;

            if (_myVariables.TryGetValue(name, out v))
                v.ValueAsObject = value;
            else
                Add(name, value);
        }
    }

    public ICollection<string> Keys => _myVariables.Keys;

    public ICollection<object> Values
    {
        get
        {
            Dictionary<string, object> dict = GetNameValueDictionary();
            return dict.Values;
        }
    }

    void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
    {
        CopyTo(array, arrayIndex);
    }

    #endregion "IDictionary Implementation"
}