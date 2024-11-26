namespace JRPG_Game.Utils;

public static class Prompt
{
    private static int Select(string message, List<object> choices)
    {
        Console.WriteLine(message);
        for (var i = 0; i < choices.Count; i++)
            Console.WriteLine($"\t{i + 1} : {choices[i]}");
        do
        {
            Console.Write("-> ");
            _ = int.TryParse(Input(), out var choice);

            if (0 < choice && choice < choices.Count)
            {
                Console.WriteLine();
                return choice;
            }

            Console.WriteLine(" - Entrée invalide");
        } while (true);
    }

    public static int Select(string message, params object[] choices) => Select(message, choices.ToList());

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