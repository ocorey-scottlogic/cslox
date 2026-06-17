namespace CsLox;
using static TokenType;
class Scanner
{
    private readonly string source;
    private readonly List<Token> tokens = [];
    private int start = 0;
    private int current = 0;
    private int line = 1;
    private static readonly Dictionary<string, TokenType> keywords =
    new Dictionary<string, TokenType> {
       {"and", AND},
       {"class", CLASS},
       {"else", ELSE},
       {"false",  FALSE},
       {"for",    FOR},
       {"fun",    FUN},
       {"if",     IF},
       {"nil",    NIL},
       {"or",     OR},
       {"print",  PRINT},
       {"return", RETURN},
       {"super",  SUPER},
       {"this",   THIS},
       {"true",   TRUE},
       {"var",    VAR},
       {"while",  WHILE}
    };

    public Scanner(string source)
    {
        this.source = source;
    }

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            start = current;
            ScanToken();
        }

        tokens.Add(new Token(EOF, "", null, line));
        return tokens;
    }

    private void ScanToken()
    {
        char c = Advance();
        switch (c)
        {
            case '(': AddToken(LEFT_PAREN); break;
            case ')': AddToken(RIGHT_PAREN); break;
            case '{': AddToken(LEFT_BRACE); break;
            case '}': AddToken(RIGHT_BRACE); break;
            case ',': AddToken(COMMA); break;
            case '.': AddToken(DOT); break;
            case '-': AddToken(MINUS); break;
            case '+': AddToken(PLUS); break;
            case ';': AddToken(SEMICOLON); break;
            case '*': AddToken(STAR); break; 
            case '!': 
                AddToken(Match('=') ? BANG_EQUAL : BANG);
                break;
            case '=':
                AddToken(Match('=') ? EQUAL_EQUAL : EQUAL);
                break;
            case '<':
                AddToken(Match('=') ? LESS_EQUAL : LESS);
                break;
            case '>':
                AddToken(Match('=') ? GREATER_EQUAL : GREATER);
                break;
            case '/':
                if (Match('/'))
                {
                    while (Peek() != '\n' && !IsAtEnd()) Advance();
                } else
                {
                    AddToken(SLASH);
                }
                break;

            case ' ':
            case '\r':
            case '\t':
                // Ignore whitespace
                break;
            
            case '\n':
                line++;
                break;

            case '"': TokenizeString(); break;

            default:
                if(IsDigit(c))
                {
                    TokenizeNumber();    
                } else if (IsAlpha(c)) 
                {
                    TokenizeIdentifier();    
                } else
                {
                    Lox.Error(line, $"Unexpected character '{c}'.");
                }
                break;
        };
    }

    private void TokenizeIdentifier()
    {
        while (IsAlphanumeric(Peek())) Advance();

        string text = source[start..current];
        bool isKeyword = keywords.TryGetValue(text, out var type);
        if (!isKeyword) type = IDENTIFIER;
        AddToken(type);
    }

    private void TokenizeNumber()
    {
        while(IsDigit(Peek())) Advance();

        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            // Consume the decimal point
            Advance();
            
            while(IsDigit(Peek())) Advance();
        }

        AddToken(NUMBER, double.Parse(source[start..current]));
    }

    private void TokenizeString()
    {
        while (Peek() != '"' && !IsAtEnd())
        {
            if (Peek() == '\n') line++;
            Advance();
        }

        if (IsAtEnd())
        {
            Lox.Error(line, "Unterminated string.");
            return;
        }

        Advance();
        string value = source[(start + 1)..(current-1)];
        AddToken(STRING, value);
    }

    private bool Match(char expected)
    {
        if (IsAtEnd()) return false;
        if (source[current] != expected) return false;

        current ++;
        return true;
    }

    private char Peek()
    {
        if (IsAtEnd()) return '\0';
        return source[current];
    }

    private char PeekNext()
    {
        if (current + 1 >= source.Length) return '\0';
        return source[current + 1];
    }

    private bool IsAlpha(char c)
    {
        return (c >= 'a' && c <= 'z') || 
               (c >= 'A' && c <= 'Z') || 
                c == '_';
    } 

    private bool IsAlphanumeric(char c)
    {
        return IsAlpha(c) || IsDigit(c);
    }

    private bool IsDigit(char c)
    {
        return c >= '0' && c <= '9';
    }

    private bool IsAtEnd()
    {
        return current >= source.Length;
    }

    private char Advance()
    {
        return source[current++];
    }

    private void AddToken(TokenType type)
    {
        AddToken(type, null);
    }

    private void AddToken(TokenType type, Object? literal)
    {
        string text = source[start..current];
        tokens.Add(new Token(type, text, literal, line));
    }
}