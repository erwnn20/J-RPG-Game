namespace JRPG_Game.Enums;

/// <summary>
/// Represents special status effects that can be applied to characters during combat.
/// </summary>
public enum StatusEffect
{
    // positive effects

    /// <summary>
    /// Restores 5% of maximum health points of a character each turn.
    /// </summary>
    Regeneration,

    /// <summary>
    /// The character takes no damage.
    /// </summary>
    Invincibility,

    /// <summary>
    /// Increases the character's chance of avoiding enemy attacks by 10% and speed by 10
    /// </summary>
    Speed,

    /// <summary>
    /// Increases the character's chance of avoiding enemy attacks by 10% and increases all types of damage by 10.
    /// </summary>
    Focus,

    // negative effects

    /// <summary>
    /// Inflicts 5 damage per turn.
    /// </summary>
    Poison,
    
    /// <summary>
    /// Reduces chance of avoidance by 25% and speed by 10
    /// </summary>
    Slowness,
    
    /// <summary>
    /// Reduces the chance of avoiding enemy attacks by 5%, the chance of parrying by 50% and the character has a 15% chance of missing skills.
    /// </summary>
    Stun,
    
    /// <summary>
    /// Reduces health points by 5% of maximum each turn and prevents regeneration.
    /// </summary>
    Bleeding,
    
    /// <summary>
    /// Prevents all skill use, reduces speed to 0, has a 15% chance of freeing itself each turn.
    /// </summary>
    Paralysis,
    
    /// <summary>
    /// Inflicts between 1 and 15 damage to the character each turn, with a 25% chance of disappearing each turn
    /// </summary>
    Burn,
}