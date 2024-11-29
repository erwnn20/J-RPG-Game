using JRPG_Game.Characters;
using JRPG_Game.Characters.Skills;
using JRPG_Game.Enums;

namespace JRPG_Game.Interfaces;

public interface IMana
{
    int MaxMana { get; set; }
    int CurrentMana { get; set; }

    public SpecialAbility Drink(Character character) => new(
        name: "Boire une potion",
        description: $"Régénère le mana de 50% (actuellement : {CurrentMana}/{MaxMana})",
        owner: character,
        target: TargetType.Self,
        reloadTime: 1,
        manaCost: 0,
        effect: () => CurrentMana += Math.Min(MaxMana - CurrentMana, MaxMana / 2));

    public int LoseMana(int manaLost) => CurrentMana -= Math.Min(MaxMana - CurrentMana, manaLost);
}