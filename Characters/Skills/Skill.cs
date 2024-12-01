using JRPG_Game.Enums;
using JRPG_Game.Interfaces;

namespace JRPG_Game.Characters.Skills;

public abstract class Skill(
    string name,
    Character owner,
    ITarget? target,
    TargetType targetType,
    string description,
    int reloadTime,
    int manaCost)
{
    public string Name { get; set; } = name;
    public Character Owner { get; set; } = owner;
    protected TargetType TargetType { get; set; } = targetType;
    public ITarget? Target { get; protected set; } = target;
    public string Description { get; set; } = description;
    private int ReloadTime { get; set; } = reloadTime;
    public int ReloadCooldown { get; private set; }
    private int ManaCost { get; set; } = manaCost;

    protected Skill(
        string name,
        Character owner,
        TargetType targetType,
        string description,
        int reloadTime,
        int manaCost) : this(name, owner, null, targetType, description, reloadTime, manaCost)
    {
    }

    public virtual bool Use(ITarget? target = null)
    {
        if (!IsUsable())
        {
            Console.WriteLine($"{Name} est en recharge pour {ReloadCooldown} tour(s).");
            return false;
        }

        if (target != null)
            Target = target;
        if (!IsTargetCorrect())
        {
            Console.WriteLine(Target != null
                ? $"La cible sélectionnée ({Target.Name} - {Target.GetType().Name}) ne correspond pas au type de cible de la compétence ({TargetType})."
                : "Pas de cible sélectionné.");

            return false;
        }

        if (ManaCost > 0)
        {
            // avec Character Owner
            if (Owner is IMana owner)
            {
                if (owner.CurrentMana < ManaCost)
                {
                    Console.WriteLine(
                        $"{Owner.Name} n'a pas assez de mana pour utiliser {Name}. Besoin : {ManaCost}, Actuel : {owner.CurrentMana}");
                    return true;
                }

                owner.LoseMana(ManaCost);
            }
            else
            {
                Console.WriteLine(
                    $"{Owner.Name} ne peux pas utiliser {Name} qui a besoin de mana");
                return true;
            }
        }

        ReloadCooldown = ReloadTime;
        Execute();
        return true;
    }

    public abstract void Execute();

    public bool IsUsable() => ReloadCooldown <= 0;

    /// <summary>
    /// Reduces the skill's reload time by 1 turn.
    /// </summary>
    public void ReduceReload()
    {
        if (ReloadCooldown > 0) ReloadCooldown--;
    }

    private bool IsTargetCorrect()
    {
        if (Target is null) return false;

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
               $"  -> {Description.Replace("\n", "\n  ")}\n" +
               $"Disponible {(IsUsable() ? "maintenant" : ReloadCooldown > 1 ? "au prochain tour" : $"dans {ReloadCooldown} tours")} - Temps de recharge : {ReloadTime} tour(s)." +
               (ManaCost > 0 ? $"\nCoût en mana : {ManaCost}" : string.Empty);
    }
}