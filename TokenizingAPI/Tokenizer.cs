using System;
using System.Collections.Generic;
using System.Linq;
using ScriptedEventsAPI.Other;
using ScriptedEventsAPI.ScriptAPI;
using ScriptedEventsAPI.TokenizingAPI.TokenLexers;
using ScriptedEventsAPI.TokenizingAPI.Tokens;

namespace ScriptedEventsAPI.TokenizingAPI;

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

    public static List<BaseToken> GetTokensFromLine(string lineContent)
    {
        return InternalGetTokensFromFileLine(lineContent.ToCharArray());
    }
    
    public static List<BaseToken> InternalGetTokensFromFileLine(char[] lineContent)
    {
        BaseTokenLexer? currentTokenLexer = null;
        List<BaseToken> tokens = [];
        bool isFirstChar = true;

        for (var index = 0; index < lineContent.Length; index++)
        {
            var character = lineContent[index];
            char? nextChar = index + 1 < lineContent.Length 
                ? lineContent[index + 1] 
                : null;
            
            if (currentTokenLexer is not null)
            {
                currentTokenLexer.TryAddChar(character, out bool shouldContinueExecution); // should also return error
                if (shouldContinueExecution)
                {
                    continue;
                }

                if (nextChar.HasValue && char.IsWhiteSpace(nextChar.Value))
                {
                    continue;
                }

                if (currentTokenLexer.IsFinalStateValid().HasErrored(out var error))
                {
                    Log.Debug($"Error! Token lexer has errored! error: '{error}' | lexer: {currentTokenLexer} | rep: '{currentTokenLexer.Token.AsString}'");
                    currentTokenLexer = null;
                    break;
                }
                
                tokens.Add(currentTokenLexer.Token);
                currentTokenLexer = null;
            }

            // we're waiting for the first 
            if (isFirstChar)
            {
                if (char.IsWhiteSpace(character))
                {
                    continue;
                }

                isFirstChar = false;
            }

            // whitespaces are ignored when no tokenizer is engaged
            if (char.IsWhiteSpace(character))
            {
                continue;
            }

            switch (character)
            {
                case '#':
                    currentTokenLexer = new CommentTokenLexer();
                    continue;
                case '!':
                    currentTokenLexer = new FlagTokenLexer();
                    continue;
                case '@':
                    currentTokenLexer = new PlayerVariableTokenLexer();
                    continue;
                case '$':
                    currentTokenLexer = new LiteralVariableTokenLexer();
                    continue;
                case '(':
                    currentTokenLexer = new ParenthesesTokenLexer();
                    continue;
            }

            if (char.IsUpper(character))
            {
                currentTokenLexer = new ActionTokenLexer(character);
                continue;
            }

            if (char.IsLower(character) && isFirstChar)
            {
                currentTokenLexer = new ControlFlowTokenLexer(character);
                continue;
            }

            if (!char.IsWhiteSpace(character))
            {
                currentTokenLexer = new UnclassifiedValueTokenLexer(character);
            }
        }

        if (currentTokenLexer is not null)
        {
            if (currentTokenLexer.IsFinalStateValid().HasErrored(out var error))
            {
                Log.Debug(
                    $"Token lexer {currentTokenLexer} has failed to end its scanning. Error: '{error}'");    
            }
            
            tokens.Add(currentTokenLexer.Token);
        }

        tokens.Add(new EndLineToken());
        return tokens;
    }
}