using JRPG_Game.Characters.Skills;

namespace JRPG_Game.Interfaces;

/// <summary>
/// Represents an entity that can be targeted by a <see cref="Skill"/> in the game, having a name for identification.
/// </summary>
public interface ITarget
{
    string Name { get; }
}