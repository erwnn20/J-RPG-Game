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
        Team team,
        int maxHealth,
        int speed,
        int physicalAttack,
        int magicalAttack,
        int distanceAttack,
        ArmorType armor,
        NumericContainer<decimal> paradeChance,
        NumericContainer<decimal> dodgeChance,
        NumericContainer<decimal> spellResistanceChance,
        List<Skill> skills)
    {
        Name = name;
        Team = team;
        Health = new NumericContainer<int>(0, maxHealth, maxHealth);
        Speed = speed;
        PhysicalAttack = physicalAttack;
        MagicalAttack = magicalAttack;
        DistanceAttack = distanceAttack;
        Armor = armor;
        ParadeChance = paradeChance;
        DodgeChance = dodgeChance;
        SpellResistanceChance = spellResistanceChance;
        Skills = skills;
        Effects = [];

        List.Add(this);
    }

    protected Character(
        string name,
        Team team,
        int maxHealth,
        int speed,
        int physicalAttack,
        int magicalAttack,
        int distanceAttack,
        ArmorType armor,
        decimal paradeChance,
        decimal dodgeChance,
        decimal spellResistanceChance,
        List<Skill> skills)
        : this(
            name: name,
            team: team,
            maxHealth: maxHealth,
            speed: speed,
            physicalAttack: physicalAttack,
            magicalAttack: magicalAttack,
            distanceAttack: distanceAttack,
            armor: armor,
            paradeChance: new NumericContainer<decimal>(0, paradeChance, 1),
            dodgeChance: new NumericContainer<decimal>(0, dodgeChance, 1),
            spellResistanceChance: new NumericContainer<decimal>(0, spellResistanceChance, 1),
            skills: skills
        )
    {
    }

    public string Name { get; }
    public Team Team { get; }
    public NumericContainer<int> Health { get; }
    public int Speed { get; set; }
    public int PhysicalAttack { get; set; }
    private int MagicalAttack { get; }
    private int DistanceAttack { get; }
    private ArmorType Armor { get; }
    protected NumericContainer<decimal> DodgeChance { get; set; }
    private NumericContainer<decimal> ParadeChance { get; }
    protected NumericContainer<decimal> SpellResistanceChance { get; set; }
    protected List<Skill> Skills { get; }
    public Dictionary<StatusEffect, int> Effects { get; }

    /// <summary>
    /// List containing all instantiated characters.
    /// </summary>
    private static readonly List<Character> List = [];

    //

    public int GetSpeed()
    {
        if (Effects.ContainsKey(StatusEffect.Paralysis)) return 0;

        return Speed + Effects.Keys.Select(effect => effect switch
        {
            StatusEffect.Speed => 10,
            StatusEffect.Slowness => -10,
            _ => 0
        }).Sum();
    }

    protected int GetAttack(DamageType damageType)
    {
        return damageType switch
        {
            DamageType.Physical => PhysicalAttack,
            DamageType.Magical => MagicalAttack,
            DamageType.Distance => DistanceAttack,
            _ => 0,
        } + Effects.Keys.Select(effect => effect switch
        {
            StatusEffect.Focus => 10,
            StatusEffect.AdrenalinRush => 15,
            StatusEffect.Strength => 20,
            _ => 0
        }).Sum();
    }

    /// <summary>
    /// Checks if the character is alive.
    /// </summary>
    /// <param name="message">If <c>true</c>, outputs a message if the character is dead.</param>
    /// <returns><c>true</c> if the character's health is above zero, otherwise <c>false</c>.</returns>
    public bool IsAlive(bool message)
    {
        if (Health.Current <= 0 && message)
            Console.WriteLine($"{Name} est mort.");
        return Health.Current > 0;
    }

    /// <summary>
    /// Applies damage to the character and reduces their health.
    /// </summary>
    /// <param name="damage">The amount of damage to apply.</param>
    /// <returns>The actual damage taken, capped at the character's remaining health.</returns>
    protected int TakeDamage(int damage)
    {
        if (!Effects.ContainsKey(StatusEffect.Invincibility))
            return (int)(Health.Subtract(damage) * Effects.Keys.Select(effect => effect switch
            {
                StatusEffect.AdrenalinRush => 0.95m,
                _ => 1m
            }).Aggregate(1m, (x, y) => x * y));

        Console.WriteLine($"{Name} est actuellement invincible et ne prends aucun dégâts.");
        return 0;
    }

    /// <summary>
    /// Heals the character by a specified amount of health points.
    /// </summary>
    /// <param name="healthPoint">The amount of health to heal.</param>
    /// <returns>The actual amount of health restored.</returns>
    public int Heal(int healthPoint) => Health.Add(healthPoint);

    /// <summary>
    /// Attempts to dodge a physical attack using <see cref="DodgeChance"/> attribute.
    /// </summary>
    /// <typeparam name="TTarget">The target type, implementing <see cref="ITarget"/>.</typeparam>
    /// <param name="attack">The attack to dodge.</param>
    /// <returns><c>true</c> if the dodge is successful; otherwise, <c>false</c>.</returns>
    public bool Dodge<TTarget>(Attack<TTarget> attack) where TTarget : class, ITarget
        => attack.AttackType is DamageType.Physical or DamageType.Distance
           && (decimal)new Random().NextDouble() < DodgeChance.Current
           * Effects.Keys.Select(effect => effect switch
           {
               StatusEffect.Speed => 1.25m,
               StatusEffect.Focus => 1.10m,
               StatusEffect.Slowness => 0.75m,
               StatusEffect.Stun => 0.95m,
               _ => 1m
           }).Aggregate(1m, (x, y) => x * y);

    /// <summary>
    /// Attempts to parry a physical or distance attack using <see cref="ParadeChance"/> attribute.
    /// </summary>
    /// <typeparam name="TTarget">The target type, implementing <see cref="ITarget"/>.</typeparam>
    /// <param name="attack">The attack to parry.</param>
    /// <returns><c>true</c> if the parry is successful; otherwise, <c>false</c>.</returns>
    public bool Parade<TTarget>(Attack<TTarget> attack) where TTarget : class, ITarget
        => (decimal)new Random().NextDouble() < ParadeChance.Current
            * attack.AttackType switch
            {
                DamageType.Physical => 1,
                DamageType.Distance => 0.70m,
                _ => 0
            } * Effects.Keys.Select(effect => effect switch
            {
                StatusEffect.Stun => 0.50m,
                _ => 1m
            }).Aggregate(1m, (x, y) => x * y);

    /// <summary>
    /// Attempts to resist a magical attack using <see cref="SpellResistanceChance"/> attribute.
    /// </summary>
    /// <typeparam name="TTarget">The target type, implementing <see cref="ITarget"/>.</typeparam>
    /// <param name="attack">The attack to resist.</param>
    /// <returns><c>true</c> if the resistance is successful; otherwise, <c>false</c>.</returns>
    public bool SpellResistance<TTarget>(Attack<TTarget> attack) where TTarget : class, ITarget
        => attack.AttackType == DamageType.Magical
           && (decimal)new Random().NextDouble() < SpellResistanceChance.Current;

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
            DamageType.Distance => Armor switch
            {
                ArmorType.Textile => 0.05m,
                ArmorType.Leather => 0.10m,
                ArmorType.Mesh => 0.25m,
                ArmorType.Plates => new Random().NextDouble() < 0.5 ? 0.50m : 0.00m,
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
        if (Effects.ContainsKey(StatusEffect.Paralysis))
        {
            Console.WriteLine($"{Name} est paralysé, il ne peut pas utiliser de compétence.");
            return null;
        }

        if (!Skills.Any(skill => skill.IsUsable()))
        {
            Console.WriteLine($"Aucune compétence utilisable par {Name} actuellement.");
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
            case TargetType.TeammateDead:
                targets = [..Team.Characters.Where(character => character != this && !character.IsAlive(false))];
                break;
            case TargetType.Enemy:
                targets = [..List.Where(character => character.Team != Team && character.IsAlive(false))];
                break;
            case TargetType.TeamAllied:
                return Team;
            case TargetType.TeamEnemy:
                targets =
                [
                    ..Team.List.Where(team =>
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
                    Character tCharacter =>
                        $"{tCharacter.Name} - {tCharacter.Health.Current}/{tCharacter.Health.Max} PV",
                    Team tTeam =>
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
            $"Utiliser une capacité{(Effects.ContainsKey(StatusEffect.Paralysis) ? $" - {Name} est paralysé, il ne pourra pas utiliser de compétence." : string.Empty)}",
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
                                    $"   - {character.Name} ({character.GetType().Name}) - {(character.Health.Current != 0 ? $"{character.Health.Current}/{character.Health.Max} PV" : "mort")}")
                        );
                    Console.WriteLine();
                    break;
                }

                Console.WriteLine($"{Name} est le seul personnage de votre équipe.\n");
                break;
            case 3:
                Console.WriteLine($"{new string('-', 10)} Status de la partie {new string('-', 10)}");
                Team.List.ForEach(team =>
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

    public int AddEffect(StatusEffect effect, int duration)
    {
        if (!Effects.TryAdd(effect, duration)) Effects[effect] += duration;
        return Effects[effect];
    }

    private void ApplyEffect(StatusEffect effect)
    {
        switch (effect)
        {
            case StatusEffect.Regeneration when !Effects.ContainsKey(StatusEffect.Bleeding):
                var healed = Heal((int)(Health.Max * 0.05m));
                if (healed > 0) Console.WriteLine($"{Name} a régénéré sa vie de {healed} PV.");
                break;
            case StatusEffect.Poison:
                Console.WriteLine($"{Name} à pris {TakeDamage(5)} dégâts de poison.");
                break;
            case StatusEffect.Bleeding:
                Console.WriteLine($"{Name} à pris {TakeDamage((int)(Health.Max * 0.05m))} dégâts de saignement.");
                break;
            case StatusEffect.Paralysis:
                if (new Random().NextDouble() < 0.15) Effects[StatusEffect.Paralysis] = 0;
                break;
            case StatusEffect.Burn:
                var random = new Random();
                Console.WriteLine($"{Name} à pris {TakeDamage(random.Next(1, 15))} dégâts de brulure.");
                if (random.NextDouble() < 0.25) Effects[StatusEffect.Burn] = 0;
                break;
        }

        Effects[effect]--;
        if (Effects[effect] <= 0) Effects.Remove(effect);
    }

    protected virtual void ApplyEndTurn()
    {
        foreach (var effect in Effects.Keys)
        {
            ApplyEffect(effect);
            if (!IsAlive(true)) break;
        }
    }

    //

    /// <summary>
    /// Creates a new character and assigns it to the specified team.
    /// </summary>
    /// <param name="team">The team to assign the new character to.</param>
    /// <returns>The created character.</returns>
    public static Character Create(Team team)
    {
        while (true)
        {
            var classList = Prompt.GetAllSubclassesOf(typeof(Character));
            var characterType =
                classList.ElementAt(Prompt.Select("Choisissez votre classe :", c => c.Name, classList) - 1);

            if (Activator.CreateInstance(characterType, Prompt.Get<string>("Entrez le nom du personnage :"), team)
                is Character character)
                return character;

            Console.WriteLine("Une erreur s'est produite, veillez réessayer.");
        }
    }

    public static void EndTurn() => List
        .Where(character => character.IsAlive(false)).ToList()
        .ForEach(character =>
        {
            character.ApplyEndTurn();
            Console.WriteLine();
        });

    //

    /// <summary>
    /// Returns a string that represents the <see cref="Character"/>.
    /// </summary>
    /// <returns>A string that represents the <c>Character</c></returns>
    public override string ToString()
    {
        return $"{GetType().Name}: {Name} ({Health.Current}/{Health.Max} PV) - Armure: {Armor}\n" +
               $"Stats :\n" +
               $" ~ Vitesse: {Speed}\n" +
               $" ~ Attaque Physique: {PhysicalAttack}\n" +
               $" ~ Attaque Magique: {MagicalAttack}\n" +
               $" ~ Attaque à Distance: {DistanceAttack}\n" +
               $" ~ Chances d'Esquiver: {DodgeChance.Current:P}\n" +
               $" ~ Chances de Parade: {ParadeChance.Current:P}\n" +
               $" ~ Chances de Resister aux Sorts: {SpellResistanceChance.Current:P}\n" +
               $" ~ Compétences :\n" +
               string.Join("\n", Skills.Select(skill => $"\t- {skill.Name} ({skill.GetType().Name})")) +
               (Effects.Count > 0
                   ? $"\n ~ Effets : {string.Join(", ", Effects.Select(effect => $"{effect.Key} ({effect.Value} tour{(effect.Value > 1 ? "s" : "")})"))}"
                   : string.Empty);
    }
}