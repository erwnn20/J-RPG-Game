using JRPG_Game.Action;
using JRPG_Game.Armors;

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

    public override void SpecialAbility()
    {
        ReducedAttack = 2;
        Console.WriteLine($"{Name} utilise sa capacité spéciale : \"Barrière de givre\"\n" +
                          $"-> Les deux prochaines attaques subies sont réduites");
    }

    public override void Attack(Character character)
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
        if (Dodge(attack)) return 0;
        if (SpellResistance(attack)) return 0;

        decimal damage = attack.Damage;

        if (ReducedAttack > 0)
        {
            damage *= 1 - attack.AttackType switch
            {
                DamageType.Physical => ReduceDamagePhysical,
                DamageType.Magical => ReduceDamageMagical,
                _ => 0
            };
            ReducedAttack--;
        }

        if (Parade(attack)) damage *= 0.5m;
        damage *= 1 - ArmorReduction(attack.AttackType);

        TakeDamage((int)damage);
        return (int)damage;
    }

    public override void Heal()
    {
        throw new NotImplementedException();
    }
}