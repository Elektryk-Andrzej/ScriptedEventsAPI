using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Exiled.API.Features;
using MEC;
using SER.ScriptSystem.ContextSystem.Extensions;
using SER.Helpers;
using SER.Helpers.ResultStructure;
using SER.Plugin;
using SER.ScriptSystem.ContextSystem;
using SER.ScriptSystem.ContextSystem.BaseContexts;
using SER.ScriptSystem.TokenSystem;
using SER.ScriptSystem.TokenSystem.Structures;
using SER.ScriptSystem.TokenSystem.Tokens;
using SER.VariableSystem;
using SER.VariableSystem.Structures;

namespace SER.ScriptSystem;

public class Script
{
    public required string Name { get; init; }
    public required string Content { get; init; }
    public List<ScriptLine> Tokens = [];
    public List<BaseContext> Contexts = [];

    private readonly HashSet<LiteralVariable> _localLiteralVariables = [];
    private readonly HashSet<PlayerVariable> _localPlayerVariables = [];
    private CoroutineHandle _coroutine;

    public static TryGet<Script> CreateByScriptName(string scriptName)
    {
        var name = Path.GetFileNameWithoutExtension(scriptName);
        
        if (!FileSystem.DoesScriptExist(name, out var path))
        {
            return $"Script '{name}' does not exist in the SER folder.";
        }

        return new Script
        {
            Name = name,
            Content = File.ReadAllText(path)
        };
    }
    
    public static TryGet<Script> CreateByPath(string path)
    {
        var name = Path.GetFileNameWithoutExtension(path);
        
        if (!FileSystem.DoesScriptExist(path))
        {
            return $"Script '{name}' does not exist in the SER folder.";
        }

        return new Script
        {
            Name = name,
            Content = File.ReadAllText(path)
        };
    }
    
    public static Script CreateByVerifiedPath(string path)
    {
        var name = Path.GetFileNameWithoutExtension(path);
        return new Script
        {
            Name = name,
            Content = File.ReadAllText(path)
        };
    }

    public List<ScriptLine> GetFlagLines()
    {
        CacheTokens();

        return Tokens.Where(l => l.Tokens.FirstOrDefault() is FlagToken).ToList();
    }

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
        if (string.IsNullOrWhiteSpace(Content))
        {
            return;
        }
        
        Log.Info($"Running script {Name}");
        
        Plugin.Plugin.RunningScripts.Add(this);
        _coroutine = InternalExecute().Run(_ => _coroutine.Kill());
    }

    public void Stop()
    {
        Plugin.Plugin.RunningScripts.Remove(this);
        _coroutine.Kill();
    }

    private void CacheTokens()
    {
        try
        {
            new Tokenizer(this).GetAllFileTokens();
        }
        catch (Exception e)
        {
            Logger.Error(this, e);
        }
    }
    
    private void CacheContexts()
    {
        try
        {
            if (new Contexter(this).LinkAllTokens(Tokens)
                .HasErrored(out var err, out var val))
            {
                Logger.Error(this, err);
            }

            Contexts = val!;
        }
        catch (Exception e)
        {
            Logger.Error(this, e);
        }
    }

    private IEnumerator<float> InternalExecute()
    {
        CacheTokens();
        CacheContexts();

        Logger.Debug($"Running script {Name}");
        foreach (var handle in Contexts.Select(context => context.ExecuteBaseContext().Run()))
        {
            while (handle.IsRunning)
            {
                yield return 0f;
            }
        }

        Plugin.Plugin.RunningScripts.Remove(this);
    }

    public Result TryGetPlayerVariable(string name, out PlayerVariable variable)
    {
        var localPlrVar = _localPlayerVariables.FirstOrDefault(
            pv => pv.Name == name);

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
            return $"There is no player variable named '@{name}'.";
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