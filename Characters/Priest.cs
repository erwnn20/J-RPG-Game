using JRPG_Game.Characters.Skills;
using JRPG_Game.Enums;
using JRPG_Game.Interfaces;

namespace JRPG_Game.Characters;

public class Priest : Character, IMana
{
    public int MaxMana { get; set; } = 100;
    public int CurrentMana { get; set; }

    public Priest(string name)
        : base(
            name: name,
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
            new Attack<object?, object?, Character>(
                name: "Châtiment",
                description: $"Inflige 75% de la puissance d’attaque magique ({MagicalAttack}) à la cible.\n" +
                             $"Inflige 150% à la cible si celle ci n'est ni un {nameof(Priest)} ni un {nameof(Paladin)}.",
                owner: this,
                target: TargetType.Other,
                reloadTime: 1,
                manaCost: 15,
                damage: target =>
                    (int)(MagicalAttack * (target.GetType() == typeof(Priest) || target.GetType() == typeof(Paladin)
                        ? 1.50m
                        : 0.75m)),
                attackType: DamageType.Magical),
            new SpecialAbility<Team.Team, object?>(
                name: "Cercle de soins",
                description: $"Soigne toute l'équipe sélectionné de {(int)(MagicalAttack * 0.75m)} PV.",
                owner: this,
                target: TargetType.Team,
                reloadTime: 2,
                manaCost: 30,
                effect: team =>
                {
                    team.Characters
                        .Where(c => c.IsAlive(false)).ToList()
                        .ForEach(c => c.Heal((int)(MagicalAttack * 0.75m)));
                    return false;
                }),
            ((IMana)this).Drink(this)
        ]);
    }

    protected override void SpecialAbility()
    {
        throw new NotImplementedException();
    }

    protected override void Attack(Character character)
    {
        throw new NotImplementedException();
    }
}