using JRPG_Game.Enums;
using JRPG_Game.Interfaces;

namespace JRPG_Game.Characters.Skills;

/// <summary>
/// Represents a skill in the form of an attack, capable of targeting a character or a team.
/// </summary>
/// <typeparam name="TTarget">The type of the target, must implement the <see cref="ITarget"/> interface.</typeparam>
public class Attack<TTarget>(
    string name,
    Func<string> description,
    Character owner,
    TTarget? target,
    TargetType targetType,
    int reloadTime,
    int manaCost,
    Func<Character, int> damage,
    DamageType attackType,
    List<Action<Attack<Character>>>? additional = null)
    : Skill(name: name,
        owner: owner,
        target: target,
        targetType: targetType,
        description: description,
        reloadTime: reloadTime,
        manaCost: manaCost)
    where TTarget : class, ITarget
{
    public Func<Character, int> Damage { get; set; } = damage;
    public DamageType AttackType { get; } = attackType;
    public List<Action<Attack<Character>>>? Additional { get; set; } = additional;
    public Status StatusInfo { get; private set; } = new();

    public Attack(
        string name,
        Func<string> description,
        Character owner,
        TargetType targetType,
        int reloadTime,
        int manaCost,
        Func<Character, int> damage,
        DamageType attackType,
        List<Action<Attack<Character>>>? additional = null) :
        this(
            name: name,
            owner: owner,
            target: null,
            targetType: targetType,
            description: description,
            reloadTime: reloadTime,
            manaCost: manaCost,
            damage: damage,
            attackType: attackType,
            additional: additional
        )
    {
    }

    public Attack(
        string name,
        Func<string> description,
        Character owner,
        TTarget? target,
        TargetType targetType,
        int reloadTime,
        int manaCost,
        int damage,
        DamageType attackType,
        List<Action<Attack<Character>>>? additional = null) :
        this(
            name: name,
            owner: owner,
            target: target,
            targetType: targetType,
            description: description,
            reloadTime: reloadTime,
            manaCost: manaCost,
            damage: _ => damage,
            attackType: attackType,
            additional: additional
        )
    {
    }

    public Attack(
        string name,
        Func<string> description,
        Character owner,
        TargetType targetType,
        int reloadTime,
        int manaCost,
        int damage,
        DamageType attackType,
        List<Action<Attack<Character>>>? additional = null) :
        this(
            name: name,
            owner: owner,
            target: null,
            targetType: targetType,
            description: description,
            reloadTime: reloadTime,
            manaCost: manaCost,
            damage: _ => damage,
            attackType: attackType,
            additional: additional
        )
    {
    }

    /// <summary>
    /// Executes the attack and applies its effects to the target based on the target type (<see cref="Character"/> or <see cref="Team"/>).
    /// </summary>
    public override void Execute()
    {
        if (!Owner.IsAlive(false)) return;
        if (Owner.Effects.ContainsKey(StatusEffect.Stun) && new Random().NextDouble() < 0.15)
        {
            Console.WriteLine($"{Owner.Name} est étourdi est rate son attaque {Name}.");
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
        StatusInfo = new Status();
    }

    /// <summary>
    /// Executes the attack on a specific character target.
    /// </summary>
    /// <param name="character">
    /// Placeholder for the character target.
    /// Will not be used in the method, uses the <see cref="Skill.Target"/> attribute after checking that it is of type <see cref="Character"/>.
    /// </param>
    /// <param name="message">Optional custom message to display during the attack.</param>
    private void Execute(Character character, Func<Character, string>? message = null)
    {
        if (Target is not Character target || this is not Attack<Character> attack)
        {
            Console.WriteLine(Target != null
                ? $"Erreur : le type de la cible de {Name} fait par {Owner.Name}. Attendu: {nameof(Character)}, Actuel: {Target.GetType().Name}."
                : $"Erreur : la cible de {Name} fait par {Owner.Name} est null.");
            return;
        }

        if (!target.IsAlive(false))
        {
            Console.WriteLine($"La cible de {Name} ({Target.Name}) par {Owner.Name} est deja morte.");
            return;
        }

        message ??= t =>
            $"{Owner.Name} fait {Name}{(TargetType != TargetType.Self ? $" sur {t.Name}" : string.Empty)}.";
        Console.WriteLine(message(target));

        StatusInfo.Damage = target.Defend(attack, target);

        if (StatusInfo.Dodged) Console.WriteLine($"{target.Name} a réussi à esquiver {Name}.");
        if (StatusInfo.Resisted) Console.WriteLine($"{target.Name} a réussi à resister à {Name}.");
        if (StatusInfo.Blocked) Console.WriteLine($"{target.Name} a paré un partie des dégâts de {Name}.");
        if (StatusInfo.Damage > 0) Console.WriteLine($"{Name} a fait {StatusInfo.Damage} de dégâts à {target.Name}.");
        target.IsAlive(true);

        Additional?.ForEach(add => add(attack));
    }

    /// <summary>
    /// Executes the attack on a team, applying the effects to all characters in the team.
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

        Console.WriteLine($"{Owner.Name} fait {Name} sur l'équipe {target.Name}.");
        target.Characters
            .Where(c => c.IsAlive(false)).ToList()
            .ForEach(trgt =>
            {
                new Attack<Character>(
                    name: Name,
                    description: Description,
                    owner: Owner,
                    target: trgt,
                    targetType: TargetType.Enemy,
                    reloadTime: 0,
                    manaCost: 0,
                    damage: Damage,
                    attackType: AttackType,
                    additional: Additional
                ).Execute(trgt, t => $"{Name} atteint {t.Name}.");
            });
    }

    /// <summary>
    /// Returns a string that represents the <see cref="Attack{TTarget}"/>.
    /// </summary>
    /// <returns>A string that represents the <c>Attack</c></returns>
    public override string ToString()
    {
        return base.ToString() + "\n" +
               $"Attaque de Type {AttackType}";
    }

    /// <summary>
    /// Represents the status of an attack, including outcomes such as damage dealt, dodged, resisted, or blocked.
    /// </summary>
    public class Status
    {
        public decimal Damage { get; set; }
        public bool Dodged { get; private set; }
        public bool Resisted { get; private set; }
        public bool Blocked { get; private set; }

        /// <summary>
        /// Sets the status of the attack based on whether it was dodged, resisted, or blocked.
        /// Using 
        /// <see cref="Character.Dodge{TTarget}"/>, <see cref="Character.SpellResistance{TTarget}"/>, <see cref="Character.Parade{TTarget}"/>
        /// methods of <see cref="Character"/>.
        /// </summary>
        /// <param name="attack">The attack being evaluated.</param>
        /// <param name="attackStatus">Optional precomputed status values.</param>
        public void Set(Attack<TTarget> attack,
            (bool Dodged, bool Resisted, bool Blocked) attackStatus = default)
        {
            if (attack.Target is not Character target) return;

            if (attackStatus.Dodged || target.Dodge(attack)) Dodged = true;
            else if (attackStatus.Resisted || target.SpellResistance(attack)) Resisted = true;
            else if (attackStatus.Blocked || target.Parade(attack)) Blocked = true;
        }

        /// <summary>
        /// Calculates and sets the damage dealt by the attack, taking into account modifiers such as blocks and armor reduction.
        /// </summary>
        /// <param name="attack">The attack being evaluated.</param>
        /// <param name="damageParameter">The character being targeted.</param>
        public void SetDamage(Attack<TTarget> attack, Character damageParameter)
        {
            if (attack.Target is not Character target) return;
            if (Dodged || Resisted)
            {
                Damage = 0;
                return;
            }

            Damage = attack.Damage(damageParameter)
                     * (Blocked ? 0.5m : 1)
                     * (1 - target.ArmorReduction(attack.AttackType));
        }
    }
}