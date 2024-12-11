using JRPG_Game.Characters.Skills;
using JRPG_Game.Enums;

namespace JRPG_Game.Characters.Classes;

/// <summary>
/// Represents a warrior character with high health and physical attack capabilities.
/// </summary>
/// <remarks>
/// Inherits from <see cref="Character"/> and specializes in dealing and resisting physical damage.
/// The Warrior is a durable and powerful melee fighter with abilities to boost allies and counterattack enemies.
/// </remarks>
public class Warrior : Character
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Warrior"/> class.
    /// </summary>
    /// <param name="name">The name of the warrior.</param>
    /// <param name="team">The team to which the warrior belongs.</param>
    /// <remarks>
    /// Configures the warrior's high health, physical attack, and defensive abilities,
    /// while adding its unique skill set.
    /// </remarks>
    public Warrior(string name, Team team)
        : base(
            name: name,
            team: team,
            maxHealth: 130,
            speed: 45,
            armor: ArmorType.Plates,
            physicalAttack: 80,
            magicalAttack: 0,
            distanceAttack: 0,
            paradeChance: 0.30m,
            dodgeChance: 0.05m,
            spellResistanceChance: 0.10m,
            skills: [])
    {
        Skills.AddRange([
            new Attack<Character>(
                name: "Frappe héroïque",
                description: () =>
                    $"Inflige 100% de la puissance d’attaque physique ({GetAttack(DamageType.Physical)}) à la cible.\n" +
                    "Si l'attaque n'est pas esquivée, la cible sera étourdi étourdi pendant 3 tours.",
                owner: this,
                targetType: TargetType.Enemy,
                reloadTime: 1,
                manaCost: 0,
                damage: _ => GetAttack(DamageType.Physical),
                attackType: DamageType.Physical,
                additional:
                [
                    attack =>
                    {
                        if (attack is { StatusInfo.Dodged: false, Target: Character target } && target.IsAlive(false))
                            Console.WriteLine(
                                $"{target.Name} est étourdi pendant {target.AddEffect(StatusEffect.Stun, 3)} tours.");
                    }
                ]),
            new SpecialAbility<Team>(
                name: "Cri de bataille",
                description: () =>
                    "Augmente de 5 la puissance d’attaque physique de tous les personnages de l’équipe.\n" +
                    "Donne un rush d'adrénaline a tous les personnages de l’équipe pendant 3 tours.",
                owner: this,
                targetType: TargetType.TeamAllied,
                reloadTime: 2,
                manaCost: 0,
                effect: target =>
                {
                    target.PhysicalAttack += 5;
                    return $"La puissance d'attaque de {target.Name} a été augmentée de 5.\n" +
                           $"{target.Name} a un rush d'adrénaline pendant {target.AddEffect(StatusEffect.AdrenalinRush, 3)} tours.";
                }),
            new Attack<Team>(
                name: "Tourbillon",
                description: () =>
                    $"Inflige 33% de la puissance d’attaque physique ({(int)(GetAttack(DamageType.Physical) * 0.33m)}) toute l’équipe ciblé.\n" +
                    "Si l'attaque n'est pas esquivée, la cible sera étourdi pendant 2 tours.",
                owner: this,
                targetType: TargetType.TeamEnemy,
                reloadTime: 2,
                manaCost: 0,
                damage: _ => (int)(GetAttack(DamageType.Physical) * 0.33m),
                attackType: DamageType.Physical,
                additional:
                [
                    attack =>
                    {
                        if (attack is { StatusInfo.Dodged: false, Target: Character target } && target.IsAlive(false))
                            Console.WriteLine(
                                $"{target.Name} est étourdi pendant {target.AddEffect(StatusEffect.Stun, 2)} tours.");
                    }
                ])
        ]);
    }

    /// <summary>
    /// Handles the warrior's defense against incoming attacks.
    /// </summary>
    /// <typeparam name="TTarget">The type of the target being attacked.</typeparam>
    /// <param name="from">The incoming attack.</param>
    /// <param name="damageParameter">The character responsible for the damage.</param>
    /// <returns>The amount of damage taken after applying the warrior's defensive effects.</returns>
    /// <remarks>
    /// This method includes the following behaviors:
    /// <list type="bullet">
    ///     <item>
    ///         <description>If the attack is blocked, the warrior guarantees a counterattack.</description>
    ///     </item>
    ///     <item>
    ///         <description>Even if the attack is not blocked, there is a 25% chance to counterattack, dealing a percentage of the damage received.</description>
    ///     </item>
    /// </list>
    /// </remarks>
    public override int Defend<TTarget>(Attack<TTarget> from, Character damageParameter)
    {
        from.StatusInfo.Set(from, (false, false, false));
        from.StatusInfo.SetDamage(from, damageParameter);

        if (from.StatusInfo.Blocked || new Random().NextDouble() < 0.25) from.Additional.List.Add(Special);

        return TakeDamage((int)from.StatusInfo.Damage);
    }

    /// <summary>
    /// Represents the Warrior's counterattack mechanics.
    /// </summary>
    /// <remarks>
    /// Creates a counterattack called "Contre-attaque" when the Warrior blocks or has a chance to retaliate.
    /// The counterattack deals damage proportional to whether the incoming attack was blocked or not.
    /// </remarks>
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
            damage: _ => (int)(GetAttack(DamageType.Physical) * (attackFrom.StatusInfo.Blocked ? 1.50m : 0.50m)),
            attackType: DamageType.Physical
        );
        conterAttack.Execute();
        attackFrom.Additional.ToRemove.Add(Special);
    };
}