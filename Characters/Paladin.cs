using JRPG_Game.Action;
using JRPG_Game.Armors;
using JRPG_Game.Utils;

namespace JRPG_Game.Characters;

public class Paladin(string name) :
    Character(
        name: name,
        maxHealth: 95,
        armor: ArmorType.Mesh,
        physicalAttack: 40,
        magicalAttack: 40,
        dodgeChance: 0.05m,
        paradeChance: 0.10m,
        spellResistanceChance: 0.20m
    )
{
    public override void SpecialAbility()
    {
        var healthPoint = (int)(MagicalAttack * 1.25);
        Console.WriteLine($"{Name} utilise sa capacité spéciale : \"Eclair lumineux\"\n" +
                          $"-> {Name} se soigne de {Heal(healthPoint, false)}");
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

    public override void Attack(Character character)
    {
        var attack = SelectAttack(character);
        var damageHealed = (int)(attack.Execute() * 0.5);

        Heal(damageHealed);
    }
}