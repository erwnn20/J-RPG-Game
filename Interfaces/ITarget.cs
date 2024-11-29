using JRPG_Game.Characters.Skills;

namespace JRPG_Game.Interfaces;

public interface ITarget
{
    int Defend<T1, T2, T3>(Attack<T1, T2, T3> from, T3 damageParameter);
}