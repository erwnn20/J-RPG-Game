using JRPG_Game.Characters.Skills;
using JRPG_Game.Enums;
using JRPG_Game.Utils;

namespace JRPG_Game.Characters.Classes;

/// <summary>
/// Represents a thief character with high speed and evasion capabilities.
/// </summary>
/// <remarks>
/// Inherits from <see cref="Character"/> and specializes in physical attacks and defensive maneuvers.
/// The Thief is agile and has abilities that exploit low-health targets and improve survival.
/// </remarks>
public class Thief : Character
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Thief"/> class.
    /// </summary>
    /// <param name="name">The name of the thief.</param>
    /// <param name="team">The team to which the thief belongs.</param>
    /// <remarks>
    /// Sets the thief's stats and initializes its unique skill set, emphasizing speed, physical attack,
    /// and enhanced defensive maneuvers like dodging and counterattacking.
    /// </remarks>
    public Thief(string name, Team team)
        : base(
            name: name,
            team: team,
            maxHealth: 75,
            speed: 120,
            armor: ArmorType.Leather,
            physicalAttack: 45,
            magicalAttack: 0,
            distanceAttack: 0,
            paradeChance: new NumericContainer<decimal>(0, 0.10m, 1),
            dodgeChance: new NumericContainer<decimal>(0, 0.40m, 0.80m),
            spellResistanceChance: new NumericContainer<decimal>(0, 0.10m, 0.50m),
            skills: [])
    {
        Skills.AddRange([
            new Attack<Character>(
                name: "Coup bas",
                description: () =>
                    $"Inflige 100% de la puissance d’attaque physique ({GetAttack(DamageType.Physical)}) à la cible.\n" +
                    $"Inflige 150% si la cible a moins de la moitié de ses points de vie.",
                owner: this,
                targetType: TargetType.Enemy,
                reloadTime: 1,
                manaCost: 0,
                damage: target =>
                    (int)(GetAttack(DamageType.Physical) *
                          (target.Health.Current < target.Health.Max / 2 ? 1.50m : 1.00m)),
                attackType: DamageType.Physical),
            new SpecialAbility<Character>(
                name: "Evasion",
                description: () =>
                {
                    const decimal dodgeAugment = 0.10m, resistAugment = 0.05m;
                    decimal newDodge = DodgeChance.Current + dodgeAugment,
                        newResist = SpellResistanceChance.Current + resistAugment;

                    return
                        $"Augmente les chances d'esquive de {dodgeAugment:P} (max {DodgeChance.Max:P}) ({DodgeChance.Current:P} {(DodgeChance.Current == DodgeChance.Max
                            ? "MAX"
                            : $"-> {Math.Min(newDodge, DodgeChance.Max ?? newDodge):P}{(Math.Min(newDodge, DodgeChance.Max ?? newDodge) == DodgeChance.Max ? " MAX" : string.Empty)}")})\n" +
                        $"Augmente les chances de resister aux sorts de {resistAugment:P} (max {SpellResistanceChance.Max:P}) ({SpellResistanceChance.Current:P} {(SpellResistanceChance.Current == SpellResistanceChance.Max
                            ? "MAX"
                            : $"-> {Math.Min(newResist, SpellResistanceChance.Max ?? newResist):P}{(Math.Min(newResist, SpellResistanceChance.Max ?? newResist) == SpellResistanceChance.Max ? " MAX" : string.Empty)}")})\n" +
                        $"Donne l'effet 'vitesse' pendant 2 tours.";
                },
                owner: this,
                targetType: TargetType.Self,
                reloadTime: 1,
                manaCost: 0,
                effect: _ =>
                {
                    const decimal dodgeAugment = 0.10m, resistAugment = 0.05m;

                    var output = "";
                    var addedDodge = DodgeChance.Add(dodgeAugment);
                    output += addedDodge > 0
                        ? $"{Name} augmente ses chances d'esquive de {addedDodge:P} ({DodgeChance.Current - addedDodge:P} -> {DodgeChance.Current:P}{(DodgeChance.Current == DodgeChance.Max ? " MAX" : string.Empty)})."
                        : $"{Name} a ses chances d'esquive au maximum -> {DodgeChance.Current:P}.";

                    var addedResistance = SpellResistanceChance.Add(resistAugment);
                    output += addedResistance > 0
                        ? $"\n{Name} augmente ses chances  de resister aux sorts de {addedResistance:P} ({SpellResistanceChance.Current - addedResistance:P} -> {SpellResistanceChance.Current:P}{(SpellResistanceChance.Current == SpellResistanceChance.Max ? " MAX" : string.Empty)})"
                        : $"\n{Name} a ses chances de resister aux sorts au maximum -> {SpellResistanceChance.Current:P}.";

                    output +=
                        $"{Name} a l'effet 'vitesse' pendant {AddEffect(StatusEffect.Speed, 2)} tours.";

                    return output;
                })
        ]);
    }

    /// <summary>
    /// Handles the thief's defense against incoming attacks.
    /// </summary>
    /// <typeparam name="TTarget">The type of the target being attacked.</typeparam>
    /// <param name="from">The incoming attack.</param>
    /// <param name="damageParameter">The character responsible for the damage.</param>
    /// <returns>The amount of damage taken after applying defensive effects.</returns>
    /// <remarks>
    /// If the Thief successfully dodges an attack, a <see cref="Special"/> counterattack is triggered.
    /// </remarks>
    public override int Defend<TTarget>(Attack<TTarget> from, Character damageParameter)
    {
        from.StatusInfo.Set(from, (false, false, false));
        from.StatusInfo.SetDamage(from, damageParameter);

        if (from.StatusInfo.Dodged) from.Additional.List.Add(Special);

        return TakeDamage((int)from.StatusInfo.Damage);
    }

    /// <summary>
    /// Represents a special counterattack that activates when the thief dodges an incoming attack.
    /// </summary>
    /// <remarks>
    /// This delegate creates a new attack named "Poignard dans le dos" and executes it against the attacker.
    /// </remarks>
    private Action<Attack<Character>> Special => attackFrom =>
    {
        var conterAttack = new Attack<Character>(
            name: "Poignard dans le dos",
            owner: this,
            description: () =>
                "Lorsque le voleur esquive une attaque ennemie, il déclenche une attaque qui inflige 15 points de dégâts physiques à l’attaquant.\n" +
                "Si l'attaque n'est ni esquivée, ni parée, cause un saignement pendant 2 tours.",
            target: attackFrom.Owner,
            targetType: TargetType.Enemy,
            reloadTime: 0,
            manaCost: 0,
            damage: 15,
            attackType: DamageType.Physical,
            addToGlobalList: false,
            additional:
            [
                attack =>
                {
                    if (attack is { StatusInfo: { Dodged: false, Blocked: false }, Target: Character target } &&
                        target.IsAlive(false))
                        Console.WriteLine(
                            $"{target.Name} subit des saignements pendant {target.AddEffect(StatusEffect.Bleeding, 2)} tours.");
                }
            ]
        );
        conterAttack.Execute();
        attackFrom.Additional.ToRemove.Add(Special);
    };
}