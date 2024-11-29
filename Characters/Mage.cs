﻿using JRPG_Game.Characters.Skills;
using JRPG_Game.Enums;
using JRPG_Game.Interfaces;

namespace JRPG_Game.Characters;

public class Mage : Character, IMana
{
    public int MaxMana { get; set; } = 100;
    public int CurrentMana { get; set; }

    public Mage(string name)
        : base(
            name: name,
            maxHealth: 60,
            speed: 75,
            armor: ArmorType.Textile,
            physicalAttack: 0,
            magicalAttack: 75,
            dodgeChance: 0.05m,
            paradeChance: 0.05m,
            spellResistanceChance: 0.25m,
            skills: [])
    {
        CurrentMana = MaxMana;
        Skills.AddRange([
            new Attack<(bool, Character), object?, object?>(
                name: "Eclair de givre",
                description: $"Inflige 100% de la puissance d’attaque magique ({MagicalAttack}) à la cible.\n" +
                             $"Réduit la vitesse de la cible de 25% si celui ci n'a pas résisté à l'attaque.",
                owner: this,
                target: TargetType.Other,
                reloadTime: 1,
                manaCost: 15,
                damage: MagicalAttack,
                attackType: DamageType.Magical,
                additional: value =>
                {
                    if (value.Item1) return false; // if target resist (SpellResistance)

                    value.Item2.Speed = (int)(value.Item2.Speed * 0.75m);
                    Console.WriteLine($"La vitesse de {value.Item2.Name} à diminué de 25%");
                    return false;
                }),
            new SpecialAbility<object?, object?>(
                name: "Barrière de givre",
                description: $"Réduit les dégâts des deux prochaines attaques subies.\n" +
                             $"\t- {ReduceDamagePhysical:P} sur les attaques physiques.\n" +
                             $"\t- {ReduceDamageMagical:P} sur les attaques magiques.",
                owner: this,
                target: TargetType.Self,
                reloadTime: 2,
                manaCost: 25,
                effect: _ => ReducedAttack += 2),
            new Attack<(bool, Character), object?, object?>(
                name: "Blizzard",
                description:
                $"Inflige 50% de la puissance d’attaque magique ({(int)(MagicalAttack * 0.50m)}) à toute l’équipe ciblé.\n" +
                $"A une chance de baisser la vitesse de chaque cible de 15%.",
                owner: this,
                target: TargetType.Team,
                reloadTime: 2,
                manaCost: 25,
                damage: (int)(MagicalAttack * 0.5m),
                attackType: DamageType.Magical,
                additional: value =>
                {
                    if (value.Item1) return false; // if target resist (SpellResistance)

                    value.Item2.Speed = (int)(value.Item2.Speed * 0.75m);
                    Console.WriteLine($"La vitesse de {value.Item2.Name} à diminué de 25%");
                    return false;
                }),
            new SpecialAbility<Character, int>(
                name: "Brulure de mana",
                description: "Réduit de moitié la quantité de points de mana de la cible.",
                owner: this,
                target: TargetType.Other,
                reloadTime: 3,
                manaCost: 20,
                effect: target =>
                {
                    if (target is not IMana t) return 0;

                    var manaTaken = Math.Max(40, t.CurrentMana / 2);
                    return t.LoseMana(manaTaken);
                }),
            ((IMana)this).Drink(this)
        ]);
    }

    private decimal ReduceDamagePhysical { get; set; } = 0.60m;
    private decimal ReduceDamageMagical { get; set; } = 0.50m;
    private int ReducedAttack { get; set; }

    /*protected override void SpecialAbility()
    {
        ReducedAttack = 2;
        Console.WriteLine($"{Name} utilise sa capacité spéciale : \"Barrière de givre\"\n" +
                          $" -> Les deux prochaines attaques subies sont réduites");
    }

    protected override void Attack(Character character)
    {
        var attack = new Attack(
            name: "Eclair de givre",
            attacker: this,
            target: character,
            damage: MagicalAttack,
            attackType: DamageType.Magical
        );
        attack.Execute();
    }

    public override int Defend(Attack attack)
    {
        if (ReducedAttack <= 0) return base.Defend(attack);
        attack.Damage = (int)(attack.Damage * (1 - attack.AttackType switch
        {
            DamageType.Physical => ReduceDamagePhysical,
            DamageType.Magical => ReduceDamageMagical,
            _ => 0
        }));
        ReducedAttack--;

        return base.Defend(attack);
    }

    public override string ToString()
    {
        return base.ToString() + "\n" +
               (ReducedAttack > 0 ? $"Dégâts réduits pendant {ReducedAttack} attaque subie" : "");
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