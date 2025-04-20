using System;
using System.Globalization;
using System.Reflection.Emit;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base.Literals;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;
using ScriptedEventsAPI.Helpers.Flee.PublicTypes;
using ScriptedEventsAPI.Helpers.Flee.Resources;

namespace ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Literals;

internal class DateTimeLiteralElement : LiteralElement
{
    private DateTime _myValue;

    public DateTimeLiteralElement(string image, ExpressionContext context)
    {
        var options = context.ParserOptions;

        if (DateTime.TryParseExact(image, options.DateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None,
                out _myValue) ==
            false)
            ThrowCompileException(CompileErrorResourceKeys.CannotParseType, CompileExceptionReason.InvalidFormat,
                typeof(DateTime).Name);
    }

    public override Type ResultType => typeof(DateTime);

    public override void Emit(FleeILGenerator ilg, IServiceProvider services)
    {
        var index = ilg.GetTempLocalIndex(typeof(DateTime));

        Utility.EmitLoadLocalAddress(ilg, index);

        EmitLoad(_myValue.Ticks, ilg);

        var ci = typeof(DateTime).GetConstructor(new[] { typeof(long) });

        ilg.Emit(OpCodes.Call, ci);

        Utility.EmitLoadLocal(ilg, index);
    }
}