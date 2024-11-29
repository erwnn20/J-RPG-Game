using JRPG_Game.Enums;
using JRPG_Game.Interfaces;

namespace JRPG_Game.Characters.Skills;

public class Attack<TTarget>(
    string name,
    Character owner,
    TargetType target,
    string description,
    int reloadTime,
    int manaCost,
    Func<TTarget, int> damage,
    DamageType attackType,
    Delegate? additional = null)
    : Skill(name: name,
        owner: owner,
        target: target,
        description: description,
        reloadTime: reloadTime,
        manaCost: manaCost) where TTarget : ITarget
{
    public Func<TTarget, int> Damage { get; set; } = damage;
    public DamageType AttackType { get; private set; } = attackType;
    public Delegate? Additional { get; set; } = additional;
    public bool Dodged { get; set; }
    public bool Resisted { get; set; }
    public bool Blocked { get; set; }

    public Attack(
        string name,
        Character owner,
        TargetType target,
        string description,
        int reloadTime,
        int manaCost,
        int damage,
        DamageType attackType,
        Delegate? additional = null) :
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

    public override string ToString()
    {
        return base.ToString() + "\n" +
               $"Attaque de Type {AttackType}";
    }
}