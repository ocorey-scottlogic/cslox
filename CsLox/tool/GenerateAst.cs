namespace CsLox.Tool;

public class GenerateAst
{
    public static void Main(String[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Usage: generate_ast <output directory>");
        }
        string outputDir = args[0];
        DefineAst(outputDir, "Expr", new List<string>
        {
            "Binary   : Expr Left, Token OperatorToken, Expr Right",
            "Grouping : Expr Expression",
            "Literal  : Object Value",
            "Unary    : Token OperatorToken, Expr Right"
        });
    } 

    private static void DefineAst(string outputDir, string baseName, List<string> types)
    {
        string path = $"{outputDir}/{baseName}.cs";

        using (StreamWriter writer = new(path))
        {
            writer.WriteLine("namespace CsLox;");
            writer.WriteLine();
            writer.WriteLine("public abstract class "+ baseName + " {");

            DefineVisitor(writer, baseName, types);

            foreach (var type in types)
            {
                string className = type.Split(":")[0].Trim();
                string fields = type.Split(":")[1].Trim();
                DefineType(writer, baseName, className, fields);
            }

            writer.WriteLine();
            writer.WriteLine("  public abstract TResult Accept<TResult>(IVisitor<TResult> visitor);");

            writer.WriteLine("}");
        }
    }

    private static void DefineVisitor(StreamWriter writer, string baseName, List<string> types)
    {
        writer.WriteLine("  public interface IVisitor<TResult> {");

        foreach (string type in types)
        {
            string typeName = type.Split(":")[0].Trim();
            writer.WriteLine("    public TResult Visit" + typeName + baseName + "(" + typeName + " " + baseName.ToLower() + ");");
        }

        writer.WriteLine("  }");
    }

    private static void DefineType(
        StreamWriter writer, string baseName, string className, string fieldList
    )
    {
        writer.WriteLine(" public class " + className + " : " + baseName + " {");
        
        //Constructor
        writer.WriteLine("    public " + className + "(" + fieldList + ") {");

        // Store parameters in fields.
        string[] fields = fieldList.Split(", ");
        foreach (string field in fields)
        {
            string name = field.Split(" ")[1];
            writer.WriteLine("        this." + name + " = " + name + ";");
        }

        writer.WriteLine("    }");

        writer.WriteLine("    public override TResult Accept<TResult>(IVisitor<TResult> visitor) {");
        writer.WriteLine("      return visitor.Visit" + className + baseName + "(this);");
        writer.WriteLine("    }");

        writer.WriteLine();
        foreach (string field in fields)
        {
            writer.WriteLine("    public " + field + " { get; }");
        }

        writer.WriteLine("  }");
        
    }
}