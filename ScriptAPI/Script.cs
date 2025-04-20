using System;
using System.Collections.Generic;
using System.Linq;
using MEC;
using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.ScriptAPI.Contexting;
using ScriptedEventsAPI.ScriptAPI.Contexting.BaseContexts;
using ScriptedEventsAPI.ScriptAPI.Contexting.Extensions;
using ScriptedEventsAPI.ScriptAPI.Tokenizing;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Structures;
using ScriptedEventsAPI.VariableAPI;
using ScriptedEventsAPI.VariableAPI.Structures;

namespace ScriptedEventsAPI.ScriptAPI;

public class Script
{
    private readonly HashSet<LiteralVariable> _localLiteralVariables = [];
    private readonly HashSet<PlayerVariable> _localPlayerVariables = [];
    private CoroutineHandle _coroutine;
    public List<BaseContext> Contexts = [];
    public List<ScriptLine> Tokens = [];
    public required string Name { get; init; }
    public string Content { get; init; } = string.Empty;

    public void AddLocalLiteralVariable(LiteralVariable variable)
    {
        RemoveLocalLiteralVariable(variable);
        _localLiteralVariables.Add(variable);
    }

    public void RemoveLocalLiteralVariable(LiteralVariable variable)
    {
        _localLiteralVariables.RemoveWhere(scrVar => scrVar.Name == variable.Name);
    }

    public void AddLocalPlayerVariable(PlayerVariable variable)
    {
        RemoveLocalPlayerVariable(variable);
        _localPlayerVariables.Add(variable);
    }

    public void RemoveLocalPlayerVariable(PlayerVariable variable)
    {
        _localPlayerVariables.RemoveWhere(scrVar => scrVar.Name == variable.Name);
    }

    public void Execute()
    {
        _coroutine = InternalExecute().Run(_ => _coroutine.Kill());
    }

    public void Stop()
    {
        _coroutine.Kill();
    }

    private IEnumerator<float> InternalExecute()
    {
        try
        {
            new Tokenizer(this).GetAllFileTokens();
        }
        catch (Exception e)
        {
            Logger.Error(e);
            yield break;
        }

        try
        {
            if (new Contexter(this).LinkAllTokens(Tokens)
                .HasErrored(out var err, out var val))
            {
                Logger.Error(err);
                yield break;
            }

            Contexts = val!;
        }
        catch (Exception e)
        {
            Logger.Error(e);
            yield break;
        }

        foreach (var context in Contexts)
        {
            Logger.Debug($"executing {context}!");
            yield return Timing.WaitUntilDone(context.ExecuteBaseContext().Run());
        }
    }

    public Result TryGetPlayerVariable(string name, out PlayerVariable variable)
    {
        var localPlrVar = _localPlayerVariables.FirstOrDefault(
            v => v.Name == name);

        if (localPlrVar != null)
        {
            variable = localPlrVar;
            return true;
        }

        var globalPlrVar = PlayerVariableIndex.GlobalPlayerVariables
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
        var localPlrVar = _localLiteralVariables.FirstOrDefault(v => v.Name == name);

        if (localPlrVar != null)
        {
            variable = localPlrVar;
            return true;
        }

        variable = null!;
        return $"There is no literal variable named '{name}'.";
    }
}