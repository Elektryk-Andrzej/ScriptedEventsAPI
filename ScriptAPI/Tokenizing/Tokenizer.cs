using System.Collections.Generic;
using System.Linq;
using ScriptedEventsAPI.OtherStructures;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.BaseTokens;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.TokenLexers;
using ScriptedEventsAPI.ScriptAPI.Tokenizing.Tokens;

namespace ScriptedEventsAPI.ScriptAPI.Tokenizing;

public class Tokenizer(Script script)
{
    public void GetAllFileTokens()
    {
        List<BaseToken> tokens = [];
        foreach (var line in script.Content.Split('\n'))
        {
            tokens.AddRange(GetTokensFromLine(line));
        }
        
        script.Tokens = tokens;
    }

    public List<BaseToken> GetTokensFromLine(string lineContent)
    {
        return InternalGetTokensFromFileLine(lineContent.ToCharArray());
    }
    
    public List<BaseToken> InternalGetTokensFromFileLine(char[] lineContent)
    {
        BaseTokenLexer? currentTokenLexer = null;
        List<BaseToken> tokens = [];

        foreach (var currentChar in lineContent)
        {
            if (currentTokenLexer is null)
            {
                currentTokenLexer = GetLexer(currentChar, tokens);
                if (currentTokenLexer is not null)
                {
                    Logger.Debug($"Set new token lexer to: {currentTokenLexer}");
                }
                
                continue;
            }
            
            currentTokenLexer.TryAddChar(currentChar, out bool shouldContinueExecution); // should also return error
            if (shouldContinueExecution)
            {
                continue;
            }

            if (currentTokenLexer.IsValid().HasErrored(out var error))
            {
                Logger.Debug($"Error! Token lexer has errored! error: '{error}' | lexer: {currentTokenLexer} | rep: '{currentTokenLexer.Token.RawRepresentation}'");
                currentTokenLexer = null;
                continue;
            }
            
            Logger.Debug($"Token lexer {currentTokenLexer} has stopped on char {currentChar}");
            tokens.Add(currentTokenLexer.Token);
            currentTokenLexer = null;
        }

        if (currentTokenLexer is not null)
        {
            // todo: fix this awful solution
            if (currentTokenLexer.IsValid())
            {
                tokens.Add(currentTokenLexer.Token);
            }
            else
            {
                var coarsedToken = new UnclassifiedValueToken();
                coarsedToken.RawCharRepresentation.AddRange(currentTokenLexer.Token.RawRepresentation);
                tokens.Add(coarsedToken);
            }
        }

        tokens.Add(new EndLineToken());
        return tokens;
    }
    
    private BaseTokenLexer? GetLexer(char character, List<BaseToken> tokens)
    {
        // whitespaces are ignored when no tokenizer is engaged
        if (char.IsWhiteSpace(character))
        {
            return null;
        }
        
        switch (character)
        {
            case '#':
                return new CommentTokenLexer();
            case '!':
                return new FlagTokenLexer();
            case '@':
                return new PlayerVariableTokenLexer();
            case '{':
                return new LiteralVariableTokenLexer(script);
            case '(':
                return new ParenthesesTokenLexer();
        }

        if (char.IsUpper(character))
        {
            return new ActionTokenLexer(character, script, tokens.LastOrDefault());
        }

        if (char.IsLower(character))
        {
            return new ControlFlowTokenLexer(character, script);
        }
            
        return new UnclassifiedValueTokenLexer(character);
    } 
}