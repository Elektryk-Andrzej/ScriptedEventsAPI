using System;
using ScriptedEventsAPI.Helpers.Flee.ExpressionElements.Base;
using ScriptedEventsAPI.Helpers.Flee.InternalTypes;

namespace ScriptedEventsAPI.Helpers.Flee.ExpressionElements.MemberElements;

internal class ExpressionMemberElement : MemberElement
{
    private readonly ExpressionElement _myElement;

    public ExpressionMemberElement(ExpressionElement element)
    {
        _myElement = element;
    }

    protected override bool SupportsInstance => true;

    protected override bool IsPublic => true;

    public override bool IsStatic => false;
    public override bool IsExtensionMethod => false;

    public override Type ResultType => _myElement.ResultType;

    protected override void ResolveInternal()
    {
    }

    public override void Emit(FleeILGenerator ilg, IServiceProvider services)
    {
        base.Emit(ilg, services);
        _myElement.Emit(ilg, services);
        if (_myElement.ResultType.IsValueType) EmitValueTypeLoadAddress(ilg, ResultType);
    }
}