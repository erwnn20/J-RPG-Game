using JRPG_Game.Characters.Skills;
using JRPG_Game.Enums;

namespace JRPG_Game.Characters;

public class Warrior : Character
{
    public Warrior(string name, Team.Team team)
        : base(
            name: name,
            team: team,
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
                description: () => $"Inflige 100% de la puissance d’attaque physique ({PhysicalAttack}) à la cible.",
                owner: this,
                targetType: TargetType.Enemy,
                reloadTime: 1,
                manaCost: 0,
                damage: PhysicalAttack,
                attackType: DamageType.Physical),
            new SpecialAbility<Team.Team>(
                name: "Cri de bataille",
                description: () =>
                    "Augmente de 25 la puissance d’attaque physique de tous les personnages de l’équipe.",
                owner: this,
                targetType: TargetType.TeamAllied,
                reloadTime: 2,
                manaCost: 0,
                effect: target =>
                {
                    target.PhysicalAttack += 25;
                    return $"La puissance d'attaque de {target.Name} a été augmentée de 25.";
                }),
            new Attack<Team.Team>(
                name: "Tourbillon",
                description: () =>
                    $"Inflige 33% de la puissance d’attaque physique ({(int)(PhysicalAttack * (1 / 3.0m))}) toute l’équipe ciblé.",
                owner: this,
                targetType: TargetType.TeamEnemy,
                reloadTime: 2,
                manaCost: 0,
                damage: (int)(PhysicalAttack * (1 / 3.0m)),
                attackType: DamageType.Physical)
        ]);
    }

    public override int Defend<TTarget>(Attack<TTarget> from, Character damageParameter)
    {
        from.StatusInfo.Set(from, (false, false, false));
        from.StatusInfo.SetDamage(from, damageParameter);

        if (from.StatusInfo.Blocked || new Random().NextDouble() < 0.25) (from.Additional ??= []).Add(Special);

        return TakeDamage((int)from.StatusInfo.Damage);
    }

    private Action<Attack<Character>> Special => attackFrom =>
    {
        var conterAttack = new Attack<Character>(
            name: "Contre-attaque",
            owner: this,
            description: () =>
                "Lorsque le guerrier reçoit une attaque physique\n" +
                " - Il a 25% de chances de contre attaque en infligeant 50% des dégâts qu’il a subi à l’attaquant,\n" +
                " - Si il a paré l’attaque, les chances sont de 100%, et les dégâts de 150%.",
            target: attackFrom.Owner,
            targetType: TargetType.Enemy,
            reloadTime: 0,
            manaCost: 0,
            damage: attackFrom.StatusInfo.Blocked
                ? _ => (int)(PhysicalAttack * 1.50m)
                : _ => (int)(PhysicalAttack * 0.50m),
            attackType: DamageType.Physical
        );
        conterAttack.Execute();
        conterAttack.Damage = _ => 0;
    };
}