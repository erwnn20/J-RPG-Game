using JRPG_Game.Characters.Skills;
using JRPG_Game.Enums;

namespace JRPG_Game.Characters.Classes.Bonus;

/// <summary>
/// Represents an Archer character with high mobility and long-range offensive capabilities.
/// </summary>
/// <remarks>
/// The Archer is a ranged damage dealer that relies on distance attacks and status effects
/// like Poison and Burn to weaken enemies over time.
/// </remarks>
public class Archer : Character
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Archer"/> class.
    /// </summary>
    /// <param name="name">The name of the Archer.</param>
    /// <param name="team">The team to which the Archer belongs.</param>
    /// <remarks>
    /// The Archer is equipped with high speed and a unique set of ranged skills
    /// that emphasize precision, status effects, and burst damage through abilities like "Flèches Triple."
    /// </remarks>
    public Archer(string name, Team team)
        : base(
            name: name,
            team: team,
            maxHealth: 85,
            speed: 85,
            armor: ArmorType.Mesh,
            physicalAttack: 0,
            magicalAttack: 0,
            distanceAttack: 80,
            paradeChance: 0.05m,
            dodgeChance: 0.25m,
            spellResistanceChance: 0.20m,
            skills: [])
    {
        Skills.AddRange([
            new Attack<Character>(
                name: "Tir précis",
                description: () =>
                    $"Inflige 100% de la puissance d’attaque à distance ({GetAttack(DamageType.Distance)}) à la cible.",
                owner: this,
                targetType: TargetType.Enemy,
                reloadTime: 1,
                manaCost: 0,
                damage: _ => (int)(GetAttack(DamageType.Distance) * (TripleArrows ? 1.33m : 1)),
                attackType: DamageType.Distance,
                additional:
                [
                    _ => TripleArrows = false
                ]),
            new Attack<Character>(
                name: "Flèche empoisonnée",
                description: () =>
                    $"Inflige 70% de la puissance d’attaque à distance ({(int)(GetAttack(DamageType.Distance) * 0.70m)}) à la cible.\n" +
                    "Si l'attaque n'est ni esquivée, ni parée, la cible est empoisonnée pendant 2 tours.",
                owner: this,
                targetType: TargetType.Enemy,
                reloadTime: 2,
                manaCost: 0,
                damage: _ => (int)(GetAttack(DamageType.Distance) * 0.70m * (TripleArrows ? 1.33m : 1)),
                attackType: DamageType.Distance,
                additional:
                [
                    attack =>
                    {
                        if (attack is { StatusInfo: { Dodged: false, Blocked: false }, Target: Character target } &&
                            target.IsAlive(false))
                            Console.WriteLine(
                                $"{target.Name} est empoisonnée pendant {target.AddEffect(StatusEffect.Poison, 2)} tours.");
                    },
                    _ => TripleArrows = false
                ]),
            new Attack<Team>(
                name: "Pluie de flèche enflammée",
                description: () =>
                    $"Inflige 20% de la puissance d’attaque à distance ({(int)(GetAttack(DamageType.Distance) * 0.20m)}) à la cible.\n" +
                    "Si l'attaque n'est pas esquivée, la cible est brulée pendant 3 tours.",
                owner: this,
                targetType: TargetType.TeamEnemy,
                reloadTime: 2,
                manaCost: 0,
                damage: _ => (int)(GetAttack(DamageType.Distance) * 0.20m * (TripleArrows ? 1.33m : 1)),
                attackType: DamageType.Distance,
                additional:
                [
                    attack =>
                    {
                        if (attack is { StatusInfo.Dodged: false, Target: Character target } && target.IsAlive(false))
                            Console.WriteLine(
                                $"{target.Name} est brulé pendant {target.AddEffect(StatusEffect.Burn, 3)} tours.");
                    },
                    _ => TripleArrows = false
                ]),
            new SpecialAbility<Character>(
                name: "Flèches Triple",
                description: () => "La prochaine attaque fait 33% de dégâts supplémentaires.",
                owner: this,
                targetType: TargetType.Self,
                reloadTime: 3,
                manaCost: 0,
                effect: target =>
                {
                    TripleArrows = true;
                    return $"La prochaine attaque de {target.Name} fait 33% de dégâts supplémentaires.";
                })
        ]);
    }

    private bool TripleArrows { get; set; }

    /// <summary>
    /// Performs defense logic when the Archer is attacked.
    /// </summary>
    /// <typeparam name="TTarget">The type of the target being attacked.</typeparam>
    /// <param name="from">The incoming attack.</param>
    /// <param name="damageParameter">The character responsible for the attack.</param>
    /// <returns>The amount of damage taken after defensive calculations.</returns>
    public override int Defend<TTarget>(Attack<TTarget> from, Character damageParameter)
    {
        from.StatusInfo.Set(from, (false, false, false));
        from.StatusInfo.SetDamage(from, damageParameter);

        return TakeDamage((int)from.StatusInfo.Damage);
    }

    /// <summary>
    /// Returns a string representation of the Archer, including its current status.
    /// </summary>
    /// <returns>A string describing the Archer, including information about "Triple Arrows" if active.</returns>
    public override string ToString()
    {
        return base.ToString() +
               (TripleArrows ? "\n La prochaine attaque fait 33% de dégâts supplémentaires." : string.Empty);
    }
}