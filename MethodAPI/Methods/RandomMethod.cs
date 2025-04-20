using ScriptedEventsAPI.MethodAPI.Arguments.Args;
using ScriptedEventsAPI.MethodAPI.Arguments.Structures;
using ScriptedEventsAPI.MethodAPI.BaseMethods;
using ScriptedEventsAPI.MethodAPI.MethodDescriptors;
using UnityEngine;

namespace ScriptedEventsAPI.MethodAPI.Methods;

public class RandomMethod : TextReturningStandardMethod, IAdditionalDescription
{
    public override string Name => "RandomNum";

    public override string ReturnDescription =>
        "Randomly generated number between provided arguments 'startingNum' and 'endingNum'.";

    public string AdditionalDescription =>
        "'startingNum' argument MUST be smaller than 'endingNum' argument.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new NumberArgument("startingNum"),
        new NumberArgument("endingNum"),
        new OptionsArgument("numberType", "integer", "real")
        {
            AdditionalDescription = 
                "'integer' -> numbers like -2, 7, 21 | 'real' -> numbers like -0.5, 420.69, 3.14",
            RequiredInfo = RequiredArgumentInfo.NotRequired("real")
        }
    ];

    public override void Execute()
    {
        var startingNum = Args.GetNumber("startingNum");
        var endingNum = Args.GetNumber("endingNum");
        var type = Args.GetOption("numberType");
        
        var val = UnityEngine.Random.Range(startingNum, endingNum);
        if (type == "integer")
        {
            val = Mathf.RoundToInt(val);
        }
        
        TextReturn = val.ToString();
    }
}