using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// TODO can i remove this?
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace DungeonGenerator
{
    public class GameplayGraphManager : ScriptableObject
    {
        public static GameplayGraph CreateStartGraph(string name, GameplayGraphSettings settings)
        {
            int minStillNeededNodes = settings.MinStartGraphLength -
                                      (settings.StartGameplay.Count + settings.EndGameplay.Count +
                                       settings.MustIncludeGameplay.Count);
            int maxStillNeededNodes = settings.MaxStartGraphLength -
                                      (settings.StartGameplay.Count + settings.EndGameplay.Count +
                                       settings.MustIncludeGameplay.Count);

            minStillNeededNodes = Mathf.Max(minStillNeededNodes, 0);
            maxStillNeededNodes = Mathf.Max(maxStillNeededNodes, 0);


            GameplayGraph startGraph = GameplayGraph.CreateGraph(name);

            foreach (Gameplay gameplay in settings.StartGameplay) // Add Nodes to beginning
            {
                GameplayRepresentation addedGameplay = startGraph.AddGameplay(gameplay);

#if UNITY_EDITOR
                AssetDatabase.AddObjectToAsset(addedGameplay, "Assets/DungeonGenerator/ScriptableObjects/GameplayGraphs/LevelGraphs/" + startGraph.name + ".asset");
                EditorUtility.SetDirty(addedGameplay);
                AssetDatabase.SaveAssets();
#endif
            }

            int stillNeededNodes =
                Random.Range(minStillNeededNodes,
                    maxStillNeededNodes); // Calculate how many random nodes need to be added
            List<Gameplay> mustHaveNodes = new List<Gameplay>(settings.MustIncludeGameplay);

            while (stillNeededNodes + mustHaveNodes.Count > 0) //  Add still wanted gameplay in random order
            {
                int gameplayToSpawn = Random.Range(0, stillNeededNodes + mustHaveNodes.Count);

                GameplayRepresentation addedGameplay;

                if (gameplayToSpawn < mustHaveNodes.Count)
                {
                    addedGameplay = startGraph.AddGameplay(mustHaveNodes[gameplayToSpawn]);
                    mustHaveNodes.RemoveAt(gameplayToSpawn);
                }
                else
                {
                    addedGameplay = startGraph.AddGameplay(settings.GetRandomPlacableGameplay());
                    stillNeededNodes--;
                }


#if UNITY_EDITOR
                AssetDatabase.AddObjectToAsset(addedGameplay, "Assets/DungeonGenerator/ScriptableObjects/GameplayGraphs/LevelGraphs/" + startGraph.name + ".asset");
                EditorUtility.SetDirty(addedGameplay);
                AssetDatabase.SaveAssets();
#endif
            }


            foreach (Gameplay gameplay in settings.EndGameplay) // Add Nodes to end
            {
                GameplayRepresentation addedGameplay = startGraph.AddGameplay(gameplay);

#if UNITY_EDITOR
                AssetDatabase.AddObjectToAsset(addedGameplay, "Assets/DungeonGenerator/ScriptableObjects/GameplayGraphs/LevelGraphs/" + startGraph.name + ".asset");
                EditorUtility.SetDirty(addedGameplay);
                AssetDatabase.SaveAssets();
#endif
            }

            for (int i = 0; i < startGraph.GameplayInGraph.Count; i++)
            {
                if (i < startGraph.GameplayInGraph.Count - 1)
                {
                    startGraph.GameplayInGraph[i].AddNextGameplay(startGraph.GameplayInGraph[i + 1]);
                    startGraph.GameplayInGraph[i + 1].AddPreviousGameplay(startGraph.GameplayInGraph[i]);
                }
            }

#if UNITY_EDITOR
            for (int i = 0; i < startGraph.GameplayInGraph.Count; i++)
            {
                startGraph.GameplayInGraph[i].Position = new Vector2(0, i * (50 + 20));
            }

            EditorUtility.SetDirty(startGraph);
            AssetDatabase.SaveAssets();
#endif

            return startGraph;
        }
    }
}
