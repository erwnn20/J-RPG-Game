using JRPG_Game.Action;
using JRPG_Game.Armors;

namespace JRPG_Game.Characters;

public class Warrior(string name) :
    Character(
        name: name,
        maxHealth: 100,
        armor: ArmorType.Plates,
        physicalAttack: 50,
        magicalAttack: 0,
        dodgeChance: 0.05m,
        paradeChance: 0.25m,
        spellResistanceChance: 0.10m
    )
{
    public override void SpecialAbility()
    {
        PhysicalAttack *= 2;
        Console.WriteLine($"{Name} utilise sa capacité spéciale : \"Cri de bataille\"\n" +
                          $"-> Les dégâts de ses attaques physiques sont multiplier par 2");
    }

    private void CounterAttack(Character attacker, int damageTaken)
    {
        var damage = (int)(damageTaken * 0.5);
        Console.WriteLine($"{Name} contre-attaque et inflige {damage} dégât(s) à {attacker.Name}.");

        attacker.TakeDamage(damage);
        attacker.CheckAlive(true);
    }

    public override void Attack(Character character)
    {
        var attack = new Attack(
            name: "Frappe héroïque",
            attacker: this,
            target: character,
            damage: PhysicalAttack,
            attackType: DamageType.Physical
        );
        attack.Execute();
    }

    public override int Defend(Attack attack)
    {
        if (Dodge(attack)) return 0;
        if (SpellResistance(attack)) return 0;

        decimal damage = attack.Damage;
        if (Parade(attack)) damage *= 0.5m;
        damage *= 1 - ArmorReduction(attack.AttackType);
        TakeDamage((int)damage);

        if (CheckAlive(false) && new Random().NextDouble() <= 0.25) CounterAttack(attack.Attacker, (int)damage);

        return (int)damage;
    }

    public override void Heal()
    {
        throw new NotImplementedException();
    }
}