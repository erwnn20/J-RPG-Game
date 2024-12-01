using JRPG_Game.Enums;
using JRPG_Game.Interfaces;

namespace JRPG_Game.Characters.Skills;

public class Attack<TTarget>(
    string name,
    string description,
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
    public DamageType AttackType { get; private set; } = attackType;
    public List<Action<Attack<Character>>>? Additional { get; set; } = additional;
    public Status StatusInfo { get; private set; } = new();

    public Attack(
        string name,
        string description,
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
        string description,
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
        string description,
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
        StatusInfo = new Status();
    }

    private void Execute(Character _, Func<Character, string>? message = null)
    {
        if (Target is not Character target || this is not Attack<Character> attack)
        {
            Console.WriteLine(Target != null
                ? $"Erreur de cible sur {Name}. Attendu: {nameof(Character)}, Actuel: {Target.GetType().Name}.)"
                : $"Erreur de cible sur {Name}. La cible est null.");
            return;
        }

        if (!target.IsAlive(false))
        {
            Console.WriteLine($"La cible de {Name} est deja morte.");
            return;
        }

        message ??= t => $"{Owner.Name} fait {Name} sur {t.Name}";
        Console.WriteLine(message(target));

        StatusInfo.Damage = Target.Defend(attack, target);

        if (StatusInfo.Dodged) Console.WriteLine($"{Target.Name} a réussi à esquiver {Name}.");
        if (StatusInfo.Resisted) Console.WriteLine($"{Target.Name} a réussi à resister à {Name}.");
        if (StatusInfo.Blocked) Console.WriteLine($"{Target.Name} a parer un partie des dégâts de {Name}.");
        if (StatusInfo.Damage > 0) Console.WriteLine($"{Name} a fait {StatusInfo.Damage} de dégâts à {Target.Name}.");
        target.IsAlive(true);

        Additional?.ForEach(add => add(attack));
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
                new Attack<Character>(
                    name: Name,
                    description: Description,
                    owner: Owner,
                    target: target,
                    targetType: TargetType.Enemy,
                    reloadTime: 0,
                    manaCost: 0,
                    damage: Damage,
                    attackType: AttackType,
                    additional: Additional
                ).Execute(target, t => $"{Name} atteint {t.Name}");
            });
    }

    public override string ToString()
    {
        return base.ToString() + "\n" +
               $"Attaque de Type {AttackType}";
    }

    public class Status
    {
        public decimal Damage { get; set; }
        public bool Dodged { get; private set; }
        public bool Resisted { get; private set; }
        public bool Blocked { get; private set; }

        public void Set(Attack<TTarget> attack,
            (bool Dodged, bool Resisted, bool Blocked) attackStatus = default)
        {
            if (attack.Target is Character target)
            {
                if (attackStatus.Dodged || target.Dodge(attack)) Dodged = true;
                else if (attackStatus.Resisted || target.SpellResistance(attack)) Resisted = true;
                else if (attackStatus.Blocked || target.Parade(attack)) Blocked = true;
            }
        }

        public decimal SetDamage(Attack<TTarget> attack, Character damageParameter)
        {
            if (attack.Target is Character target)
            {
                if (Dodged || Resisted) return 0;

                Damage = attack.Damage(damageParameter);
                if (Blocked) Damage *= 0.5m;
                Damage *= 1 - target.ArmorReduction(attack.AttackType);
            }

            return Damage;
        }
    }
}