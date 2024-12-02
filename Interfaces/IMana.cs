using JRPG_Game.Characters;
using JRPG_Game.Characters.Skills;
using JRPG_Game.Enums;

namespace JRPG_Game.Interfaces;

public interface IMana
{
    int MaxMana { get; }
    int CurrentMana { get; set; }

    public SpecialAbility<ITarget> Drink(Character character) => new(
        name: "Boire une potion",
        description: () => $"Régénère le mana de 50% (actuellement : {CurrentMana}/{MaxMana})",
        owner: character,
        targetType: TargetType.Self,
        reloadTime: 1,
        manaCost: 0,
        effect: target =>
        {
            var output = "";
            var oldMana = CurrentMana;
            CurrentMana += Math.Min(MaxMana - CurrentMana, MaxMana / 2);
            output += oldMana != CurrentMana
                ? $"{target.Name} régénère son mana de {CurrentMana - oldMana} ({CurrentMana}/{MaxMana})"
                : $"{target.Name} a déja son mana au maximum : {CurrentMana}/{MaxMana}";

            return output;
        });

    public int LoseMana(int manaLost)
    {
        var manaUsed = Math.Min(CurrentMana, manaLost);
        CurrentMana -= manaUsed;
        return manaUsed;
    }
}