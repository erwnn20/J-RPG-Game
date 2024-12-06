using JRPG_Game.Characters;
using JRPG_Game.Characters.Skills;
using JRPG_Game.Enums;
using JRPG_Game.Utils;

namespace JRPG_Game.Interfaces;

/// <summary>
/// Interface representing entities that have mana and can perform mana-related actions such as regenerating or losing mana.
/// </summary>
public interface IMana
{
    NumericContainer Mana { get; }

    /// <summary>
    /// Creates a special ability to drink a potion and regenerate mana by 50%.
    /// </summary>
    /// <param name="character">The character performing the action of drinking the potion.</param>
    /// <returns>A special ability to drink a potion and regenerate mana.</returns>
    public SpecialAbility<ITarget> Drink(Character character) => new(
        name: "Boire une potion",
        description: () =>
            $"Régénère le mana de 50% ({Mana.Current} -> {Math.Min(Mana.Max, Mana.Current + (int)(Mana.Max * 0.50m))}/{Mana.Max})",
        owner: character,
        targetType: TargetType.Self,
        reloadTime: 1,
        manaCost: 0,
        effect: target =>
        {
            var added = Mana.Add((int)(Mana.Max * 0.50m));
            return added > 0
                ? $"{target.Name} régénère son mana de {added} ({Mana.Current}/{Mana.Max})"
                : $"{target.Name} a déja son mana au maximum : {Mana.Current}/{Mana.Max}";
        });

    /// <summary>
    /// Reduces the current mana by the specified amount, and returns the actual amount of mana lost.
    /// </summary>
    /// <param name="manaLost">The amount of mana to lose.</param>
    /// <returns>The amount of mana actually lost.</returns>
    public int LoseMana(int manaLost) => Mana.Subtract(manaLost);
}