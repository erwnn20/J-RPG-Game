using JRPG_Game.Enums;
using JRPG_Game.Interfaces;

namespace JRPG_Game.Characters.Skills;

public class SpecialAbility(
    string name,
    Character owner,
    TargetType target,
    string description,
    int reloadTime,
    int manaCost,
    Delegate effect) :
    Skill(
        name: name,
        owner: owner,
        target: target,
        description: description,
        reloadTime: reloadTime,
        manaCost: manaCost
    )
{
    public Delegate Effect { get; set; } = effect;

    protected override void Execute(ITarget target)
    {
        throw new NotImplementedException();
    }
}