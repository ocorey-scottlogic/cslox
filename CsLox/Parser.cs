namespace CsLox;

using static TokenType;

public class Parser(List<Token> tokens)
{
    class ParseError : Exception {}
    List<Token> Tokens { get; } = tokens;
    int Current { get; set; } = 0;

    public Expr? Parse()
    {
        try
        {
            return Expression();
        }
        catch (ParseError)
        {
            Console.Error.WriteLine("Parse error");
            return null;
        }
    }



    Expr Expression()
    {
        return Comma();
    }

    Expr Comma()
    {
        Expr expr = Ternary();

        while (Match(COMMA))
        {
            Token operatorToken = Previous();
            Expr right = Ternary();
            expr = new Expr.Binary(expr, operatorToken, right);
        }
        
        return expr;
    }

    Expr Ternary()
    {
        Expr expr = Equality();

        if (Match(QUESTION))
        {
            Token operatorToken = Previous();
            
            Expr middle = Equality();
            Consume(COLON, "Expect ':' after conditional operator.");
            Expr right = Ternary();

            expr = new Expr.Ternary(expr, operatorToken, middle, right);
        }
        return expr;
    }

    Expr Equality()
    {
        Expr expr = Comparison();

        while (Match(BANG_EQUAL, EQUAL_EQUAL))
        {
            Token operatorToken = Previous();
            Expr right = Comparison();
            expr = new Expr.Binary(expr, operatorToken, right);
        }

        return expr;
    }

    Expr Comparison()
    {
        Expr expr = Term();

        while (Match(GREATER, GREATER_EQUAL, LESS, LESS_EQUAL))
        {
            Token operatorToken = Previous();
            Expr right = Term();
            expr = new Expr.Binary(expr, operatorToken, right);
        }

        return expr;
    }

    Expr Term()
    {
        Expr expr = Factor();

        while(Match(MINUS, PLUS))
        {
            Token operatorToken = Previous();
            Expr right = Factor();
            expr = new Expr.Binary(expr, operatorToken, right);
        }

        return expr;
    }

    Expr Factor()
    {
        Expr expr = Unary();

        while(Match(SLASH, STAR))
        {
            Token operatorToken = Previous();
            Expr right = Unary();
            expr = new Expr.Binary(expr, operatorToken, right);
        }

        return expr;
    }

    Expr Unary()
    {
        if(Match(BANG,MINUS))
        {
            Token operatorToken = Previous();
            Expr right = Unary();
            return new Expr.Unary(operatorToken, right);
        }

        return Primary();
    }

    Expr Primary()
    {
        if(Match(FALSE)) return new Expr.Literal(false);
        if(Match(TRUE)) return new Expr.Literal(true);
        if(Match(NIL)) return new Expr.Literal(null);

        if(Match(NUMBER, STRING))
        {
            return new Expr.Literal(Previous().Literal);
        }

        if(Match(LEFT_PAREN))
        {
            Expr expr = Expression();
            Consume(RIGHT_PAREN, "Expect ')' after expresssion.");
            return new Expr.Grouping(expr);
        }

        throw Error(Peek(), "Expect expression.");
    }

    bool Match(params TokenType[] types)
    {
       foreach(var type in types)
        {
            if (Check(type))
            {
                Advance();
                return true;
            }
        } 

        return false;
    }

    Token Consume(TokenType type, string message)
    {
        if (Check(type)) return Advance();
        throw Error(Peek(), message);
    }

    bool Check(TokenType type)
    {
        if (IsAtEnd()) return false;
        return Peek().Type == type;
    }

    Token Advance()
    {
        if(!IsAtEnd()) Current++;
        return Previous();
    }

    bool IsAtEnd()
    {
        return Peek().Type == EOF;
    }

    Token Peek()
    {
        return Tokens[Current];
    }

    Token Previous()
    {
        return Tokens[Current - 1];
    }

    ParseError Error(Token token , string message)
    {
        Lox.Error(token, message);
        return new ParseError();
    }

    void Synchronise()
    {
        Advance();
        while(!IsAtEnd())
        {
            if (Previous().Type == SEMICOLON) return;

            switch (Peek().Type)
            {
                case CLASS:
                case FUN:
                case VAR:
                case FOR:
                case IF:
                case WHILE:
                case PRINT:
                case RETURN:
                return;
            }

            Advance();
        }
    }


}