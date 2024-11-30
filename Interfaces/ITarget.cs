using JRPG_Game.Characters;
using JRPG_Game.Characters.Skills;

namespace JRPG_Game.Interfaces;

public interface ITarget
{
    string Name { get; }

    int Defend<TTarget>(Attack<TTarget> from, Character damageParameter) where TTarget : class, ITarget;
}