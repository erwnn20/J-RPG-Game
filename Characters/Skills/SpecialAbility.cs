using JRPG_Game.Enums;
using JRPG_Game.Interfaces;

namespace JRPG_Game.Characters.Skills;

public class SpecialAbility(
    string name,
    Character owner,
    ITarget? target,
    TargetType targetType,
    string description,
    int reloadTime,
    int manaCost,
    Delegate effect) :
    Skill(
        name: name,
        owner: owner,
        target: target,
        targetType: targetType,
        description: description,
        reloadTime: reloadTime,
        manaCost: manaCost
    )
{
    public Delegate Effect { get; set; } = effect;

    public SpecialAbility(
        string name,
        Character owner,
        TargetType targetType,
        string description,
        int reloadTime,
        int manaCost,
        Delegate effect) :
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