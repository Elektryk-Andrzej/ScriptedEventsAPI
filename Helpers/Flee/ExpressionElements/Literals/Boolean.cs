using System;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base.Literals;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;

namespace ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Literals;

internal class BooleanLiteralElement : LiteralElement
{
    private readonly bool _myValue;

    public BooleanLiteralElement(bool value)
    {
        _myValue = value;
    }

    public override Type ResultType => typeof(bool);

    public override void Emit(FleeILGenerator ilg, IServiceProvider services)
    {
        EmitLoad(_myValue, ilg);
    }
}