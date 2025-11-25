using Lox.Parser;
using Lox.Parser.Visitors;
using Lox.Scanner;

internal class Program
{
    private static bool _hadError = false;

    private static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: cs-lox [script]");
            Environment.Exit(64);
        }
        else if (args.Length == 1)
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
        using var fileReader = new StreamReader(path);
        Run(fileReader.ReadToEnd());
        if (_hadError)
        {
            Environment.Exit(65);
        }
    }

    private static void RunPrompt()
    {
        while (true)
        {
            Console.Write("> ");
            if (Console.ReadLine() is string line)
            {
                Run(line);
                _hadError = false;
            }
            else
            {
                break;
            }
        }
    }

    private static void Run(string source)
    {
        try
        {
            var parser = new Parser([.. new Scanner(source).Scan()]);
            var expr = parser.Parse();
            if (expr is null || parser.AccumulatedErrors.Any())
            {
                ReportParseErrors(parser.AccumulatedErrors);
                return;
            }
            Console.WriteLine(new AstPrinter().Print(expr));
        }
        catch (ScannerException scanException)
        {
            Report(scanException.Line, string.Empty, scanException.Message);
        }
    }

    private static void ReportParseErrors(IEnumerable<ParseError> errors)
    {
        foreach (var error in errors)
        {
            Report(error.Token, error.Message);
            return;
        }
    }

    private static void Report(Token token, string message)
    {
        Report(token.Line, token.Type == TokenType.Eof ? " at end" : $" at '{token.Lexeme}'", message);
    }

    private static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
        _hadError = true;
    }
}