using System.Security.Principal;

namespace CsLox;

public class Lox
{
    static bool hadError = false;
    public static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: cslox [script]");
            Environment.Exit(64);
        } else if (args.Length == 1)
        {
            RunFile(args[0]);
        }
        else
        {
            RunPrompt();
        }
    }

    private static void RunFile(string path)
    {
        byte[] bytes = File.ReadAllBytes(path);
        Run(System.Text.Encoding.UTF8.GetString(bytes));
        if (hadError) Environment.Exit(65);
    }

    private static void RunPrompt()
    {
        while (true)
        {
            Console.WriteLine("cslox > ");
            string line = Console.ReadLine();
            if (line is null) break;
            Run(line);
            hadError = false;
        }
    }

    private static void Run(String source)
    {
        Scanner scanner = new Scanner(source);
        List<Token> tokens = scanner.scanTokens();

        foreach (var token in tokens)
        {
            Console.WriteLine(token);
        }
    }

    static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    private static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[line {line}] Error {where}: {message}");
        hadError = true;
    }
}