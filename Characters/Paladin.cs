using JRPG_Game.Characters.Skills;
using JRPG_Game.Enums;
using JRPG_Game.Interfaces;

namespace JRPG_Game.Characters;

public class Paladin : Character, IMana
{
    public int MaxMana => 60;
    public int CurrentMana { get; set; }

    public Paladin(string name, Team.Team team)
        : base(
            name: name,
            team: team,
            maxHealth: 95,
            speed: 65,
            armor: ArmorType.Mesh,
            physicalAttack: 40,
            magicalAttack: 40,
            dodgeChance: 0.05m,
            paradeChance: 0.10m,
            spellResistanceChance: 0.20m,
            skills: [])
    {
        CurrentMana = MaxMana;
        Skills.AddRange([
            new Attack<Character>(
                name: "Frappe du croisé",
                description: () => $"Inflige 100% de la puissance d’attaque physique ({PhysicalAttack}) à la cible.",
                owner: this,
                targetType: TargetType.Enemy,
                reloadTime: 1,
                manaCost: 5,
                damage: PhysicalAttack,
                attackType: DamageType.Physical),
            new Attack<Character>(
                name: "Jugement",
                description: () => $"Inflige 100% de la puissance d’attaque magique ({MagicalAttack}) à la cible.",
                owner: this,
                targetType: TargetType.Enemy,
                reloadTime: 1,
                manaCost: 10,
                damage: MagicalAttack,
                attackType: DamageType.Magical),
            new SpecialAbility<Character>(
                name: "Eclair lumineux",
                description: () =>
                    $"Soigne la cible d’un montant de 125% de la puissance d’attaque magique ({(int)(MagicalAttack * 1.25m)} PV).",
                owner: this,
                targetType: TargetType.Teammate,
                reloadTime: 1,
                manaCost: 25,
                effect: target => $"{target.Name} est soigné de {target.Heal((int)(MagicalAttack * 1.25m))} PV"),
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