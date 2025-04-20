using System;
using System.Reflection.Emit;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base.Literals;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;

namespace ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Literals;

internal class NullLiteralElement : LiteralElement
{
    public override Type ResultType => typeof(Null);

    public override void Emit(FleeILGenerator ilg, IServiceProvider services)
    {
        ilg.Emit(OpCodes.Ldnull);
    }
}