using JRPG_Game.Characters;
using JRPG_Game.Utils;

namespace JRPG_Game;

public static class Program
{
    public static void Main()
    {
        // game start
        Console.WriteLine("Bienvenue dans le jeu !\n");

        var nbrPlayers = Prompt.Get<int>("Entrez le nombre de joueurs :", i => i < 2);

        Next();

        for (var i = 0; i < nbrPlayers; i++)
        {
            Console.WriteLine("Creation de Joueur " + (i + 1) + ":");
            CreateCharacter();
            Next();
        }

        Console.WriteLine($"{Character.List.Count} joueurs créés.");
        Console.WriteLine("Bon jeu !");

        Next(2000);

        // game content
        while (Character.CombatIsOn())
            foreach (var character in Character.List
                         .TakeWhile(_ => Character.CombatIsOn())
                         .Where(character => character.IsAlive(true)))
            {
                bool next;
                do
                {
                    Console.WriteLine($"Au tour de {character.Name}");
                    next = character.SelectAction();
                } while (!next);

                _ = Prompt.Input("Appuyez sur 'Entrée' pour finir le tour", key => key != ConsoleKey.Enter);
                Next(0);
            }

        // game end
        var winners = Character.List.Where(player => player.IsAlive(true)).ToList();

        switch (winners.Count)
        {
            case 1:
                Console.WriteLine($"{winners.First().Name} a gagné, Félicitations !");
                break;
            case > 1:
                Console.WriteLine("Une erreur est survenue : il n'y a pas qu'un seul gagnant.");
                break;
            default:
                Console.WriteLine("Une erreur est survenue : aucun gagnant.");
                break;
        }

    }

    private static Character CreateCharacter()
    {
        while (true)
        {
            List<Type> classList = [typeof(Mage), typeof(Paladin), typeof(Thief), typeof(Warrior)];
            var characterType =
                classList.ElementAt(Prompt.Select("Choisissez votre classe :", c => c.Name, classList) - 1);

            if (Activator.CreateInstance(characterType, Prompt.Get<string>("Entrez votre nom :"))
                is Character character)
                return character;

            Console.WriteLine("Une erreur s'est produite : " + characterType);
        }
    }

    public static void Next(int millisecondsTimeout = 500)
    {
        Thread.Sleep(millisecondsTimeout);
        Console.Clear();
    }
}