using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Enums;
using Exiled.API.Features;
using SER.MethodSystem.ArgumentSystem.Structures;
using SER.ScriptSystem;
using SER.ScriptSystem.TokenSystem.BaseTokens;
using SER.VariableSystem;

namespace SER.MethodSystem.ArgumentSystem.Arguments;

public class RoomsArgument(string name) : BaseMethodArgument(name)
{
    public override string OperatingValueDescription => $"{nameof(RoomType)} enum value, {nameof(ZoneType)} enum value, or a room reference.";
    
    public ArgumentEvaluation<Room[]> GetConvertSolution(BaseToken token, Script scr)
    {
        return DefaultConvertSolution(token, scr, InternalConvert);
    }

    private ArgumentEvaluation<Room[]>.EvalRes InternalConvert(string value)
    {
        var error = Rs.Add($"Value '{value}' is not a {nameof(RoomType)}, {nameof(ZoneType)} or a valid door reference.");
        
        if (Enum.TryParse(value, true, out RoomType roomType))
        {
            return Room.Get(room => room.Type == roomType).ToArray();
        }

        if (Enum.TryParse(value, true, out ZoneType zoneType))
        {
            return Room.Get(room => room.Zone == zoneType).ToArray();
        }
        
        if (!ObjectReferenceSystem.TryRetreiveObject(value, out var obj))
        {
            return error;
        }
        
        return obj switch
        {
            IEnumerable<Room> roomList => roomList.ToArray(),
            Room room => new[] { room },
            _ => error
        };
    }
}