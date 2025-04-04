﻿using System.Collections.Generic;
using Exiled.API.Features;
using MEC;
using ScriptedEventsAPI.ConditionAPI;
using ScriptedEventsAPI.Helpers;
using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.ScriptAPI.Contexting.BaseContexts;
using ScriptedEventsAPI.ScriptAPI.Contexting.Extensions;
using ScriptedEventsAPI.ScriptAPI.Contexting.Structures;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;

namespace ScriptedEventsAPI.ScriptAPI.Contexting.Contexts;

public class IfStatementContext(Script scr) : TreeContext
{
    private string? _condition;
    
    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        _condition = token.RawRepresentation;
        return TryAddTokenRes.End();
    }

    public override Result VerifyCurrentState()
    {
        return _condition is not null
            ? true
            : "An if statement expects to have a condition, but none was provided!";
    }

    public override IEnumerator<float> Execute()
    {
        if (_condition is null) yield break;

        var evaluator = new ConditionEvaluator(_condition, scr);
        var res = evaluator.Evaluate(out var result);
        if (res.HasErrored(out var error))
        {
            Log.Error($"condtion {_condition} is malformed! Reason: {error}");
            yield break;
        }

        Logger.Debug($"result of {_condition} was {result}");
        if (result == false)
        {
            yield break;
        }

        Logger.Debug($"if statement has children: {string.Join(", ", Children)}");
        foreach (var child in Children)
        {
            yield return Timing.WaitUntilDone(child.ExecuteBaseContext().Run());
        }
    }
}