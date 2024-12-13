using JRPG_Game.Characters;
using JRPG_Game.Characters.Elements.Skills;
using JRPG_Game.Utils;

namespace JRPG_Game;

/// <summary>
/// The main entry point for the game, responsible for initializing teams and characters, executing turns, and determining the winner.
/// </summary>
public static class Program
{
    /// <summary>
    /// Entry point of the game. Handles the game loop and restart logic.
    /// </summary>
    public static void Main()
    {
        Console.Clear();
        Console.WriteLine("Bienvenue dans le jeu !\n");

        bool restart;
        do
        {
            GameInit();
            var winningTeam = GameContent();
            GameEnd(winningTeam);

            restart = Prompt.Select(
                "Voulez vous refaire une partie ?\n" +
                "Tout les personnages de l'équipe gagnante sera soigné.",
                s => s,
                "Oui", "Non") == 1;
            Next();
        } while (restart);

        Console.WriteLine("Merci d'avoir jouer !");
        Next(2000);
    }

    /// <summary>
    /// Initializes the game by setting up teams and their characters.
    /// </summary>
    /// <remarks>
    ///         This method performs the following steps:
    ///     <list type="bullet">
    ///         <item>
    ///             <description>Prompts the user to specify the number of teams and characters per team.</description>
    ///         </item>
    ///         <item>
    ///             <description>Updates existing teams or creates new ones if needed.</description>
    ///         </item>
    ///         <item>
    ///             <description>Displays the setup summary before starting the game.</description>
    ///         </item>
    ///     </list>
    /// </remarks>
    private static void GameInit()
    {
        var nbrTeam =
            Prompt.Get<int>(
                $"Entrez le nombre d'équipes{(Team.List.Count > 0 ? $" que vous voulez rajouter (actuellement : {Team.List.Count})" : string.Empty)} :",
                i => i < Math.Max(0, 2 - Team.List.Count)) + Team.List.Count;

        var minCharacters =
            Math.Max(1, Team.List.Count > 0 ? Team.List.Select(team => team.Characters.Count).Max() : 0);
        var nbrCharacters = Prompt.Get<int>($"Entrez le nombre de joueurs par équipe (minimum : {minCharacters}) :",
            i => i < minCharacters);

        Next();

        Team.List.ForEach(team =>
        {
            team.Reset();
            Console.WriteLine($"L'équipe {team.UpdateSize(nbrCharacters).Name} a été mise à jour.");
            Next(2000);
        });
        for (var i = Team.List.Count; i < nbrTeam; i++)
        {
            Console.WriteLine($"Creation de l'équipe {i + 1}...");
            Console.WriteLine($"L'équipe {Team.Create(nbrCharacters).Name} a été créée.");
            Next(2000);
        }

        Console.WriteLine($"{Team.List.Count} équipes créées.\n");
        Team.List.ForEach(team =>
        {
            Console.WriteLine($"{new string('-', 10)} {team.Name} {new string('-', 10)}");
            team.Characters.ForEach(character =>
                Console.WriteLine($"  {character.Name} - {character.GetType().Name}"));
            Console.WriteLine(new string('-', 10 * 2 + $" {team.Name} ".Length) + "\n");
        });

        Prompt.Input("Appuyez sur 'Entrée' pour commencer la partie",
            key => key != ConsoleKey.Enter);
        Next(0);
    }

    /// <summary>
    /// Contains the main game loop where teams take turns, actions are selected and executed, 
    /// and the game ends when a winner is determined.
    /// </summary>
    /// <returns>
    /// The winning team if one exists; otherwise, null if an error occurred or there was no winner.
    /// </returns>
    /// <remarks>
    /// This method manages the flow of a turn-based game. The process includes:
    /// <list type="bullet">
    ///     <item>
    ///         <description>Handling turn-based action selection for each character.</description>
    ///     </item>
    ///     <item>
    ///         <description>Executing actions based on character speed and processing end-of-turn effects.</description>
    ///     </item>
    ///     <item>
    ///         <description>Terminating the turn when only one team has characters alive.</description>
    ///     </item>
    /// </list>
    /// </remarks>
    private static Team? GameContent()
    {
        Console.WriteLine("Bon jeu !");
        Next(2000);

        // game content
        var turn = 1;
        while (Team.IsCombatOn())
        {
            Skill.UpdateReloadCooldowns();
            Console.WriteLine($"{new string('=', 10)} Tour {turn++} {new string('=', 10)}");

            List<Skill> turnActions = [];
            Team.List.ForEach(team =>
            {
                Console.WriteLine($"Au tour de l'équipe {team.Name}");
                team.Characters
                    .Where(character => character.IsAlive(true)).ToList()
                    .ForEach(character =>
                    {
                        bool next, first = true;
                        do
                        {
                            Console.WriteLine($"Au tour de {character.Name} - {character.Team.Name}");
                            character.LevelUp();

                            if (first)
                            {
                                Console.WriteLine($"{character}\n");
                                first = false;
                            }

                            var status = character.SelectAction();

                            next = status.Next;
                            if (status.Skill != null) turnActions.Add(status.Skill);
                        } while (!next);

                        Prompt.Input("Appuyez sur 'Entrée' pour finir le tour du personnage",
                            key => key != ConsoleKey.Enter);
                        Next(0);
                    });
                Console.WriteLine($"L'équipe {team.Name} à fini de choisi ses action pour tout ses personnages.");
                Prompt.Input("Appuyez sur 'Entrée' pour finir le tour de l'équipe",
                    key => key != ConsoleKey.Enter);
                Next(0);
            });

            Console.WriteLine($"Execution du tour {turn}...");

            turnActions = turnActions.OrderByDescending(action => action.Owner.GetSpeed()).ToList();
            turnActions.ForEach(action =>
            {
                action.Execute();
                Console.WriteLine();
            });

            Console.WriteLine();
            Character.EndTurn();

            Prompt.Input("Appuyez sur 'Entrée' pour finir le tour", key => key != ConsoleKey.Enter);
            Next(0);
        }

        // game end
        var winners = Team.List.Where(team => team.Characters.Any(character => character.IsAlive(false))).ToList();

        Team? winnerTeam = null;
        switch (winners.Count)
        {
            case 1:
                winnerTeam = winners.First();
                Console.WriteLine($"{winnerTeam.Name} a gagné, Félicitations !");
                Console.WriteLine(
                    $"Membres : {(winnerTeam.Characters.Count != 0
                        ? string.Join(", ", winnerTeam.Characters.Select(character => $"{character.Name}{(!character.IsAlive(false) ? " (mort au combat)" : string.Empty)}"))
                        : "Aucun personnage")}");
                break;
            case > 1:
                Console.WriteLine("Une erreur est survenue : il n'y a pas qu'un seul gagnant.");
                break;
            default:
                Console.WriteLine("Une erreur est survenue : aucun gagnant.");
                break;
        }

        Prompt.Input("Appuyez sur 'Entrée' pour finir la partie",
            key => key != ConsoleKey.Enter);
        Next(0);
        return winnerTeam;
    }

    /// <summary>
    /// Cleans up game data and resets teams for a new game if desired.
    /// </summary>
    /// <param name="winningTeam">
    /// The team that won the game, or null if no winner was determined.
    /// </param>
    /// <remarks>
    /// This method manages the cleanup of teams and characters at the end of the game. It follows these rules:
    /// <list type="bullet">
    ///     <item>
    ///         <description>If there is a winning team, it is preserved while all other teams and characters are removed.</description>
    ///     </item>
    ///     <item>
    ///         <description>If no winner exists, all teams and characters are cleared.</description>
    ///     </item>
    /// </list>
    /// </remarks>
    private static void GameEnd(Team? winningTeam)
    {
        if (winningTeam == null)
        {
            Team.List.Clear();
            Character.List.Clear();
            Skill.List.Clear();
        }
        else Team.List.Clear(winningTeam);
    }

    /// <summary>
    /// Clears teams and characters from the game except for the winning team.
    /// </summary>
    /// <param name="teamList">The list of all teams in the game.</param>
    /// <param name="winningTeam">The team that won the game.</param>
    private static void Clear(this List<Team> teamList, Team winningTeam)
    {
        teamList
            .Where(team => team != winningTeam).ToList()
            .ForEach(team =>
            {
                team.Characters.ForEach(character =>
                {
                    Skill.List.RemoveAll(skill => character.Skills.Contains(skill));
                    Character.List.Remove(character);
                });
                teamList.Remove(team);
            });
    }

    /// <summary>
    /// Clears the console screen and waits for a specified amount of time.
    /// </summary>
    /// <param name="millisecondsTimeout">The number of milliseconds to wait before clearing the screen.</param>
    public static void Next(int millisecondsTimeout = 500)
    {
        Thread.Sleep(millisecondsTimeout);
        Console.Clear();
    }
}