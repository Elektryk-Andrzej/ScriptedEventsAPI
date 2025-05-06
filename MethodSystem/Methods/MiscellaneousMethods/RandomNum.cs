using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;
using ScriptedEventsAPI.MethodSystem.BaseMethods;
using ScriptedEventsAPI.MethodSystem.MethodDescriptors;
using UnityEngine;

namespace ScriptedEventsAPI.MethodSystem.Methods.MiscellaneousMethods;

public class RandomNum : TextReturningStandardMethod, IAdditionalDescription
{
    public override string Description =>
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
            DefaultValue = "real"
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