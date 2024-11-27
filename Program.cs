using JRPG_Game.Characters;
using JRPG_Game.Utils;

namespace JRPG_Game;

public static class Program
{
    public static void Main()
    {
        Console.WriteLine("Bienvenue dans le jeu !\n");

        var nbrPlayers = Prompt.GetInt("Entrez le nombre de joueurs :", i => i < 2);

        Next();

        for (var i = 0; i < nbrPlayers; i++)
        {
            Console.WriteLine("Creation de Joueur " + (i + 1) + ":");
            CreateCharacter();
            Next();
        }

        Console.WriteLine($"{Character.List.Count} joueurs créés.");
        Console.WriteLine("Bon jeu !");

        Thread.Sleep(1500);
        Next();

        // game start
    }

    private static Character CreateCharacter()
    {
        while (true)
        {
            List<Type> classList = [typeof(Mage), typeof(Paladin), typeof(Thief), typeof(Warrior)];
            var characterType = classList.ElementAt(Prompt.Select("Choisissez votre classe :", c => c.Name, classList));

            if (Activator.CreateInstance(characterType, Prompt.GetString("Entrez votre nom :")) is Character
                character)
                return character;

            Console.WriteLine("Une erreur s'est produite : " + characterType);
        }
    }

    private static void Next()
    {
        Thread.Sleep(500);
        Console.Clear();
    }
}