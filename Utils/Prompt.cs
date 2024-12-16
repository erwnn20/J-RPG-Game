using System.Reflection;

namespace JRPG_Game.Utils;

/// <summary>
/// A utility class for interactive console input and selection.
/// </summary>
public static class Prompt
{
    /// <summary>
    /// Displays a list of choices to the user and prompts them to select one.
    /// </summary>
    /// <typeparam name="T">The type of the choices.</typeparam>
    /// <param name="message">The message to display before the list of choices.</param>
    /// <param name="displayFunc">A function that formats each choice for display.</param>
    /// <param name="choices">A list of choices for the user to select from.</param>
    /// <returns>The index of the selected choice (1-based).</returns>
    /// <exception cref="IndexOutOfRangeException">Thrown if the choices list is empty.</exception>
    /// <remarks>
    /// If there is only one choice, it is automatically selected.
    /// </remarks>
    /// <example>
    /// <code>
    /// var choices = new List&lt;string&gt; { "Option 1", "Option 2", "Option 3" };
    /// int selectedIndex = Prompt.Select("Choose an option:", choice => choice, choices);
    /// Console.WriteLine($"Selected index: {selectedIndex}");
    /// </code>
    /// </example>
    public static int Select<T>(string message, Func<T, string> displayFunc, List<T> choices)
    {
        switch (choices.Count)
        {
            case 0:
                throw new IndexOutOfRangeException("You must have at least one choice");
            case 1:
                return 1;
        }

        Console.WriteLine(message);
        for (var i = 0; i < choices.Count; i++)
            Console.WriteLine($"    {i + 1} : {displayFunc(choices[i])}");

        return Get<int>("->", choice => choice < 1, choice => choice > choices.Count);
    }

    /// <summary>
    /// Displays a list of choices to the user and prompts them to select one.
    /// </summary>
    /// <typeparam name="T">The type of the choices.</typeparam>
    /// <param name="message">The message to display before the list of choices.</param>
    /// <param name="displayFunc">A function that formats each choice for display.</param>
    /// <param name="choices">An array of choices for the user to select from.</param>
    /// <returns>The index of the selected choice (1-based).</returns>
    /// <remarks>
    /// This overload converts the array of choices into a list and delegates to the other <see cref="Select{T}(string,System.Func{T,string},System.Collections.Generic.List{T})"/> method.
    /// </remarks>
    /// <example>
    /// <code>
    /// int selectedIndex = Prompt.Select("Choose an option:", choice => choice, "Option 1", "Option 2", "Option 3");
    /// Console.WriteLine($"Selected index: {selectedIndex}");
    /// </code>
    /// </example>
    public static int Select<T>(string message, Func<T, string> displayFunc, params T[] choices) =>
        Select(message, displayFunc, choices.ToList());

    //

    /// <summary>
    /// Prompts the user to input a value of a specific type and validates it.
    /// </summary>
    /// <typeparam name="T">The expected type of the input.</typeparam>
    /// <param name="message">The message displayed to the user.</param>
    /// <param name="excludedCondition">
    /// An optional condition to exclude invalid inputs. Defaults to no exclusions.
    /// </param>
    /// <returns>The validated input of type <typeparamref name="T"/>.</returns>
    /// <remarks>
    /// This method loops until the user provides a valid input that passes the exclusion conditions.
    /// </remarks>
    /// <example>
    /// <code>
    /// int age = Prompt.Get&lt;int&gt;("Enter your age:", value => value &lt; 0);
    /// Console.WriteLine($"Your age: {age}");
    /// </code>
    /// </example>
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

    /// <summary>
    /// Prompts the user to input a value of a specific type, ensuring it does not match any excluded values.
    /// </summary>
    /// <typeparam name="T">The expected type of the input.</typeparam>
    /// <param name="message">The message to be displayed to the user.</param>
    /// <param name="excluded">A list of values to be excluded from valid inputs.</param>
    /// <returns>The validated input of type <typeparamref name="T"/>.</returns>
    /// <remarks>
    /// This method ensures the input is not in the list of excluded values.
    /// </remarks>
    /// <example>
    /// <code>
    /// int age = Prompt.Get&lt;int&gt;("Enter your age:", new List&lt;int&gt; { 0, 120 });
    /// Console.WriteLine($"Your age is: {age}");
    /// </code>
    /// </example>
    public static T Get<T>(string message, List<T> excluded) where T : IConvertible =>
        Get<T>(message, excluded.Contains);

    /// <summary>
    /// Prompts the user to input a value of a specific type, ensuring it does not match any excluded values (provided as params).
    /// </summary>
    /// <typeparam name="T">The expected type of the input.</typeparam>
    /// <param name="message">The message to be displayed to the user.</param>
    /// <param name="excluded">A list of values to be excluded from valid inputs, provided as params.</param>
    /// <returns>The validated input of type <typeparamref name="T"/>.</returns>
    /// <remarks>
    /// This method ensures the input does not match any of the excluded values provided as arguments.<br/>
    /// Overload <see cref="Get{T}(string,List{T}?)"/>
    /// </remarks>
    /// <example>
    /// <code>
    /// string name = Prompt.Get&lt;string&gt;("Enter your name:", "Admin", "Guest");
    /// Console.WriteLine($"Your name is: {name}");
    /// </code>
    /// </example>
    public static T Get<T>(string message, params T[] excluded) where T : IConvertible =>
        Get(message, excluded.ToList());

    /// <summary>
    /// Prompts the user to input a value of a specific type, ensuring it does not meet any of the provided exclusion conditions.
    /// </summary>
    /// <typeparam name="T">The expected type of the input.</typeparam>
    /// <param name="message">The message to be displayed to the user.</param>
    /// <param name="excludedConditions">An array of conditions to exclude certain inputs.</param>
    /// <returns>The validated input of type <typeparamref name="T"/>.</returns>
    /// <remarks>
    /// This method continuously prompts the user until a valid input is entered that does not satisfy any of the exclusion conditions.
    /// </remarks>
    /// <example>
    /// <code>
    /// int age = Prompt.Get&lt;int&gt;("Enter your age:", value => value &lt; 0, value => value > 100);
    /// Console.WriteLine($"Your age is: {age}");
    /// </code>
    /// </example>
    public static T Get<T>(string message, params Func<T, bool>[] excludedConditions) where T : IConvertible =>
        Get<T>(message, value => excludedConditions.Any(condition => condition(value)));

    /// <summary>
    /// Prompts the user to input a value of a specific type, ensuring it does not match any of the excluded values (provided as params).
    /// </summary>
    /// <typeparam name="T">The expected type of the input.</typeparam>
    /// <param name="message">The message to be displayed to the user.</param>
    /// <param name="excludedCondition">A condition to exclude certain inputs based on their values.</param>
    /// <param name="excluded">An array of excluded values to be avoided.</param>
    /// <returns>The validated input of type <typeparamref name="T"/>.</returns>
    /// <remarks>
    /// This method ensures that the input does not match any of the excluded values provided as arguments.
    /// </remarks>
    /// <example>
    /// <code>
    /// int age = Prompt.Get&lt;int&gt;("Enter your age :", value => value &lt; 0, 0, 120);
    /// Console.WriteLine($"Your age is : {age}");
    /// </code>
    /// </example>
    public static T Get<T>(string message, Func<T, bool> excludedCondition, params T[] excluded)
        where T : IConvertible =>
        Get(message, excludedCondition, excluded.Contains);

    //

    /// <summary>
    /// Captures raw input from the user while applying exclusion conditions.
    /// </summary>
    /// <param name="message">An optional message to display before capturing input.</param>
    /// <param name="excludedCondition">
    /// A condition to exclude invalid inputs based on the key pressed. Defaults to no exclusions.
    /// </param>
    /// <returns>The user's input as a string.</returns>
    /// <remarks>
    /// This method handles input character by character, including handling backspace and Enter keys.
    /// </remarks>
    /// <example>
    /// <code>
    /// string name = Prompt.Input("Enter your name:");
    /// Console.WriteLine($"Hello, {name}!");
    /// </code>
    /// </example>
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
            else if (!new List<ConsoleKey>
                         {
                             ConsoleKey.Backspace, ConsoleKey.Tab, ConsoleKey.UpArrow, ConsoleKey.DownArrow,
                             ConsoleKey.LeftArrow, ConsoleKey.RightArrow
                         }
                         .Contains(key.Key) &&
                     !excludedCondition(key.Key))
            {
                input += key.KeyChar;
                Console.Write(key.KeyChar);
            }
        }

        return input;
    }

    /// <summary>
    /// Captures raw input while excluding specific keys (provided as params) during input.
    /// </summary>
    /// <param name="message">A message to display before capturing input.</param>
    /// <param name="excluded">An array of keys to exclude during input.</param>
    /// <returns>The user's input as a string.</returns>
    /// <remarks>
    /// This method ensures that the specified keys are excluded during input capture.
    /// </remarks>
    /// <example>
    /// <code>
    /// string input = Prompt.Input("Enter a command:", ConsoleKey.Delete, ConsoleKey.Escape);
    /// Console.WriteLine($"You entered: {input}");
    /// </code>
    /// </example>
    private static string Input(string message, params ConsoleKey[] excluded) =>
        Input(message, excluded.Contains);

    /// <summary>
    /// Captures raw input while excluding keys based on multiple conditions (provided as params).
    /// </summary>
    /// <param name="message">A message to display before capturing input.</param>
    /// <param name="excludedConditions">An array of conditions to exclude certain keys during input.</param>
    /// <returns>The user's input as a string.</returns>
    /// <remarks>
    /// This method applies multiple exclusion conditions based on the key pressed.
    /// </remarks>
    /// <example>
    /// <code>
    /// string input = Prompt.Input("Enter a command:", key => key == ConsoleKey.Delete, key => key == ConsoleKey.Backspace);
    /// Console.WriteLine($"You entered: {input}");
    /// </code>
    /// </example>
    private static string Input(string message, params Func<ConsoleKey, bool>[] excludedConditions) =>
        Input(message, i => excludedConditions.Any(condition => condition(i)));

    /// <summary>
    /// Captures raw input while applying a custom exclusion condition and also excluding specific keys (provided as params).
    /// </summary>
    /// <param name="message">A message to display before capturing input.</param>
    /// <param name="excludedCondition">A condition to exclude certain keys based on the key pressed.</param>
    /// <param name="excluded">An array of specific keys to be excluded from input.</param>
    /// <returns>The user's input as a string.</returns>
    /// <remarks>
    /// This method applies a custom exclusion condition and also checks if specific keys are excluded.
    /// </remarks>
    /// <example>
    /// <code>
    /// string input = Prompt.Input("Enter a command:", key => key == ConsoleKey.Delete, ConsoleKey.F1, ConsoleKey.F2);
    /// Console.WriteLine($"You entered: {input}");
    /// </code>
    /// </example>
    private static string Input(string message, Func<ConsoleKey, bool> excludedCondition,
        params ConsoleKey[] excluded) =>
        Input(message, excludedCondition, excluded.Contains);

    /// <summary>
    /// Retrieves all concrete subclasses of a specified base type within the current assembly.
    /// </summary>
    /// <param name="baseType">The base type for which to find subclasses.</param>
    /// <returns>
    /// A list of <see cref="System.Type"/> objects representing all non-abstract subclasses 
    /// of the specified base type found in the current assembly.
    /// </returns>
    /// <exception cref="System.ArgumentNullException">
    /// Thrown if the <paramref name="baseType"/> parameter is null.
    /// </exception>
    /// <example>
    /// Example usage:
    /// <code>
    /// List&lt;Type&gt; subclasses = GetAllSubclassesOf(typeof(Character));
    /// foreach (var type in subclasses)
    /// {
    ///     Console.WriteLine(type.Name);
    /// }
    /// </code>
    /// </example>
    public static List<Type> GetAllSubclassesOf(Type baseType)
    {
        return Assembly.GetExecutingAssembly().GetTypes()
            .Where(type =>
                type is { IsClass: true, IsAbstract: false } && type.IsSubclassOf(baseType))
            .ToList();
    }
}