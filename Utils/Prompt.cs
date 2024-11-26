namespace JRPG_Game.Utils;

public static class Prompt
{
    public static int Select(string message, List<object> choices)
    {
        switch (choices.Count)
        {
            case 0:
                throw new IndexOutOfRangeException("You must have at least one choice");
            case 1:
                return 0;
        }

        Console.WriteLine(message);
        for (var i = 0; i < choices.Count; i++)
            Console.WriteLine($"\t{i + 1} : {choices[i]}");

        while (true)
        {
            Console.Write("-> ");
            _ = int.TryParse(Input(), out var choice);

            if (0 < choice && choice < choices.Count)
            {
                Console.WriteLine();
                return choice;
            }

            Console.WriteLine(" - Entrée invalide");
        }
    }

    public static int Select(string message, params object[] choices) => Select(message, choices.ToList());

    public static string GetValidInput(string message, List<string>? excluded = null)
    {
        excluded ??= [];

        while (true)
        {
            Console.Write($"{message} ");
            var input = Input();

            if (string.IsNullOrWhiteSpace(input))
                Console.WriteLine(" - Entrée invalide");
            else if (excluded.Contains(input, StringComparer.OrdinalIgnoreCase))
                Console.WriteLine($" - '{input}' est interdit. Veuillez en saisir un autre.");
            else return input;
        }
    }

    public static string GetValidInput(string message, params string[] excluded) =>
        GetValidInput(message, excluded.ToList());

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