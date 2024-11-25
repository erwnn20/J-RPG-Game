using JRPG_Game.Action;
using JRPG_Game.Armors;

namespace JRPG_Game.Characters;

public class Paladin(string name) :
    Character(
        name: name,
        maxHealth: 95,
        armor: ArmorType.Mesh,
        physicalAttack: 40,
        magicalAttack: 40,
        dodgeChance: 0.05m,
        paradeChance: 0.10m,
        spellResistanceChance: 0.20m
    )
{
    private Attack SelectAttack(Character character)
    {
        var attackList = new List<string>
            { "Frappe du croisé (attaque physique)", "Jugement (attaque magique)" };

        Console.WriteLine("Choisissez une attaque a effectuer :");
        foreach (var (item, index) in attackList.Select((item, index) => (item, index)))
            Console.WriteLine($"\t{index + 1}: {item}");
        do
        {
            Console.Write("-> ");
            var valid = int.TryParse(Console.ReadLine(), out var input);

            if (valid && 1 <= input && input <= attackList.Count)
            {
                return input switch
                {
                    1 => new Attack(name: "Frappe du croisé", attacker: this, target: character, damage: PhysicalAttack,
                        attackType: DamageType.Physical),
                    2 => new Attack(name: "Jugement", attacker: this, target: character, damage: MagicalAttack,
                        attackType: DamageType.Magical),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }

            Console.WriteLine("Entrée invalide");
        } while (true);
    }

    public override void Attack(Character character)
    {
        var attack = SelectAttack(character);
        var damageHealed = (int)(attack.Execute() * 0.5);

        if (damageHealed <= 0) return;
        Console.WriteLine($"{Name} se soigne de {damageHealed}");
        HealDamage(damageHealed);
    }

    public override int Defend(Attack attack)
    {
        if (Dodge(attack)) return 0;
        if (SpellResistance(attack)) return 0;

        decimal damage = attack.Damage;

        if (Parade(attack)) damage *= 0.5m;
        damage *= 1 - ArmorReduction(attack.AttackType);

        TakeDamage((int)damage);
        return (int)damage;
    }

    public override void Heal()
    {
        Console.WriteLine($"{Name} se soigne en faisant Eclair lumineux");
        HealDamage((int)(MagicalAttack * 1.25));
    }
}