using JRPG_Game.Characters.Skills;
using JRPG_Game.Enums;
using JRPG_Game.Interfaces;
using JRPG_Game.Utils;

namespace JRPG_Game.Characters;

/// <summary>
/// Represents a character with various attributes, skills, and actions.
/// </summary>
public abstract class Character : ITarget
{
    protected Character(
        string name,
        Team.Team team,
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
        Team = team;
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

    public string Name { get; }
    public Team.Team Team { get; }
    public int MaxHealth { get; }
    public int CurrentHealth { get; private set; }
    public int Speed { get; set; }
    public int PhysicalAttack { get; set; }
    protected int MagicalAttack { get; }
    private ArmorType Armor { get; }
    protected decimal DodgeChance { get; set; }
    private decimal ParadeChance { get; }
    protected decimal SpellResistanceChance { get; set; }
    protected List<Skill> Skills { get; }

    /// <summary>
    /// List containing all instantiated characters.
    /// </summary>
    private static readonly List<Character> List = [];

    //

    /// <summary>
    /// Checks if the character is alive.
    /// </summary>
    /// <param name="message">If <c>true</c>, outputs a message if the character is dead.</param>
    /// <returns><c>true</c> if the character's health is above zero, otherwise <c>false</c>.</returns>
    public bool IsAlive(bool message)
    {
        if (CurrentHealth <= 0 && message)
            Console.WriteLine($"{Name} est mort.");
        return CurrentHealth > 0;
    }

    /// <summary>
    /// Applies damage to the character and reduces their health.
    /// </summary>
    /// <param name="damage">The amount of damage to apply.</param>
    /// <returns>The actual damage taken, capped at the character's remaining health.</returns>
    protected int TakeDamage(int damage)
    {
        var damageTaken = Math.Min(CurrentHealth, damage);
        CurrentHealth -= damageTaken;
        return damageTaken;
    }

    /// <summary>
    /// Heals the character by a specified amount of health points.
    /// </summary>
    /// <param name="healthPoint">The amount of health to heal.</param>
    /// <param name="message">If <c>true</c>, outputs a message indicating the amount healed.</param>
    /// <returns>The actual amount of health restored.</returns>
    public int Heal(int healthPoint, bool message = true)
    {
        var healed = Math.Min(MaxHealth - CurrentHealth, healthPoint);
        CurrentHealth += healed;

        if (healed <= 0 || !message) return healed;

        Console.WriteLine($"{Name} à été soigné de {healed}.");
        return healed;
    }

    /// <summary>
    /// Attempts to dodge a physical attack using <see cref="DodgeChance"/> attribute.
    /// </summary>
    /// <typeparam name="TTarget">The target type, implementing <see cref="ITarget"/>.</typeparam>
    /// <param name="attack">The attack to dodge.</param>
    /// <returns><c>true</c> if the dodge is successful; otherwise, <c>false</c>.</returns>
    public bool Dodge<TTarget>(Attack<TTarget> attack) where TTarget : class, ITarget
        => attack.AttackType == DamageType.Physical
           && (decimal)new Random().NextDouble() <= DodgeChance;

    /// <summary>
    /// Attempts to parry a physical attack using <see cref="ParadeChance"/> attribute.
    /// </summary>
    /// <typeparam name="TTarget">The target type, implementing <see cref="ITarget"/>.</typeparam>
    /// <param name="attack">The attack to parry.</param>
    /// <returns><c>true</c> if the parry is successful; otherwise, <c>false</c>.</returns>
    public bool Parade<TTarget>(Attack<TTarget> attack) where TTarget : class, ITarget
        => attack.AttackType == DamageType.Physical
           && (decimal)new Random().NextDouble() <= ParadeChance;

    /// <summary>
    /// Attempts to resist a magical attack using <see cref="SpellResistanceChance"/> attribute.
    /// </summary>
    /// <typeparam name="TTarget">The target type, implementing <see cref="ITarget"/>.</typeparam>
    /// <param name="attack">The attack to resist.</param>
    /// <returns><c>true</c> if the resistance is successful; otherwise, <c>false</c>.</returns>
    public bool SpellResistance<TTarget>(Attack<TTarget> attack) where TTarget : class, ITarget
        => attack.AttackType == DamageType.Magical
           && (decimal)new Random().NextDouble() <= SpellResistanceChance;

    /// <summary>
    /// Calculates damage reduction based on the character's armor type (<see cref="ArmorType"/>)
    /// and damage type (<see cref="DamageType"/>).
    /// </summary>
    /// <param name="damageType">The type of damage being dealt.</param>
    /// <returns>The percentage of damage reduced.</returns>
    public decimal ArmorReduction(DamageType damageType)
        => damageType switch
        {
            DamageType.Physical => Armor switch
            {
                ArmorType.Textile => 0.0m,
                ArmorType.Leather => 0.15m,
                ArmorType.Mesh => 0.30m,
                ArmorType.Plates => 0.45m,
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

    /// <summary>
    /// Abstract method to defend against an attack.
    /// </summary>
    /// <typeparam name="TTarget">The target type, implementing <see cref="ITarget"/>.</typeparam>
    /// <param name="from">The attack to defend against.</param>
    /// <param name="damageParameter">The character responsible for the attack.</param>
    /// <returns>The amount of damage mitigated or deflected.</returns>
    public abstract int Defend<TTarget>(Attack<TTarget> from, Character damageParameter) where TTarget : class, ITarget;

    //

    /// <summary>
    /// Allows the player to select a skill to use.
    /// </summary>
    /// <returns>
    /// The selected <see cref="Skill"/> if a usable skill is chosen; otherwise, <c>null</c>.
    /// </returns>
    /// <remarks>
    /// Prompts the player to choose a skill from the character's list of <see cref="Skills"/>. 
    /// Displays details about the selected skill and checks if it is usable before confirming the selection.
    /// If no usable skills are available, the method returns <c>null</c>.
    /// </remarks>
    private Skill? SelectSkill()
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
                    $"{skill.Name} ({skill.GetType().Name}){(!skill.IsUsable() ? $" - Disponible dans : {skill.ReloadCooldown} tour{(skill.ReloadCooldown > 1 ? "s" : string.Empty)}" : string.Empty)}",
                Skills) - 1);
            Console.WriteLine();

            List<string> choices = ["Afficher details", "Sélectionné une autre capacité"];
            if (selectedSkill.IsUsable()) choices.Add("Confirmer");
            else
                Console.WriteLine(
                    $"Vous ne pouvez pas utiliser {selectedSkill.Name} actuellement. Disponible dans : {selectedSkill.ReloadCooldown} tour{(selectedSkill.ReloadCooldown > 1 ? "s" : string.Empty)}.");

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

    /// <summary>
    /// Allows the player to select a target based on the specified target type.
    /// </summary>
    /// <param name="targetType">
    /// The type of target to select, defined by <see cref="TargetType"/>.
    /// </param>
    /// <returns>
    /// The selected <see cref="ITarget"/> if a valid target is chosen; otherwise, <c>null</c>.
    /// </returns>
    /// <remarks>
    /// Depending on the target type, the method filters available targets. 
    /// Prompts the player to select one of the eligible targets.
    /// If no valid targets are available, the method returns <c>null</c>.
    /// </remarks>
    /// <exception cref="ArgumentOutOfRangeException">
    /// Thrown if the <paramref name="targetType"/> is not recognized.
    /// </exception>
    private ITarget? SelectTarget(TargetType targetType)
    {
        List<ITarget> targets;
        switch (targetType)
        {
            case TargetType.Self:
                return this;
            case TargetType.Teammate:
                targets = [..Team.Characters.Where(character => character != this && character.IsAlive(false))];
                break;
            case TargetType.Enemy:
                targets = [..List.Where(character => character.Team != Team && character.IsAlive(false))];
                break;
            case TargetType.TeamAllied:
                return Team;
            case TargetType.TeamEnemy:
                targets =
                [
                    ..JRPG_Game.Team.Team.List.Where(team =>
                        team != Team && team.Characters.Any(character => character.IsAlive(false)))
                ];
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(targetType), targetType,
                    $"{targetType} n'est pas un type de cible reconnu.");
        }

        if (targets.Count != 0)
            return targets.ElementAt(Prompt.Select(
                "Sur qui voulez vous utiliser cette capacité ?",
                target => target switch
                {
                    Character tCharacter => $"{tCharacter.Name} - {tCharacter.CurrentHealth}/{tCharacter.MaxHealth} PV",
                    Team.Team tTeam =>
                        $"{tTeam.Name} - {tTeam.Characters.Count(character => character.IsAlive(false))} personnage(s) en vie",
                    _ => target.Name
                },
                targets) - 1);

        Console.WriteLine("Pas de cible disponible pour cette capacité.");
        return null;
    }

    /// <summary>
    /// Handles the player's turn by allowing them to select an action.
    /// </summary>
    /// <returns>
    /// A tuple containing:
    /// <list type="bullet">
    ///     <item>
    ///         <term>Next</term>
    ///         <description>indicating whether to proceed to the next turn.</description>
    ///     </item>
    ///     <item>
    ///         <term>Skill</term>
    ///         <description>the selected <see cref="Skill"/> if applicable.</description>
    ///     </item>
    /// </list>
    /// </returns>
    /// <remarks>
    /// Displays a menu of actions for the player, such as viewing information, using a skill, or skipping their turn. 
    /// Executes the chosen action and returns the result. If a skill is selected, the method prompts for a target
    /// and uses the skill accordingly.
    /// </remarks>
    public (bool Next, Skill? Skill) SelectAction()
    {
        var selected = Prompt.Select("Que voulez-vous faire ?", s => s,
            "Afficher informations",
            "Afficher équipe",
            "Afficher le status de la partie",
            "Utiliser une capacité",
            "Passer son tour",
            "Effacer le terminal");
        Console.WriteLine();

        switch (selected)
        {
            case 1:
                Console.WriteLine(this + "\n");
                break;
            case 2:
                if (Team.Characters.Any(character => character != this))
                {
                    Console.WriteLine($"Info de l'équipe : {Team.Name}");
                    Team.Characters
                        .Where(character => character != this).ToList()
                        .ForEach(
                            character =>
                                Console.WriteLine(
                                    $"   - {character.Name} ({character.GetType().Name}) - {(character.CurrentHealth != 0 ? $"{character.CurrentHealth}/{character.MaxHealth} PV" : "mort")}")
                        );
                    Console.WriteLine();
                    break;
                }

                Console.WriteLine($"{Name} est le seul personnage de votre équipe.\n");
                break;
            case 3:
                Console.WriteLine($"{new string('-', 10)} Status de la partie {new string('-', 10)}");
                JRPG_Game.Team.Team.List.ForEach(team =>
                    Console.WriteLine(
                        $"  - {team.Name} {(team == Team ? "(votre équipe) " : string.Empty)}~ {(team.Characters.Any(character => character.IsAlive(false))
                            ? $"{team.Characters.Count(character => character.IsAlive(false))} en vie"
                            : "éliminé")}"));
                Console.WriteLine(new string('-', 10 * 2 + " Status de la partie ".Length));
                Console.WriteLine();
                break;
            case 4:
                var skill = SelectSkill();
                if (skill == null) return (true, null);
                var status = skill.Use(SelectTarget(skill.TargetType));
                return (status.Next, status.Execute ? skill : null);
            case 5:
                return (true, null);
            case 6:
                Program.Next();
                Console.WriteLine($"Au tour de {Name} - {Team.Name}");
                Console.WriteLine($"{this}\n");
                break;
            default:
                Console.WriteLine("Action inconnue, veillez réessayer.");
                break;
        }

        return (false, null);
    }

    //

    /// <summary>
    /// Creates a new character and assigns it to the specified team.
    /// </summary>
    /// <param name="team">The team to assign the new character to.</param>
    /// <returns>The created character.</returns>
    public static Character Create(Team.Team team)
    {
        while (true)
        {
            List<Type> classList = [typeof(Mage), typeof(Paladin), typeof(Priest), typeof(Thief), typeof(Warrior)];
            var characterType =
                classList.ElementAt(Prompt.Select("Choisissez votre classe :", c => c.Name, classList) - 1);

            if (Activator.CreateInstance(characterType, Prompt.Get<string>("Entrez le nom du personnage :"), team)
                is Character character)
                return character;

            Console.WriteLine("Une erreur s'est produite, veillez réessayer.");
        }
    }

    //

    /// <summary>
    /// Returns a string that represents the <see cref="Character"/>.
    /// </summary>
    /// <returns>A string that represents the <c>Character</c></returns>
    public override string ToString()
    {
        return $"{GetType().Name}: {Name} ({CurrentHealth}/{MaxHealth} PV) - Armure: {Armor}\n" +
               $"Stats :\n" +
               $" ~ Vitesse: {Speed}\n" +
               $" ~ Attaque Physique: {PhysicalAttack}\n" +
               $" ~ Attaque Magique: {MagicalAttack}\n" +
               $" ~ Chances d'Esquiver: {DodgeChance:P}\n" +
               $" ~ Chances de Parade: {ParadeChance:P}\n" +
               $" ~ Chances de Resister aux Sorts: {SpellResistanceChance:P}\n" +
               $" ~ Compétences :\n" +
               string.Join("\n", Skills.Select(skill => $"\t- {skill.Name} ({skill.GetType().Name})"));
    }
}