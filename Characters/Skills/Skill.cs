using JRPG_Game.Enums;
using JRPG_Game.Interfaces;

namespace JRPG_Game.Characters.Skills;

public abstract class Skill
{
    public string Name { get; set; }
    public Character Owner { get; set; }
    public TargetType TargetType { get; set; }
    public ITarget? Target { get; protected set; }
    public Func<string> Description { get; set; }
    private int ReloadTime { get; set; }
    public int ReloadCooldown { get; private set; }
    private int ManaCost { get; set; }

    private static readonly List<Skill> List = [];

    protected Skill(
        string name,
        Character owner,
        ITarget? target,
        TargetType targetType,
        Func<string> description,
        int reloadTime,
        int manaCost)
    {
        Name = name;
        Owner = owner;
        Target = target;
        TargetType = targetType;
        Description = description;
        ReloadTime = reloadTime;
        ManaCost = manaCost;

        List.Add(this);
    }

    protected Skill(
        string name,
        Character owner,
        TargetType targetType,
        Func<string> description,
        int reloadTime,
        int manaCost) : this(name, owner, null, targetType, description, reloadTime, manaCost)
    {
    }

    public (bool Next, bool Execute) Use(ITarget? target = null)
    {
        if (!IsUsable())
        {
            Console.WriteLine($"{Name} est en recharge pour {ReloadCooldown} tour(s).");
            return (false, false);
        }

        if (target != null) Target = target;
        if (!IsTargetCorrect())
        {
            Console.WriteLine(Target != null
                ? $"La cible sélectionnée ({Target.Name} - {Target.GetType().Name}) ne correspond pas au type de cible de la compétence ({TargetType})."
                : $"Pas de cible sélectionné pour {Name}.");

            return (false, false);
        }

        if (ManaCost > 0)
        {
            if (Owner is IMana owner)
            {
                if (owner.CurrentMana < ManaCost)
                {
                    Console.WriteLine(
                        $"{Owner.Name} n'a pas assez de mana pour utiliser {Name}. Besoin : {ManaCost}, Actuel : {owner.CurrentMana}");
                    return (true, false);
                }

                owner.LoseMana(ManaCost);
            }
            else
            {
                Console.WriteLine(
                    $"{Owner.Name} ne peux pas utiliser {Name} qui a besoin de mana");
                return (true, false);
            }
        }

        ReloadCooldown = ReloadTime;
        return (true, true);
    }

    public abstract void Execute();

    public bool IsUsable() => ReloadCooldown <= 0;

    /// <summary>
    /// Reduces the skill's reload time by 1 turn.
    /// </summary>
    private void ReduceReload()
    {
        if (ReloadCooldown > 0) ReloadCooldown--;
    }

    protected bool IsTargetCorrect(ITarget? target = null)
    {
        var checkedTarget = target ?? Target;
        if (checkedTarget is null) return false;

        return TargetType switch
        {
            TargetType.Self => checkedTarget == Owner,
            TargetType.Teammate => checkedTarget is Character t && t != Owner && t.Team == Owner.Team,
            TargetType.Enemy => checkedTarget is Character t && t.Team != Owner.Team,
            TargetType.TeamAllied => checkedTarget is Team.Team t && t == Owner.Team,
            TargetType.TeamEnemy => checkedTarget is Team.Team t && t != Owner.Team,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    //

    public static void UpdateReloadCooldowns() => List.ForEach(skill => skill.ReduceReload());

    //

    public override string ToString()
    {
        return $"{Name} ({GetType().Name}) : par {Owner.Name} à {(Target != null ? Target.Name : TargetType switch
        {
            TargetType.Self => "soi-même",
            TargetType.Teammate => "un allié",
            TargetType.Enemy => "un ennemi",
            TargetType.TeamAllied => "son équipe",
            TargetType.TeamEnemy => "une équipe ennemi",
            _ => throw new ArgumentOutOfRangeException()
        })}\n" +
               $"  -> {Description().Replace("\n", "\n     ")}\n" +
               $"Disponible {(IsUsable() ? "maintenant" : ReloadCooldown > 1 ? "au prochain tour" : $"dans {ReloadCooldown} tours")} - Temps de recharge : {ReloadTime} tour(s)." +
               (ManaCost > 0 ? $"\nCoût en mana : {ManaCost}" : string.Empty);
    }
}