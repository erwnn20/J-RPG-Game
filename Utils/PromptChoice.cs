namespace JRPG_Game.Utils;

public class PromptChoice(string message, params object[] choices)
{
    public PromptChoice(string message) : this(message, [])
    {
    }

    public PromptChoice(params object[] choices) : this(string.Empty, choices)
    {
    }

    private string Message { get; set; } = message;
    private List<object> Choices { get; set; } = [..choices];

    public int MakeChoice()
    {
        Console.WriteLine(Message);
        for (var i = 0; i < Choices.Count; i++)
            Console.WriteLine($"\t{i + 1} : {Choices[i]}");
        do
        {
            Console.Write("-> ");
            _ = int.TryParse(Input(), out var choice);

            if (0 < choice && choice < Choices.Count)
            {
                Console.WriteLine();
                return choice;
            }

            Console.WriteLine(" - Entrée invalide");
        } while (true);
    }

    private static string Input()
    {
        var input = "";
        while (true)
        {
            var key = Console.ReadKey(intercept: true);

            if (key.Key == ConsoleKey.Enter) break;
            if (key.Key == ConsoleKey.Backspace && input.Length > 0)
            {
                input = input[..^1];
                Console.Write("\b \b");
            }
            else
            {
                input += key.KeyChar;
                Console.Write(key.KeyChar);
            }
        }

        return input;
    }
}