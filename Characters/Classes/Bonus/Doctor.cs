using JRPG_Game.Characters.Elements.Skills;
using JRPG_Game.Enums;

namespace JRPG_Game.Characters.Classes.Bonus;

/// <summary>
/// Represents a doctor character with unique healing and support abilities.
/// </summary>
/// <remarks>
/// The Doctor class specializes in healing allies and providing team-wide support.
/// It features a balance of moderate health and speed, with low offensive stats compensated by unique skills.
/// </remarks>
public class Doctor : Character
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Doctor"/> class.
    /// </summary>
    /// <param name="name">The name of the doctor.</param>
    /// <param name="team">The team to which the doctor belongs.</param>
    /// <remarks>
    /// The Doctor class is equipped with healing and utility skills such as "Guérison rapide," "Soin de groupe,"
    /// and the unique "Transfusion" ability that utilizes health drained from enemies.
    /// </remarks>
    public Doctor(string name, Team team)
        : base(
            name: name,
            team: team,
            maxHealth: 105,
            speed: 85,
            armor: ArmorType.Textile,
            physicalAttack: 30,
            magicalAttack: 0,
            distanceAttack: 0,
            paradeChance: 0.10m,
            dodgeChance: 0.35m,
            spellResistanceChance: 0.35m,
            skills: [])
    {
        Skills.AddRange([
            new SpecialAbility<Character>(
                name: "Guérison rapide",
                description: () => "Restaure la santé d'un allié 20% de sa santé maximale.",
                owner: this,
                targetType: TargetType.Teammate,
                reloadTime: 2,
                manaCost: 0,
                effect: target =>
                    $"{target.Name} est soigné de {target.Heal((int)(target.Health.Max * 0.20m))} PV."),
            new SpecialAbility<Team>(
                name: "Soin de groupe",
                description: () => "Soigne tous les alliés de 35% de leurs santé maximale.\n" +
                                   "Donne l'effet 'régénération' à la cible pendant 2 tours.",
                owner: this,
                targetType: TargetType.TeamAllied,
                reloadTime: 4,
                manaCost: 0,
                effect: target =>
                    $"{target.Name} est soigné de {target.Heal((int)(target.Health.Max * 0.35m))} PV.\n" +
                    $"{target.Name} a l'effet 'régénération' pendant {target.AddEffect(StatusEffect.Regeneration, 2)} tours."),
            new Attack<Character>(
                name: "Saignée",
                description: () =>
                    $"Inflige 100% de la puissance d'attaque physique ({GetAttack(DamageType.Physical)}) à la cible.\n" +
                    "Draine les dégâts infligés pour les utiliser avec 'Transfusion'.\n" +
                    "Si l'attaque n'est pas esquivée, cause un saignement pendant 4 tours.",
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
                        HeathPoints += (int)attack.StatusInfo.Damage;

                        if (attack is { StatusInfo.Dodged: false, Target: Character target } && target.IsAlive(false))
                            Console.WriteLine(
                                $"{target.Name} subit des saignements pendant {target.AddEffect(StatusEffect.Bleeding, 4)} tours.");
                    }
                ]),
            new SpecialAbility<Character>(
                name: "Transfusion",
                description: () => $"Soigne un allié de la quantité de PV drainé par 'Saignée' ({HeathPoints} PV).\n" +
                                   "Cause des saignement pendant 1 tour.\n" +
                                   "Donne l'effet régénération pendant 3 tours.",
                owner: this,
                targetType: TargetType.Teammate,
                reloadTime: 2,
                manaCost: 0,
                effect: target =>
                {
                    if (HeathPoints <= 0) return "";
                    var healed = target.Heal(HeathPoints);
                    HeathPoints -= healed;
                    return $"{target.Name} a été soigné de {healed} PV.\n" +
                           $"{target.Name} subit des saignements pendant {target.AddEffect(StatusEffect.Bleeding, 1)} tour(s).\n" +
                           $"{target.Name} a l'effet 'régénération' pendant {target.AddEffect(StatusEffect.Regeneration, 3)} tours.";
                })
        ]);
    }

    private int HeathPoints { get; set; }

    /// <summary>
    /// Handles the doctor's defense logic against incoming attacks.
    /// </summary>
    /// <typeparam name="TTarget">The type of the target being attacked.</typeparam>
    /// <param name="from">The incoming attack.</param>
    /// <param name="damageParameter">The character responsible for the damage.</param>
    /// <returns>The amount of damage taken after applying defensive effects.</returns>
    public override int Defend<TTarget>(Attack<TTarget> from, Character damageParameter)
    {
        from.StatusInfo.Set(from, (false, false, false));
        from.StatusInfo.SetDamage(from, damageParameter);

        return TakeDamage((int)from.StatusInfo.Damage);
    }

    //

    /// <summary>
    /// Returns a string that represents the <see cref="Doctor"/>.
    /// </summary>
    /// <returns>A string that represents the <c>Doctor</c></returns>
    public override string ToString()
    {
        return base.ToString() +
               (HeathPoints > 0 ? $"\n ~ PV drainés: {HeathPoints}" : string.Empty);
    }
}