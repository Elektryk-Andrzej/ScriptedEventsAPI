using Exiled.API.Features;
using ScriptedEventsAPI.Helpers.ResultStructure;
using ScriptedEventsAPI.ScriptAPI.Contexting.BaseContexts;
using ScriptedEventsAPI.ScriptAPI.Contexting.Structures;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;

namespace ScriptedEventsAPI.ScriptAPI.Contexting.Contexts;

public class NoopContext : StandardContext
{
    public override TryAddTokenRes TryAddToken(BaseToken token)
    {
        return TryAddTokenRes.Continue();
    }

    public override Result VerifyCurrentState()
    {
        return true;
    }

    public override void Execute()
    {
        var num = 1;

        for (int i = 0; i < 32; i++)
        {
            Log.Info($"Currently at: {2 * num}");

            num += 1;
        }
    }
}