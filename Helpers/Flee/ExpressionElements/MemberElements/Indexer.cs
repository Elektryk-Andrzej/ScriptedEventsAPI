using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;
using ScriptedEventsAPI.Helpers.Flee.PublicTypes;
using ScriptedEventsAPI.Helpers.Flee.Resources;

namespace ScriptedEventsAPI.Helpers.Flee.ExpressionElements.MemberElements;

[Obsolete("Element representing an array index")]
internal class IndexerElement : MemberElement
{
    private readonly ArgumentList _myIndexerElements;
    private ExpressionElement _myIndexerElement;

    public IndexerElement(ArgumentList indexer)
    {
        _myIndexerElements = indexer;
    }

    private Type ArrayType
    {
        get
        {
            if (IsArray) return MyPrevious.TargetType;

            return null;
        }
    }

    private bool IsArray => MyPrevious.TargetType.IsArray;

    protected override bool RequiresAddress => IsArray == false;

    public override Type ResultType
    {
        get
        {
            if (IsArray) return ArrayType.GetElementType();

            return _myIndexerElement.ResultType;
        }
    }

    protected override bool IsPublic
    {
        get
        {
            if (IsArray) return true;

            return IsElementPublic((MemberElement)_myIndexerElement);
        }
    }

    public override bool IsStatic => false;
    public override bool IsExtensionMethod => false;

    protected override void ResolveInternal()
    {
        // Are we are indexing on an array?
        var target = MyPrevious.TargetType;

        // Yes, so setup for an array index
        if (target.IsArray)
        {
            SetupArrayIndexer();
            return;
        }

        // Not an array, so try to find an indexer on the type
        if (FindIndexer(target) == false)
            ThrowCompileException(CompileErrorResourceKeys.TypeNotArrayAndHasNoIndexerOfType,
                CompileExceptionReason.TypeMismatch, target.Name, _myIndexerElements);
    }

    private void SetupArrayIndexer()
    {
        _myIndexerElement = _myIndexerElements[0];

        if (_myIndexerElements.Count > 1)
            ThrowCompileException(CompileErrorResourceKeys.MultiArrayIndexNotSupported,
                CompileExceptionReason.TypeMismatch);
        else if (ImplicitConverter.EmitImplicitConvert(_myIndexerElement.ResultType, typeof(int), null) == false)
            ThrowCompileException(CompileErrorResourceKeys.ArrayIndexersMustBeOfType,
                CompileExceptionReason.TypeMismatch, typeof(int).Name);
    }

    private bool FindIndexer(Type targetType)
    {
        // Get the default members
        MemberInfo[] members = targetType.GetDefaultMembers();

        List<MethodInfo> methods = new List<MethodInfo>();

        // Use the first one that's valid for our indexer type
        foreach (var mi in members)
        {
            var pi = mi as PropertyInfo;
            if (pi != null) methods.Add(pi.GetGetMethod(true));
        }

        var func = new FunctionCallElement("Indexer", methods.ToArray(), _myIndexerElements);
        func.Resolve(MyServices);
        _myIndexerElement = func;

        return true;
    }

    public override void Emit(FleeILGenerator ilg, IServiceProvider services)
    {
        base.Emit(ilg, services);

        if (IsArray)
            EmitArrayLoad(ilg, services);
        else
            EmitIndexer(ilg, services);
    }

    private void EmitArrayLoad(FleeILGenerator ilg, IServiceProvider services)
    {
        _myIndexerElement.Emit(ilg, services);
        ImplicitConverter.EmitImplicitConvert(_myIndexerElement.ResultType, typeof(int), ilg);

        var elementType = ResultType;

        if (elementType.IsValueType == false)
            // Simple reference load
            ilg.Emit(OpCodes.Ldelem_Ref);
        else
            EmitValueTypeArrayLoad(ilg, elementType);
    }

    private void EmitValueTypeArrayLoad(FleeILGenerator ilg, Type elementType)
    {
        if (NextRequiresAddress)
            ilg.Emit(OpCodes.Ldelema, elementType);
        else
            Utility.EmitArrayLoad(ilg, elementType);
    }

    private void EmitIndexer(FleeILGenerator ilg, IServiceProvider services)
    {
        var func = (FunctionCallElement)_myIndexerElement;
        func.EmitFunctionCall(NextRequiresAddress, ilg, services);
    }
}