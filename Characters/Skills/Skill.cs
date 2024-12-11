using JRPG_Game.Enums;
using JRPG_Game.Interfaces;
using JRPG_Game.Utils;

namespace JRPG_Game.Characters.Skills;

/// <summary>
/// Class representing the skills used by the characters
/// </summary>
public abstract class Skill
{
    public string Name { get; }
    public Character Owner { get; }
    public TargetType TargetType { get; }
    public ITarget? Target { get; protected set; }
    public Func<string> Description { get; }
    public NumericContainer<int> Reload { get; }
    private int ManaCost { get; }

    /// <summary>
    /// List containing all instantiated skills.
    /// </summary>
    public static readonly List<Skill> List = [];

    protected Skill(
        string name,
        Character owner,
        ITarget? target,
        TargetType targetType,
        Func<string> description,
        int reloadTime,
        int manaCost,
        bool addToGlobalList)
    {
        Name = name;
        Owner = owner;
        Target = target;
        TargetType = targetType;
        Description = description;
        Reload = new NumericContainer<int>(0, 0, reloadTime);
        ManaCost = manaCost;

        if (addToGlobalList) List.Add(this);
    }

    /// <summary>
    /// Method used to apply a skill
    /// </summary>
    /// <param name="target">The target of the skill. If null, uses the skill's current target.</param>
    ///  <returns>
    /// A tuple containing:
    /// <list type="bullet">
    ///     <item>
    ///         <term>Next</term>
    ///         <description><c>true</c> if the action is considered finished and can move on to the next action.</description>
    ///     </item>
    ///     <item>
    ///         <term>Execute</term>
    ///         <description><c>true</c> if the skill can be executed later. (see <see cref="Execute"/>)</description>
    ///     </item>
    /// </list>
    /// </returns>
    public (bool Next, bool Execute) Use(ITarget? target = null)
    {
        if (!IsUsable())
        {
            Console.WriteLine(
                $"{Name} est en recharge pour {Reload.Current} tour{(Reload.Current > 1 ? "s" : string.Empty)}.");
            return (false, false);
        }

        if (target != null) Target = target;
        if (!IsTargetCorrect())
        {
            Console.WriteLine(Target != null
                ? $"La cible sélectionnée ({Target.Name} - {Target.GetType().Name}) ne correspond pas au type de cible de la compétence ({TargetType})."
                : $"Pas de cible sélectionné pour {Name} fait par {Owner.Name}.");
            return (false, false);
        }

        if (ManaCost > 0)
        {
            if (Owner is IMana owner)
            {
                if (owner.Mana.Current < ManaCost)
                {
                    Console.WriteLine(
                        $"{Owner.Name} n'a pas assez de mana pour utiliser {Name}. Besoin : {ManaCost}, Actuel : {owner.Mana.Current}/{owner.Mana.Max}.");
                    return (true, false);
                }

                owner.LoseMana(ManaCost);
            }
            else
            {
                Console.WriteLine(
                    $"{Owner.Name} ne peux pas utiliser {Name} car {Name} qui a besoin de mana pour être utilisé.");
                return (true, false);
            }
        }

        Reload.Add(Reload.Max);
        return (true, true);
    }

    /// <summary>
    /// Executes the skill. Implemented in derived classes.
    /// </summary>
    public abstract void Execute();

    /// <summary>
    /// Checks if the skill is not in cooldown.
    /// </summary>
    /// <returns><c>true</c> if the skill is in a cooldown, otherwise <c>false</c>.</returns>
    public bool IsUsable() => Reload.Current <= 0;

    /// <summary>
    /// Reduces the skill's reload time by 1 turn.
    /// </summary>
    private void ReduceReload()
    {
        if (Reload.Current > 0) Reload.Subtract(1);
    }

    /// <summary>
    /// Checks whether the target corresponds to the type required by the skill.
    /// </summary>
    /// <param name="target">Target to check. If null, uses the skill's current target.</param>
    /// <returns><c>true</c> if the target is correct, otherwise <c>false</c>.</returns>
    protected bool IsTargetCorrect(ITarget? target = null)
    {
        var checkedTarget = target ?? Target;
        if (checkedTarget is null) return false;

        return TargetType switch
        {
            TargetType.Self => checkedTarget == Owner,
            TargetType.Teammate => checkedTarget is Character t && t != Owner && t.Team == Owner.Team &&
                                   t.IsAlive(false),
            TargetType.TeammateDead => checkedTarget is Character t && t != Owner && t.Team == Owner.Team &&
                                       !t.IsAlive(false),
            TargetType.Enemy => checkedTarget is Character t && t.Team != Owner.Team && t.IsAlive(false),
            TargetType.TeamAllied => checkedTarget is Team t && t == Owner.Team,
            TargetType.TeamEnemy => checkedTarget is Team t && t != Owner.Team,
            _ => HandleUnknownTargetType(TargetType)
        };
    }

    /// <summary>
    /// Handles unknown target types. (see <see cref="IsTargetCorrect"/>)
    /// </summary>
    /// <param name="targetType">The unknown type of target.</param>
    /// <returns><c>false</c>, indicating the error.</returns>
    private static bool HandleUnknownTargetType(TargetType targetType)
    {
        Console.WriteLine($"{targetType} n'est pas un type de cible reconnu.");
        return false;
    }

    //

    /// <summary>
    /// Updates the recharge time for all instantiated skills.
    /// </summary>
    public static void UpdateReloadCooldowns() => List.ForEach(skill => skill.ReduceReload());

    //

    /// <summary>
    /// Returns a string that represents the <see cref="Skill"/>.
    /// </summary>
    /// <returns>A string that represents the <c>Skill</c></returns>
    public override string ToString()
    {
        return $"{Name} ({GetType().Name}) : par {Owner.Name} à {(Target != null ? Target.Name : TargetType switch
        {
            TargetType.Self => "soi-même",
            TargetType.Teammate => "un allié",
            TargetType.TeammateDead => "un allié mort",
            TargetType.Enemy => "un ennemi",
            TargetType.TeamAllied => "son équipe",
            TargetType.TeamEnemy => "une équipe ennemi",
            _ => $"une cible ({TargetType} non reconnu)"
        })}\n" +
               $"  -> {Description().Replace("\n", "\n     ")}\n" +
               $"Disponible {(IsUsable() ? "maintenant" : Reload.Current > 1 ? $"dans {Reload.Current} tours" : "au prochain tour")} - Temps de recharge : {Reload.Max} tour(s)." +
               (ManaCost > 0 ? $"\nCoût en mana : {ManaCost}" : string.Empty);
    }
}