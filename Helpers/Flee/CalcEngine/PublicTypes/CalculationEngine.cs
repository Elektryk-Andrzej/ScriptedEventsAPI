using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ScriptedEventsAPI.Helpers.Flee.CalcEngine.InternalTypes;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;
using ScriptedEventsAPI.Helpers.Flee.PublicTypes;

namespace ScriptedEventsAPI.Helpers.Flee.CalcEngine.PublicTypes;

public class CalculationEngine
{
    #region "Constructor"

    public CalculationEngine()
    {
        _myDependencies = new DependencyManager<ExpressionResultPair>(new PairEqualityComparer());
        _myNameNodeMap = new Dictionary<string, ExpressionResultPair>(StringComparer.OrdinalIgnoreCase);
    }

    #endregion

    #region "Events"

    public event EventHandler<NodeEventArgs> NodeRecalculated;

    #endregion

    #region "Fields"

    private readonly DependencyManager<ExpressionResultPair> _myDependencies;

    /// <summary>
    ///     Map of name to node
    /// </summary>
    private readonly Dictionary<string, ExpressionResultPair> _myNameNodeMap;

    #endregion

    #region "Methods - Private"

    private void AddTemporaryHead(string headName)
    {
        var pair = new GenericExpressionResultPair<int>();
        pair.SetName(headName);

        if (_myNameNodeMap.ContainsKey(headName) == false)
        {
            _myDependencies.AddTail(pair);
            _myNameNodeMap.Add(headName, pair);
        }
        else
        {
            throw new ArgumentException($"An expression already exists at '{headName}'");
        }
    }

    private void DoBatchLoadAdd(BatchLoadInfo info)
    {
        try
        {
            Add(info.Name, info.ExpressionText, info.Context);
        }
        catch (ExpressionCompileException ex)
        {
            Clear();
            throw new BatchLoadCompileException(info.Name, info.ExpressionText, ex);
        }
    }

    private ExpressionResultPair GetTail(string tailName)
    {
        Utility.AssertNotNull(tailName, "name");
        ExpressionResultPair pair = null;
        _myNameNodeMap.TryGetValue(tailName, out pair);
        return pair;
    }

    private ExpressionResultPair GetTailWithValidate(string tailName)
    {
        Utility.AssertNotNull(tailName, "name");
        var pair = GetTail(tailName);

        if (pair == null) throw new ArgumentException($"No expression is associated with the name '{tailName}'");

        return pair;
    }

    private string[] GetNames(IList<ExpressionResultPair> pairs)
    {
        string[] names = new string[pairs.Count];

        for (var i = 0; i <= names.Length - 1; i++) names[i] = pairs[i].Name;

        return names;
    }

    private ExpressionResultPair[] GetRootTails(string[] roots)
    {
        // No roots supplied so get everything
        if (roots.Length == 0) return _myDependencies.GetTails();

        // Get the tail for each name
        ExpressionResultPair[] arr = new ExpressionResultPair[roots.Length];

        for (var i = 0; i <= arr.Length - 1; i++) arr[i] = GetTailWithValidate(roots[i]);

        return arr;
    }

    #endregion

    #region "Methods - Internal"

    internal void FixTemporaryHead(IDynamicExpression expression, ExpressionContext context, Type resultType)
    {
        var pairType = typeof(GenericExpressionResultPair<>);
        pairType = pairType.MakeGenericType(resultType);

        var pair = (ExpressionResultPair)Activator.CreateInstance(pairType);
        var headName = context.CalcEngineExpressionName;
        pair.SetName(headName);
        pair.SetExpression(expression);

        var oldPair = _myNameNodeMap[headName];
        _myDependencies.ReplaceDependency(oldPair, pair);
        _myNameNodeMap[headName] = pair;

        // Let the pair store the result of its expression
        pair.Recalculate();
    }

    /// <summary>
    ///     Called by an expression when it references another expression in the engine
    /// </summary>
    /// <param name="tailName"></param>
    /// <param name="context"></param>
    internal void AddDependency(string tailName, ExpressionContext context)
    {
        var actualTail = GetTail(tailName);
        var headName = context.CalcEngineExpressionName;
        var actualHead = GetTail(headName);

        // An expression could depend on the same reference more than once (ie: "a + a * a")
        _myDependencies.AddDepedency(actualTail, actualHead);
    }

    internal Type ResolveTailType(string tailName)
    {
        var actualTail = GetTail(tailName);
        return actualTail.ResultType;
    }

    internal bool HasTail(string tailName)
    {
        return _myNameNodeMap.ContainsKey(tailName);
    }

    internal void EmitLoad(string tailName, FleeILGenerator ilg)
    {
        var pi = typeof(ExpressionContext).GetProperty("CalculationEngine");
        ilg.Emit(OpCodes.Callvirt, pi.GetGetMethod());

        // Load the tail
        MemberInfo[] methods = typeof(CalculationEngine).FindMembers(MemberTypes.Method,
            BindingFlags.Instance | BindingFlags.Public, Type.FilterNameIgnoreCase, "GetResult");
        MethodInfo mi = null;

        foreach (MethodInfo method in methods)
            if (method.IsGenericMethod)
            {
                mi = method;
                break; // TODO: might not be correct. Was : Exit For
            }

        var resultType = ResolveTailType(tailName);

        mi = mi.MakeGenericMethod(resultType);

        ilg.Emit(OpCodes.Ldstr, tailName);
        ilg.Emit(OpCodes.Call, mi);
    }

    #endregion

    #region "Methods - Public"

    public void Add(string atomName, string expression, ExpressionContext context)
    {
        Utility.AssertNotNull(atomName, "atomName");
        Utility.AssertNotNull(expression, "expression");
        Utility.AssertNotNull(context, "context");

        AddTemporaryHead(atomName);

        context.SetCalcEngine(this, atomName);

        context.CompileDynamic(expression);
    }

    public bool Remove(string name)
    {
        var tail = GetTail(name);

        if (tail == null) return false;

        ExpressionResultPair[] dependents = _myDependencies.GetDependents(tail);
        _myDependencies.Remove(dependents);

        foreach (var pair in dependents) _myNameNodeMap.Remove(pair.Name);

        return true;
    }

    public BatchLoader CreateBatchLoader()
    {
        var loader = new BatchLoader();
        return loader;
    }

    public void BatchLoad(BatchLoader loader)
    {
        Utility.AssertNotNull(loader, "loader");
        Clear();

        BatchLoadInfo[] infos = loader.GetBachInfos();

        foreach (var info in infos) DoBatchLoadAdd(info);
    }

    public T GetResult<T>(string name)
    {
        var tail = GetTailWithValidate(name);

        if (!ReferenceEquals(typeof(T), tail.ResultType))
        {
            var msg =
                $"The result type of '{name}' ('{tail.ResultType.Name}') does not match the supplied type argument ('{typeof(T).Name}')";
            throw new ArgumentException(msg);
        }

        var actualTail = (GenericExpressionResultPair<T>)tail;
        return actualTail.Result;
    }

    public object GetResult(string name)
    {
        var tail = GetTailWithValidate(name);
        return tail.ResultAsObject;
    }

    public IExpression GetExpression(string name)
    {
        var tail = GetTailWithValidate(name);
        return tail.Expression;
    }

    public string[] GetDependents(string name)
    {
        var pair = GetTail(name);
        List<ExpressionResultPair> dependents = new List<ExpressionResultPair>();

        if (pair != null) _myDependencies.GetDirectDependents(pair, dependents);

        return GetNames(dependents);
    }

    public string[] GetPrecedents(string name)
    {
        var pair = GetTail(name);
        List<ExpressionResultPair> dependents = new List<ExpressionResultPair>();

        if (pair != null) _myDependencies.GetDirectPrecedents(pair, dependents);

        return GetNames(dependents);
    }

    public bool HasDependents(string name)
    {
        var pair = GetTail(name);
        return pair != null && _myDependencies.HasDependents(pair);
    }

    public bool HasPrecedents(string name)
    {
        var pair = GetTail(name);
        return pair != null && _myDependencies.HasPrecedents(pair);
    }

    public bool Contains(string name)
    {
        Utility.AssertNotNull(name, "name");
        return _myNameNodeMap.ContainsKey(name);
    }

    public void Recalculate(params string[] roots)
    {
        // Get the tails corresponding to the names
        ExpressionResultPair[] rootTails = GetRootTails(roots);
        // Create a dependency list based on the tails
        DependencyManager<ExpressionResultPair> tempDependents = _myDependencies.CloneDependents(rootTails);
        // Get the sources (ie: nodes with no incoming edges) since that's what the sort requires
        Queue<ExpressionResultPair> sources = tempDependents.GetSources(rootTails);
        // Do the topological sort
        IList<ExpressionResultPair> calcList = tempDependents.TopologicalSort(sources);

        var args = new NodeEventArgs();

        // Recalculate the sorted expressions
        foreach (var pair in calcList)
        {
            pair.Recalculate();
            args.SetData(pair.Name, pair.ResultAsObject);
            if (NodeRecalculated != null) NodeRecalculated(this, args);
        }
    }

    public void Clear()
    {
        _myDependencies.Clear();
        _myNameNodeMap.Clear();
    }

    #endregion

    #region "Properties - Public"

    public int Count => _myDependencies.Count;

    public string DependencyGraph => _myDependencies.DependencyGraph;

    #endregion
}