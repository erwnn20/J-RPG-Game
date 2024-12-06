namespace JRPG_Game.Utils;

/// <summary>
/// Represents a container for numeric values with a minimum, current, and maximum value.
/// Provides methods to add or subtract values while respecting the defined limits.
/// </summary>
public class NumericContainer
{
    public int Min { get; private set; }
    public int Current { get; private set; }
    public int Max { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NumericContainer"/> class.
    /// Ensures that the minimum value is not greater than the maximum value and adjusts the current value
    /// to be within the valid range defined by the minimum and maximum values.
    /// </summary>
    /// <param name="min">The minimum value allowed in the container.</param>
    /// <param name="current">The initial current value of the container.</param>
    /// <param name="max">The maximum value allowed in the container.</param>
    /// <exception cref="ArgumentException">Thrown if <paramref name="min"/> is greater than <paramref name="max"/>.</exception>
    public NumericContainer(int min, int current, int max)
    {
        if (min > max) throw new ArgumentException("Minimum value cannot be greater than the maximum value.");

        Min = min;
        Max = max;

        Current = Math.Clamp(current, Min, Max);
    }

    /// <summary>
    /// Adds a specified value to the current value while ensuring it does not exceed the maximum value.
    /// </summary>
    /// <param name="add">The amount to add.</param>
    /// <returns>The actual amount added to the current value.</returns>
    public int Add(int add)
    {
        var added = Math.Min(Max - Current, add);
        Current += added;
        return added;
    }

    /// <summary>
    /// Subtracts a specified value from the current value while ensuring it does not fall below the minimum value.
    /// </summary>
    /// <param name="sub">The amount to subtract.</param>
    /// <returns>The actual amount subtracted from the current value.</returns>
    public int Subtract(int sub)
    {
        var subtracted = Math.Min(Current - Min, sub);
        Current -= subtracted;
        return subtracted;
    }

    /// <returns>A string representation of the numeric container in the format "Min/Current/Max".</returns>
    public override string ToString() => $"{Min}/{Current}/{Max}";
}