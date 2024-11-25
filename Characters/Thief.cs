using JRPG_Game.Action;
using JRPG_Game.Armors;

namespace JRPG_Game.Characters;

public class Thief(string name) :
    Character(
        name: name,
        maxHealth: 80,
        armor: ArmorType.Leather,
        physicalAttack: 55,
        magicalAttack: 0,
        dodgeChance: 0.15m,
        paradeChance: 0.25m,
        spellResistanceChance: 0.25m
    )
{
    public override void SpecialAbility()
    {
        DodgeChance = Math.Min(0.5m, DodgeChance + 0.2m);
        SpellResistanceChance = Math.Min(0.5m, SpellResistanceChance + 0.2m);
        Console.WriteLine($"{Name} utilise sa capacité spéciale : \"Evasion\"\n" +
                          $"-> Augmente les chances d'esquive ({DodgeChance:P}{(DodgeChance == 0.5m ? " MAX" : string.Empty)})\n" +
                          $"-> Augmente sa resistance aux sorts ({SpellResistanceChance:P}{(SpellResistanceChance == 0.5m ? " MAX" : string.Empty)})");
    }

    public override void Attack(Character character)
    {
        var attack = new Attack(
            name: "Coup bas",
            attacker: this,
            target: character,
            damage: (int)(PhysicalAttack * (character.CurrentHealth < character.MaxHealth / 2 ? 1.5 : 1)),
            attackType: DamageType.Physical
        );
        attack.Execute();
    }

    public override int Defend(Attack attack)
    {
        if (Dodge(attack)) return 0;
        if (SpellResistance(attack)) return 0;

        decimal damage = attack.Damage;

        if (Parade(attack)) damage *= 0.5m;
        damage *= 1 - ArmorReduction(attack.AttackType);

        TakeDamage((int)damage);
        return (int)damage;
    }

    public override void Heal()
    {
        throw new NotImplementedException();
    }
}