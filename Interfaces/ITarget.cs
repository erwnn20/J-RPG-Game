using JRPG_Game.Characters.Skills;

namespace JRPG_Game.Interfaces;

public interface ITarget
{
    string Name { get; }

    int Defend<TTarget, TDamagePara>(Attack<TTarget, TDamagePara> from, TDamagePara damageParameter) where TTarget : class, ITarget;
}