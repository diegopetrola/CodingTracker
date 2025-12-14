using CodingTracker.Controller;
using Spectre.Console;

namespace CodingTracker;

static class UserInterface
{
    public async static Task MainMenu(CodingSessionController controller)
    {
        Dictionary<string, Func<Task>> menuActions = new()
        {
            { "Start Live Session", controller.LiveSession },
            { "Add Session", controller.AddSession },
            { "Delete Session", controller.DeleteSession },
            { "Update Session", controller.UpdateSession },
            { "Filter Sessions", controller.FilterSessions },
            { "Coding Report", controller.CodingReport },
            { "Hear a Joke", controller.HearAJoke},
            { "Quit", () => { Environment.Exit(0); return Task.CompletedTask; } }
        };

        while (true)
        {
            AnsiConsole.WriteLine();
            var selectionString = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("[bold]What would you like to do?[/]")
                    .AddChoices(menuActions.Keys));

            await menuActions[selectionString]();
        }
    }
}
