using System;
using System.Reflection.Emit;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base.Literals;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;
using ScriptedEventsAPI.Helpers.Flee.PublicTypes;
using ScriptedEventsAPI.Helpers.Flee.Resources;

namespace ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Literals;

internal class TimeSpanLiteralElement : LiteralElement
{
    private TimeSpan _myValue;

    public TimeSpanLiteralElement(string image)
    {
        if (TimeSpan.TryParse(image, out _myValue) == false)
            ThrowCompileException(CompileErrorResourceKeys.CannotParseType, CompileExceptionReason.InvalidFormat,
                typeof(TimeSpan).Name);
    }

    public override Type ResultType => typeof(TimeSpan);

    public override void Emit(FleeILGenerator ilg, IServiceProvider services)
    {
        var index = ilg.GetTempLocalIndex(typeof(TimeSpan));

        Utility.EmitLoadLocalAddress(ilg, index);

        EmitLoad(_myValue.Ticks, ilg);

        var ci = typeof(TimeSpan).GetConstructor(new[] { typeof(long) });

        ilg.Emit(OpCodes.Call, ci);

        Utility.EmitLoadLocal(ilg, index);
    }
}