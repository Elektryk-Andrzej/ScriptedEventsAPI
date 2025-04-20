using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;
using ScriptedEventsAPI.Helpers.Flee.PublicTypes;
using ScriptedEventsAPI.Helpers.Flee.Resources;

namespace ScriptedEventsAPI.Helpers.Flee.ExpressionElements;

internal class InElement : ExpressionElement
{
    // Elements we will compare against
    private readonly List<ExpressionElement> MyArguments;

    // Element we will search for
    private readonly ExpressionElement MyOperand;

    // Collection to look in
    private readonly ExpressionElement MyTargetCollectionElement;
    // Type of the collection

    private Type MyTargetCollectionType;

    // Initialize for searching a list of values
    public InElement(ExpressionElement operand, IList listElements)
    {
        MyOperand = operand;

        ExpressionElement[] arr = new ExpressionElement[listElements.Count];
        listElements.CopyTo(arr, 0);

        MyArguments = new List<ExpressionElement>(arr);
        ResolveForListSearch();
    }

    // Initialize for searching a collection
    public InElement(ExpressionElement operand, ExpressionElement targetCollection)
    {
        MyOperand = operand;
        MyTargetCollectionElement = targetCollection;
        ResolveForCollectionSearch();
    }

    public override Type ResultType => typeof(bool);

    private void ResolveForListSearch()
    {
        var ce = new CompareElement();

        // Validate that our operand is comparable to all elements in the list
        foreach (var argumentElement in MyArguments)
        {
            ce.Initialize(MyOperand, argumentElement, LogicalCompareOperation.Equal);
            ce.Validate();
        }
    }

    private void ResolveForCollectionSearch()
    {
        // Try to find a collection type
        MyTargetCollectionType = GetTargetCollectionType();

        if (MyTargetCollectionType == null)
            ThrowCompileException(CompileErrorResourceKeys.SearchArgIsNotKnownCollectionType,
                CompileExceptionReason.TypeMismatch, MyTargetCollectionElement.ResultType.Name);

        // Validate that the operand type is compatible with the collection
        var mi = GetCollectionContainsMethod();
        var p1 = mi.GetParameters()[0];

        if (ImplicitConverter.EmitImplicitConvert(MyOperand.ResultType, p1.ParameterType, null) == false)
            ThrowCompileException(CompileErrorResourceKeys.OperandNotConvertibleToCollectionType,
                CompileExceptionReason.TypeMismatch, MyOperand.ResultType.Name, p1.ParameterType.Name);
    }

    private Type GetTargetCollectionType()
    {
        var collType = MyTargetCollectionElement.ResultType;

        // Try to see if the collection is a generic ICollection or IDictionary
        Type[] interfaces = collType.GetInterfaces();

        foreach (var interfaceType in interfaces)
        {
            if (interfaceType.IsGenericType == false) continue;

            var genericTypeDef = interfaceType.GetGenericTypeDefinition();

            if (ReferenceEquals(genericTypeDef, typeof(ICollection<>)) |
                ReferenceEquals(genericTypeDef, typeof(IDictionary<,>))) return interfaceType;
        }

        // Try to see if it is a regular IList or IDictionary
        if (typeof(IList<>).IsAssignableFrom(collType)) return typeof(IList<>);

        if (typeof(IDictionary<,>).IsAssignableFrom(collType)) return typeof(IDictionary<,>);

        // Not a known collection type
        return null;
    }

    public override void Emit(FleeILGenerator ilg, IServiceProvider services)
    {
        if (MyTargetCollectionType != null)
            EmitCollectionIn(ilg, services);
        else
            // Do the real emit
            EmitListIn(ilg, services);
    }

    private void EmitCollectionIn(FleeILGenerator ilg, IServiceProvider services)
    {
        // Get the contains method
        var mi = GetCollectionContainsMethod();
        var p1 = mi.GetParameters()[0];

        // Load the collection
        MyTargetCollectionElement.Emit(ilg, services);
        // Load the argument
        MyOperand.Emit(ilg, services);
        // Do an implicit convert if necessary
        ImplicitConverter.EmitImplicitConvert(MyOperand.ResultType, p1.ParameterType, ilg);
        // Call the contains method
        ilg.Emit(OpCodes.Callvirt, mi);
    }

    private MethodInfo GetCollectionContainsMethod()
    {
        var methodName = "Contains";

        if (MyTargetCollectionType.IsGenericType &&
            ReferenceEquals(MyTargetCollectionType.GetGenericTypeDefinition(), typeof(IDictionary<,>)))
            methodName = "ContainsKey";

        return MyTargetCollectionType.GetMethod(methodName,
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
    }

    private void EmitListIn(FleeILGenerator ilg, IServiceProvider services)
    {
        var ce = new CompareElement();
        var endLabel = ilg.DefineLabel();
        var trueTerminal = ilg.DefineLabel();

        // Cache the operand since we will be comparing against it a lot
        var lb = ilg.DeclareLocal(MyOperand.ResultType);
        var targetIndex = lb.LocalIndex;

        MyOperand.Emit(ilg, services);
        Utility.EmitStoreLocal(ilg, targetIndex);

        // Wrap our operand in a local shim
        var targetShim = new LocalBasedElement(MyOperand, targetIndex);

        // Emit the compares
        foreach (var argumentElement in MyArguments)
        {
            ce.Initialize(targetShim, argumentElement, LogicalCompareOperation.Equal);
            ce.Emit(ilg, services);

            EmitBranchToTrueTerminal(ilg, trueTerminal);
        }

        ilg.Emit(OpCodes.Ldc_I4_0);
        ilg.Emit(OpCodes.Br_S, endLabel);

        ilg.MarkLabel(trueTerminal);

        ilg.Emit(OpCodes.Ldc_I4_1);

        ilg.MarkLabel(endLabel);
    }

    private static void EmitBranchToTrueTerminal(FleeILGenerator ilg, Label trueTerminal)
    {
        ilg.EmitBranchTrue(trueTerminal);
    }
}