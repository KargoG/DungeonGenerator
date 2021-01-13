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

        #region LoadingData

        private static void CreateGameplayContainer()
        {
            _gameplay = ScriptableObject.CreateInstance<GameplayContainer>();
            AssetDatabase.CreateAsset(_gameplay, "Assets/DungeonGenerator/ScriptableObjects/Gameplay.asset");
        }

        private static void LoadGameplayContainer(string guid)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            _gameplay = AssetDatabase.LoadAssetAtPath<GameplayContainer>(assetPath);
        }

        private static void CreateGameplayElementContainer()
        {
            _gameplayElements = ScriptableObject.CreateInstance<GameplayElementContainer>();
            AssetDatabase.CreateAsset(_gameplayElements,
                "Assets/DungeonGenerator/ScriptableObjects/GameplayElements.asset");
        }

        private static void LoadGameplayElementContainer(string guid)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            _gameplayElements = AssetDatabase.LoadAssetAtPath<GameplayElementContainer>(assetPath);
        }

        public static void CreateAction(Action action)
        {
            AssetDatabase.CreateAsset(action,
                "Assets/DungeonGenerator/ScriptableObjects/GameplayElements/Actions/" + action.name + ".asset");
            _gameplayElements.AddElement(action, GameplayElementTypes.Action);
        }

        public static void CreateEntity(Entity entity)
        {
            AssetDatabase.CreateAsset(entity,
                "Assets/DungeonGenerator/ScriptableObjects/GameplayElements/Entities/" + entity.name + ".asset");
            _gameplayElements.AddElement(entity, GameplayElementTypes.Entity);
        }

        public static void CreateAbility(Ability ability)
        {
            AssetDatabase.CreateAsset(ability,
                "Assets/DungeonGenerator/ScriptableObjects/GameplayElements/Abilities/" + ability.name + ".asset");
            _gameplayElements.AddElement(ability, GameplayElementTypes.Ability);
        }

        public static void CreateConsumable(Consumable consumable)
        {
            AssetDatabase.CreateAsset(consumable,
                "Assets/DungeonGenerator/ScriptableObjects/GameplayElements/Consumables/" + consumable.name + ".asset");
            _gameplayElements.AddElement(consumable, GameplayElementTypes.Consumable);
        }

        public static GameplayGraph CreateGameplayGraph(string name)
        {
            GameplayGraph newGraph = GameplayGraph.CreateGraph(name);
            AssetDatabase.CreateAsset(newGraph,
                "Assets/DungeonGenerator/ScriptableObjects/GameplayGraphs/LevelGraphs/" + name + ".asset");

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

            GetGameplayContainer().AddGameplay(createdGameplay);
        }

        public static void DeleteGameplay(Gameplay gameplayToDelete)
        {
            GetGameplayContainer().RemoveGameplay(gameplayToDelete);
            AssetDatabase.DeleteAsset("Assets/DungeonGenerator/ScriptableObjects/Gameplay/" + gameplayToDelete.name +
                                      ".asset");
        }

        public static RoomGraph CreateRoomGraph(string name, GameplayGraph graphToTransform)
        {
            RoomGraph newGraph = RoomGraph.CreateStartingGraph(name, graphToTransform);
            AssetDatabase.CreateAsset(newGraph,
                "Assets/DungeonGenerator/ScriptableObjects/RoomGraphs/" + name + ".asset");

            return newGraph;
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

        public static List<GameplayGraph> GetGameplayGraphs()
        {
            List<GameplayGraph> graphs = new List<GameplayGraph>();

            string[] assetsGUID = AssetDatabase.FindAssets("t:GameplayGraph");

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
