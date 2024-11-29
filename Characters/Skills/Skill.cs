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
    private Character Owner { get; set; } = owner;
    private TargetType TargetType { get; set; } = targetType;
    private ITarget? Target { get; set; } = target;
    private string Description { get; set; } = description;
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

    public virtual bool Use(ITarget target)
    {
        if (!IsUsable())
        {
            Console.WriteLine($"{Name} est en recharge pour {ReloadCooldown} tour(s).");
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
        Execute(target);
        // Console.WriteLine($"{utilisateur.Name} utilise {Nom} sur {cible.Name}.");
        return true;
    }

    protected abstract void Execute(ITarget target);

    public bool IsUsable() => ReloadCooldown <= 0;

    /// <summary>
    /// Reduces the skill's reload time by 1 turn.
    /// </summary>
    public void ReduceReload()
    {
        if (ReloadCooldown > 0) ReloadCooldown--;
    }

    public bool IsTargetCorrect()
    {
        if (Target is null) return false;

        return TargetType switch
        {
            TargetType.Self => Target == Owner,
            TargetType.Other => Target.GetType() == typeof(Character),
            TargetType.Team => Target.GetType() == typeof(Team.Team),
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    //

    public override string ToString()
    {
        return $"{Name} ({GetType().Name}) : par {Owner.Name} à {(Target != null ? Target.Name : TargetType switch
        {
            TargetType.Self => "soi-même",
            TargetType.Other => "un autre personnage",
            TargetType.Team => "une équipe",
            _ => throw new ArgumentOutOfRangeException()
        })}\n" +
               $"  -> {Description.Replace("\n", "\n  ")}\n" +
               $"Disponible {(IsUsable() ? "maintenant" : ReloadCooldown > 1 ? "au prochain tour" : $"dans {ReloadCooldown} tours")} - Temps de recharge : {ReloadTime} tour(s)." +
               (ManaCost > 0 ? $"\nCoût en mana : {ManaCost}" : string.Empty);
    }
}