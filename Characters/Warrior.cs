using JRPG_Game.Characters.Skills;
using JRPG_Game.Enums;

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
    private void CounterAttack(Character attacker, int damageTaken)
    {
        var damage = (int)(damageTaken * 0.5);

        if (damage <= 0) return;

        var conterAttack = new Attack(
            name: "Contre Attaque",
            attacker: this,
            target: attacker,
            damage: damage,
            attackType: DamageType.Physical
        );
        conterAttack.Execute();
    }

    protected override void SpecialAbility()
    {
        PhysicalAttack *= 2;
        Console.WriteLine($"{Name} utilise sa capacité spéciale : \"Cri de bataille\"\n" +
                          $" -> Les dégâts de ses attaques physiques sont multiplier par 2");
    }

    protected override void Attack(Character character)
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
        var damage = base.Defend(attack);

        if (IsAlive(false) && new Random().NextDouble() <= 0.25) CounterAttack(attack.Attacker, damage);

        return damage;
    }
}