using System;
using System.Collections.Generic;
using ScriptedEventsAPI.Helpers.Flee.CalcEngine.InternalTypes;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;
using ScriptedEventsAPI.Helpers.Flee.PublicTypes;

namespace ScriptedEventsAPI.Helpers.Flee.CalcEngine.PublicTypes;

public sealed class BatchLoader
{
    private readonly DependencyManager<string> _myDependencies;

    private readonly IDictionary<string, BatchLoadInfo> _myNameInfoMap;

    internal BatchLoader()
    {
        _myNameInfoMap = new Dictionary<string, BatchLoadInfo>(StringComparer.OrdinalIgnoreCase);
        _myDependencies = new DependencyManager<string>(StringComparer.OrdinalIgnoreCase);
    }

    public void Add(string atomName, string expression, ExpressionContext context)
    {
        Utility.AssertNotNull(atomName, "atomName");
        Utility.AssertNotNull(expression, "expression");
        Utility.AssertNotNull(context, "context");

        var info = new BatchLoadInfo(atomName, expression, context);
        _myNameInfoMap.Add(atomName, info);
        _myDependencies.AddTail(atomName);

        var references = GetReferences(expression, context);

        foreach (var reference in references)
        {
            _myDependencies.AddTail(reference);
            _myDependencies.AddDepedency(reference, atomName);
        }
    }

    public bool Contains(string atomName)
    {
        return _myNameInfoMap.ContainsKey(atomName);
    }

    internal BatchLoadInfo[] GetBachInfos()
    {
        string[] tails = _myDependencies.GetTails();
        Queue<string> sources = _myDependencies.GetSources(tails);

        IList<string> result = _myDependencies.TopologicalSort(sources);

        BatchLoadInfo[] infos = new BatchLoadInfo[result.Count];

        for (var i = 0; i <= result.Count - 1; i++) infos[i] = _myNameInfoMap[result[i]];

        return infos;
    }

    private ICollection<string> GetReferences(string expression, ExpressionContext context)
    {
        var analyzer = context.ParseIdentifiers(expression);

        return analyzer.GetIdentifiers(context);
    }
}