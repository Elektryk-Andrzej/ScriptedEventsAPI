using System;
using System.ComponentModel.Design;
using System.Reflection;
using System.Reflection.Emit;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base;
using ScriptedEventsAPI.Helpers.Flee.PublicTypes;
using ScriptedEventsAPI.Helpers.Flee.Resources;
using PublicTypes_IDynamicExpression = ScriptedEventsAPI.Helpers.Flee.PublicTypes.IDynamicExpression;

namespace ScriptedEventsAPI.Helpers.Flee.InternalTypes;

internal class Expression<T> : IExpression, PublicTypes_IDynamicExpression, IGenericExpression<T>
{
    private const string EmitAssemblyName = "FleeExpression";

    private const string DynamicMethodName = "Flee Expression";
    private ExpressionEvaluator<T> _myEvaluator;
    private ExpressionOptions _myOptions;

    private object _myOwner;

    public Expression(string expression, ExpressionContext context, bool isGeneric)
    {
        Utility.AssertNotNull(expression, "expression");
        Text = expression;
        _myOwner = context.ExpressionOwner;

        Context = context;

        if (context.NoClone == false) Context = context.CloneInternal(false);

        Info1 = new ExpressionInfo();

        SetupOptions(Context.Options, isGeneric);

        Context.Imports.ImportOwner(_myOptions.OwnerType);

        ValidateOwner(_myOwner);

        Compile(expression, _myOptions);

        Context.CalculationEngine?.FixTemporaryHead(this, Context, _myOptions.ResultType);
    }

    internal Type ResultType => _myOptions.ResultType;

    public ExpressionInfo Info1 { get; }

    public IExpression Clone()
    {
        Expression<T> copy = (Expression<T>)MemberwiseClone();
        copy.Context = Context.CloneInternal(true);
        copy._myOptions = copy.Context.Options;
        return copy;
    }

    public string Text { get; }

    ExpressionInfo IExpression.Info => Info1;

    public object Owner
    {
        get => _myOwner;
        set
        {
            ValidateOwner(value);
            _myOwner = value;
        }
    }

    public ExpressionContext Context { get; private set; }

    T IGenericExpression<T>.Evaluate()
    {
        return EvaluateGeneric();
    }

    public object Evaluate()
    {
        return _myEvaluator(_myOwner, Context, Context.Variables);
    }

    private void SetupOptions(ExpressionOptions options, bool isGeneric)
    {
        // Make sure we clone the options
        _myOptions = options;
        _myOptions.IsGeneric = isGeneric;

        if (isGeneric) _myOptions.ResultType = typeof(T);

        _myOptions.SetOwnerType(_myOwner.GetType());
    }

    private void Compile(string expression, ExpressionOptions options)
    {
        // Add the services that will be used by elements during the compile
        IServiceContainer services = new ServiceContainer();
        AddServices(services);

        // Parse and get the root element of the parse tree
        var topElement = Context.Parse(expression, services);

        if (options.ResultType == null) options.ResultType = topElement.ResultType;

        var rootElement = new RootExpressionElement(topElement, options.ResultType);

        var dm = CreateDynamicMethod();

        var ilg = new FleeILGenerator(dm.GetILGenerator());

        // Emit the IL
        rootElement.Emit(ilg, services);
        if (ilg.NeedsSecondPass())
        {
            // second pass required due to long branches.
            dm = CreateDynamicMethod();
            ilg.PrepareSecondPass(dm.GetILGenerator());
            rootElement.Emit(ilg, services);
        }

        ilg.ValidateLength();

        // Emit to an assembly if required
        if (options.EmitToAssembly) EmitToAssembly(ilg, rootElement, services);

        var delegateType = typeof(ExpressionEvaluator<>).MakeGenericType(typeof(T));
        _myEvaluator = (ExpressionEvaluator<T>)dm.CreateDelegate(delegateType);
    }

    private DynamicMethod CreateDynamicMethod()
    {
        // Create the dynamic method
        Type[] parameterTypes =
        {
            typeof(object),
            typeof(ExpressionContext),
            typeof(VariableCollection)
        };
        var dm = default(DynamicMethod);

        dm = new DynamicMethod(DynamicMethodName, typeof(T), parameterTypes, _myOptions.OwnerType);

        return dm;
    }

    private void AddServices(IServiceContainer dest)
    {
        dest.AddService(typeof(ExpressionOptions), _myOptions);
        dest.AddService(typeof(ExpressionParserOptions), Context.ParserOptions);
        dest.AddService(typeof(ExpressionContext), Context);
        dest.AddService(typeof(IExpression), this);
        dest.AddService(typeof(ExpressionInfo), Info1);
    }

    /// <summary>
    ///     Emit to an assembly. We've already computed long branches at this point,
    ///     so we emit as a second pass
    /// </summary>
    /// <param name="ilg"></param>
    /// <param name="rootElement"></param>
    /// <param name="services"></param>
    private static void EmitToAssembly(FleeILGenerator ilg, ExpressionElement rootElement, IServiceContainer services)
    {
        var assemblyName = new AssemblyName(EmitAssemblyName);

        var assemblyFileName = string.Format("{0}.dll", EmitAssemblyName);

        var assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        var moduleBuilder = assemblyBuilder.DefineDynamicModule(assemblyFileName);

        var mb = moduleBuilder.DefineGlobalMethod("Evaluate", MethodAttributes.Public | MethodAttributes.Static,
            typeof(T), new[]
            {
                typeof(object), typeof(ExpressionContext), typeof(VariableCollection)
            });
        // already emitted once for local use,
        ilg.PrepareSecondPass(mb.GetILGenerator());

        rootElement.Emit(ilg, services);

        moduleBuilder.CreateGlobalFunctions();
        //assemblyBuilder.Save(assemblyFileName);
        assemblyBuilder.CreateInstance(assemblyFileName);
    }

    private void ValidateOwner(object owner)
    {
        Utility.AssertNotNull(owner, "owner");
        if (_myOptions.OwnerType.IsAssignableFrom(owner.GetType()) == false)
        {
            var msg = Utility.GetGeneralErrorMessage(GeneralErrorResourceKeys.NewOwnerTypeNotAssignableToCurrentOwner);
            throw new ArgumentException(msg);
        }
    }

    public T EvaluateGeneric()
    {
        return _myEvaluator(_myOwner, Context, Context.Variables);
    }

    public override string ToString()
    {
        return Text;
    }
}