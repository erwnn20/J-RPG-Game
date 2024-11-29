using JRPG_Game.Characters;
using JRPG_Game.Characters.Skills;
using JRPG_Game.Interfaces;

namespace JRPG_Game.Team;

public class Team : ITarget
{
    public List<Character> Characters { get; set; }


    public int Defend<T1, T2, T3>(Attack<T1, T2, T3> from, T3 damageParameter)
    {
        throw new NotImplementedException();
    }
}