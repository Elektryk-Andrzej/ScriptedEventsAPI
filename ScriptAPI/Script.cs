using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using ScriptedEventsAPI.EaqoldHelpers;
using ScriptedEventsAPI.OtherStructures;
using ScriptedEventsAPI.OtherStructures.ResultStructure;
using ScriptedEventsAPI.ScriptAPI.Contexting;
using ScriptedEventsAPI.ScriptAPI.Contexting.BaseContexts;
using ScriptedEventsAPI.ScriptAPI.Contexting.Extensions;
using ScriptedEventsAPI.ScriptAPI.Tokenizing;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.VariableAPI;
using ScriptedEventsAPI.VariableAPI.Structures;

namespace ScriptedEventsAPI.ScriptAPI;

public class Script
{
    public required string Name { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<BaseToken> Tokens = [];
    public List<BaseContext> Contexts = [];
    public readonly HashSet<LiteralVariable> LocalLiteralVariables = [];
    public readonly HashSet<PlayerVariable> LocalPlayerVariables = [];

    public void Execute()
    {
        InternalExecute().Run();
    }

    private IEnumerator<float> InternalExecute()
    {
        Logger.Debug("1");
        try
        {
            new Tokenizer(this).GetAllFileTokens();
        }
        catch (Exception e)
        {
            Logger.Debug(e.Message);
        }

        Logger.Debug(2);
        try
        {
            new Contexter(this).LinkAllTokens();
        }
        catch (Exception e)
        {
            Logger.Debug(e.Message);
        }
        
        foreach (var context in Contexts)
        {
            Logger.Debug($"executing {context}!");
            yield return Timing.WaitUntilDone(context.ExecuteBaseContext());
        }
    }

    public Result TryGetPlayerVariable(string name, out PlayerVariable variable)
    {
        var localPlrVar = LocalPlayerVariables.FirstOrDefault(
            v => v.Name == name);
        
        if (localPlrVar != null)
        {
            variable = localPlrVar;
            return true;
        }

        var globalPlrVar = PlayerVariableIndex.GlobalVariables
            .FirstOrDefault(v => v.Name == name);
        if (globalPlrVar == null)
        {
            variable = null!;
            return $"There is no player variable named '{name}'.";
        }

        variable = globalPlrVar;
        return true;
    }
    
    public Result TryGetLiteralVariable(string name, out LiteralVariable variable)
    {
        var localPlrVar = LocalLiteralVariables.FirstOrDefault(
            v => v.Name == name);
        
        if (localPlrVar != null)
        {
            variable = localPlrVar;
            return true;
        }
        
        variable = null!;
        return $"There is no literal variable named '{name}'.";
    }
}