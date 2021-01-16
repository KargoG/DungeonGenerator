using System;
using System.Collections.Generic;
using DungeonGenerator.Editor;
using UnityEditor;
using UnityEngine;

namespace DungeonGenerator
{
    [Serializable]
    public class RoomGraph : ScriptableObject
    {
        [SerializeField] private string _name;

        public string Name
        {
            get { return _name; }
        }

        [SerializeField] private List<DungeonRoom> _dungeonGraph;

        public List<DungeonRoom> DungeonGraph
        {
            get { return _dungeonGraph; }
        }

        [SerializeField] private DungeonRoom _firstRoom;

        void OnEnable()
        {
            if (_dungeonGraph == null)
                _dungeonGraph = new List<DungeonRoom>();
        }

        public static RoomGraph CreateStartingGraph(string name, GameplayGraph gameplayGraph)
        {
            RoomGraph newGraph = DataAccess.CreateRoomGraph(name);

            newGraph._name = name;

            List<GameplayRepresentation> gameplayInDungeon = gameplayGraph.CreateCopyOfGameplay();

            foreach (GameplayRepresentation gameplayRepresentation in gameplayInDungeon)
            {
                newGraph.AddRoom(gameplayRepresentation);
            }

            foreach (DungeonRoom room in newGraph.DungeonGraph)
            {
                room.PrepareConnections();
            }

            return newGraph;
        }

        public void AddRoom(GameplayRepresentation gameplayInRoom)
        {
            DungeonRoom newRoom = DungeonRoom.Create(gameplayInRoom);
            _dungeonGraph.Add(newRoom);
            gameplayInRoom.RoomGameplayIsIn = newRoom;

            if (!_firstRoom)
            {
                _firstRoom = newRoom;
            }

            AssetDatabase.AddObjectToAsset(gameplayInRoom, "Assets/DungeonGenerator/ScriptableObjects/RoomGraphs/" + name + ".asset");
            AssetDatabase.AddObjectToAsset(_dungeonGraph[_dungeonGraph.Count - 1], "Assets/DungeonGenerator/ScriptableObjects/RoomGraphs/" + name + ".asset");
        }
    }
}
