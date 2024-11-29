using JRPG_Game.Enums;
using JRPG_Game.Interfaces;

namespace JRPG_Game.Characters.Skills;

public class Attack<TTarget, TDamagePara>(
    string name,
    string description,
    Character owner,
    TTarget? target,
    TargetType targetType,
    int reloadTime,
    int manaCost,
    Func<TDamagePara, int> damage,
    DamageType attackType,
    List<Delegate>? additional = null)
    : Skill(name: name,
        owner: owner,
        target: target,
        targetType: targetType,
        description: description,
        reloadTime: reloadTime,
        manaCost: manaCost)
    where TTarget : class, ITarget
{
    public Func<TDamagePara, int> Damage { get; set; } = damage;
    public DamageType AttackType { get; private set; } = attackType;
    public List<Delegate>? Additional { get; set; } = additional;
    public Status StatusInfo { get; set; } = new();

    public Attack(
        string name,
        string description,
        Character owner,
        TargetType targetType,
        int reloadTime,
        int manaCost,
        Func<TDamagePara, int> damage,
        DamageType attackType,
        List<Delegate>? additional = null) :
        this(
            name: name,
            owner: owner,
            target: null,
            targetType: targetType,
            description: description,
            reloadTime: reloadTime,
            manaCost: manaCost,
            damage: damage,
            attackType: attackType,
            additional: additional
        )
    {
    }

    public Attack(
        string name,
        string description,
        Character owner,
        TTarget? target,
        TargetType targetType,
        int reloadTime,
        int manaCost,
        int damage,
        DamageType attackType,
        List<Delegate>? additional = null) :
        this(
            name: name,
            owner: owner,
            target: target,
            targetType: targetType,
            description: description,
            reloadTime: reloadTime,
            manaCost: manaCost,
            damage: _ => damage,
            attackType: attackType,
            additional: additional
        )
    {
    }

    public Attack(
        string name,
        string description,
        Character owner,
        TargetType targetType,
        int reloadTime,
        int manaCost,
        int damage,
        DamageType attackType,
        List<Delegate>? additional = null) :
        this(
            name: name,
            owner: owner,
            target: null,
            targetType: targetType,
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
    public override void Execute()
    {
        throw new NotImplementedException();
    }

    public override string ToString()
    {
        return base.ToString() + "\n" +
               $"Attaque de Type {AttackType}";
    }

    public class Status
    {
        public decimal Damage { get; set; }
        public bool Dodged { get; private set; }
        private bool Resisted { get; set; }
        public bool Blocked { get; private set; }

        public void Set(Attack<TTarget, TDamagePara> attack,
            (bool Dodged, bool Resisted, bool Blocked) attackStatus = default)
        {
            if (attack.Target is Character target)
            {
                if (attackStatus.Dodged || target.Dodge(attack)) Dodged = true;
                else if (attackStatus.Resisted || target.SpellResistance(attack)) Resisted = true;
                else if (attackStatus.Blocked || target.Parade(attack)) Blocked = true;
            }
        }

        public decimal SetDamage(Attack<TTarget, TDamagePara> attack, TDamagePara damageParameter)
        {
            if (attack.Target is Character target)
            {
                if (Dodged || Resisted) return 0;

                Damage = attack.Damage(damageParameter);
                if (Blocked) Damage *= 0.5m;
                Damage *= 1 - target.ArmorReduction(attack.AttackType);
            }

            return Damage;
        }
    }
}

public class Attack<TTarget> : Attack<TTarget, object?> where TTarget : class, ITarget
{
    public Attack(string name, string description, Character owner, TargetType targetType, int reloadTime, int manaCost,
        Func<object?, int> damage, DamageType attackType, List<Delegate>? additional = null) : base(name, description,
        owner, targetType, reloadTime, manaCost, damage, attackType, additional)
    {
    }

    public Attack(string name, string description, Character owner, TTarget? target, TargetType targetType,
        int reloadTime, int manaCost, int damage, DamageType attackType, List<Delegate>? additional = null) : base(name,
        description, owner, target, targetType, reloadTime, manaCost, damage, attackType, additional)
    {
    }

    public Attack(string name, string description, Character owner, TargetType targetType, int reloadTime, int manaCost,
        int damage, DamageType attackType, List<Delegate>? additional = null) : base(name, description, owner,
        targetType, reloadTime, manaCost, damage, attackType, additional)
    {
    }

    public Attack(string name, string description, Character owner, TTarget? target, TargetType targetType,
        int reloadTime, int manaCost, Func<object?, int> damage, DamageType attackType,
        List<Delegate>? additional = null) : base(name, description, owner, target, targetType, reloadTime, manaCost,
        damage, attackType, additional)
    {
    }
}