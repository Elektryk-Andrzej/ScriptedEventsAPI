using System;
using System.IO;
using ScriptedEventsAPI.Helpers.Flee.CalcEngine.InternalTypes;
using ScriptedEventsAPI.Helpers.Flee.CalcEngine.PublicTypes;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;
using ScriptedEventsAPI.Helpers.Flee.Parsing;
using ScriptedEventsAPI.Helpers.Flee.Resources;

namespace ScriptedEventsAPI.Helpers.Flee.PublicTypes;

public sealed class ExpressionContext
{
    #region "Properties - Private"

    private ExpressionParser IdentifierParser
    {
        get
        {
            var parser = _myProperties.GetValue<ExpressionParser>("IdentifierParser");

            if (parser == null)
            {
                var analyzer = new IdentifierAnalyzer();
                parser = new ExpressionParser(TextReader.Null, analyzer, this);
                //parser = new ExpressionParser(System.IO.StringReader.Null, analyzer, this);
                _myProperties.SetValue("IdentifierParser", parser);
            }

            return parser;
        }
    }

    #endregion

    #region "Fields"

    private PropertyDictionary _myProperties;

    private readonly object _mySyncRoot = new();

    #endregion

    #region "Constructor"

    public ExpressionContext() : this(DefaultExpressionOwner.Instance)
    {
    }

    public ExpressionContext(object expressionOwner)
    {
        Utility.AssertNotNull(expressionOwner, "expressionOwner");
        _myProperties = new PropertyDictionary();

        _myProperties.SetValue("CalculationEngine", null);
        _myProperties.SetValue("CalcEngineExpressionName", null);
        _myProperties.SetValue("IdentifierParser", null);

        _myProperties.SetValue("ExpressionOwner", expressionOwner);

        _myProperties.SetValue("ParserOptions", new ExpressionParserOptions(this));

        _myProperties.SetValue("Options", new ExpressionOptions(this));
        _myProperties.SetValue("Imports", new ExpressionImports());
        Imports.SetContext(this);
        Variables = new VariableCollection(this);

        _myProperties.SetToDefault<bool>("NoClone");

        RecreateParser();
    }

    #endregion

    #region "Methods - Private"

    private void AssertTypeIsAccessibleInternal(Type t)
    {
        var isPublic = t.IsPublic;

        if (t.IsNested) isPublic = t.IsNestedPublic;

        var isSameModuleAsOwner = ReferenceEquals(t.Module, ExpressionOwner.GetType().Module);

        // Public types are always accessible.  Otherwise they have to be in the same module as the owner
        var isAccessible = isPublic | isSameModuleAsOwner;

        if (isAccessible == false)
        {
            var msg = Utility.GetGeneralErrorMessage(GeneralErrorResourceKeys.TypeNotAccessibleToExpression, t.Name);
            throw new ArgumentException(msg);
        }
    }

    private void AssertNestedTypeIsAccessible(Type t)
    {
        while (t != null)
        {
            AssertTypeIsAccessibleInternal(t);
            t = t.DeclaringType;
        }
    }

    #endregion

    #region "Methods - Internal"

    internal ExpressionContext CloneInternal(bool cloneVariables)
    {
        var context = (ExpressionContext)MemberwiseClone();
        context._myProperties = _myProperties.Clone();
        context._myProperties.SetValue("Options", context.Options.Clone());
        context._myProperties.SetValue("ParserOptions", context.ParserOptions.Clone());
        context._myProperties.SetValue("Imports", context.Imports.Clone());
        context.Imports.SetContext(context);

        if (cloneVariables)
        {
            context.Variables = new VariableCollection(context);
            Variables.Copy(context.Variables);
        }

        return context;
    }

    internal void AssertTypeIsAccessible(Type t)
    {
        if (t.IsNested)
            AssertNestedTypeIsAccessible(t);
        else
            AssertTypeIsAccessibleInternal(t);
    }

    internal ExpressionElement Parse(string expression, IServiceProvider services)
    {
        lock (_mySyncRoot)
        {
            var sr = new StringReader(expression);
            var parser = Parser;
            parser.Reset(sr);
            parser.Tokenizer.Reset(sr);
            var analyzer = (FleeExpressionAnalyzer)parser.Analyzer;

            analyzer.SetServices(services);

            var rootNode = DoParse();
            analyzer.Reset();
            var topElement = (ExpressionElement)rootNode.Values[0];
            return topElement;
        }
    }

    internal void RecreateParser()
    {
        lock (_mySyncRoot)
        {
            var analyzer = new FleeExpressionAnalyzer();
            var parser = new ExpressionParser(TextReader.Null, analyzer, this);
            _myProperties.SetValue("ExpressionParser", parser);
        }
    }

    internal Node DoParse()
    {
        try
        {
            return Parser.Parse();
        }
        catch (ParserLogException ex)
        {
            // Syntax error; wrap it in our exception and rethrow
            throw new ExpressionCompileException(ex);
        }
    }

    internal void SetCalcEngine(CalculationEngine engine, string calcEngineExpressionName)
    {
        _myProperties.SetValue("CalculationEngine", engine);
        _myProperties.SetValue("CalcEngineExpressionName", calcEngineExpressionName);
    }

    internal IdentifierAnalyzer ParseIdentifiers(string expression)
    {
        var parser = IdentifierParser;
        var sr = new StringReader(expression);
        parser.Reset(sr);
        parser.Tokenizer.Reset(sr);

        var analyzer = (IdentifierAnalyzer)parser.Analyzer;
        analyzer.Reset();

        parser.Parse();

        return (IdentifierAnalyzer)parser.Analyzer;
    }

    #endregion

    #region "Methods - Public"

    public ExpressionContext Clone()
    {
        return CloneInternal(true);
    }

    public IDynamicExpression CompileDynamic(string expression)
    {
        return new Expression<object>(expression, this, false);
    }

    public IGenericExpression<TResultType> CompileGeneric<TResultType>(string expression)
    {
        return new Expression<TResultType>(expression, this, true);
    }

    #endregion

    #region "Properties - Internal"

    internal bool NoClone
    {
        get => _myProperties.GetValue<bool>("NoClone");
        set => _myProperties.SetValue("NoClone", value);
    }

    internal object ExpressionOwner => _myProperties.GetValue<object>("ExpressionOwner");

    internal string CalcEngineExpressionName => _myProperties.GetValue<string>("CalcEngineExpressionName");

    internal ExpressionParser Parser => _myProperties.GetValue<ExpressionParser>("ExpressionParser");

    #endregion

    #region "Properties - Public"

    public ExpressionOptions Options => _myProperties.GetValue<ExpressionOptions>("Options");

    public ExpressionImports Imports => _myProperties.GetValue<ExpressionImports>("Imports");

    public VariableCollection Variables { get; private set; }

    public CalculationEngine CalculationEngine => _myProperties.GetValue<CalculationEngine>("CalculationEngine");

    public ExpressionParserOptions ParserOptions => _myProperties.GetValue<ExpressionParserOptions>("ParserOptions");

    #endregion
}