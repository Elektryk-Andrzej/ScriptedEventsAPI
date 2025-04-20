using System;
using System.Reflection.Emit;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;

namespace ScriptedEventsAPI.Helpers.Flee.ExpressionElements;

internal class NegateElement : UnaryElement
{
    protected override Type GetResultType(Type childType)
    {
        var tc = Type.GetTypeCode(childType);

        var mi = Utility.GetSimpleOverloadedOperator("UnaryNegation", childType, null);
        if (mi != null) return mi.ReturnType;

        switch (tc)
        {
            case TypeCode.Single:
            case TypeCode.Double:
            case TypeCode.Int32:
            case TypeCode.Int64:
                return childType;
            case TypeCode.UInt32:
                return typeof(long);
            default:
                return null;
        }
    }

    public override void Emit(FleeILGenerator ilg, IServiceProvider services)
    {
        var resultType = ResultType;
        MyChild.Emit(ilg, services);
        ImplicitConverter.EmitImplicitConvert(MyChild.ResultType, resultType, ilg);

        var mi = Utility.GetSimpleOverloadedOperator("UnaryNegation", resultType, null);

        if (mi == null)
            ilg.Emit(OpCodes.Neg);
        else
            ilg.Emit(OpCodes.Call, mi);
    }
}