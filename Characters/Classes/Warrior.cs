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
            new SpecialAbility<Team>(
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
            new Attack<Team>(
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

        if (from.StatusInfo.Blocked || new Random().NextDouble() < 0.25) (from.Additional ??= []).Add(Special);

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
            damage: attackFrom.StatusInfo.Blocked
                ? _ => (int)(PhysicalAttack * 1.50m)
                : _ => (int)(PhysicalAttack * 0.50m),
            attackType: DamageType.Physical
        );
        conterAttack.Execute();
        conterAttack.Damage = _ => 0;
    };
}