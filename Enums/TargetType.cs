﻿using JRPG_Game.Characters.Elements.Skills;

namespace JRPG_Game.Enums;

/// <summary>
/// Defines the different target types for a <see cref="Skill"/> in the game.
/// </summary>
public enum TargetType
{
    Self,
    Teammate,
    TeammateDead,
    Enemy,

    //

    TeamAllied,
    TeamEnemy
}