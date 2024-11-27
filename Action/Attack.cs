using JRPG_Game.Characters;

namespace JRPG_Game.Action;

public class Attack(string name, Character attacker, Character target, int damage, DamageType attackType)
{
    public string Name { get; private set; } = name;
    public Character Attacker { get; private set; } = attacker;
    private Character Target { get; set; } = target;
    public int Damage { get; set; } = damage;
    public DamageType AttackType { get; private set; } = attackType;

    public int Execute()
    {
        Console.WriteLine($"{Attacker.Name} fait {Name} sur {Target.Name}");
        var damage = Target.Defend(this);
        Console.WriteLine($"{Name} à fais {damage} dégât(s) à {Target.Name}.");

        Target.IsAlive(true);
        return damage;
    }
}