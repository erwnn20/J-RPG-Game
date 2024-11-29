using JRPG_Game.Characters.Skills;
using JRPG_Game.Enums;
using JRPG_Game.Interfaces;

namespace JRPG_Game.Characters;

public class Paladin : Character, IMana
{
    public int MaxMana { get; set; } = 60;
    public int CurrentMana { get; set; }

    public Paladin(string name)
        : base(
            name: name,
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
            new Attack<ITarget>(
                name: "Frappe du croisé",
                description: $"Inflige 100% de la puissance d’attaque physique ({PhysicalAttack}) à la cible.",
                owner: this,
                targetType: TargetType.Other,
                reloadTime: 1,
                manaCost: 5,
                damage: PhysicalAttack,
                attackType: DamageType.Physical),
            new Attack<ITarget>(
                name: "Jugement",
                description: $"Inflige 100% de la puissance d’attaque magique ({MagicalAttack}) à la cible.",
                owner: this,
                targetType: TargetType.Other,
                reloadTime: 1,
                manaCost: 10,
                damage: MagicalAttack,
                attackType: DamageType.Magical),
            new SpecialAbility(
                name: "Eclair lumineux",
                description:
                $"Soigne la cible d’un montant de 125% de la puissance d’attaque magique ({(int)(MagicalAttack * 1.25m)} PV).",
                owner: this,
                targetType: TargetType.Other,
                reloadTime: 1,
                manaCost: 25,
                effect: (Character target) => target.Heal((int)(MagicalAttack * 1.25m))),
            ((IMana)this).Drink(this)
        ]);
    }

    //

    public override string ToString()
    {
        return base.ToString() + "\n" +
               $" - Mana: {CurrentMana}/{MaxMana}";
    }

    /*protected override void SpecialAbility()
    {
        var healthPoint = (int)(MagicalAttack * 1.25);
        Console.WriteLine($"{Name} utilise sa capacité spéciale : \"Eclair lumineux\"\n" +
                          $" -> {Name} se soigne de {Heal(healthPoint, false)}");
    }

    private Attack SelectAttack(Character character)
    {
        return Prompt.Select("Choisissez une attaque a effectuer :", s => s,
                "Frappe du croisé (attaque physique)", "Jugement (attaque magique)")
            switch
            {
                1 => new Attack(name: "Frappe du croisé", attacker: this, target: character, damage: PhysicalAttack,
                    attackType: DamageType.Physical),
                2 => new Attack(name: "Jugement", attacker: this, target: character, damage: MagicalAttack,
                    attackType: DamageType.Magical),
                _ => throw new InvalidOperationException(
                    "L'attaque sélectionnée n'existe pas, la valeur doit être 1 ou 2.")
            };
    }

    protected override void Attack(Character character)
    {
        var attack = SelectAttack(character);
        Console.WriteLine();

        Heal((int)(attack.Execute() * 0.5));
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