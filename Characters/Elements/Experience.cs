namespace JRPG_Game.Characters.Elements;

/// <summary>
/// Represents the experience of a <see cref="Character"/>, tracking their current level, current experience, next experience threshold, and added levels.
/// </summary>
public class Experience(int level, int current, int next, int addedLevel)
{
    public int Level { get; private set; } = level;
    public int Current { get; private set; } = current;
    public int Next { get; private set; } = next;
    public int AddedLevel { get; private set; } = addedLevel;

    /// <summary>
    /// Adds experience points to the current experience.
    /// </summary>
    /// <param name="addedXp">The amount of experience points to add.</param>
    public void Add(int addedXp) => Current += addedXp;

    /// <summary>
    /// Upgrades the character, deducting the required experience for the next level and increasing the threshold for the next upgrade.
    /// </summary>
    public void Up()
    {
        Current -= Next;
        Next += 25;
        AddedLevel++;
    }

    /// <summary>
    /// Checks if the character can level up by comparing the current experience to the next threshold.
    /// </summary>
    /// <returns><c>true</c> if the character can level up, <c>false</c> otherwise.</returns>
    public bool CanXpUp() => Current >= Next;

    /// <summary>
    /// Levels up the character, applying the added level and advancing the level.
    /// </summary>
    public void LevelUp()
    {
        AddedLevel--;
        Level++;
    }

    /// <summary>
    /// Checks if the character has any unspent levels that can be applied.
    /// </summary>
    /// <returns><c>true</c> if the character can level up, <c>false</c> otherwise.</returns>
    public bool CanLevelUp() => AddedLevel > 0;
}