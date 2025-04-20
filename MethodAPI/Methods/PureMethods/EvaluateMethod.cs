using System;
using ScriptedEventsAPI.Helpers.Flee.PublicTypes;
using ScriptedEventsAPI.MethodAPI.Arguments.Args;
using ScriptedEventsAPI.MethodAPI.BaseMethods;

namespace ScriptedEventsAPI.MethodAPI.Methods.PureMethods;

public class EvaluateMethod : TextReturningStandardMethod
{
    public override string Name => "Eval";
    public override string ReturnDescription => null!;
    public override string Description => null!;

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new TextArgument("value")
    ];

    public override void Execute()
    {
        var value = Args.GetText("value");

        var context = new ExpressionContext();
        context.Imports.AddType(typeof(Math));

        var expression = context.CompileGeneric<double>(value);

        TextReturn = expression.Evaluate().ToString();
    }
}