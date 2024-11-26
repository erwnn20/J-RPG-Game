using JRPG_Game.Action;
using JRPG_Game.Armors;

namespace JRPG_Game.Characters;

public abstract class Character
{
    protected Character(
        string name,
        int maxHealth,
        int physicalAttack,
        int magicalAttack,
        ArmorType armor,
        decimal dodgeChance,
        decimal paradeChance,
        decimal spellResistanceChance)
    {
        Name = name;
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
        PhysicalAttack = physicalAttack;
        MagicalAttack = magicalAttack;
        Armor = armor;
        DodgeChance = dodgeChance;
        ParadeChance = paradeChance;
        SpellResistanceChance = spellResistanceChance;

        List.Add(this);
    }

    public string Name { get; private set; }
    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }
    protected int PhysicalAttack { get; set; }
    protected int MagicalAttack { get; set; }
    private ArmorType Armor { get; set; }
    protected decimal DodgeChance { get; set; }
    private decimal ParadeChance { get; set; }
    protected decimal SpellResistanceChance { get; set; }

    public static readonly List<Character> List = [];

    public bool CheckAlive(bool message)
    {
        if (CurrentHealth <= 0 && message)
            Console.WriteLine($"{Name} est mort.");
        return CurrentHealth > 0;
    }

    public abstract void SpecialAbility();
    public abstract void Attack(Character character);

    public virtual int Defend(Attack attack)
    {
        if (Dodge(attack)) return 0;
        if (SpellResistance(attack)) return 0;

        decimal damage = attack.Damage;

        if (Parade(attack)) damage *= 0.5m;
        damage *= 1 - ArmorReduction(attack.AttackType);

        TakeDamage((int)damage);
        return (int)damage;
    }

    protected int Heal(int healthPoint, bool message = true)
    {
        var healed = Math.Min(MaxHealth - CurrentHealth, healthPoint);
        CurrentHealth += healed;

        if (healed <= 0 || !message) return healed;
        Console.WriteLine($"{Name} se soigne de {healed}");

        return healed;
    }

    private decimal ArmorReduction(DamageType damageType)
    {
        return damageType switch
        {
            DamageType.Physical => Armor switch
            {
                ArmorType.Textile => 0.0m,
                ArmorType.Leather => 0.15m,
                ArmorType.Mesh => 0.30m,
                ArmorType.Plates => 0.44m,
                _ => 0
            },
            DamageType.Magical => Armor switch
            {
                ArmorType.Textile => 0.30m,
                ArmorType.Leather => 0.20m,
                ArmorType.Mesh => 0.10m,
                ArmorType.Plates => 0.0m,
                _ => 0
            },
            _ => 0
        };
    }

    private bool Dodge(Attack attack)
    {
        if (attack.AttackType != DamageType.Physical
            || (decimal)new Random().NextDouble() > DodgeChance) return false;

        Console.WriteLine($"{Name} a réussi à esquiver {attack.Name}");
        return true;
    }

    private bool Parade(Attack attack)
    {
        if (attack.AttackType != DamageType.Physical
            || (decimal)new Random().NextDouble() > ParadeChance) return false;

        Console.WriteLine($"{Name} a réussi à parer une partie de {attack.Name}");
        return true;
    }

    private bool SpellResistance(Attack attack)
    {
        if (attack.AttackType != DamageType.Magical
            || (decimal)new Random().NextDouble() > SpellResistanceChance) return false;

        Console.WriteLine($"{Name} a réussi à resister à {attack.Name}");
        return true;
    }

    public void TakeDamage(int damage) => CurrentHealth = Math.Max(0, CurrentHealth - damage);

    //

    public override string ToString()
    {
        return $"Character: {Name}\n" +
               $"Max Health: {MaxHealth}\n" +
               $"Current Health: {CurrentHealth}\n" +
               $"Physical Attack: {PhysicalAttack}\n" +
               $"Magical Attack: {MagicalAttack}\n" +
               $"Armor: {Armor}\n" +
               $"Dodge Chance: {DodgeChance:P}\n" +
               $"Parade Chance: {ParadeChance:P}\n" +
               $"Spell Resistance Chance: {SpellResistanceChance:P}";
    }
}