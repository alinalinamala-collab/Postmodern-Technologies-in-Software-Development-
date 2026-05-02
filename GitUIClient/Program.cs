using Spectre.Console;
using GitUIClient.Services;

IGitService gitService = new GitService();

while (true)
{
    AnsiConsole.Clear();

    AnsiConsole.Write(
        new FigletText("Git UI Client")
            .LeftJustified()
            .Color(Color.Cyan1));

    var action = AnsiConsole.Prompt(
        new SelectionPrompt<string>()
            .Title("\n[white]Оберіть дію:[/]")
            .PageSize(6)
            .HighlightStyle(new Style(foreground: Color.Green))
            .AddChoices(new[] {
                "1. Переглянути статус (Status)", 
                "2. Додати всі зміни до індексу (Add .)", 
                "3. Зробити коміт (Commit)",
                "4. Переглянути історію (Log)",
                "0. Вихід"
            }));

    try
    {
        if (action.Contains("1"))
        {
            AnsiConsole.MarkupLine("[bold yellow]Статус репозиторію:[/]");
            var status = gitService.GetStatus();
            if (string.IsNullOrWhiteSpace(status))
                AnsiConsole.MarkupLine("[grey]Немає змін. Робоче дерево чисте.[/]");
            else
                Console.WriteLine(status);
        }
        else if (action.Contains("2"))
        {
            gitService.Add();
            AnsiConsole.MarkupLine("[bold green]Всі файли успішно додані до індексу (staged).[/]");
        }
        else if (action.Contains("3"))
        {
            var message = AnsiConsole.Ask<string>("[bold cyan]Введіть повідомлення для коміту:[/] ");
            var result = gitService.Commit(message);
            AnsiConsole.MarkupLine("[bold green]Коміт створено![/]");
            Console.WriteLine(result); }
        else if (action.Contains("4"))
        {
            AnsiConsole.MarkupLine("[bold yellow]Останні коміти:[/]");
            var log = gitService.GetLog();
            Console.WriteLine(log);
        }
        else if (action.Contains("0"))
        {
            AnsiConsole.MarkupLine("[bold cyan]Роботу завершено. До побачення![/]");
            break; 
        }
        }
    catch (Exception ex)
    {
        AnsiConsole.MarkupLine("\n[bold red]Помилка Git:[/]");
        Console.WriteLine(ex.Message);
    }
    AnsiConsole.MarkupLine("\n[grey]Натисніть будь-яку клавішу для повернення в меню...[/]");
    Console.ReadKey(true);
}