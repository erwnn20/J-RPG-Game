using JRPG_Game.Characters.Skills;

namespace JRPG_Game.Interfaces;

public interface ITarget
{
    int Defend<TTarget>(Attack<TTarget> from, TTarget damageParameter) where TTarget : ITarget ;
}