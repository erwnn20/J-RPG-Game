using JRPG_Game.Characters.Skills;
using JRPG_Game.Enums;

namespace JRPG_Game.Characters;

public class Warrior : Character
{
    public Warrior(string name)
        : base(
            name: name,
            maxHealth: 100,
            speed: 50,
            armor: ArmorType.Plates,
            physicalAttack: 50,
            magicalAttack: 0,
            dodgeChance: 0.05m,
            paradeChance: 0.25m,
            spellResistanceChance: 0.10m,
            skills: [])
    {
        Skills.AddRange([
            new Attack<object?, object?, object?>(
                name: "Frappe héroïque",
                description: $"Inflige 100% de la puissance d’attaque physique ({PhysicalAttack}) à la cible.",
                owner: this,
                target: TargetType.Other,
                reloadTime: 1,
                manaCost: 0,
                damage: PhysicalAttack,
                attackType: DamageType.Physical),
            new SpecialAbility<Team.Team, object?>(
                name: "Cri de bataille",
                description: "Augmente de 25 la puissance d’attaque physique de tous les personnages de l’équipe.",
                owner: this,
                target: TargetType.Team,
                reloadTime: 2,
                manaCost: 0,
                effect: team =>
                {
                    team.Characters
                        .Where(c => c.IsAlive(false)).ToList()
                        .ForEach(c => c.PhysicalAttack += 25);
                    return false;
                }),
            new Attack<object?, object?, object?>(
                name: "Tourbillon",
                description:
                $"Inflige 33% de la puissance d’attaque physique ({(int)(PhysicalAttack * (1 / 3.0m))}) toute l’équipe ciblé.",
                owner: this,
                target: TargetType.Team,
                reloadTime: 2,
                manaCost: 0,
                damage: (int)(PhysicalAttack * (1 / 3.0m)),
                attackType: DamageType.Physical)
        ]);
    }

    /*private void CounterAttack(Character attacker, int damageTaken)
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
    }*/
    protected override void SpecialAbility()
    {
        throw new NotImplementedException();
    }

    protected override void Attack(Character character)
    {
        throw new NotImplementedException();
    }
}