using System;
using ScriptedEventsAPI.Helpers.Flee.PublicTypes;
using ScriptedEventsAPI.Helpers.Flee.Resources;

namespace ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base;

internal abstract class UnaryElement : ExpressionElement
{
    private Type _myResultType;

    protected ExpressionElement MyChild;

    public override Type ResultType => _myResultType;

    public void SetChild(ExpressionElement child)
    {
        MyChild = child;
        _myResultType = GetResultType(child.ResultType);

        if (_myResultType == null)
            ThrowCompileException(CompileErrorResourceKeys.OperationNotDefinedForType,
                CompileExceptionReason.TypeMismatch, MyChild.ResultType.Name);
    }

    protected abstract Type GetResultType(Type childType);
}