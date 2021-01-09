using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayGraphManager : ScriptableObject
{
    public static GameplayGraph CreateStartGraph(string name, GameplayGraphSettings settings)
    {
        int minStillNeededNodes = settings.MinStartGraphLength - (settings.StartGameplay.Count + settings.EndGameplay.Count + settings.MustIncludeGameplay.Count);
        int maxStillNeededNodes = settings.MaxStartGraphLength - (settings.StartGameplay.Count + settings.EndGameplay.Count + settings.MustIncludeGameplay.Count);

        minStillNeededNodes = Mathf.Max(minStillNeededNodes, 0);
        maxStillNeededNodes = Mathf.Max(maxStillNeededNodes, 0);

        GameplayGraph startGraph = DataAccess.CreateGameplayGraph(name);

        foreach (Gameplay gameplay in settings.StartGameplay) // Add Nodes to beginning
        {
            startGraph.AddGameplay(gameplay);
        }

        int stillNeededNodes = Random.Range(minStillNeededNodes, maxStillNeededNodes); // Calculate how many random nodes need to be added
        List<Gameplay> mustHaveNodes = settings.MustIncludeGameplay;

        while (stillNeededNodes + mustHaveNodes.Count > 0) //  Add still wanted gameplay in random order
        {
            int gameplayToSpawn = Random.Range(0, stillNeededNodes + mustHaveNodes.Count);

            if (gameplayToSpawn < mustHaveNodes.Count)
            {
                startGraph.AddGameplay(mustHaveNodes[gameplayToSpawn]);
                mustHaveNodes.RemoveAt(gameplayToSpawn);
            }
            else
            {
                startGraph.AddGameplay(DataAccess.GetGameplayContainer().GetRandomPlacableGameplay());
                stillNeededNodes--;
            }
        }


        foreach (Gameplay gameplay in settings.EndGameplay) // Add Nodes to end
        {
            startGraph.AddGameplay(gameplay);
        }

        return startGraph;
    }
}
