using JRPG_Game.Enums;
using JRPG_Game.Interfaces;

namespace JRPG_Game.Characters.Skills;

public class SpecialAbility<TTarget>(
    string name,
    Character owner,
    TTarget? target,
    TargetType targetType,
    string description,
    int reloadTime,
    int manaCost,
    Func<Character, string> effect) :
    Skill(
        name: name,
        owner: owner,
        target: target,
        targetType: targetType,
        description: description,
        reloadTime: reloadTime,
        manaCost: manaCost
    ) where TTarget : class, ITarget
{
    private Func<Character, string> Effect { get; set; } = effect;

    public SpecialAbility(
        string name,
        Character owner,
        TargetType targetType,
        string description,
        int reloadTime,
        int manaCost,
        Func<Character, string> effect) :
        this(
            name: name,
            owner: owner,
            target: null,
            targetType: targetType,
            description: description,
            reloadTime: reloadTime,
            manaCost: manaCost,
            effect: effect
        )
    {
    }

    public override void Execute()
    {
        if (!IsTargetCorrect())
        {
            Console.WriteLine(Target != null
                ? $"La cible sélectionnée ({Target.Name} - {Target.GetType().Name}) ne correspond pas au type de cible de la compétence ({TargetType})."
                : "Pas de cible sélectionné.");

            return;
        }

        switch (Target)
        {
            case Character character:
                Execute(character);
                break;
            case Team.Team team:
                Execute(team);
                break;
            default:
                Console.WriteLine(Target != null
                    ? $"Erreur de type de cible sur {Name}. Attendu: {nameof(Character)} ou {nameof(Team.Team)}, Actuel: {Target.GetType().Name}."
                    : $"Erreur de cible sur {Name}. La cible est null.");
                break;
        }

        Target = null;
    }

    private void Execute(Character _, Func<Character, string>? message = null)
    {
        if (Target is not Character target || this is not SpecialAbility<Character>)
        {
            Console.WriteLine(Target != null
                ? $"Erreur de cible sur {Name}. Attendu: {nameof(Character)}, Actuel: {Target.GetType().Name}.)"
                : $"Erreur de cible sur {Name}. La cible est null.");
            return;
        }

        if (!target.IsAlive(false))
        {
            Console.WriteLine($"La cible de {Name} est morte, la capacité ne peux donc pas être utilisée");
            return;
        }

        message ??= t => $"{Owner.Name} utilise {Name}" +
                         (TargetType != TargetType.Self ? $" sur {t.Name}" : string.Empty);
        Console.WriteLine(message(target));
        Console.WriteLine(Effect(target));
    }

    private void Execute(Team.Team _)
    {
        if (Target is not Team.Team team)
        {
            Console.WriteLine(Target != null
                ? $"Erreur de cible sur {Name}. Attendu: {nameof(Team.Team)}, Actuel: {Target.GetType().Name}.)"
                : $"Erreur de cible sur {Name}. La cible est null.");
            return;
        }

        Console.WriteLine($"{Owner.Name} fait {Name} sur l'équipe {Target.Name}");
        team.Characters
            .Where(c => c.IsAlive(false)).ToList()
            .ForEach(target =>
            {
                new SpecialAbility<Character>(
                    name: Name,
                    description: Description,
                    owner: Owner,
                    target: target,
                    targetType: TargetType.Other,
                    reloadTime: 0,
                    manaCost: 0,
                    effect: Effect
                ).Execute(target, t => $"{Name} atteint {t.Name}");
            });
    }
}