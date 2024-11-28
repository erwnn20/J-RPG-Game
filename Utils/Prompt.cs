namespace JRPG_Game.Utils;

public static class Prompt
{
    public static int Select<T>(string message, Func<T, string> displayFunc, List<T> choices)
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
            Console.WriteLine($"\t{i + 1} : {displayFunc(choices[i])}");

        return Get<int>("-> ", choice => choice < 1, choice => choice > choices.Count);
    }

    public static int Select<T>(string message, Func<T, string> displayFunc, params T[] choices) =>
        Select(message, displayFunc, choices.ToList());

    //

    public static T Get<T>(string message, Func<T, bool>? excludedCondition = null) where T : IConvertible
    {
        excludedCondition ??= _ => false;

        while (true)
        {
            Console.Write($"{message} ");
            var input = Input();

            try
            {
                var value = (T)Convert.ChangeType(input, typeof(T));

                if (excludedCondition(value) || (value is string s && string.IsNullOrWhiteSpace(s)))
                {
                    Console.WriteLine($" - '{input}' n'est pas une entrée valide. Veuillez en saisir un autre.");
                    continue;
                }

                Console.WriteLine();
                return value;
            }
            catch
            {
                Console.WriteLine($" - Entrée invalide.");
            }
        }
    }

    public static T Get<T>(string message, List<T> excluded) where T : IConvertible =>
        Get<T>(message, excluded.Contains);

    public static T Get<T>(string message, params T[] excluded) where T : IConvertible =>
        Get(message, excluded.ToList());

    public static T Get<T>(string message, params Func<T, bool>[] excludedConditions) where T : IConvertible =>
        Get<T>(message, value => excludedConditions.Any(condition => condition(value)));

    public static T Get<T>(string message, Func<T, bool> excludedCondition, params T[] excluded)
        where T : IConvertible =>
        Get(message, excludedCondition, excluded.Contains);

    //

    public static bool GetBool(string message, string trueValue, string falseValue)
    {
        do
        {
            var input = trueValue.Length == 1 && falseValue.Length == 1
                ? Input(message,
                    key => key != (ConsoleKey)char.ToUpper(trueValue[0])
                           && key != (ConsoleKey)char.ToUpper(falseValue[0]))
                : Get<string>(message,
                    s => !s.Equals(trueValue, StringComparison.CurrentCultureIgnoreCase)
                         && !s.Equals(falseValue, StringComparison.CurrentCultureIgnoreCase));

            if (input.Equals(trueValue, StringComparison.CurrentCultureIgnoreCase)) return true;
            if (input.Equals(falseValue, StringComparison.CurrentCultureIgnoreCase)) return false;

            Console.WriteLine($" - '{input}' n'est pas une valide.");
        } while (true);
    }

    //

    public static string Input(string? message = null, Func<ConsoleKey, bool>? excludedCondition = null)
    {
        excludedCondition ??= _ => false;

        if (message != null) Console.Write($"{message} ");

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
            else if (key.Key != ConsoleKey.Backspace && !excludedCondition(key.Key))
            {
                input += key.KeyChar;
                Console.Write(key.KeyChar);
            }
        }

        return input;
    }

    public static string Input(string message, params ConsoleKey[] excluded) =>
        Input(message, excluded.Contains);

    public static string Input(string message, params Func<ConsoleKey, bool>[] excludedConditions) =>
        Input(message, i => excludedConditions.Any(condition => condition(i)));

    public static string Input(string message, Func<ConsoleKey, bool> excludedCondition,
        params ConsoleKey[] excluded) =>
        Input(message, excludedCondition, excluded.Contains);
}