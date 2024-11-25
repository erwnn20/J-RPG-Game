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
    private int MagicalAttack { get; set; } = magicalAttack;
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
    public abstract int Defend(Attack taken);
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

    protected bool Dodge(DamageType damageType)
    {
        if (damageType == DamageType.Physical)
            return (decimal)new Random().NextDouble() <= DodgeChance;
        return false;
    }

    protected bool Parade(DamageType damageType)
    {
        if (damageType == DamageType.Physical)
            return (decimal)new Random().NextDouble() <= ParadeChance;
        return false;
    }

    protected bool SpellResistance(DamageType damageType)
    {
        if (damageType == DamageType.Magical)
            return (decimal)new Random().NextDouble() <= SpellResistanceChance;
        return false;
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