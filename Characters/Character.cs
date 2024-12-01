﻿using JRPG_Game.Characters.Skills;
using JRPG_Game.Enums;
using JRPG_Game.Interfaces;
using JRPG_Game.Utils;

namespace JRPG_Game.Characters;

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

    public string Name { get; private set; }
    public Team.Team Team { get; private set; }
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

    private static readonly List<Character> List = [];

    //

    public bool IsAlive(bool message)
    {
        if (CurrentHealth <= 0 && message)
            Console.WriteLine($"{Name} est mort.");
        return CurrentHealth > 0;
    }

    protected int TakeDamage(int damage) {
        var damageTaken = Math.Min(CurrentHealth, damage);
        CurrentHealth -= damageTaken;
        return damageTaken;
    }

    public int Heal(int healthPoint, bool message = true)
    {
        var healed = Math.Min(MaxHealth - CurrentHealth, healthPoint);
        CurrentHealth += healed;

        if (healed <= 0 || !message) return healed;
        Console.WriteLine($"{Name} à été soigné de {healed}");

        return healed;
    }

    public bool Dodge<TTarget>(Attack<TTarget> attack) where TTarget : class, ITarget
    {
        return attack.AttackType == DamageType.Physical
               && (decimal)new Random().NextDouble() <= DodgeChance;
    }

    public bool Parade<TTarget>(Attack<TTarget> attack) where TTarget : class, ITarget
    {
        return attack.AttackType == DamageType.Physical
               && (decimal)new Random().NextDouble() <= ParadeChance;
    }

    public bool SpellResistance<TTarget>(Attack<TTarget> attack) where TTarget : class, ITarget
    {
        return attack.AttackType == DamageType.Magical
               && (decimal)new Random().NextDouble() <= SpellResistanceChance;
    }

    public decimal ArmorReduction(DamageType damageType)
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

    public abstract int Defend<TTarget>(Attack<TTarget> from, Character damageParameter) where TTarget : class, ITarget;

    //

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

    private ITarget SelectTarget(TargetType targetType)
    {
        List<ITarget> targets;
        switch (targetType)
        {
            case TargetType.Self:
                return this;
            case TargetType.Teammate:
                targets = [..Team.Characters.Where(character => character.IsAlive(false))];
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
                        team != Team && team.Characters.Any(character => character.IsAlive(false))).ToList()
                ];
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(targetType), targetType,
                    $"Erreur sur le paramètre targetType : {targetType}");
        }

        return targets.ElementAt(Prompt.Select("Sur qui voulez vous utiliser cette capacité ?", target => target.Name,
            targets) - 1);
    }

    public (bool Next, Skill? Skill) SelectAction()
    {
        var selected = Prompt.Select("Que voulez-vous faire ?", s => s,
            "Afficher informations", "Utiliser une capacité", "Passer son tour", "Effacer le terminal");
        Console.WriteLine();

        switch (selected)
        {
            case 1:
                Console.WriteLine(this + "\n");
                break;
            case 2:
                var skill = SelectSkill();

                if (skill == null) return (true, null);
                var status = skill.Use(SelectTarget(skill.TargetType));

                return (status.Next, status.Execute ? skill : null);
            case 3:
                return (true, null);
            case 4:
                Program.Next();
                break;
            default:
                Console.WriteLine("Action inconnue, veillez réessayer.");
                break;
        }

        return (false, null);
    }

    //

    protected abstract void SpecialAbility();
    protected abstract void Attack(Character character);

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

    public static Character Create(Team.Team team)
    {
        while (true)
        {
            List<Type> classList = [typeof(Mage), typeof(Paladin), typeof(Thief), typeof(Warrior)];
            var characterType =
                classList.ElementAt(Prompt.Select("Choisissez votre classe :", c => c.Name, classList) - 1);

            if (Activator.CreateInstance(characterType, Prompt.Get<string>("Entrez le nom du personnage :"), team) is Character
                character)
                return character;

            Console.WriteLine("Une erreur s'est produite : " + characterType);
        }
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