using JRPG_Game.Characters.Skills;
using JRPG_Game.Enums;
using JRPG_Game.Interfaces;
using JRPG_Game.Utils;

namespace JRPG_Game.Characters.Classes;

/// <summary>
/// Represents a paladin character with a balance of physical and magical abilities, as well as healing powers.
/// </summary>
/// <remarks>
/// Inherits from <see cref="Character"/> and implements the <see cref="IMana"/> interface.
/// The paladin excels in versatile combat roles, capable of dealing physical and magical damage and healing allies.
/// </remarks>
public class Paladin : Character, IMana
{
    public NumericContainer Mana { get; } = new(0, 60, 60);

    /// <summary>
    /// Initializes a new instance of the <see cref="Paladin"/> class.
    /// </summary>
    /// <param name="name">The name of the paladin.</param>
    /// <param name="team">The team to which the paladin belongs.</param>
    /// <remarks>
    /// Sets the paladin's stats and initializes its skill set with abilities for physical and magical attacks, as well as healing.
    /// </remarks>
    public Paladin(string name, Team team)
        : base(
            name: name,
            team: team,
            maxHealth: 80,
            speed: 65,
            armor: ArmorType.Mesh,
            physicalAttack: 40,
            magicalAttack: 40,
            dodgeChance: 0.05m,
            paradeChance: 0.10m,
            spellResistanceChance: 0.20m,
            skills: [])
    {
        Skills.AddRange([
            new Attack<Character>(
                name: "Frappe du croisé",
                description: () => $"Inflige 100% de la puissance d’attaque physique ({PhysicalAttack}) à la cible.",
                owner: this,
                targetType: TargetType.Enemy,
                reloadTime: 1,
                manaCost: 5,
                damage: PhysicalAttack,
                attackType: DamageType.Physical,
                additional:
                [
                    Special
                ]),
            new Attack<Character>(
                name: "Jugement",
                description: () => $"Inflige 100% de la puissance d’attaque magique ({MagicalAttack}) à la cible.",
                owner: this,
                targetType: TargetType.Enemy,
                reloadTime: 1,
                manaCost: 10,
                damage: MagicalAttack,
                attackType: DamageType.Magical,
                additional:
                [
                    Special
                ]),
            new SpecialAbility<Character>(
                name: "Eclair lumineux",
                description: () =>
                    $"Soigne la cible d’un montant de 125% de la puissance d’attaque magique ({(int)(MagicalAttack * 1.25m)} PV).",
                owner: this,
                targetType: TargetType.Teammate,
                reloadTime: 1,
                manaCost: 25,
                effect: target => $"{target.Name} est soigné de {target.Heal((int)(MagicalAttack * 1.25m))} PV."),
            ((IMana)this).Drink(this)
        ]);
    }

    /// <summary>
    /// Handles the paladin's defense against incoming attacks.
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

    /// <summary>
    /// A special ability triggered during certain attacks to heal the paladin based on the damage dealt.
    /// </summary>
    /// <remarks>
    /// Heals the paladin for 50% of the damage received if the special ability is active.
    /// </remarks>
    private Action<Attack<Character>> Special => attack =>
    {
        if (Heal((int)attack.StatusInfo.Damage / 2) is var healed and > 0)
            Console.WriteLine(
                $"{Name} se soigne de {healed} PV garce à sa capacité spéciale.");
    };

    //

    /// <summary>
    /// Returns a string that represents the <see cref="Paladin"/>.
    /// </summary>
    /// <returns>A string that represents the <c>Paladin</c></returns>
    public override string ToString()
    {
        return base.ToString() + "\n" +
               $" ~ Mana: {Mana.Current}/{Mana.Max}";
    }
}