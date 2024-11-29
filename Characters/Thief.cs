using JRPG_Game.Characters.Skills;
using JRPG_Game.Enums;

namespace JRPG_Game.Characters;

public class Thief : Character
{
    public Thief(string name)
        : base(
            name: name,
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
                description: $"Inflige 100% de la puissance d’attaque physique ({PhysicalAttack}) à la cible.\n" +
                             $"Inflige 150% si la cible a moins de la moitié de ses points de vie.",
                owner: this,
                targetType: TargetType.Other,
                reloadTime: 1,
                manaCost: 0,
                damage: target =>
                    (int)(PhysicalAttack *
                          (target.CurrentHealth < target.MaxHealth / 2 ? 1.50m : 1.00m)),
                attackType: DamageType.Physical),
            new SpecialAbility(
                name: "Evasion",
                description:
                $"Augmente les chances d'esquive de 20% ({DodgeChance:P} {(DodgeChance == 0.5m ? "MAX" : $"+ {0.20m:P}")})\n" +
                $"Augmente les chances de resister aux sorts de 20% ({SpellResistanceChance:P} {(SpellResistanceChance == 0.5m ? "MAX" : $"+ {0.20m:P}")})",
                owner: this,
                targetType: TargetType.Self,
                reloadTime: 1,
                manaCost: 0,
                effect: () =>
                {
                    DodgeChance = Math.Min(0.5m, DodgeChance + 0.2m);
                    SpellResistanceChance = Math.Min(0.5m, SpellResistanceChance + 0.2m);
                    return true;
                })
        ]);
    }

    /*protected override void SpecialAbility()
    {
        DodgeChance = Math.Min(0.5m, DodgeChance + 0.2m);
        SpellResistanceChance = Math.Min(0.5m, SpellResistanceChance + 0.2m);
        Console.WriteLine($"{Name} utilise sa capacité spéciale : \"Evasion\"\n" +
                          $" -> Augmente les chances d'esquive ({DodgeChance:P}{(DodgeChance == 0.5m ? " MAX" : string.Empty)})\n" +
                          $" -> Augmente sa resistance aux sorts ({SpellResistanceChance:P}{(SpellResistanceChance == 0.5m ? " MAX" : string.Empty)})");
    }

    protected override void Attack(Character character)
    {
        var attack = new Attack(
            name: "Coup bas",
            attacker: this,
            target: character,
            damage: (int)(PhysicalAttack * (character.CurrentHealth < character.MaxHealth / 2 ? 1.5 : 1)),
            attackType: DamageType.Physical
        );
        attack.Execute();
    }*/
    protected override void SpecialAbility()
    {
        throw new NotImplementedException();
    }

    protected override void Attack(Character character)
    {
        throw new NotImplementedException();
    }
}