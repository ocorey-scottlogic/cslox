
namespace CsLox;

class Token(TokenType type, String lexeme, Object literal, int line)
{
    sealed TokenType Type = type;
    readonly string Lexeme = lexeme;
    readonly Object Literal = literal;
    readonly int Line = line;

    public string ToString()
    {
        return $"{type} {Lexeme} {Literal}";
    }
}