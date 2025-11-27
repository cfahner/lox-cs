using Lox.Interpreter;
using Lox.Parser;
using Lox.Scanner;

internal class Program
{
    private static bool _hadError = false;

    private static bool _hadRuntimeError = false;

    private static readonly Interpreter _interpreter = new();

    private static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: cs-lox [script]");
            System.Environment.Exit(64);
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
            System.Environment.Exit(65);
        }
        if (_hadRuntimeError)
        {
            System.Environment.Exit(70);
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
                _hadRuntimeError = false;
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
            var stmts = parser.Parse();
            try
            {
                // stmts is a generator function, so the stack only gets unwound when it is actually used
                // which is why the Interpret() step needs to be try-catched, and not Parse()
                _interpreter.Interpret(stmts);
            }
            catch (ParseError)
            {
            }
            if (parser.AccumulatedErrors.Any())
            {
                ReportParseErrors(parser.AccumulatedErrors);
            }
        }
        catch (ScannerException scanException)
        {
            Report(scanException.Line, string.Empty, scanException.Message);
        }
        catch (RuntimeError runTimeError)
        {
            Report(runTimeError);
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

    private static void Report(RuntimeError runTimeError)
    {
        Console.Error.WriteLine($"{runTimeError.Message}\n[line {runTimeError.Token.Line}]");
        _hadRuntimeError = true;
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