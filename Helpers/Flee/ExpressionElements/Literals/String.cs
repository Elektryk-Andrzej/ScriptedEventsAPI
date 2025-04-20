using System;
using System.Reflection.Emit;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base.Literals;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;

namespace ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Literals;

internal class StringLiteralElement : LiteralElement
{
    private readonly string _myValue;

    public StringLiteralElement(string value)
    {
        _myValue = value;
    }

    public override Type ResultType => typeof(string);

    public override void Emit(FleeILGenerator ilg, IServiceProvider services)
    {
        ilg.Emit(OpCodes.Ldstr, _myValue);
    }
}