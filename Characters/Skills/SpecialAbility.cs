using JRPG_Game.Enums;
using JRPG_Game.Interfaces;

namespace JRPG_Game.Characters.Skills;

/// <summary>
/// Represents a skill in the form of a special ability, capable of targeting a character or a team.
/// </summary>
/// <typeparam name="TTarget">The type of the target, must implement the <see cref="ITarget"/> interface.</typeparam>
public class SpecialAbility<TTarget>(
    string name,
    Character owner,
    TTarget? target,
    TargetType targetType,
    Func<string> description,
    int reloadTime,
    int manaCost,
    Func<Character, string> effect,
    bool addToGlobalList = true) :
    Skill(
        name: name,
        owner: owner,
        target: target,
        targetType: targetType,
        description: description,
        reloadTime: reloadTime,
        manaCost: manaCost,
        addToGlobalList: addToGlobalList
    ) where TTarget : class, ITarget
{
    private Func<Character, string> Effect { get; } = effect;

    public SpecialAbility(
        string name,
        Character owner,
        TargetType targetType,
        Func<string> description,
        int reloadTime,
        int manaCost,
        Func<Character, string> effect,
        bool addToGlobalList = true) :
        this(
            name: name,
            owner: owner,
            target: null,
            targetType: targetType,
            description: description,
            reloadTime: reloadTime,
            manaCost: manaCost,
            effect: effect,
            addToGlobalList: addToGlobalList
        )
    {
    }

    /// <summary>
    /// Executes the special ability and applies its effects to the target based on the target type (<see cref="Character"/> or <see cref="Team"/>).
    /// </summary>
    public override void Execute()
    {
        if (!Owner.IsAlive(false)) return;
        if (Owner.Effects.ContainsKey(StatusEffect.Stun) && new Random().NextDouble() < 0.15)
        {
            Console.WriteLine($"{Owner.Name} est étourdi est rate sa capacité {Name}.");
            return;
        }

        if (!IsTargetCorrect())
        {
            Console.WriteLine(Target != null
                ? $"La cible sélectionnée ({Target.Name} - {Target.GetType().Name}) ne correspond pas au type de cible de la compétence ({TargetType})."
                : $"Pas de cible sélectionné pour {Name} fait par {Owner.Name}.");
            return;
        }

        switch (Target)
        {
            case Character character:
                Execute(character);
                break;
            case Team team:
                Execute(team);
                break;
            default:
                Console.WriteLine(Target != null
                    ? $"Erreur : le type de la cible de {Name} fait par {Owner.Name}. Attendu: {nameof(Character)} ou {nameof(Team)}, Actuel: {Target.GetType().Name}."
                    : $"Erreur : la cible de {Name} fait par {Owner.Name} est null.");
                break;
        }

        Target = null;
    }

    /// <summary>
    /// Executes the special ability on a specific character target.
    /// </summary>
    /// <param name="character">
    /// Placeholder for the character target.
    /// Will not be used in the method, uses the <see cref="Skill.Target"/> attribute after checking that it is of type <see cref="Character"/>.
    /// </param>
    /// <param name="message">Optional custom message to display during the attack.</param>
    private void Execute(Character character, Func<Character, string>? message = null)
    {
        if (Target is not Character target)
        {
            Console.WriteLine(Target != null
                ? $"Erreur : le type de la cible de {Name} fait par {Owner.Name}. Attendu: {nameof(Character)}, Actuel: {Target.GetType().Name}."
                : $"Erreur : la cible de {Name} fait par {Owner.Name} est null.");
            return;
        }

        message ??= t =>
            $"{Owner.Name} fait {Name}{(TargetType != TargetType.Self ? $" sur {t.Name}" : string.Empty)}.";

        Console.WriteLine(message(target));
        Console.WriteLine(Effect(target));
    }

    /// <summary>
    /// Executes the special ability on a team, applying the effects to all characters in the team.
    /// </summary>
    /// <param name="team">
    /// Placeholder for the team target.
    /// Will not be used in the method, uses the <see cref="Skill.Target"/> attribute after checking that it is of type <see cref="Team"/>
    /// </param>
    private void Execute(Team team)
    {
        if (Target is not Team target)
        {
            Console.WriteLine(Target != null
                ? $"Erreur : le type de la cible de {Name} fait par {Owner.Name}. Attendu: {nameof(Team)}, Actuel: {Target.GetType().Name}."
                : $"Erreur : la cible de {Name} fait par {Owner.Name} est null.");
            return;
        }

        Console.WriteLine($"{Owner.Name} fait {Name} sur l'équipe {Target.Name}.");
        target.Characters
            .Where(c => c.IsAlive(false)).ToList()
            .ForEach(trgt =>
            {
                new SpecialAbility<Character>(
                    name: Name,
                    description: Description,
                    owner: Owner,
                    target: trgt,
                    targetType: TargetType.Enemy,
                    reloadTime: 0,
                    manaCost: 0,
                    effect: Effect,
                    addToGlobalList: false
                ).Execute(trgt, t => $"{Name} atteint {t.Name}.");
            });
    }
}