using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;

namespace Hyde.Logging;

internal class ColoredConsoleFormatter : ConsoleFormatter
{
    public ColoredConsoleFormatter() : base("testFormatter") { }

    public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider scopeProvider, TextWriter textWriter)
    {
        var message = logEntry.Formatter?.Invoke(logEntry.State, logEntry.Exception);
        if (message is null) { return; }

        textWriter.Write(DateTime.UtcNow.ToString("hh:mm:ss.fff"));
        textWriter.Write(" ");
        textWriter.WriteColored($"[{logEntry.LogLevel}]", GetLogLevelConsoleColors(logEntry.LogLevel));
        textWriter.Write(" ");
        textWriter.WriteColored($"[{logEntry.Category.Split('.').Last()}]", new ConsoleColors(ConsoleColor.Cyan, ConsoleColor.Black));
        textWriter.Write(" ");
        textWriter.Write($"{message}");
        textWriter.Write(Environment.NewLine);
    }

    private static ConsoleColors GetLogLevelConsoleColors(LogLevel logLevel) =>
        logLevel switch
        {
            LogLevel.Trace => new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black),
            LogLevel.Debug => new ConsoleColors(ConsoleColor.Gray, ConsoleColor.Black),
            LogLevel.Information => new ConsoleColors(ConsoleColor.DarkGreen, ConsoleColor.Black),
            LogLevel.Warning => new ConsoleColors(ConsoleColor.Yellow, ConsoleColor.Black),
            LogLevel.Error => new ConsoleColors(ConsoleColor.Black, ConsoleColor.DarkRed),
            LogLevel.Critical => new ConsoleColors(ConsoleColor.White, ConsoleColor.DarkRed),
            _ => new ConsoleColors(null, null)
        };
}