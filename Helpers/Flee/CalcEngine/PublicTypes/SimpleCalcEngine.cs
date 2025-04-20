using System;
using System.Collections.Generic;
using ScriptedEventsAPI.Helpers.Flee.CalcEngine.InternalTypes;
using ScriptedEventsAPI.Helpers.Flee.PublicTypes;

namespace ScriptedEventsAPI.Helpers.Flee.CalcEngine.PublicTypes;

public class SimpleCalcEngine
{
    #region "Constructor"

    public SimpleCalcEngine()
    {
        _myExpressions = new Dictionary<string, IExpression>(StringComparer.OrdinalIgnoreCase);
        Context = new ExpressionContext();
    }

    #endregion

    #region "Fields"

    private readonly IDictionary<string, IExpression> _myExpressions;

    #endregion

    #region "Methods - Private"

    private void AddCompiledExpression(string expressionName, IExpression expression)
    {
        if (_myExpressions.ContainsKey(expressionName))
            throw new InvalidOperationException(
                $"The calc engine already contains an expression named '{expressionName}'");

        _myExpressions.Add(expressionName, expression);
    }

    private ExpressionContext ParseAndLink(string expressionName, string expression)
    {
        var analyzer = Context.ParseIdentifiers(expression);

        var context2 = Context.CloneInternal(true);
        LinkExpression(expressionName, context2, analyzer);

        // Tell the expression not to clone the context since it's already been cloned
        context2.NoClone = true;

        // Clear our context's variables
        Context.Variables.Clear();

        return context2;
    }

    private void LinkExpression(string expressionName, ExpressionContext context, IdentifierAnalyzer analyzer)
    {
        foreach (var identifier in analyzer.GetIdentifiers(context))
            LinkIdentifier(identifier, expressionName, context);
    }

    private void LinkIdentifier(string identifier, string expressionName, ExpressionContext context)
    {
        IExpression child = null;

        if (_myExpressions.TryGetValue(identifier, out child) == false)
        {
            var msg = $"Expression '{expressionName}' references unknown name '{identifier}'";
            throw new InvalidOperationException(msg);
        }

        context.Variables.Add(identifier, child);
    }

    #endregion

    #region "Methods - Public"

    public void AddDynamic(string expressionName, string expression)
    {
        var linkedContext = ParseAndLink(expressionName, expression);
        IExpression e = linkedContext.CompileDynamic(expression);
        AddCompiledExpression(expressionName, e);
    }

    public void AddGeneric<T>(string expressionName, string expression)
    {
        var linkedContext = ParseAndLink(expressionName, expression);
        IExpression e = linkedContext.CompileGeneric<T>(expression);
        AddCompiledExpression(expressionName, e);
    }

    public void Clear()
    {
        _myExpressions.Clear();
    }

    #endregion

    #region "Properties - Public"

    public IExpression this[string name]
    {
        get
        {
            IExpression e = null;
            _myExpressions.TryGetValue(name, out e);
            return e;
        }
    }

    public ExpressionContext Context { get; set; }

    #endregion
}