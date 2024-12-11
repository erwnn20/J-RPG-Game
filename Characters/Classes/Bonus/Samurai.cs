using JRPG_Game.Characters.Skills;
using JRPG_Game.Enums;

namespace JRPG_Game.Characters.Classes.Bonus;

/// <summary>
/// Represents a samurai character with high physical attack power and unique defensive abilities.
/// </summary>
/// <remarks>
/// Inherits from <see cref="Character"/> and provides special mechanics such as increased physical attack and evasion.
/// Includes unique abilities like "Force intérieure" and "Dernier souffle."
/// </remarks>
public class Samurai : Character
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Samurai"/> class.
    /// </summary>
    /// <param name="name">The name of the samurai.</param>
    /// <param name="team">The team to which the samurai belongs.</param>
    /// <remarks>
    /// Sets the samurai's stats and initializes its skill set with powerful physical attacks
    /// and unique abilities to enhance performance during critical moments.
    /// </remarks>
    public Samurai(string name, Team team)
        : base(
            name: name,
            team: team,
            maxHealth: 85,
            speed: 80,
            armor: ArmorType.Plates,
            physicalAttack: 75,
            magicalAttack: 0,
            distanceAttack: 0,
            paradeChance: 0.35m,
            dodgeChance: 0.15m,
            spellResistanceChance: 0.10m,
            skills: [])
    {
        Skills.AddRange([
            new Attack<Character>(
                name: "Coup tranchant",
                description: () =>
                    $"Inflige 100% de la puissance d’attaque physique ({GetAttack(DamageType.Physical)}) à la cible.\n" +
                    "Si l'attaque n'est pas esquivée, cause un saignement pendant 2 tours.",
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
                        if (attack is { StatusInfo.Dodged: false, Target: Character target })
                            Console.WriteLine(
                                $"{target.Name} subit des saignements pendant {target.AddEffect(StatusEffect.Bleeding, 2)} tours.");
                    }
                ]),
            new Attack<Character>(
                name: "Coup fatal",
                description: () =>
                    $"Inflige 120% de la puissance d’attaque physique ({(int)(GetAttack(DamageType.Physical) * 1.20m)}) à la cible.\n" +
                    $"A une chance d'infliger 160% de la puissance d’attaque physique ({(int)(GetAttack(DamageType.Physical) * 1.60m)}).\n" +
                    $"Inflige a {Name} 25% des dégâts fait.",
                owner: this,
                targetType: TargetType.Enemy,
                reloadTime: 4,
                manaCost: 0,
                damage: _ => (int)(GetAttack(DamageType.Physical) * (new Random().NextDouble() < 0.10 ? 1.60m : 1.20m)),
                attackType: DamageType.Physical,
                additional:
                [
                    attack =>
                    {
                        if (attack.StatusInfo.Damage > 0)
                            Console.WriteLine(
                                $"{attack.Owner.Name} s'inflige {attack.Owner.TakeDamage((int)(attack.StatusInfo.Damage * 0.25m))} de dégâts suite à {attack.Name}.");
                        IsAlive(true);
                    }
                ]),
            new Attack<Team>(
                name: "Danse des lames",
                description: () =>
                    $"Inflige 40% de la puissance d’attaque physique ({(int)(GetAttack(DamageType.Physical) * 0.40m)}) toute l’équipe ciblé.",
                owner: this,
                targetType: TargetType.TeamEnemy,
                reloadTime: 2,
                manaCost: 0,
                damage: _ => (int)(GetAttack(DamageType.Physical) * 0.40m),
                attackType: DamageType.Physical),
            new SpecialAbility<Character>(
                name: "Force intérieure",
                description: () => "Donne l'effet 'Focus' pendant 3 tours.",
                owner: this,
                targetType: TargetType.Self,
                reloadTime: 2,
                manaCost: 0,
                effect: target =>
                    $"{target.Name} est maintenant 'Focus' pendant {target.AddEffect(StatusEffect.Focus, 3)} tours.")
        ]);
    }

    /// <summary>
    /// Handles the samurai's defense logic against incoming attacks.
    /// </summary>
    /// <typeparam name="TTarget">The type of the target being attacked.</typeparam>
    /// <param name="from">The incoming attack.</param>
    /// <param name="damageParameter">The character responsible for the damage.</param>
    /// <returns>The amount of damage taken after applying defensive effects.</returns>
    /// <remarks>
    /// Activates the "Dernier souffle" ability if the samurai's health drops below 5.
    /// </remarks>
    public override int Defend<TTarget>(Attack<TTarget> from, Character damageParameter)
    {
        from.StatusInfo.Set(from, (false, false, false));
        from.StatusInfo.SetDamage(from, damageParameter);

        var damages = TakeDamage((int)from.StatusInfo.Damage);

        if (Health.Current <= 5) from.Additional.List.Add(Special);

        return damages;
    }

    /// <summary>
    /// Handles the special counterattack logic for the samurai's "Dernier souffle" ability.
    /// </summary>
    /// <remarks>
    /// Triggers a powerful counterattack when the samurai's health drops below 5,
    /// with a 50% chance of causing the samurai's own death after the attack.
    /// </remarks>
    private Action<Attack<Character>> Special => attackFrom =>
    {
        var conterAttack = new Attack<Character>(
            name: "Dernier souffle",
            owner: this,
            description: () =>
                $"Si il reste mon de 5 PV au samouraï, il déclenche une attaque qui inflige 200% de la puissance d’attaque physique ({(int)(GetAttack(DamageType.Physical) * 2.00m)}) à l’attaquant.\n" +
                $"Si l'attaque n'est pas esquivée, cause des saignements et étourdi la cible pendant 3 tours.\n" +
                $"A 50% de chances de tuer le samouraï.",
            target: attackFrom.Owner,
            targetType: TargetType.Enemy,
            reloadTime: 0,
            manaCost: 0,
            damage: _ => (int)(GetAttack(DamageType.Physical) * 2.00m),
            attackType: DamageType.Physical,
            additional:
            [
                attack =>
                {
                    if (attack is not { StatusInfo.Dodged: false, Target: Character target }) return;

                    Console.WriteLine(
                        $"{target.Name} subit des saignements pendant {target.AddEffect(StatusEffect.Bleeding, 3)} tours.");
                    Console.WriteLine(
                        $"{target.Name} est étourdi pendant {target.AddEffect(StatusEffect.Stun, 3)} tours.");
                },
                attack =>
                {
                    if (!(new Random().NextDouble() < 0.5)) return;
                    Health.Subtract(Health.Current);
                    Console.WriteLine($"{attack.Owner.Name} est mort après avoir fait 'Dernier souffle'.");
                }
            ]
        );
        conterAttack.Execute();
        attackFrom.Additional.ToRemove.Add(Special);
    };
}