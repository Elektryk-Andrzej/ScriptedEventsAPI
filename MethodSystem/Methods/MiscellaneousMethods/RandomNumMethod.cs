using SER.MethodSystem.ArgumentSystem.Arguments;
using SER.MethodSystem.BaseMethods;
using SER.MethodSystem.MethodDescriptors;
using UnityEngine;

namespace SER.MethodSystem.Methods.MiscellaneousMethods;

public class RandomNumMethod : TextReturningMethod, IAdditionalDescription
{
    public override string Description =>
        "Returns a randomly generated number between provided arguments 'startingNum' and 'endingNum'.";

    public string AdditionalDescription =>
        "'startingNum' argument MUST be smaller than 'endingNum' argument.";

    public override BaseMethodArgument[] ExpectedArguments =>
    [
        new NumberArgument("startingNum"),
        new NumberArgument("endingNum"),
        new OptionsArgument("numberType", "integer", "real")
        {
            Description = 
                "'integer' -> numbers like -2, 7, 21 | 'real' -> numbers like -0.5, 420.69, 3.14",
            DefaultValue = "real"
        }
    ];

    public override void Execute()
    {
        var startingNum = Args.GetNumber("startingNum");
        var endingNum = Args.GetNumber("endingNum");
        var type = Args.GetOption("numberType");
        
        var val = Random.Range(startingNum, endingNum);
        if (type == "integer")
        {
            val = Mathf.RoundToInt(val);
        }
        
        TextReturn = val.ToString();
    }
}