
using CodingTracker.Data;
using CodingTracker.Services;
using Spectre.Console;
using System.Diagnostics;
using static CodingTracker.InputValidator;

namespace CodingTracker.Controller;

public class CodingSessionController : ICodingSessionController
{
    private readonly ICodingSessionService service;
    private readonly JokeService jService = new ();

    public CodingSessionController(ICodingSessionService service)
    {
        this.service = service;
    }

    public async Task AddSession()
    {
        PrintDateHelp();
        DateTime startDate = PromptDate($"Type the starting date of your session ([yellow]{dateFormat}[/]):");

        DateTime endDate = DateTime.MinValue;
        while (true)
        {
            PromptDate($"Type the end date of your session ([yellow]{dateFormat}[/]):");
            if (endDate > startDate) break;
            else
            {
                AnsiConsole.MarkupLine("\n\n[red]Ending date should be bigger than starting date.\n\n");
            }
        }

        var session = new CodingSession { StartTime = startDate, EndTime = endDate };
        try
        {
            await service.InsertSession(session);
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine($"\n\n[red]{e.Message}[/]\n\n");
            return;
        }
        AnsiConsole.MarkupLine("\n\n[green]Session inserted successfully![/]\n\n");
    }
    public async Task DeleteSession()
    {
        int ID = AnsiConsole.Prompt(new TextPrompt<int>("Type the id of the session you want to delete"));
        try
        {
            await service.DeleteSession(ID);
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine($"\n\n[red]{e.Message}[/]\n\n");
            return;
        }
        AnsiConsole.MarkupLine("\n\n[yellow]Session deleted.[/]\n\n");
    }
    public async Task UpdateSession()
    {
        CodingSession session;
        int ID = AnsiConsole.Prompt(new TextPrompt<int>("Type the id of the session you want to update: "));
        try
        {
            session = await service.GetSessionByID(ID) ?? throw new Exception("Session not found");
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine($"\n\n[red]{e.Message}[/]\n\n");
            return;
        }

        PrintDateHelp();
        session.StartTime = PromptDate($"""
            The current value is: [yellow]{session.StartTime}[/]

            Type the new [blue]start time[/]: 
            """);
        session.EndTime = PromptDate($"""
            The current value is: [yellow]{session.EndTime}[/]

            Type the new [blue]end time[/]: 
            """);
        try
        {
            await service.UpdateSession(session);
        }
        catch (Exception e)
        {
            AnsiConsole.MarkupLine($"\n\n{e.Message}\n\n");
            return;
        }
        AnsiConsole.MarkupLine($"\n\n[green]Session updated![/]\n\n");
    }

    public async Task FilterSessions()
    {
        List<CodingSession>? sessions = null;
        DateTime startDate = DateTime.Now;
        DateTime endDate = DateTime.Now;
        while (sessions is null)
        {
            PrintDateHelp();

            startDate = PromptDate("[bold]Please type the [blue]start date[/][/]: ");
            endDate = PromptDate("[bold]Please type the [blue]end date[/][/]: ");
            var orderDesc = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Choose your ordering.")
                    .AddChoices(["Descending", "Ascending"])) == "Descending";
            try
            {
                sessions = await service.FilterSessionsByDate(startDate, endDate, orderDesc);
            }
            catch (Exception e)
            {
                AnsiConsole.MarkupLine($"[red]{e.Message}[/]");
            }
        }
        var table = new Table().Centered();
        table.AddColumn("[bold]ID[/]");
        table.AddColumn("[blue]Start Time[/]");
        table.AddColumn("[yellow]End Time[/]");
        table.Title($"[bold]CODING SESSIONS[/]");
        foreach (var session in sessions)
        {
            table.AddRow($"[bold]{session.ID}[/]", session.StartTime.ToString(), session.EndTime.ToString());
        }
        AnsiConsole.Write(table);
    }

    public async Task CodingReport()
    {
        Color[] colors = [Color.Red1, Color.MediumPurple1, Color.Gold1, Color.Green1, Color.Blue1, Color.Orange1, Color.Olive];

        string dateGrouping = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                .Title("How would you like to group your hours?")
                .AddChoices(Enum.GetNames<DateGrouping>())
             );

        string reportGrouping = AnsiConsole.Prompt(
                        new SelectionPrompt<string>()
                        .Title("How would you like the total?")
                        .AddChoices(Enum.GetNames<ReportGrouping>())
                     );

        PrintDateHelp();
        DateTime startTime = PromptDate("Type the [bold]starting time[/]: ");
        DateTime endTime = PromptDate("Type the [bold]ending time[/]: ");

        var reports = await service.GroupByDate(startTime, endTime,
            Enum.Parse<DateGrouping>(dateGrouping),
            Enum.Parse<ReportGrouping>(reportGrouping));

        if (reports is null || reports.Count == 0)
        {
            AnsiConsole.MarkupLine($"\n[yellow]Nothing to report[/]\n");
            return;
        }

        var barChart = new BarChart()
            .Width(60)
            .Label("[green bold underline]CODING REPORT (in hours)[/]\n\n")
            .CenterLabel();

        barChart.UseValueFormatter((val) => val.ToString("N"));

        for (int i = 0; i < reports.Count; i++)
        {
            barChart.AddItem(reports[i].Period, reports[i].Duration.TotalHours, colors[i % colors.Length]);
        }

        AnsiConsole.Write(barChart);
    }

    public async Task HearAJoke()
    {
        // Why was joke null?
        // Because the programmer did get the joke! :D
        var joke = jService.GetJoke();
        AnsiConsole.MarkupLine(joke.Question);
        Console.ReadKey();
        AnsiConsole.MarkupLine($"[bold]{joke.Answer}[/]");
        AnsiConsole.Markup("\nBa-dum...tish! Press any button to continue.");
        Console.ReadKey();
        AnsiConsole.MarkupLine("");
    }

    public async Task LiveSession()
    {
        var stopwatch = Stopwatch.StartNew();
        var startTime = DateTime.Now;

        var contentPanel = new Table().Centered();
        contentPanel.AddColumn("");
        // I couldn't find a way to dynamically update the headers so I am just hiding them... :)
        contentPanel.HideHeaders();

        var timeSpan = stopwatch.Elapsed;

        contentPanel.AddRow("[bold]Coding Session[/]").Centered();
        contentPanel.AddRow("--------------------");
        contentPanel.AddRow($"Time: [yellow]{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}[/]");
        contentPanel.AddRow("--------------------");
        contentPanel.AddRow("[green]S = Save & Quit[/]");
        contentPanel.AddRow("[red]Q = Quit without saving[/]");

        await AnsiConsole.Live(contentPanel)
            .AutoClear(false)
            .Overflow(VerticalOverflow.Ellipsis)
            .StartAsync(async ctx =>
            {
                while (true)
                {
                    if (Console.KeyAvailable)
                    {
                        var key = Console.ReadKey(true).KeyChar;
                        if (key == 'q') return;
                        else if (key == 's')
                        {
                            contentPanel.UpdateCell(0, 0, "[green]Saving...[/]");
                            ctx.Refresh();
                            try
                            {
                                await service.InsertSession(new CodingSession { StartTime = startTime, EndTime = DateTime.Now });
                            }
                            catch (Exception e)
                            {
                                AnsiConsole.WriteLine($"[red]{e.Message}[/]");
                            }
                            await Task.Delay(1500);
                            return;
                        }
                    }
                    timeSpan = stopwatch.Elapsed;
                    contentPanel.UpdateCell(2, 0, $"Time: [yellow]{timeSpan.Hours:D2}:{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}[/]");

                    ctx.Refresh();
                    await Task.Delay(100);
                }
            });
    }
}
