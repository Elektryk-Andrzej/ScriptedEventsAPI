using System;
using System.Reflection.Emit;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;

namespace ScriptedEventsAPI.Helpers.Flee.ExpressionElements.LogicalBitwise;

internal class XorElement : BinaryExpressionElement
{
    protected override Type GetResultType(Type leftType, Type rightType)
    {
        var bitwiseType = Utility.GetBitwiseOpType(leftType, rightType);

        if (bitwiseType != null) return bitwiseType;

        if (AreBothChildrenOfType(typeof(bool))) return typeof(bool);

        return null;
    }

    public override void Emit(FleeILGenerator ilg, IServiceProvider services)
    {
        var resultType = ResultType;

        MyLeftChild.Emit(ilg, services);
        ImplicitConverter.EmitImplicitConvert(MyLeftChild.ResultType, resultType, ilg);
        MyRightChild.Emit(ilg, services);
        ImplicitConverter.EmitImplicitConvert(MyRightChild.ResultType, resultType, ilg);
        ilg.Emit(OpCodes.Xor);
    }


    protected override void GetOperation(object operation)
    {
    }
}