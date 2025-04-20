using System;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base.Literals;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;

namespace ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Literals;

internal class CharLiteralElement : LiteralElement
{
    private readonly char _myValue;

    public CharLiteralElement(char value)
    {
        _myValue = value;
    }

    public override Type ResultType => typeof(char);

    public override void Emit(FleeILGenerator ilg, IServiceProvider services)
    {
        var intValue = Convert.ToInt32(_myValue);
        EmitLoad(intValue, ilg);
    }
}