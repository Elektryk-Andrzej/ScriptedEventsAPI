using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features.Doors;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Structures;
using ScriptedEventsAPI.ScriptSystem;
using ScriptedEventsAPI.ScriptSystem.TokenSystem.BaseTokens;
using ScriptedEventsAPI.VariableAPI;

namespace ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;

public class DoorArgument(string name) : BaseMethodArgument(name)
{
    public static ArgEvalRes<Door[]> GetConvertSolution(BaseToken token, Script scr)
    {
        return VariableParser.IsVariableUsedInString(token.RawRepresentation, scr,
            out var getProcessedVariableValueFunc)
            ? new(() => InternalConvert(getProcessedVariableValueFunc()))
            : new(InternalConvert(token.RawRepresentation));
    }

    private static ArgEvalRes<Door[]>.ResInfo InternalConvert(string value)
    {
        if (Enum.TryParse(value, true, out DoorType doorType))
        {
            return Door.List.Where(d => d.Type == doorType).ToArray();
        }

        if (!ObjectReferenceSystem.TryRetreiveObject(value, out var obj))
        {
            return $"Value '{value}' is not a valid door type or object reference.";
        }

        return obj switch
        {
            IEnumerable<Door> doorList => doorList.ToArray(),
            Door door => new[] { door },
            _ => $"Value '{value}' is not a valid door type or object reference."
        };
    }
}