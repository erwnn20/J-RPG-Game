using JRPG_Game.Characters.Skills;
using JRPG_Game.Enums;

namespace JRPG_Game.Characters;

public class Mage(string name) :
    Character(
        name: name,
        maxHealth: 60,
        armor: ArmorType.Textile,
        physicalAttack: 0,
        magicalAttack: 75,
        dodgeChance: 0.05m,
        paradeChance: 0.05m,
        spellResistanceChance: 0.25m
    )
{
    private decimal ReduceDamagePhysical { get; set; } = 0.60m;
    private decimal ReduceDamageMagical { get; set; } = 0.50m;
    private int ReducedAttack { get; set; }

    protected override void SpecialAbility()
    {
        ReducedAttack = 2;
        Console.WriteLine($"{Name} utilise sa capacité spéciale : \"Barrière de givre\"\n" +
                          $" -> Les deux prochaines attaques subies sont réduites");
    }

    protected override void Attack(Character character)
    {
        var attack = new Attack(
            name: "Eclair de givre",
            attacker: this,
            target: character,
            damage: MagicalAttack,
            attackType: DamageType.Magical
        );
        attack.Execute();
    }

    public override int Defend(Attack attack)
    {
        if (ReducedAttack <= 0) return base.Defend(attack);
        attack.Damage = (int)(attack.Damage * (1 - attack.AttackType switch
        {
            DamageType.Physical => ReduceDamagePhysical,
            DamageType.Magical => ReduceDamageMagical,
            _ => 0
        }));
        ReducedAttack--;

        return base.Defend(attack);
    }

    public override string ToString()
    {
        return base.ToString() + "\n" +
               (ReducedAttack > 0 ? $"Dégâts réduits pendant {ReducedAttack} attaque subie" : "");
    }
}