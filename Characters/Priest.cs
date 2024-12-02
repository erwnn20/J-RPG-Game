﻿using JRPG_Game.Characters.Skills;
using JRPG_Game.Enums;
using JRPG_Game.Interfaces;

namespace JRPG_Game.Characters;

public class Priest : Character, IMana
{
    public int MaxMana => 100;
    public int CurrentMana { get; set; }

    public Priest(string name, Team.Team team)
        : base(
            name: name,
            team: team,
            maxHealth: 70,
            speed: 70,
            armor: ArmorType.Textile,
            physicalAttack: 0,
            magicalAttack: 65,
            dodgeChance: 0.10m,
            paradeChance: 0.00m,
            spellResistanceChance: 0.20m,
            skills: [])
    {
        CurrentMana = MaxMana;
        Skills.AddRange([
            new Attack<Character>(
                name: "Châtiment",
                description: () => $"Inflige 75% de la puissance d’attaque magique ({MagicalAttack}) à la cible.\n" +
                                   $"Inflige 150% à la cible si celle ci n'est ni un {nameof(Priest)} ni un {nameof(Paladin)}.",
                owner: this,
                targetType: TargetType.Enemy,
                reloadTime: 1,
                manaCost: 15,
                damage: target =>
                    (int)(MagicalAttack * (target.GetType() == typeof(Priest) || target.GetType() == typeof(Paladin)
                        ? 1.50m
                        : 0.75m)),
                attackType: DamageType.Magical),
            new SpecialAbility<Team.Team>(
                name: "Cercle de soins",
                description: () => $"Soigne toute l'équipe sélectionné de {(int)(MagicalAttack * 0.75m)} PV.",
                owner: this,
                targetType: TargetType.TeamAllied,
                reloadTime: 2,
                manaCost: 30,
                effect: target => $"{target.Name} a été soigné de {target.Heal((int)(MagicalAttack * 0.75m))}"),
            ((IMana)this).Drink(this)
        ]);
    }

    public override int Defend<TTarget>(Attack<TTarget> from, Character damageParameter)
    {
        from.StatusInfo.Set(from, (false, false, false));
        from.StatusInfo.SetDamage(from, damageParameter);

        return TakeDamage((int)from.StatusInfo.Damage);
    }

    //

    public override string ToString()
    {
        return base.ToString() + "\n" +
               $" - Mana: {CurrentMana}/{MaxMana}";
    }
}