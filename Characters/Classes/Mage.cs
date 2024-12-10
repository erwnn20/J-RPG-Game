using JRPG_Game.Characters.Skills;
using JRPG_Game.Enums;
using JRPG_Game.Interfaces;
using JRPG_Game.Utils;

namespace JRPG_Game.Characters.Classes;

/// <summary>
/// Represents a mage character with magical abilities and a mana pool.
/// </summary>
/// <remarks>
/// Inherits from <see cref="Character"/> and implements the <see cref="IMana"/> interface.
/// The mage has unique skills and special mechanics such as mana management and spell reflection.
/// </remarks>
public class Mage : Character, IMana
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Mage"/> class.
    /// </summary>
    /// <param name="name">The name of the mage.</param>
    /// <param name="team">The team to which the mage belongs.</param>
    /// <remarks>
    /// Sets the mage's stats and initializes its skill set with unique magical abilities.
    /// </remarks>
    public Mage(string name, Team team)
        : base(
            name: name,
            team: team,
            maxHealth: 80,
            speed: 60,
            armor: ArmorType.Textile,
            physicalAttack: 0,
            magicalAttack: 80,
            distanceAttack: 35,
            paradeChance: 0.05m,
            dodgeChance: 0.10m,
            spellResistanceChance: 0.30m,
            skills: [])
    {
        Skills.AddRange([
            new Attack<Character>(
                name: "Eclair de givre",
                description: () =>
                    $"Inflige 100% de la puissance d’attaque magique ({GetAttack(DamageType.Magical)}) à la cible.\n" +
                    "Si la cible ne résiste pas à l'attaque :\n" +
                    "   - elle est ralenti pendant pendant 3 tours\n" +
                    "   - a 15% de chance d'être paralysée pendant 2 tours\n" +
                    "   - a 5% de chance de voir sa vitesse diminuée définitivement de 15%",
                owner: this,
                targetType: TargetType.Enemy,
                reloadTime: 1,
                manaCost: 15,
                damage: _ => GetAttack(DamageType.Magical),
                attackType: DamageType.Magical,
                additional:
                [
                    attack =>
                    {
                        if (attack is not { StatusInfo.Resisted: false, Target: Character target }) return;

                        Console.WriteLine(
                            $"{target.Name} est ralenti pendant {target.AddEffect(StatusEffect.Slowness, 3)} tours.");

                        var random = new Random();
                        if (random.NextDouble() < 0.15)
                            Console.WriteLine(
                                $"{target.Name} est paralysé pendant {target.AddEffect(StatusEffect.Paralysis, 2)} tours.");

                        if (random.NextDouble() < 0.05)
                        {
                            target.Speed = (int)(target.Speed * 0.85m);
                            Console.WriteLine($"La vitesse de {target.Name} à diminué de 15%.");
                        }
                    }
                ]),
            new SpecialAbility<Character>(
                name: "Barrière de givre",
                description: () => "Réduit les dégâts des deux prochaines attaques subies de :\n" +
                                   $"   - {ReduceDamagePhysical:P} sur les attaques physiques\n" +
                                   $"   - {ReduceDamageMagical:P} sur les attaques magiques\n" +
                                   $"   - {ReduceDamageDistance:P} sur les attaques à distance",
                owner: this,
                targetType: TargetType.Self,
                reloadTime: 2,
                manaCost: 25,
                effect: _ =>
                {
                    const int attackReduced = 2;
                    ReducedAttack += attackReduced;
                    return $"Les {attackReduced} prochaines attaques subies par {Name} sont réduites.";
                }),
            new Attack<Team>(
                name: "Blizzard",
                description: () =>
                    $"Inflige 50% de la puissance d’attaque magique ({(int)(GetAttack(DamageType.Magical) * 0.50m)}) à toute l’équipe ciblé.\n" +
                    "Si la cible ne résiste pas à l'attaque :\n" +
                    "   - elle est ralenti pendant pendant 2 tours\n" +
                    "   - a 10% de chance d'être paralysée pendant 1 tours\n" +
                    "   - a 5% de chance de voir sa vitesse diminuée définitivement de 5%",
                owner: this,
                targetType: TargetType.TeamEnemy,
                reloadTime: 2,
                manaCost: 25,
                damage: _ => (int)(GetAttack(DamageType.Magical) * 0.5m),
                attackType: DamageType.Magical,
                additional:
                [
                    attack =>
                    {
                        if (attack is not { StatusInfo.Resisted: false, Target: Character target }) return;

                        Console.WriteLine(
                            $"{target.Name} est ralenti pendant {target.AddEffect(StatusEffect.Slowness, 2)} tours.");

                        var random = new Random();
                        if (random.NextDouble() < 0.10)
                            Console.WriteLine(
                                $"{target.Name} est paralysé pendant {target.AddEffect(StatusEffect.Paralysis, 1)} tours.");

                        if (random.NextDouble() < 0.05)
                        {
                            target.Speed = (int)(target.Speed * 0.95m);
                            Console.WriteLine($"La vitesse de {target.Name} à diminué de 5%.");
                        }
                    }
                ]),
            new Attack<Character>(
                name: "Boule de feu",
                description: () =>
                {
                    int distanceDamage = (int)(GetAttack(DamageType.Distance) * 0.5m),
                        magicDamage = (int)(GetAttack(DamageType.Magical) * 0.5m);
                    return
                        $"Inflige 50% des dégâts d'attaque à distance ({distanceDamage}) plus 50% des dégâts d'attaque magique ({magicDamage}, total: {distanceDamage + magicDamage}) à la cible.\n" +
                        "Si l'attaque n'est pas esquivée, inflige des dégâts de brulure pendant 1 à 3 tours.";
                },
                owner: this,
                targetType: TargetType.Enemy,
                reloadTime: 1,
                manaCost: 15,
                damage: _ => (int)(GetAttack(DamageType.Distance) * 0.5m) + (int)(GetAttack(DamageType.Magical) * 0.5m),
                attackType: DamageType.Distance,
                additional:
                [
                    attack =>
                    {
                        if (attack is { StatusInfo.Dodged: false, Target: Character target })
                            Console.WriteLine(
                                $"{target.Name} est brulé pendant {target.AddEffect(StatusEffect.Burn, new Random().Next(1, 3))} tours.");
                    }
                ]),
            new SpecialAbility<Character>(
                name: "Brulure de mana",
                description: () => "Réduit de moitié la quantité de points de mana de la cible.",
                owner: this,
                targetType: TargetType.Enemy,
                reloadTime: 3,
                manaCost: 20,
                effect: target =>
                    target is IMana t
                        ? $"{target.Name} perd {t.LoseMana(Math.Max(40, t.Mana.Current / 2))} de mana."
                        : $"{target.Name} n'utilise pas de mana."),
            new SpecialAbility<Character>(
                name: "Renvoi de sort",
                description: () => "Renvoie la prochaine attaque magique subie à l’assaillant.",
                owner: this,
                targetType: TargetType.Self,
                reloadTime: 1,
                manaCost: 25,
                effect: _ =>
                {
                    SpellReturn = true;
                    return $"La prochaine attaque magique subie par {Name} sera renvoyée.";
                }),
            ((IMana)this).Drink(this)
        ]);
    }

    public NumericContainer<int> Mana { get; } = new(0, 100, 100);
    private static decimal ReduceDamagePhysical => 0.60m;
    private static decimal ReduceDamageMagical => 0.50m;
    private static decimal ReduceDamageDistance => 0.70m;
    private int ReducedAttack { get; set; }
    private bool SpellReturn { get; set; }

    /// <summary>
    /// Handles the mage's defense against incoming attacks.
    /// </summary>
    /// <typeparam name="TTarget">The type of the target being attacked.</typeparam>
    /// <param name="from">The incoming attack.</param>
    /// <param name="damageParameter">The character responsible for the damage.</param>
    /// <returns>The amount of damage taken after applying defensive effects.</returns>
    /// <remarks>
    /// This method applies the following effects:
    /// <list type="bullet">
    ///     <item>
    ///         <description>Reflects magical attacks if the spell reflection ability is active.</description>
    ///     </item>
    ///     <item>
    ///         <description>Reduces damage based on <c>Barrière de givre</c> if it is active.</description>
    ///     </item>
    /// </list>
    /// </remarks>
    public override int Defend<TTarget>(Attack<TTarget> from, Character damageParameter)
    {
        from.StatusInfo.Set(from, (false, SpellReturn, false));
        from.StatusInfo.SetDamage(from, damageParameter);

        if (from.AttackType == DamageType.Magical && SpellReturn)
        {
            from.Additional.List.Add(Special);
            SpellReturn = false;
        }

        if (ReducedAttack > 0)
        {
            from.StatusInfo.Damage *= 1 - from.AttackType switch
            {
                DamageType.Physical => ReduceDamagePhysical,
                DamageType.Magical => ReduceDamageMagical,
                DamageType.Distance => ReduceDamageDistance,
                _ => 0
            };
            ReducedAttack--;
        }

        return TakeDamage((int)from.StatusInfo.Damage);
    }

    /// <summary>
    /// Handles the special counterattack logic for the mage's spell reflection ability.
    /// </summary>
    /// <remarks>
    /// Creates a counterattack that deals the same damage as the original magical attack
    /// and sends it back to the attacker.
    /// </remarks>
    private Action<Attack<Character>> Special => attackFrom =>
    {
        Console.WriteLine($"{Name} revoie {attackFrom.Name}.");
        var conterAttack = new Attack<Character>(
            name: attackFrom.Name,
            description: attackFrom.Description,
            owner: this,
            target: attackFrom.Owner,
            targetType: TargetType.Enemy,
            reloadTime: 0,
            manaCost: 0,
            damage: attackFrom.Damage,
            attackType: attackFrom.AttackType
        );
        conterAttack.Execute();
        attackFrom.Additional.ToRemove.Add(Special);
    };

    //

    /// <summary>
    /// Returns a string that represents the <see cref="Mage"/>.
    /// </summary>
    /// <returns>A string that represents the <c>Mage</c></returns>
    public override string ToString()
    {
        return base.ToString() + "\n" +
               $" ~ Mana: {Mana.Current}/{Mana.Max}" +
               (ReducedAttack > 0
                   ? $"\n Dégâts réduits pendant {(ReducedAttack > 1 ? $"les {ReducedAttack} prochaines attaques subies." : "la prochaine attaque subie.")}"
                   : string.Empty) +
               (SpellReturn ? "\n La prochaine attaque magique subie sera renvoyée." : string.Empty);
    }
}