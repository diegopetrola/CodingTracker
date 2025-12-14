using CodingTracker.Data;
using Spectre.Console;
using System.Globalization;
using System.Text.RegularExpressions;

namespace CodingTracker;

public static class InputValidator
{
    public static readonly string dateFormat = "dd/MM/yy HH:mm:ss";
    public static readonly string dateRegex = @"-?[0-9]+[m|d|w]|^now$";
    public static void PrintDateHelp()  
    {
        AnsiConsole.MarkupLine($"""
                            [bold]Help when typing dates[/]

                    1. Type in the format "[bold]{dateFormat}[/]"
                    2. Type an integer finishing with [bold]d[/],[bold]w[/] or [bold]m[/] to get
                        Today - [bold]units[/] where [bold]units[/] is [bold]d[/]ay, [bold]w[/]eek and [bold]m[/]onth respectively.
                    3. Type [bold]now[/] to get the current time

                Example:
                    -5d => Today - 5 days
                    10w => Today + 10 weeks
                    5 m => [red]invalid input[/], the number and the character must be together!

                """);
    }
    public static bool ValidateSession(CodingSession session)
    {
        if (session.EndTime <= session.StartTime)
        {
            throw new ArgumentException("End time must be after Start time.");
        }
        if (session.StartTime > DateTime.Now)
        {
            throw new ArgumentException("Cannot log sessions in the future.");
        }
        return true;
    }
    public static DateTime PromptDate(string message)
    {
        var dateString = AnsiConsole.Prompt(
            new TextPrompt<string>(message)
            .Validate(input =>
            {
                if (Regex.IsMatch(input ?? "", dateRegex, RegexOptions.IgnoreCase))
                    return ValidationResult.Success();
    
                var isValid = DateTime.TryParseExact(
                    input,
                    dateFormat,
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out _);

                return isValid
                    ? ValidationResult.Success()
                    : ValidationResult.Error($"[red]Invalid format.[/]");
            }));

        if (dateString.ToLower() == "now") return DateTime.Now;
        if (int.TryParse(dateString.AsSpan(0, dateString.Length - 1), out int val))
        {
            return dateString[^1] switch
            {
                'm' => DateTime.Now.AddMonths(val),
                'w' => DateTime.Now.AddDays(val * 7),
                _ => DateTime.Now.AddDays(val),
            };
        }

        return DateTime.ParseExact(dateString, dateFormat, CultureInfo.InvariantCulture);
    }
}

public enum DateGrouping
{
    Week,
    Month
}

public enum ReportGrouping
{
    Total,
    Average,
}
