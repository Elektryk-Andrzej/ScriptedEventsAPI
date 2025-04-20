using System;
using System.Reflection.Emit;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;
using ScriptedEventsAPI.Helpers.Flee.PublicTypes;
using ScriptedEventsAPI.Helpers.Flee.Resources;

namespace ScriptedEventsAPI.Helpers.Flee.ExpressionElements;

internal class RootExpressionElement : ExpressionElement
{
    private readonly ExpressionElement _myChild;
    private readonly Type _myResultType;

    public RootExpressionElement(ExpressionElement child, Type resultType)
    {
        _myChild = child;
        _myResultType = resultType;
        Validate();
    }

    public override Type ResultType => typeof(object);

    public override void Emit(FleeILGenerator ilg, IServiceProvider services)
    {
        _myChild.Emit(ilg, services);
        ImplicitConverter.EmitImplicitConvert(_myChild.ResultType, _myResultType, ilg);

        var options = (ExpressionOptions)services.GetService(typeof(ExpressionOptions));

        if (options.IsGeneric == false) ImplicitConverter.EmitImplicitConvert(_myResultType, typeof(object), ilg);

        ilg.Emit(OpCodes.Ret);
    }

    private void Validate()
    {
        if (ImplicitConverter.EmitImplicitConvert(_myChild.ResultType, _myResultType, null) == false)
            ThrowCompileException(CompileErrorResourceKeys.CannotConvertTypeToExpressionResult,
                CompileExceptionReason.TypeMismatch, _myChild.ResultType.Name, _myResultType.Name);
    }
}