using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DungeonGenerator.Editor
{
    public class DataAccess : ScriptableObject
    {
        private static GameplayElementContainer _gameplayElements;
        private static GameplayContainer _gameplay;
        private static SubgraphConnections _connections;

        #region LoadingData

        private static void CreateGameplayContainer()
        {
            _gameplay = ScriptableObject.CreateInstance<GameplayContainer>();
            AssetDatabase.CreateAsset(_gameplay, "Assets/DungeonGenerator/ScriptableObjects/Gameplay.asset");
        }

        private static void CreateGameplayElementContainer()
        {
            _gameplayElements = ScriptableObject.CreateInstance<GameplayElementContainer>();
            AssetDatabase.CreateAsset(_gameplayElements,
                "Assets/DungeonGenerator/ScriptableObjects/GameplayElements.asset");
        }

        private static void CreateGameplaySubgraphConnections()
        {
            _connections = ScriptableObject.CreateInstance<SubgraphConnections>();
            AssetDatabase.CreateAsset(_connections,
                "Assets/DungeonGenerator/ScriptableObjects/GameplayGraphs/SubGraphs/Connections/SubgraphConnections.asset");
        }

        public static void CreateAction(Action action)
        {
            AssetDatabase.CreateAsset(action,
                "Assets/DungeonGenerator/ScriptableObjects/GameplayElements/Actions/" + action.name + ".asset");
            _gameplayElements.AddElement(action, GameplayElementTypes.Action);

            EditorUtility.SetDirty(_gameplayElements);
            AssetDatabase.SaveAssets();
        }

        public static void CreateEntity(Entity entity)
        {
            AssetDatabase.CreateAsset(entity,
                "Assets/DungeonGenerator/ScriptableObjects/GameplayElements/Entities/" + entity.name + ".asset");
            _gameplayElements.AddElement(entity, GameplayElementTypes.Entity);

            EditorUtility.SetDirty(_gameplayElements);
            AssetDatabase.SaveAssets();
        }

        public static void CreateAbility(Ability ability)
        {
            AssetDatabase.CreateAsset(ability,
                "Assets/DungeonGenerator/ScriptableObjects/GameplayElements/Abilities/" + ability.name + ".asset");
            _gameplayElements.AddElement(ability, GameplayElementTypes.Ability);

            EditorUtility.SetDirty(_gameplayElements);
            AssetDatabase.SaveAssets();
        }

        public static void CreateConsumable(Consumable consumable)
        {
            AssetDatabase.CreateAsset(consumable,
                "Assets/DungeonGenerator/ScriptableObjects/GameplayElements/Consumables/" + consumable.name + ".asset");
            _gameplayElements.AddElement(consumable, GameplayElementTypes.Consumable);

            EditorUtility.SetDirty(_gameplayElements);
            AssetDatabase.SaveAssets();
        }

        public static void DeleteAction(Action toDelete)
        {
            _gameplayElements.DeleteElement(toDelete, GameplayElementTypes.Action);
            AssetDatabase.DeleteAsset("Assets/DungeonGenerator/ScriptableObjects/GameplayElements/Actions/" + toDelete.name + ".asset");
            EditorUtility.SetDirty(_gameplayElements);
            AssetDatabase.SaveAssets();
        }
        public static void DeleteEntity(Entity toDelete)
        {
            _gameplayElements.DeleteElement(toDelete, GameplayElementTypes.Entity);
            AssetDatabase.DeleteAsset("Assets/DungeonGenerator/ScriptableObjects/GameplayElements/Entities/" + toDelete.name + ".asset");
            EditorUtility.SetDirty(_gameplayElements);
            AssetDatabase.SaveAssets();
        }
        public static void DeleteAbility(Ability toDelete)
        {
            _gameplayElements.DeleteElement(toDelete, GameplayElementTypes.Ability);
            AssetDatabase.DeleteAsset("Assets/DungeonGenerator/ScriptableObjects/GameplayElements/Abilities/" + toDelete.name + ".asset");
            EditorUtility.SetDirty(_gameplayElements);
            AssetDatabase.SaveAssets();
        }
        public static void DeleteConsumable(Consumable toDelete)
        {
            _gameplayElements.DeleteElement(toDelete, GameplayElementTypes.Consumable);
            AssetDatabase.DeleteAsset("Assets/DungeonGenerator/ScriptableObjects/GameplayElements/Consumables/" + toDelete.name + ".asset");
            EditorUtility.SetDirty(_gameplayElements);
            AssetDatabase.SaveAssets();
        }

        public static GameplayGraph CreateGameplayGraph(string name)
        {
            GameplayGraph newGraph = GameplayGraph.CreateGraph(name);
            AssetDatabase.CreateAsset(newGraph,
                "Assets/DungeonGenerator/ScriptableObjects/GameplayGraphs/LevelGraphs/" + name + ".asset");

            return newGraph;
        }

        public static GameplayGraph CreateGameplaySubGraph(string name)
        {
            GameplayGraph newGraph = GameplayGraph.CreateGraph(name);
            AssetDatabase.CreateAsset(newGraph,
                "Assets/DungeonGenerator/ScriptableObjects/GameplayGraphs/SubGraphs/" + name + ".asset");

            return newGraph;
        }

        private static GameplayGraph LoadGameplayGraph(string guid)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAssetAtPath<GameplayGraph>(assetPath);
        }

        public static void CreateGameplay(Gameplay createdGameplay)
        {
            AssetDatabase.CreateAsset(createdGameplay,
                "Assets/DungeonGenerator/ScriptableObjects/Gameplay/" + createdGameplay.name + ".asset");

            _gameplay.AddGameplay(createdGameplay);

            EditorUtility.SetDirty(_gameplay);
            AssetDatabase.SaveAssets();
        }

        public static void DeleteGameplay(Gameplay gameplayToDelete)
        {
            _gameplay.RemoveGameplay(gameplayToDelete);
            AssetDatabase.DeleteAsset("Assets/DungeonGenerator/ScriptableObjects/Gameplay/" + gameplayToDelete.name +
                                      ".asset");

            EditorUtility.SetDirty(_gameplay);
            AssetDatabase.SaveAssets();
        }

        public static RoomGraph CreateRoomGraph(string name)
        {
            RoomGraph newGraph = CreateInstance<RoomGraph>();
            AssetDatabase.CreateAsset(newGraph,
                "Assets/DungeonGenerator/ScriptableObjects/RoomGraphs/" + name + ".asset");

            return newGraph;
            //newGraph = RoomGraph.CreateStartingGraph(name, graphToTransform);
            //EditorUtility.SetDirty(newGraph);
            //AssetDatabase.SaveAssets();
        }

        private static RoomGraph LoadRoomGraph(string guid)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            return AssetDatabase.LoadAssetAtPath<RoomGraph>(assetPath);
        }

        #endregion

        #region AccessData

        public static GameplayContainer GetGameplayContainer()
        {
            //string assetsGUID = AssetDatabase.AssetPathToGUID("Assets/DungeonGenerator/ScriptableObjects/Gameplay.asset");

            if (_gameplay == null)
            {

                _gameplay = AssetDatabase.LoadAssetAtPath<GameplayContainer>(
                    "Assets/DungeonGenerator/ScriptableObjects/Gameplay.asset");

                if (!_gameplay)
                    CreateGameplayContainer();
            }

            return _gameplay;
        }

        public static GameplayElementContainer GetGameplayElementContainer()
        {
            string assetsGUID =
                AssetDatabase.AssetPathToGUID("Assets/DungeonGenerator/ScriptableObjects/GameplayElements.asset");


            if (_gameplayElements == null)
            {
                _gameplayElements =
                    AssetDatabase.LoadAssetAtPath<GameplayElementContainer>(
                        "Assets/DungeonGenerator/ScriptableObjects/GameplayElements.asset");

                if (!_gameplayElements)
                    CreateGameplayElementContainer();
            }

            return _gameplayElements;
        }

        public static SubgraphConnections GetGameplaySubgraphConnections()
        {
            if (_connections == null)
            {

                _connections = AssetDatabase.LoadAssetAtPath<SubgraphConnections>(
                    "Assets/DungeonGenerator/ScriptableObjects/GameplayGraphs/SubGraphs/Connections/SubgraphConnections.asset");

                if (!_connections)
                    CreateGameplaySubgraphConnections();
            }

            return _connections;
        }

        public static List<GameplayGraph> GetGameplayGraphs()
        {
            List<GameplayGraph> graphs = new List<GameplayGraph>();

            string[] assetsGUID = AssetDatabase.FindAssets("t:GameplayGraph", new[] { "Assets/DungeonGenerator/ScriptableObjects/GameplayGraphs/LevelGraphs" });

            foreach (string guid in assetsGUID)
            {
                graphs.Add(LoadGameplayGraph(guid));
            }

            return graphs;
        }

        public static List<GameplayGraph> GetGameplaySubGraphs()
        {
            List<GameplayGraph> graphs = new List<GameplayGraph>();

            string[] assetsGUID = AssetDatabase.FindAssets("t:GameplayGraph", new []{ "Assets/DungeonGenerator/ScriptableObjects/GameplayGraphs/SubGraphs" });
            
            foreach (string guid in assetsGUID)
            {
                graphs.Add(LoadGameplayGraph(guid));
            }

            return graphs;
        }

        public static List<RoomGraph> GetRoomGraphs()
        {
            List<RoomGraph> graphs = new List<RoomGraph>();

            string[] assetsGUID = AssetDatabase.FindAssets("t:RoomGraph");

            foreach (string guid in assetsGUID)
            {
                graphs.Add(LoadRoomGraph(guid));
            }

            return graphs;
        }
        #endregion
    }
}
