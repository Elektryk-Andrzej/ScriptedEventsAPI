using System;
using System.Collections.Generic;
using System.Linq;
using ScriptedEventsAPI.OtherStructures;
using ScriptedEventsAPI.ScriptAPI;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;
using ScriptedEventsAPI.VariableAPI;

namespace ScriptedEventsAPI.ActionAPI.ActionArguments.Arguments;

public class EnumArgument(string name, Type enumType, bool required = true) : BaseActionArgument(name, required)
{
    public Result TryConvert(BaseToken token, out Func<object> value) 
    {
        value = null!;
        
        if (!Enum.IsDefined(enumType, token.RawRepresentation))
        {
            return $"Enum {enumType.Name} does not include {token.RawRepresentation} as a valid value.";
        }

        var enumValue = Enum.Parse(enumType, token.RawRepresentation, true);
        value = () => enumValue;
        return true;
    }
}