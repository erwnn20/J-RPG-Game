using JRPG_Game.Characters.Elements.Skills;

namespace JRPG_Game.Enums;

/// <summary>
/// Defines the types of damage that can be inflicted by an <see cref="Attack{TTarget}"/> in the game.
/// </summary>
public enum DamageType
{
    Physical,
    Magical,
    
    Distance
}