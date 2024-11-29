using JRPG_Game.Characters.Skills;
using JRPG_Game.Enums;
using JRPG_Game.Interfaces;

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
            new Attack<Character>(
                name: "Frappe héroïque",
                description: $"Inflige 100% de la puissance d’attaque physique ({PhysicalAttack}) à la cible.",
                owner: this,
                targetType: TargetType.Other,
                reloadTime: 1,
                manaCost: 0,
                damage: PhysicalAttack,
                attackType: DamageType.Physical),
            new SpecialAbility<Team.Team>(
                name: "Cri de bataille",
                description: "Augmente de 25 la puissance d’attaque physique de tous les personnages de l’équipe.",
                owner: this,
                targetType: TargetType.Team,
                reloadTime: 2,
                manaCost: 0,
                effect: (Team.Team team) =>
                {
                    team.Characters
                        .Where(c => c.IsAlive(false)).ToList()
                        .ForEach(c => c.PhysicalAttack += 25);
                    return false;
                }),
            new Attack<Team.Team>(
                name: "Tourbillon",
                description:
                $"Inflige 33% de la puissance d’attaque physique ({(int)(PhysicalAttack * (1 / 3.0m))}) toute l’équipe ciblé.",
                owner: this,
                targetType: TargetType.Team,
                reloadTime: 2,
                manaCost: 0,
                damage: (int)(PhysicalAttack * (1 / 3.0m)),
                attackType: DamageType.Physical)
        ]);
    }

    public override int Defend<TTarget, TDamagePara>(Attack<TTarget, TDamagePara> from, TDamagePara damageParameter)
    {
        var damage = base.Defend(from, damageParameter);

        var counterAttack = new Attack<Character>(
            name: "Contre-attaque",
            owner: this,
            description:
            "Lorsque le guerrier reçoit une attaque physique\n" +
            " - Il a 25% de chances de contre attaque en infligeant 50% des dégâts qu’il a subi à l’attaquant\n" +
            " - Si il a paré l’attaque, les chances sont de 100%, et les dégâts de 150%\n",
            target: from.Owner,
            targetType: TargetType.Other,
            reloadTime: 0,
            manaCost: 0,
            damage: 0, // defined later
            attackType: DamageType.Physical
        );

        if (from.Blocked)
        {
            counterAttack.Damage = _ => (int)(PhysicalAttack * 1.50m);
            if (from.Additional != null)
                from.Additional.Add(() => counterAttack.Execute());
            else from.Additional = [() => counterAttack.Execute()];
        }
        else if (new Random().NextDouble() < 0.25)
        {
            counterAttack.Damage = _ => (int)(PhysicalAttack * 0.50m);
            if (from.Additional != null)
                from.Additional.Add(() => counterAttack.Execute());
            else from.Additional = [() => counterAttack.Execute()];
        }
        
        TakeDamage(damage);
        return damage;
    }

    /*
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
*/
    protected override void SpecialAbility()
    {
        throw new NotImplementedException();
    }

    protected override void Attack(Character character)
    {
        throw new NotImplementedException();
    }
}