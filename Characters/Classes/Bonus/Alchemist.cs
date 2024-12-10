using JRPG_Game.Characters.Skills;
using JRPG_Game.Enums;
using JRPG_Game.Interfaces;
using JRPG_Game.Utils;

namespace JRPG_Game.Characters.Classes.Bonus;

/// <summary>
/// Represents an Alchemist character, a versatile support and magical damage dealer.
/// </summary>
/// <remarks>
/// The Alchemist combines magical attacks with utility skills, such as healing and buffing allies.
/// Their abilities often have area-of-effect or damage-over-time mechanics.
/// </remarks>
public class Alchemist : Character, IMana
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Alchemist"/> class.
    /// </summary>
    /// <param name="name">The name of the Alchemist.</param>
    /// <param name="team">The team to which the Alchemist belongs.</param>
    /// <remarks>
    /// The Alchemist has moderate survivability and excels at providing support while dealing 
    /// consistent magical damage. Its skills revolve around healing, buffing, and applying 
    /// debuffs to enemies.
    /// </remarks>
    public Alchemist(string name, Team team)
        : base(
            name: name,
            team: team,
            maxHealth: 80,
            speed: 75,
            armor: ArmorType.Textile,
            physicalAttack: 25,
            magicalAttack: 70,
            distanceAttack: 0,
            paradeChance: 0.10m,
            dodgeChance: 0.15m,
            spellResistanceChance: 0.25m,
            skills: [])
    {
        Skills.AddRange([
            new SpecialAbility<Character>(
                name: "Potion de guérison",
                description: () =>
                    $"Donne l'effet 'Régénération' à un allié pendant un nombre de tour égale à la puissance d'attaque magique divisé par 10 ({GetAttack(DamageType.Magical) / 10}).",
                owner: this,
                targetType: TargetType.Teammate,
                reloadTime: 2,
                manaCost: 25,
                effect: target =>
                    $"{target.Name} a l'effet 'Régénération' pendant {target.AddEffect(StatusEffect.Regeneration, GetAttack(DamageType.Magical) / 10)} tours"),
            new Attack<Team>(
                name: "Explosion chimique",
                description: () =>
                    $"Inflige la puissance d'attaque magique ({GetAttack(DamageType.Magical)}) divisé par le nombre de personnage en vie dans l'équipe attaquée.\n" +
                    $"Si la cible ne résiste pas à l'attaque, inflige des brulures pendant un à deux tours.",
                owner: this,
                targetType: TargetType.TeamEnemy,
                reloadTime: 1,
                manaCost: 15,
                damage: target =>
                    GetAttack(DamageType.Magical) / target.Team.Characters.Count(character => character.IsAlive(false)),
                attackType: DamageType.Magical,
                additional:
                [
                    attack =>
                    {
                        if (attack is { StatusInfo.Resisted: false, Target: Character target })
                            Console.WriteLine(
                                $"{target.Name} est brulé pendant {target.AddEffect(StatusEffect.Burn, new Random().Next(1, 2))} tour(s).");
                    }
                ]),
            new SpecialAbility<Character>(
                name: "Serum de force",
                description: () => "Donne l'effet force pendant 3 tours à un allié.",
                owner: this,
                targetType: TargetType.Teammate,
                reloadTime: 2,
                manaCost: 20,
                effect: target =>
                    $"{target.Name} a l'effet 'Force' pendant {target.AddEffect(StatusEffect.Strength, 3)} tours"),
            new Attack<Character>(
                name: "Lame Empoisonnée",
                description: () =>
                    $"Inflige 100% de la puissance de l'attaque physique ({GetAttack(DamageType.Physical)}).\n" +
                    $"Si l'attaque n'est ni esquiver, ni parée, inflige des dégâts de poison pendant 1 à 3 tours.",
                owner: this,
                targetType: TargetType.Enemy,
                reloadTime: 1,
                manaCost: 5,
                damage: _ => GetAttack(DamageType.Physical),
                attackType: DamageType.Physical,
                additional:
                [
                    attack =>
                    {
                        if (attack is { StatusInfo : { Dodged: false, Blocked: false }, Target: Character target })
                            Console.WriteLine(
                                $"{target.Name} est empoisonné pendant {target.AddEffect(StatusEffect.Poison, new Random().Next(1, 3))} tours.");
                    }
                ]),
            ((IMana)this).Drink(this)
        ]);
    }

    public NumericContainer<int> Mana { get; } = new(0, 70, 70);

    /// <summary>
    /// Performs defense logic when the Alchemist is attacked.
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

    //

    /// <summary>
    /// Returns a string representation of the Alchemist, including its current mana.
    /// </summary>
    /// <returns>A string describing the Alchemist and its status.</returns>
    public override string ToString()
    {
        return base.ToString() + "\n" +
               $" ~ Mana: {Mana.Current}/{Mana.Max}";
    }
}