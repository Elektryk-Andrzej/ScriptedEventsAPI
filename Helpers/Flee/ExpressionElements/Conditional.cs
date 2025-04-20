using System;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;
using ScriptedEventsAPI.Helpers.Flee.PublicTypes;
using ScriptedEventsAPI.Helpers.Flee.Resources;

namespace ScriptedEventsAPI.Helpers.Flee.ExpressionElements;

internal class ConditionalElement : ExpressionElement
{
    private readonly ExpressionElement _myCondition;
    private readonly Type _myResultType;
    private readonly ExpressionElement _myWhenFalse;
    private readonly ExpressionElement _myWhenTrue;

    public ConditionalElement(ExpressionElement condition, ExpressionElement whenTrue, ExpressionElement whenFalse)
    {
        _myCondition = condition;
        _myWhenTrue = whenTrue;
        _myWhenFalse = whenFalse;

        if (!ReferenceEquals(_myCondition.ResultType, typeof(bool)))
            ThrowCompileException(CompileErrorResourceKeys.FirstArgNotBoolean, CompileExceptionReason.TypeMismatch);

        // The result type is the type that is common to the true/false operands
        if (ImplicitConverter.EmitImplicitConvert(_myWhenFalse.ResultType, _myWhenTrue.ResultType, null))
            _myResultType = _myWhenTrue.ResultType;
        else if (ImplicitConverter.EmitImplicitConvert(_myWhenTrue.ResultType, _myWhenFalse.ResultType, null))
            _myResultType = _myWhenFalse.ResultType;
        else
            ThrowCompileException(CompileErrorResourceKeys.NeitherArgIsConvertibleToTheOther,
                CompileExceptionReason.TypeMismatch, _myWhenTrue.ResultType.Name, _myWhenFalse.ResultType.Name);
    }

    public override Type ResultType => _myResultType;

    public override void Emit(FleeILGenerator ilg, IServiceProvider services)
    {
        EmitConditional(ilg, services);
    }

    private void EmitConditional(FleeILGenerator ilg, IServiceProvider services)
    {
        var falseLabel = ilg.DefineLabel();
        var endLabel = ilg.DefineLabel();

        // Emit the condition
        _myCondition.Emit(ilg, services);

        // On false go to the false operand
        ilg.EmitBranchFalse(falseLabel);

        // Emit the true operand
        _myWhenTrue.Emit(ilg, services);
        ImplicitConverter.EmitImplicitConvert(_myWhenTrue.ResultType, _myResultType, ilg);

        // Jump to end
        ilg.EmitBranch(endLabel);

        ilg.MarkLabel(falseLabel);

        // Emit the false operand
        _myWhenFalse.Emit(ilg, services);
        ImplicitConverter.EmitImplicitConvert(_myWhenFalse.ResultType, _myResultType, ilg);
        // Fall through to end
        ilg.MarkLabel(endLabel);
    }
}