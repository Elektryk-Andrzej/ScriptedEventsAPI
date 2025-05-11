using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features.Doors;
using ScriptedEventsAPI.MethodSystem.ArgumentSystem.Structures;
using ScriptedEventsAPI.ScriptSystem;
using ScriptedEventsAPI.ScriptSystem.TokenSystem.BaseTokens;
using ScriptedEventsAPI.VariableSystem;

namespace ScriptedEventsAPI.MethodSystem.ArgumentSystem.Arguments;

public class DoorsArgument(string name) : BaseMethodArgument(name)
{
    public override string OperatingValueDescription => $"{nameof(DoorType)} enum, {nameof(ZoneType)} enum or a valid door reference";
    
    public ArgumentEvaluation<Door[]> GetConvertSolution(BaseToken token, Script scr)
    {
        return DefaultConvertSolution(token, scr, InternalConvert);
    }

    private static ArgumentEvaluation<Door[]>.EvalRes InternalConvert(string value)
    {
        if (Enum.TryParse(value, true, out DoorType doorType))
        {
            return Door.Get(d => d.Type == doorType).ToArray();
        }

        if (Enum.TryParse(value, true, out ZoneType zoneType))
        {
            return Door.Get(door => door.Zone == zoneType).ToArray();
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