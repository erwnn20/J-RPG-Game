using JRPG_Game.Characters.Elements.Skills;
using JRPG_Game.Enums;
using JRPG_Game.Interfaces;
using JRPG_Game.Utils;

namespace JRPG_Game.Characters.Classes.Bonus;

/// <summary>
/// Represents a Necromancer character specializing in dark magic and soul manipulation.
/// </summary>
/// <remarks>
/// The Necromancer is a spell caster with abilities to resurrect allies, deal magical damage, 
/// and reflect incoming magical attacks. Its skills often come with trade-offs, like self-damage
/// or paralysis after use.
/// </remarks>
public class Necromancer : Character, IMana
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Necromancer"/> class.
    /// </summary>
    /// <param name="name">The name of the Necromancer.</param>
    /// <param name="team">The team to which the Necromancer belongs.</param>
    /// <remarks>
    /// The Necromancer has strong magical abilities and medium survivability, relying on mana to
    /// execute its skills effectively. It excels in utility and magical offense.
    /// </remarks>
    public Necromancer(string name, Team team)
        : base(
            name: name,
            team: team,
            maxHealth: 85,
            speed: 70,
            armor: ArmorType.Leather,
            physicalAttack: 0,
            magicalAttack: 85,
            distanceAttack: 0,
            paradeChance: 0.10m,
            dodgeChance: 0.15m,
            spellResistanceChance: 0.35m,
            skills: [])
    {
        Skills.AddRange([
            new SpecialAbility<Character>(
                name: "Ressusciter les morts",
                description: () => "Ramène un allié tombé au combat avec 75% de sa vie maximum.\n" +
                                   $"{Name} sera paralysé pendant 3 tours.",
                owner: this,
                targetType: TargetType.TeammateDead,
                reloadTime: 3,
                manaCost: 35,
                effect: target =>
                {
                    target.Health.Add((int)((target.Health.Max ?? 0) * 0.75m));
                    return $"{target.Name} a été ressuscité.\n" +
                           $"{Name} est paralysé pendant {AddEffect(StatusEffect.Paralysis, 3)} tours.";
                }),
            new Attack<Character>(
                name: "Force magique",
                description: () =>
                    $"Inflige 135% de la puissance magique ({GetAttack(DamageType.Magical)}) à la cible.\n" +
                    $"Inflige 35% des dégâts infligé par l'attaque à {Name}.",
                owner: this,
                targetType: TargetType.Enemy,
                reloadTime: 2,
                manaCost: 30,
                damage: _ => (int)(GetAttack(DamageType.Magical) * 1.35m),
                attackType: DamageType.Magical,
                additional:
                [
                    attack =>
                    {
                        if (attack.StatusInfo.Damage > 0)
                            Console.WriteLine(
                                $"{attack.Owner.Name} s'inflige {attack.Owner.TakeDamage((int)(attack.StatusInfo.Damage * 0.35m))} de dégâts suite à {attack.Name}.");
                        IsAlive(true);
                    }
                ]),
            new Attack<Character>(
                name: "Drain d'âme",
                description: () =>
                {
                    const decimal min = 0.20m, max = 0.50m;
                    return
                        $"Inflige entre 20% ({(int)(GetAttack(DamageType.Magical) * min)}) et 50% ({(int)(GetAttack(DamageType.Magical) * max)}) de la puissance magique à la cible.\n" +
                        $"Soigne {Name} des dégâts infligés.";
                },
                owner: this,
                targetType: TargetType.Enemy,
                reloadTime: 1,
                manaCost: 10,
                damage: _ =>
                {
                    const decimal min = 0.20m, max = 0.50m;
                    return (int)(GetAttack(DamageType.Magical) *
                                 (new Random().NextDouble() * (double)Math.Abs(max - min) + (double)min));
                },
                attackType: DamageType.Magical,
                additional:
                [
                    attack =>
                    {
                        if (attack.StatusInfo.Damage > 0)
                            Console.WriteLine(
                                $"{attack.Owner.Name} a drainé {attack.Owner.Heal((int)attack.StatusInfo.Damage)} PV.");
                    }
                ]),
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

    public NumericContainer<int> Mana { get; } = new(0, 80, 80);
    private bool SpellReturn { get; set; }

    /// <summary>
    /// Performs defense logic when the Necromancer is attacked.
    /// </summary>
    /// <typeparam name="TTarget">The type of the target being attacked.</typeparam>
    /// <param name="from">The incoming attack.</param>
    /// <param name="damageParameter">The character responsible for the attack.</param>
    /// <returns>The amount of damage taken after defensive calculations.</returns>
    /// <remarks>
    /// If the incoming attack is magical and <see cref="SpellReturn"/> is true, the attack is reflected
    /// back to its source.
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

        return TakeDamage((int)from.StatusInfo.Damage);
    }

    /// <summary>
    /// Logic for reflecting a magical attack back to its source.
    /// </summary>
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
            addToGlobalList: false,
            attackType: attackFrom.AttackType
        );
        conterAttack.Execute();
        attackFrom.Additional.ToRemove.Add(Special);
    };

    //

    /// <summary>
    /// Returns a string representation of the Necromancer.
    /// </summary>
    /// <returns>A string describing the Necromancer.</returns>
    public override string ToString()
    {
        return base.ToString() + "\n" +
               $" ~ Mana: {Mana.Current}/{Mana.Max}" +
               (SpellReturn ? "\n La prochaine attaque magique subie sera renvoyée." : string.Empty);
    }
}