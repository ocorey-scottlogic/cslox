
namespace CsLox;

public class Token(TokenType type, String lexeme, Object? literal, int line)
{
    public TokenType Type { get; } = type;
    public string Lexeme { get; } = lexeme;
    public Object? Literal { get; } = literal;
    public int Line { get; } = line;

    public override string ToString()
    {
        return $"{Type} {Lexeme} {Literal}";
    }
}