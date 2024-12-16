using JRPG_Game.Utils;

namespace JRPG_Game.Characters.Elements;

/// <summary>
/// Represents an upgrade for a specific attribute of a character.
/// </summary>
/// <typeparam name="TAttribute">The type of the attribute being upgraded (e.g., int, decimal).</typeparam>
public class AttributeUpgrade<TAttribute>(
    string name,
    int upgradeCount,
    NumericContainer<TAttribute> attribute,
    TAttribute increment,
    Func<bool> condition) : IAttributeUpgrade where TAttribute : struct, IComparable<TAttribute>, IComparable
{
    private string Name { get; } = name;
    private int UpgradeCount { get; set; } = upgradeCount;
    private NumericContainer<TAttribute> Attribute { get; } = attribute;
    private TAttribute Increment { get; } = increment;
    public Func<bool> Condition { get; } = condition;

    /// <summary>
    /// Upgrades the attribute by incrementing its value and increasing the upgrade count.
    /// </summary>
    public void Upgrade()
    {
        UpgradeCount++;
        Attribute.Add(Increment);
    }

    /// <summary>
    /// Converts a value of type <typeparamref name="TAttribute"/> to a string representation.
    /// </summary>
    /// <param name="value">The value to convert to a string.</param>
    /// <returns>A string representation of the value.</returns>
    private static string ValueString(TAttribute value) => value is decimal v ? $"{v:P}" : $"{value}";

    /// <summary>
    /// Multiplies a value of type <typeparamref name="TAttribute"/> by an integer.
    /// </summary>
    /// <param name="a">The value to multiply.</param>
    /// <param name="b">The multiplier.</param>
    /// <returns>The result of the multiplication.</returns>
    private static TAttribute Multiply(TAttribute a, int b)
    {
        dynamic da = a;
        dynamic db = b;
        return (TAttribute)(da * db);
    }

    //

    /// <summary>
    /// Gets a string representing the current and potential value of the attribute after upgrading.
    /// </summary>
    /// <returns>A formatted string displaying the attribute name, its current value, and the upgrade value.</returns>
    public override string ToString()
    {
        var addedValue = Multiply(Increment, UpgradeCount);
        var baseValue = NumericContainer<TAttribute>.SubtractValues(Attribute.Current, addedValue);
        return
            $"{Name} : {ValueString(baseValue)}{(UpgradeCount > 0 ? $" (+{ValueString(addedValue)})" : string.Empty)}";
    }
}

/// <summary>
/// Defines the operations for upgrading an attribute.
/// </summary>
public interface IAttributeUpgrade
{
    /// <summary>
    /// Gets the condition that determines whether the attribute can be upgraded.
    /// </summary>
    Func<bool> Condition { get; }

    /// <summary>
    /// Upgrades the attribute by incrementing its value and increasing the upgrade count.
    /// </summary>
    void Upgrade();

    /// <summary>
    /// Gets a string representing the current and potential value of the attribute after upgrading.
    /// </summary>
    /// <returns>A formatted string displaying the attribute name, its current value, and the upgrade value.</returns>
    string ToString();
}