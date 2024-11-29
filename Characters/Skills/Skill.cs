using JRPG_Game.Enums;
using JRPG_Game.Interfaces;

namespace JRPG_Game.Characters.Skills;

public abstract class Skill(
    string name,
    Character owner,
    TargetType target,
    string description,
    int reloadTime,
    int manaCost)
{
    public string Name { get; set; } = name;
    private Character Owner { get; set; } = owner;
    protected TargetType TargetType { get; set; } = target;
    private string Description { get; set; } = description;
    private int ReloadTime { get; set; } = reloadTime;
    private int ReloadCooldown { get; set; }
    private int ManaCost { get; set; } = manaCost;

    public virtual bool Use(ITarget target)
    {
        if (ReloadCooldown > 0)
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

    /// <summary>
    /// Reduces the skill's reload time by 1 turn.
    /// </summary>
    public void ReduceReload()
    {
        if (ReloadCooldown > 0) ReloadCooldown--;
    }
}