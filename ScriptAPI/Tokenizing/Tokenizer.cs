﻿using System.Collections.Generic;
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
        bool isFirstChar = true;
        bool nextCharNotFirst = false;

        foreach (var currentChar in lineContent)
        {
            if (nextCharNotFirst)
            {
                isFirstChar = false;
            }
            
            if (isFirstChar)
            {
                // ignore indenting
                if (char.IsWhiteSpace(currentChar))
                {
                    continue;
                }
                
                nextCharNotFirst = true;
            }

            if (currentTokenLexer is null)
            {
                currentTokenLexer = GetLexer(currentChar, isFirstChar);
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
                break;
            }
            
            Logger.Debug($"Token lexer {currentTokenLexer} has stopped on char {currentChar}");
            tokens.Add(currentTokenLexer.Token);
            currentTokenLexer = null;
        }

        if (currentTokenLexer is not null)
        {
            if (currentTokenLexer.IsValid().HasErrored(out var error))
            {
                Logger.Debug(
                    $"Token lexer {currentTokenLexer} has failed to end its scanning. Error: '{error}'");    
            }
            
            tokens.Add(currentTokenLexer.Token);
        }

        tokens.Add(new EndLineToken());
        return tokens;
    }
    
    private BaseTokenLexer? GetLexer(char character, bool isFirstChar)
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

        if (char.IsUpper(character) && isFirstChar)
        {
            return new ActionTokenLexer(character, script);
        }

        if (char.IsLower(character))
        {
            return new ControlFlowTokenLexer(character);
        }
            
        return new UnclassifiedValueTokenLexer(character);
    } 
}