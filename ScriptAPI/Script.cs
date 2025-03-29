using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using ScriptedEventsAPI.EaqoldHelpers;
using ScriptedEventsAPI.OtherStructures;
using ScriptedEventsAPI.ScriptAPI.Contexting;
using ScriptedEventsAPI.ScriptAPI.Contexting.BaseContexts;
using ScriptedEventsAPI.ScriptAPI.Contexting.Extensions;
using ScriptedEventsAPI.ScriptAPI.Tokenizing;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.VariableAPI.Structures;

namespace ScriptedEventsAPI.ScriptAPI;

public class Script
{
    public required string Name { get; set; }
    public string Content { get; set; } = string.Empty;
    public List<BaseToken> Tokens = [];
    public List<BaseContext> Contexts = [];
    public readonly HashSet<LiteralVariable> LocalLiteralVariables = [];

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
}