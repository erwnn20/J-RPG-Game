using JRPG_Game.Characters.Skills;
using JRPG_Game.Enums;

namespace JRPG_Game.Characters;

public class Thief : Character
{
    public Thief(string name, Team.Team team)
        : base(
            name: name,
            team: team,
            maxHealth: 80,
            speed: 100,
            armor: ArmorType.Leather,
            physicalAttack: 55,
            magicalAttack: 0,
            dodgeChance: 0.15m,
            paradeChance: 0.25m,
            spellResistanceChance: 0.25m,
            skills: [])
    {
        Skills.AddRange([
            new Attack<Character>(
                name: "Coup bas",
                description: () => $"Inflige 100% de la puissance d’attaque physique ({PhysicalAttack}) à la cible.\n" +
                                   $"Inflige 150% si la cible a moins de la moitié de ses points de vie.",
                owner: this,
                targetType: TargetType.Enemy,
                reloadTime: 1,
                manaCost: 0,
                damage: target =>
                    (int)(PhysicalAttack *
                          (target.CurrentHealth < target.MaxHealth / 2 ? 1.50m : 1.00m)),
                attackType: DamageType.Physical),
            new SpecialAbility<Character>(
                name: "Evasion",
                description: () =>
                    $"Augmente les chances d'esquive de 20% (max 50%) ({DodgeChance:P} {(DodgeChance == 0.5m
                        ? "MAX"
                        : $"-> {Math.Min(DodgeChance + 0.20m, 0.5m):P}{(Math.Min(DodgeChance + 0.20m, 0.5m) == 0.5m ? " MAX" : string.Empty)}")})\n" +
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
                    DodgeChance = Math.Min(0.5m, DodgeChance + 0.2m);
                    output += oldDodgeChance != DodgeChance
                        ? $"{Name} augmente ses chances d'esquive de {DodgeChance - oldDodgeChance:P} ({oldDodgeChance:P} -> {DodgeChance:P}{(DodgeChance == 0.5m ? " MAX" : string.Empty)})"
                        : $"{Name} a ses chances d'esquive au max : {DodgeChance:P}{(DodgeChance == 0.5m ? " MAX" : string.Empty)}";

                    var oldSpellResistanceChance = SpellResistanceChance;
                    SpellResistanceChance = Math.Min(0.5m, SpellResistanceChance + 0.2m);
                    output += oldSpellResistanceChance != SpellResistanceChance
                        ? $"\n{Name} augmente ses chances  de resister aux sorts de {SpellResistanceChance - oldSpellResistanceChance:P} ({oldSpellResistanceChance:P} -> {SpellResistanceChance:P}{(SpellResistanceChance == 0.5m ? " MAX" : string.Empty)})"
                        : $"\n{Name} a ses chances de resister aux sorts au maximum : {SpellResistanceChance:P}{(SpellResistanceChance == 0.5m ? " MAX" : string.Empty)}";

                    return output;
                })
        ]);
    }

    public override int Defend<TTarget>(Attack<TTarget> from, Character damageParameter)
    {
        from.StatusInfo.Set(from, (false, false, false));
        from.StatusInfo.SetDamage(from, damageParameter);

        if (from.StatusInfo.Dodged) (from.Additional ??= []).Add(Special);

        return TakeDamage((int)from.StatusInfo.Damage);
    }

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