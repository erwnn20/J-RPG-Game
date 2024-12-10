namespace JRPG_Game.Utils;

/// <summary>
/// Represents a container for a numeric value with defined minimum, maximum, and current values.
/// </summary>
/// <typeparam name="T">
/// The type of the numeric value. Must be a value type that implements <see cref="IComparable{T}"/> and <see cref="IComparable"/>.
/// </typeparam>
public class NumericContainer<T> where T : struct, IComparable<T>, IComparable
{
    private T Min { get; }
    public T Current { get; private set; }
    public T Max { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="NumericContainer{T}"/> class.
    /// </summary>
    /// <param name="min">The minimum value allowed in the container.</param>
    /// <param name="current">The initial current value of the container.</param>
    /// <param name="max">The maximum value allowed in the container.</param>
    /// <exception cref="ArgumentException">
    /// Thrown when <paramref name="min"/> is greater than <paramref name="max"/>.
    /// </exception>
    public NumericContainer(T min, T current, T max)
    {
        if (min.CompareTo(max) > 0)
            throw new ArgumentException("Minimum value cannot be greater than the maximum value.");

        Min = min;
        Max = max;
        Current = Clamp(current, Min, Max);
    }

    /// <summary>
    /// Adds the specified value to the current value while respecting the maximum constraint.
    /// </summary>
    /// <param name="value">The value to add.</param>
    /// <returns>The actual value added to <see cref="Current"/>.</returns>
    public T Add(T value)
    {
        var result = AddValues(Current, value);
        var clamped = Clamp(result, Min, Max);
        var added = SubtractValues(clamped, Current);
        Current = clamped;
        return added;
    }

    /// <summary>
    /// Subtracts the specified value from the current value while respecting the minimum constraint.
    /// </summary>
    /// <param name="value">The value to subtract.</param>
    /// <returns>The actual value subtracted from <see cref="Current"/>.</returns>
    public T Subtract(T value)
    {
        var result = SubtractValues(Current, value);
        var clamped = Clamp(result, Min, Max);
        var subtracted = SubtractValues(Current, clamped);
        Current = clamped;
        return subtracted;
    }

    /// <summary>
    /// Clamps a value between the specified minimum and maximum values.
    /// </summary>
    /// <param name="value">The value to clamp.</param>
    /// <param name="min">The minimum value.</param>
    /// <param name="max">The maximum value.</param>
    /// <returns>The clamped value.</returns>
    private static T Clamp(T value, T min, T max)
    {
        if (value.CompareTo(min) < 0) return min;
        if (value.CompareTo(max) > 0) return max;
        return value;
    }

    /// <summary>
    /// Adds two values of type <typeparamref name="T"/> using dynamic operations.
    /// </summary>
    /// <param name="a">The first value.</param>
    /// <param name="b">The second value.</param>
    /// <returns>The sum of the two values.</returns>
    private static T AddValues(T a, T b)
    {
        dynamic da = a;
        dynamic db = b;
        return (T)(da + db);
    }

    /// <summary>
    /// Subtracts the second value from the first value of type <typeparamref name="T"/> using dynamic operations.
    /// </summary>
    /// <param name="a">The first value.</param>
    /// <param name="b">The second value.</param>
    /// <returns>The difference between the two values.</returns>
    private static T SubtractValues(T a, T b)
    {
        dynamic da = a;
        dynamic db = b;
        return (T)(da - db);
    }

    /// <returns>A string representation of the numeric container in the format "Min/Current/Max".</returns>
    public override string ToString() => $"{Min}/{Current}/{Max}";
}