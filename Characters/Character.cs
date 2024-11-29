using JRPG_Game.Characters.Skills;
using JRPG_Game.Enums;
using JRPG_Game.Interfaces;
using JRPG_Game.Utils;

namespace JRPG_Game.Characters;

public abstract class Character : ITarget
{
    protected Character(
        string name,
        int maxHealth,
        int speed,
        int physicalAttack,
        int magicalAttack,
        ArmorType armor,
        decimal dodgeChance,
        decimal paradeChance,
        decimal spellResistanceChance,
        List<Skill> skills)
    {
        Name = name;
        MaxHealth = maxHealth;
        CurrentHealth = maxHealth;
        Speed = speed;
        PhysicalAttack = physicalAttack;
        MagicalAttack = magicalAttack;
        Armor = armor;
        DodgeChance = dodgeChance;
        ParadeChance = paradeChance;
        SpellResistanceChance = spellResistanceChance;
        Skills = skills;

        List.Add(this);
    }

    public string Name { get; private set; }
    public int MaxHealth { get; private set; }
    public int CurrentHealth { get; private set; }
    public int Speed { get; set; }
    public int PhysicalAttack { get; set; }
    protected int MagicalAttack { get; set; }
    private ArmorType Armor { get; set; }
    protected decimal DodgeChance { get; set; }
    private decimal ParadeChance { get; set; }
    protected decimal SpellResistanceChance { get; set; }
    protected List<Skill> Skills { get; set; }

    public static readonly List<Character> List = [];

    public bool IsAlive(bool message)
    {
        if (CurrentHealth <= 0 && message)
            Console.WriteLine($"{Name} est mort.");
        return CurrentHealth > 0;
    }

    public int Heal(int healthPoint, bool message = true)
    {
        var healed = Math.Min(MaxHealth - CurrentHealth, healthPoint);
        CurrentHealth += healed;

        if (healed <= 0 || !message) return healed;
        Console.WriteLine($"{Name} à été soigné de {healed}");

        return healed;
    }

    public Skill? SelectSkill()
    {
        if (!Skills.Any(skill => skill.IsUsable()))
        {
            Console.WriteLine("Aucune compétence utilisable actuellement.");
            return null;
        }

        do
        {
            var selectedSkill = Skills.ElementAt(Prompt.Select("Quel compétence voulez vous utiliser ?",
                skill =>
                    $"{skill.Name} ({skill.GetType().Name}){(!skill.IsUsable() ? $" - Disponible dans : {skill.ReloadCooldown} tour(s)" : string.Empty)}",
                Skills) - 1);
            Console.WriteLine();

            List<string> choices = ["Afficher details", "Sélectionné une autre capacité"];
            if (selectedSkill.IsUsable())
                choices.Add("Confirmer");
            else
                Console.WriteLine(
                    $"Vous ne pouvez pas utiliser {selectedSkill.Name} actuellement. Disponible dans : {selectedSkill.ReloadCooldown} tour(s)");

            var exit = false;
            do
            {
                var selected = Prompt.Select($"{selectedSkill.Name} ({selectedSkill.GetType().Name})", s => s, choices);
                Console.WriteLine();

                switch (selected)
                {
                    case 1:
                        Console.WriteLine(selectedSkill + "\n");
                        break;
                    case 2:
                        Program.Next();
                        exit = true;
                        break;
                    case 3:
                        return selectedSkill;
                    default:
                        Console.WriteLine("Une erreur s'est produite, veuillez réessayer.");
                        break;
                }
            } while (!exit);
        } while (true);
    }

    private bool Dodge<TTarget>(Attack<TTarget> attack) where TTarget : ITarget
    {
        if (attack.AttackType != DamageType.Physical
            || (decimal)new Random().NextDouble() > DodgeChance) return false;

        Console.WriteLine($"{Name} a réussi à esquiver {attack.Name}");
        return true;
    }

    private bool Parade<TTarget>(Attack<TTarget> attack) where TTarget : ITarget
    {
        if (attack.AttackType != DamageType.Physical
            || (decimal)new Random().NextDouble() > ParadeChance) return false;

        Console.WriteLine($"{Name} a réussi à parer de {attack.Name}");
        return true;
    }

    private bool SpellResistance<TTarget>(Attack<TTarget> attack) where TTarget : ITarget
    {
        if (attack.AttackType != DamageType.Magical
            || (decimal)new Random().NextDouble() > SpellResistanceChance) return false;

        Console.WriteLine($"{Name} a réussi à resister à {attack.Name}");
        return true;
    }

    private decimal ArmorReduction(DamageType damageType)
    {
        return damageType switch
        {
            DamageType.Physical => Armor switch
            {
                ArmorType.Textile => 0.0m,
                ArmorType.Leather => 0.15m,
                ArmorType.Mesh => 0.30m,
                ArmorType.Plates => 0.44m,
                _ => 0
            },
            DamageType.Magical => Armor switch
            {
                ArmorType.Textile => 0.30m,
                ArmorType.Leather => 0.20m,
                ArmorType.Mesh => 0.10m,
                ArmorType.Plates => 0.0m,
                _ => 0
            },
            _ => 0
        };
    }

    public virtual int Defend<TTarget>(Attack<TTarget> from, TTarget damageParameter) where TTarget : ITarget
    {
        if (Dodge(from))
        {
            from.Dodged = true;
            return 0;
        }

        if (SpellResistance(from))
        {
            from.Resisted = true;
            return 0;
        }

        decimal damage = from.Damage(damageParameter);

        if (Parade(from))
        {
            from.Blocked = true;
            damage *= 0.5m;
        }

        damage *= 1 - ArmorReduction(from.AttackType);

        // TakeDamage((int)damage);
        return (int)damage;
    }

    //

    protected abstract void SpecialAbility();
    protected abstract void Attack(Character character);

    private void TakeDamage(int damage) => CurrentHealth = Math.Max(0, CurrentHealth - damage);

    public bool SelectAction()
    {
        var selected = Prompt.Select("Que voulez-vous faire ?", s => s,
            "Afficher informations", "Attaquer un autre joueur", "Utiliser sa capacité spéciale",
            "Effacer le terminal");
        Console.WriteLine();

        switch (selected)
        {
            case 1:
                Console.WriteLine(this + "\n");
                break;
            case 2:
                Character attackedPlayer;
                do
                {
                    attackedPlayer = Select("Quel joueur voulez vous attaquer ?");
                    Console.WriteLine();
                } while (!ActionOn(attackedPlayer));

                Attack(attackedPlayer);
                Console.WriteLine();
                return true;
            case 3:
                SpecialAbility();
                Console.WriteLine();
                return true;
            case 4:
                Program.Next();
                break;
            default:
                Console.WriteLine("Action inconnue, veillez réessayer.");
                break;
        }

        return false;
    }

    private bool ActionOn(Character player)
    {
        if (player == this)
        {
            Console.WriteLine("Vous ne pouvez pas pour attaquer vous même !\n");
            Program.Next(2000);
            return false;
        }

        while (true)
        {
            var selected = Prompt.Select(
                $"Cible => {player.Name} ({player.CurrentHealth}/{player.MaxHealth} PV) - {player.GetType().Name}",
                displayFunc: s => s,
                "Afficher informations", "Confirmé", "Changer de joueur");
            Console.WriteLine();

            switch (selected)
            {
                case 1:
                    Console.WriteLine(player + "\n");
                    break;
                case 2:
                    return true;
                case 3:
                    Program.Next();
                    return false;
                default:
                    Console.WriteLine("Action inconnue, veillez réessayer.");
                    break;
            }
        }
    }

    public static bool CombatIsOn() => List.Count(character => character.IsAlive(false)) >= 2;

    private static Character Select(string message = "Quel joueur voulez vous choisir ?")
    {
        var alivePlayer = List.FindAll(player => player.IsAlive(false));
        return alivePlayer.ElementAt(
            Prompt.Select(message, player => $"{player.Name} - ({player.GetType().Name})", alivePlayer) - 1
        );
    }

    //

    public override string ToString()
    {
        return $"{GetType().Name}: {Name} ({CurrentHealth}/{MaxHealth} PV) - Armure: {Armor}\n" +
               $"Stats :\n" +
               $" - Vitesse: {Speed}\n" +
               $" - Attaque Physique: {PhysicalAttack}\n" +
               $" - Attaque Magique: {MagicalAttack}\n" +
               $" - Chances d'Esquiver: {DodgeChance:P}\n" +
               $" - Chances de Parade: {ParadeChance:P}\n" +
               $" - Chances de Resister aux Sorts: {SpellResistanceChance:P}\n" +
               $" - Compétences :\n" +
               string.Join("\n", Skills.Select(skill => $"\t- {skill.Name} ({skill.GetType().Name})"));
    }
}