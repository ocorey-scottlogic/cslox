
namespace CsLox;

class Token(TokenType type, String lexeme, Object? literal, int line)
{
    readonly TokenType Type = type;
    readonly string Lexeme = lexeme;
    readonly Object? Literal = literal;
    readonly int Line = line;

    public override string ToString()
    {
        return $"{type} {Lexeme} {Literal}";
    }
}