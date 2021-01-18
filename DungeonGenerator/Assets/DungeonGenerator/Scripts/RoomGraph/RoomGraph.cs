using System;
using System.Collections.Generic;
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
            RoomGraph newGraph = CreateInstance<RoomGraph>();
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

            // TODO check if this works
            //AssetDatabase.AddObjectToAsset(gameplayInRoom, "Assets/DungeonGenerator/ScriptableObjects/RoomGraphs/" + name + ".asset");
            //AssetDatabase.AddObjectToAsset(_dungeonGraph[_dungeonGraph.Count - 1], "Assets/DungeonGenerator/ScriptableObjects/RoomGraphs/" + name + ".asset");
        }

        public List<DungeonRoom> ApplyMerges(Tuple<Gameplay, Gameplay> gameplayToMerge, float mergeLikeliness)
        {
            List<DungeonRoom> hasToBeDeleted = new List<DungeonRoom>();

            List<DungeonRoom> roomThatCanBeMerged = _dungeonGraph.FindAll((DungeonRoom room) =>
            {
                bool hasGameplay = room.LastGameplay.Gameplay == gameplayToMerge.Item1;

                bool hasNextGameplay = false;

                foreach (DungeonRoom nextRoom in room.NextRoom)
                {
                    if (nextRoom.FirstGameplay.Gameplay == gameplayToMerge.Item2)
                    {
                        hasNextGameplay = true;
                        break;
                    }
                }

                return hasGameplay && hasNextGameplay;
            });

            foreach (DungeonRoom roomToMerge in roomThatCanBeMerged)
            {
                if (UnityEngine.Random.Range(0.0f, 1.0f) <= mergeLikeliness)
                {
                    DungeonRoom removedRoom = roomToMerge.MergeWithNext(gameplayToMerge.Item2);
                    _dungeonGraph.Remove(removedRoom);
                    // TODO check if this works
                    hasToBeDeleted.Add(removedRoom);
                    //AssetDatabase.RemoveObjectFromAsset(removedRoom);
                }

            }

            return hasToBeDeleted;
        }
    }
}
