using JRPG_Game.Characters.Skills;
using JRPG_Game.Enums;

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
            paradeChance: 0.10m,
            dodgeChance: 0.40m,
            spellResistanceChance: 0.10m,
            skills: [])
    {
        Skills.AddRange([
            new Attack<Character>(
                name: "Coup bas",
                description: () => $"Inflige 100% de la puissance d’attaque physique ({GetAttack(DamageType.Physical)}) à la cible.\n" +
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
                    $"Augmente les chances d'esquive de 20% (max 80%) ({DodgeChance:P} {(DodgeChance == 0.80m
                        ? "MAX"
                        : $"-> {Math.Min(DodgeChance + 0.20m, 0.80m):P}{(Math.Min(DodgeChance + 0.20m, 0.80m) == 0.5m ? " MAX" : string.Empty)}")})\n" +
                    $"Augmente les chances de resister aux sorts de 20% (max 50%) ({SpellResistanceChance:P} {(SpellResistanceChance == 0.5m
                        ? "MAX"
                        : $"-> {Math.Min(SpellResistanceChance + 0.20m, 0.5m):P}{(Math.Min(SpellResistanceChance + 0.20m, 0.5m) == 0.5m ? " MAX" : string.Empty)}")})",
                owner: this,
                targetType: TargetType.Self,
                reloadTime: 1,
                manaCost: 0,
                effect: _ =>
                {
                    var output = "";
                    var oldDodgeChance = DodgeChance;
                    DodgeChance = Math.Min(0.80m, DodgeChance + 0.20m);
                    output += oldDodgeChance != DodgeChance
                        ? $"{Name} augmente ses chances d'esquive de {DodgeChance - oldDodgeChance:P} ({oldDodgeChance:P} -> {DodgeChance:P}{(DodgeChance == 0.80m ? " MAX" : string.Empty)})"
                        : $"{Name} a ses chances d'esquive au max : {DodgeChance:P}{(DodgeChance == 0.80m ? " MAX" : string.Empty)}";

                    var oldSpellResistanceChance = SpellResistanceChance;
                    SpellResistanceChance = Math.Min(0.5m, SpellResistanceChance + 0.2m);
                    output += oldSpellResistanceChance != SpellResistanceChance
                        ? $"\n{Name} augmente ses chances  de resister aux sorts de {SpellResistanceChance - oldSpellResistanceChance:P} ({oldSpellResistanceChance:P} -> {SpellResistanceChance:P}{(SpellResistanceChance == 0.5m ? " MAX" : string.Empty)})"
                        : $"\n{Name} a ses chances de resister aux sorts au maximum : {SpellResistanceChance:P}{(SpellResistanceChance == 0.5m ? " MAX" : string.Empty)}";

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

        if (from.StatusInfo.Dodged) (from.Additional ??= []).Add(Special);

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
                "Lorsque le voleur esquive une attaque ennemie, il déclenche une attaque qui inflige 15 points de dégâts physiques à l’attaquant.",
            target: attackFrom.Owner,
            targetType: TargetType.Enemy,
            reloadTime: 0,
            manaCost: 0,
            damage: 15,
            attackType: DamageType.Physical
        );
        conterAttack.Execute();
        conterAttack.Damage = _ => 0;
    };
}