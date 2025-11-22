using Lox.Scanner;

internal class Program
{
    private static bool _hadError = false;

    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }

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
            foreach (var token in new Scanner(source).Scan())
            {
                Console.WriteLine(token);
            }
        }
        catch (ScannerException scanException)
        {
            Error(scanException.Line, scanException.Message);
        }
    }

    private static void Report(int line, string where, string message)
    {
        Console.Error.WriteLine($"[line {line}] Error{where}: {message}");
        _hadError = true;
    }
}