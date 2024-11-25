using JRPG_Game.Action;
using JRPG_Game.Armors;

namespace JRPG_Game.Characters;

public abstract class Character(
    string name,
    int maxHealth,
    int physicalAttack,
    int magicalAttack,
    ArmorType armor,
    decimal dodgeChance,
    decimal paradeChance,
    decimal spellResistanceChance)
{
    public string Name { get; private set; } = name;
    private int MaxHealth { get; set; } = maxHealth;
    private int CurrentHealth { get; set; } = maxHealth;
    protected int PhysicalAttack { get; set; } = physicalAttack;
    protected int MagicalAttack { get; set; } = magicalAttack;
    private ArmorType Armor { get; set; } = armor;
    private decimal DodgeChance { get; set; } = dodgeChance;
    private decimal ParadeChance { get; set; } = paradeChance;
    private decimal SpellResistanceChance { get; set; } = spellResistanceChance;

    public bool CheckAlive(bool message)
    {
        if (CurrentHealth <= 0 && message)
            Console.WriteLine($"{Name} est mort.");
        return CurrentHealth > 0;
    }

    public abstract void Attack(Character character);
    public abstract int Defend(Attack attack);
    public abstract void Heal();

    protected decimal ArmorReduction(DamageType damageType)
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

    protected bool Dodge(Attack attack)
    {
        if (attack.AttackType != DamageType.Physical
            || (decimal)new Random().NextDouble() > DodgeChance) return false;

        Console.WriteLine($"{Name} a réussi à esquiver {attack.Name}");
        return true;
    }

    protected bool Parade(Attack attack)
    {
        if (attack.AttackType != DamageType.Physical
            || (decimal)new Random().NextDouble() > ParadeChance) return false;

        Console.WriteLine($"{Name} a réussi à parer une partie de {attack.Name}");
        return true;
    }

    protected bool SpellResistance(Attack attack)
    {
        if (attack.AttackType != DamageType.Magical
            || (decimal)new Random().NextDouble() > SpellResistanceChance) return false;

        Console.WriteLine($"{Name} a réussi à resister à {attack.Name}");
        return true;
    }

    public void TakeDamage(int damage) => CurrentHealth = Math.Max(0, CurrentHealth - damage);
    protected void HealDamage(int healthPoint) => CurrentHealth = Math.Min(MaxHealth, CurrentHealth + healthPoint);

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