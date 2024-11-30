using JRPG_Game.Enums;
using JRPG_Game.Interfaces;

namespace JRPG_Game.Characters.Skills;

public class SpecialAbility<TTarget>(
    string name,
    Character owner,
    TTarget? target,
    TargetType targetType,
    string description,
    int reloadTime,
    int manaCost,
    Func<Character, string> effect) :
    Skill(
        name: name,
        owner: owner,
        target: target,
        targetType: targetType,
        description: description,
        reloadTime: reloadTime,
        manaCost: manaCost
    ) where TTarget : class, ITarget
{
    private Func<Character, string> Effect { get; set; } = effect;

    public SpecialAbility(
        string name,
        Character owner,
        TargetType targetType,
        string description,
        int reloadTime,
        int manaCost,
        Func<Character, string> effect) :
        this(
            name: name,
            owner: owner,
            target: null,
            targetType: targetType,
            description: description,
            reloadTime: reloadTime,
            manaCost: manaCost,
            effect: effect
        )
    {
    }

    public override void Execute()
    {
        throw new NotImplementedException();
    }
}