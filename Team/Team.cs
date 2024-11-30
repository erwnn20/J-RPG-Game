using JRPG_Game.Characters;
using JRPG_Game.Characters.Skills;
using JRPG_Game.Interfaces;

namespace JRPG_Game.Team;

public class Team : ITarget
{
    public List<Character> Characters { get; set; }
    public string Name { get; set; }

    public int Defend<TTarget>(Attack<TTarget> from, Character damageParameter) where TTarget : class, ITarget
    {
        throw new NotImplementedException();
    }
}