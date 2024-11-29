using JRPG_Game.Enums;
using JRPG_Game.Interfaces;

namespace JRPG_Game.Characters.Skills;

public class Attack<T1, T2, T3>(
    string name,
    Character owner,
    TargetType target,
    string description,
    int reloadTime,
    int manaCost,
    Func<T3, int> damage,
    DamageType attackType,
    Func<T1, T2>? additional = null)
    : Skill(name: name,
        owner: owner,
        target: target,
        description: description,
        reloadTime: reloadTime,
        manaCost: manaCost)
{
    public Func<T3, int> Damage { get; set; } = damage;
    public DamageType AttackType { get; private set; } = attackType;
    public Func<T1, T2>? Additional { get; set; } = additional;
    private bool Dodged { get; set; } = false;
    private bool Resisted { get; set; } = false;
    private bool Blocked { get; set; } = false;

    public Attack(
        string name,
        Character owner,
        TargetType target,
        string description,
        int reloadTime,
        int manaCost,
        int damage,
        DamageType attackType,
        Func<T1, T2>? additional = null) :
        this(
            name: name,
            owner: owner,
            target: target,
            description: description,
            reloadTime: reloadTime,
            manaCost: manaCost,
            damage: _ => damage,
            attackType: attackType,
            additional: additional
        )
    {
    }

    /*public int Execute()
    {
        Console.WriteLine($"{Owner.Name} fait {Name} sur {Target.Name}");
        var damage = Target.Defend(this);
        Console.WriteLine($"{Name} à fais {damage} dégât(s) à {Target.Name}.");

        Target.IsAlive(true);
        return damage;
    }*/
    protected override void Execute(ITarget target)
    {
        throw new NotImplementedException();
    }
}